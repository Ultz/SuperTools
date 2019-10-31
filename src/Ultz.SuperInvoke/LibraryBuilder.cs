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
        private static readonly string[] RefRemovals = {
            "System.Private.CoreLib",
            "mscorlib",
            "System.Runtime"
        };
        public static AssemblyDefinition CreateAssembly(IEnumerable<BuilderOptions> workload,
            AssemblyNameDefinition name = null)
        {
            AssemblyDefinition asm = null;
            var whitelist = new List<string>();
            foreach (var item in workload)
            {
                var opts = item;
                var thisName = $"Ultz.Private.SIG.{item.Type.Namespace}.{item.Type.Name}";
                asm ??= AssemblyDefinition.CreateAssembly(
                    name ?? new AssemblyNameDefinition(thisName, new Version(1, 0)), $"{thisName}.dll", ModuleKind.Dll);
                asm.MainModule.Types.Add(CreateImplementation(ref opts, asm.MainModule));
                whitelist.AddRange(item._typeDef.Module.AssemblyReferences.Where(x => RefRemovals.Contains(x.Name))
                    .Where(x => !whitelist.Contains(x.Name)).Select(x => x.Name));
            }

            if (!(asm is null))
            {
                var removals = asm.MainModule.AssemblyReferences.Select((x, i) => (x, i)).Where(x =>
                        RefRemovals.Contains(x.x.Name) && !whitelist.Contains(x.x.Name)).ToList()
                    .OrderByDescending(x => x.i);
                foreach (var y in removals)
                {
                    asm.MainModule.AssemblyReferences.RemoveAt(y.i);
                }
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
                var load = module.ImportReference(
                    typeof(NativeApiContainer).GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic));
                var impl = CreateImplementation(ref opts, load,
                    module, out var eps);
                var ctor = CreateConstructor(opts._typeDef, eps, module, !opts.UseLazyBinding, load);
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

        public static MethodDefinition CreateConstructor(TypeDefinition type, IReadOnlyList<string> slots, ModuleDefinition module, bool preload, MethodReference load)
        {
            var ctor = new MethodDefinition(".ctor",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName, module.TypeSystem.Void);
            ctor.Parameters.Add(new ParameterDefinition(module.ImportReference(typeof(NativeLibrary))));
            var ctorIl = ctor.Body.GetILProcessor();
            ctorIl.Emit(OpCodes.Ldarg_0);
            ctorIl.Emit(OpCodes.Ldarg_1);
            ctorIl.Emit(OpCodes.Ldc_I4, slots.Count);
            ctorIl.Emit(OpCodes.Call,
                module.ImportReference(type.GetConstructors().FirstOrDefault(x =>
                    x.Parameters.Select(y => y.ParameterType.ToString()).SequenceEqual(new[]
                        {"Ultz.SuperInvoke.Loader.NativeLibrary", module.TypeSystem.Int32.FullName})) ??
                throw new ArgumentException("Type must preserve the (NativeLibrary, int) constructor", nameof(type))));
            if (preload)
            {
                for (var i = 0; i < slots.Count; i++)
                {
                    var ep = slots[i];
                    ctorIl.Emit(OpCodes.Ldarg_0);
                    ctorIl.Emit(OpCodes.Ldc_I4, i);
                    ctorIl.Emit(OpCodes.Ldstr, ep);
                    ctorIl.Emit(OpCodes.Call, load);
                    ctorIl.Emit(OpCodes.Pop);
                }
            }

            ctorIl.Emit(OpCodes.Ret);
            return ctor;
        }

        public static IReadOnlyList<MethodDefinition> CreateImplementation(ref BuilderOptions opts, MethodReference load,
            ModuleDefinition mod, out IReadOnlyList<string> epl)
        {
            var eps = new List<string>();
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
                    MethodAttributes.HideBySig, mod);

                var il = def.Body.GetILProcessor();
                var ctx = new MethodContext(il, i, mod);
                EmitParameters(ctx, method.Parameters, opts.ParameterMarshallers, out var paramTypes, out var epilogue);
                EmitEntryPoint(il, mod.ImportReference(load), i, ep);
                EmitCalli(il, attr, mainAttr, def, paramTypes);
                foreach (var e in epilogue)
                {
                    e(ctx);
                }
                EmitReturn(ctx, method.MethodReturnType, opts.ReturnTypeMarshallers, out var retType);
                ret.Add(def);
                if (opts.IsPInvokeProxyEnabled)
                    ret.AddRange(PInvokeGenerator.Generate(ep, paramTypes, retType,
                            NativeApiAttribute.GetCallingConvention(attr, mainAttr), mod, opts.PInvokeName)
                        .Where(x => ret.All(y => y.Name != x.Name)));
                eps.Add(ep);
            }

            epl = eps;
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

            void EmitParameters(MethodContext ctx, Collection<ParameterDefinition> parameterInfo, IParameterMarshaller[] marshal,
                out TypeReference[] finalTypes, out List<Action<MethodContext>> epilogue)
            {
                var il = ctx.Processor;
                finalTypes = new TypeReference[parameterInfo.Count];
                epilogue = new List<Action<MethodContext>>();
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
                            var newType = stage.Write(currentType, ctx, parameterInfo[i], out var ep);
                            currentType = newType;
                            changed = true;
                            if (!(ep is null)) epilogue.Insert(0, ep);
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
                MethodReference reference, TypeReference[] finalTypes)
            {
                var callSite = new CallSite(mod.ImportReference(reference.ReturnType))
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

                foreach (var type in finalTypes)
                    callSite.Parameters.Add(new ParameterDefinition(mod.ImportReference(type)));

                processor.Emit(OpCodes.Calli, callSite);
            }

            void EmitReturn(MethodContext ctx, MethodReturnType parameterInfo, IReturnTypeMarshaller[] marshal,
                out TypeReference finalType)
            {
                var il = ctx.Processor;
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
                            currentType = stage.Write(currentType, ctx, parameterInfo);
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