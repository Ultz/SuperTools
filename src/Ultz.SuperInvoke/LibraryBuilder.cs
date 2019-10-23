using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using Ultz.SuperInvoke.Generation;
using Ultz.SuperInvoke.Native;
using Ultz.SuperInvoke.Proxy;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;
using NativeLibrary = Ultz.SuperInvoke.Loader.NativeLibrary;

namespace Ultz.SuperInvoke
{
    public static class LibraryBuilder
    {
        public static AssemblyDefinition CreateAssembly(IEnumerable<BuilderOptions> workload,
            AssemblyNameDefinition name = null)
        {
            var thisDepn = typeof(LibraryBuilder).Assembly.GetName();
            AssemblyDefinition asm = null;
            foreach (var item in workload)
            {
                var opts = item;
                var depn = opts.Type.Assembly.GetName();
                var thisName = $"Ultz.Private.SIG.{item.Type.Namespace}.{item.Type.Name}";
                asm ??= AssemblyDefinition.CreateAssembly(
                    name ?? new AssemblyNameDefinition(thisName, new Version(1, 0)), $"{thisName}.dll", ModuleKind.Dll);
                asm.MainModule.Types.Add(CreateImplementation(ref opts, asm.MainModule));
            }

            return asm;
        }

        public static TypeDefinition CreateImplementation(ref BuilderOptions opts, ModuleDefinition module)
        {
            var type = opts.Type;
            if (type.IsClass && type.IsAbstract && type.IsSubclassOf(typeof(NativeApiContainer)))
            {
                var def = new TypeDefinition($"Ultz.Private.SIG.{type.Namespace}",
                    $"{type.Name}_SuperInvokeGenerated",
                    TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class,
                    module.ImportReference(opts._typeDef));
                var impl = CreateImplementation(ref opts,
                    module.ImportReference(
                        typeof(NativeApiContainer).GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic)),
                    module, out var slots);
                var ctor = CreateConstructor(opts._typeDef, slots, module);
                foreach (var method in impl) def.Methods.Add(method);

                def.Methods.Add(ctor);

                if (opts.IsPInvokeProxyEnabled)
                    def.CustomAttributes.Add(new CustomAttribute(
                        module.ImportReference(typeof(PInvokeProxyAttribute).GetConstructor(new Type[0]))));

                return def;
            }

            throw new ArgumentException
            (
                "The type to implement must be an abstract class extending NativeApiContainer.",
                nameof(opts.Type)
            );
        }

        public static MethodDefinition CreateConstructor(TypeDefinition type, int slots, ModuleDefinition module)
        {
            var ctor = new MethodDefinition(".ctor",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName, module.TypeSystem.Void);
            ctor.Parameters.Add(new ParameterDefinition(module.ImportReference(typeof(NativeLibrary))));
            var ctorIl = ctor.Body.GetILProcessor();
            ctorIl.Emit(OpCodes.Ldarg_0);
            ctorIl.Emit(OpCodes.Ldarg_1);
            ctorIl.Emit(OpCodes.Ldc_I4, slots);
            ctorIl.Emit(OpCodes.Call,
                module.ImportReference(type.GetConstructors().FirstOrDefault(x =>
                    x.Parameters.Select(y => y.ParameterType.ToString()).SequenceEqual(new[]
                        {"Ultz.SuperInvoke.Loader.NativeLibrary", module.TypeSystem.Int32.FullName})) ??
                throw new ArgumentException("Type must preserve the (NativeLibrary, int) constructor", nameof(type))));
            ctorIl.Emit(OpCodes.Ret);
            return ctor;
        }

        public static IReadOnlyList<MethodDefinition> CreateImplementation(ref BuilderOptions opts, MethodReference load,
            ModuleDefinition mod, out int slots)
        {
            slots = 0;
            var type = opts._typeDef;
            var mainAttr = type.GetNativeApiAttribute();
            var methods = type.Methods
                .Where(x => x.IsAbstract)
                .ToArray();
            var ret = new List<MethodDefinition>();
            for (var i = 0; i < methods.Length; i++)
            {
                var method = methods[i];
                var attr = GetAttribute(method, type);
                var ep = NativeApiAttribute.GetEntryPoint(attr, mainAttr, method.Name);
                var def = Utilities.CreateEmptyDefinition(method,
                    MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual |
                    MethodAttributes.HideBySig);

                var il = def.Body.GetILProcessor();
                EmitParameters(il, method.Parameters, opts.ParameterMarshallers, out var paramTypes, out var epilogue);
                EmitEntryPoint(il, mod.ImportReference(load), i, ep);
                EmitCalli(il, attr, mainAttr, def);
                EmitReturn(il, method.MethodReturnType, opts.ReturnTypeMarshallers, out var retType);
                ret.Add(def);
                if (opts.IsPInvokeProxyEnabled)
                    ret.AddRange(PInvokeGenerator.Generate(ep, paramTypes, retType,
                        NativeApiAttribute.GetCallingConvention(attr, mainAttr), mod, opts.PInvokeName));
                slots++;
            }

            return ret;

            NativeApiAttribute GetAttribute(MethodDefinition info, TypeDefinition containingType)
            {
                var attribute = info.GetNativeApiAttribute();
                if (attribute is null)
                {
                    foreach (var newTypeRef in containingType.Interfaces.Select(x => x.InterfaceType).Concat(new[] {containingType.BaseType}))
                    {
                        var newType = newTypeRef.Resolve();
                        if (newType is null)
                        {
                            continue;
                        }

                        var newMethod = newType.Methods.FirstOrDefault
                        (
                            x => x.Name == info.Name &&
                                 info.Parameters.Select(y => y.ParameterType)
                                     .SequenceEqual(
                                         info.Parameters.Select(y =>
                                             y.ParameterType))
                        );
                        attribute ??= GetAttribute(newMethod, newType);
                    }
                }

                return attribute;
            }

            void EmitParameters(ILProcessor il, Collection<ParameterDefinition> parameterInfo, IParameterMarshaller[] marshal,
                out TypeReference[] finalTypes, out List<Action<ILProcessor>> epilogue)
            {
                finalTypes = new TypeReference[parameterInfo.Count];
                epilogue = new List<Action<ILProcessor>>();
                for (var i = 0; i < parameterInfo.Count; i++)
                {
                    var ili = i + 1;
                    if (ili < 4)
                        il.Emit(ili switch
                        {
                            1 => OpCodes.Ldarg_1,
                            2 => OpCodes.Ldarg_2,
                            3 => OpCodes.Ldarg_3,
                            _ => throw new ArgumentOutOfRangeException() // impossible
                        });
                    else
                        il.Emit(OpCodes.Ldarg, ili);

                    var currentType = parameterInfo[i].ParameterType;
                    bool changed;
                    do
                    {
                        changed = false;
                        foreach (var stage in marshal)
                        {
                            if (!stage.IsApplicable(currentType)) continue;
                            var newType = stage.Write(currentType, il, parameterInfo[i], out var ep);
                            currentType = newType;
                            changed = true;
                            if (!(ep is null)) epilogue.Add(ep);
                        }
                    } while (changed);

                    finalTypes[i] = currentType;
                }
            }

            void EmitEntryPoint(ILProcessor il, MethodReference gep, int slot, string entryPoint)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldc_I4, slot);
                il.Emit(OpCodes.Ldstr, entryPoint);
                il.Emit(OpCodes.Call, gep);
            }

            void EmitCalli(ILProcessor processor, NativeApiAttribute method, NativeApiAttribute parent,
                MethodReference reference)
            {
                var callSite = new CallSite(reference.ReturnType)
                {
                    CallingConvention = NativeApiAttribute.GetCallingConvention(method, parent) switch
                    {
                        CallingConvention.Cdecl => MethodCallingConvention.C,
                        CallingConvention.Winapi => MethodCallingConvention.StdCall,
                        CallingConvention.FastCall => MethodCallingConvention.FastCall,
                        CallingConvention.StdCall => MethodCallingConvention.StdCall,
                        CallingConvention.ThisCall => MethodCallingConvention.ThisCall,
                        _ => MethodCallingConvention.C
                    }
                };

                foreach (var parameterDefinition in reference.Parameters) callSite.Parameters.Add(parameterDefinition);

                processor.Emit(OpCodes.Calli, callSite);
            }

            void EmitReturn(ILProcessor il, MethodReturnType parameterInfo, IReturnTypeMarshaller[] marshal,
                out TypeReference finalType)
            {
                var currentType = parameterInfo.ReturnType;
                if (parameterInfo.ReturnType.FullName != "System.Void")
                {
                    bool changed;
                    do
                    {
                        changed = false;
                        foreach (var stage in marshal)
                        {
                            if (!stage.IsApplicable(currentType)) continue;
                            currentType = stage.Write(currentType, il, parameterInfo);
                            changed = true;
                        }
                    } while (changed);
                }

                finalType = currentType;
                il.Emit(OpCodes.Ret);
            }
        }
    }
}