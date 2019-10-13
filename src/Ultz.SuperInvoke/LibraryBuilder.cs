using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Ultz.SuperCecil;
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
                if (!asm.MainModule.AssemblyReferences.Any(x => x.Name == depn.Name && x.Version == depn.Version))
                {
                    asm.MainModule.AssemblyReferences.Add(new AssemblyNameReference(depn.Name, depn.Version));
                }
                
                asm.MainModule.ModuleReferences.Add(new ModuleReference(item.PInvokeName));
            }
            asm?.MainModule.AssemblyReferences.Add(new AssemblyNameReference(thisDepn.Name, thisDepn.Version));
            return asm;
        }

        public static TypeDefinition CreateImplementation(ref BuilderOptions opts, ModuleDefinition module)
        {
            var type = opts.Type;
            if (type.GetCustomAttribute<NativeApiAttribute>() is null)
                throw new ArgumentException
                (
                    "The type provided does not have NativeApiAttribute applied.",
                    nameof(type)
                );

            if (type.IsClass && type.IsAbstract && type.IsSubclassOf(typeof(NativeApiContainer)) || type.IsInterface)
            {
                var def = new TypeDefinition($"Ultz.Private.SIG.{type.Namespace}",
                    $"{type.Name}_SuperInvokeGenerated",
                    TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class,
                    type.IsClass ? Utilities.GetReference(type, module) : Utilities.GetReference(typeof(NativeApiContainer), module));
                var impl = CreateImplementation(ref opts,
                    typeof(NativeApiContainer).GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic),
                    module, out var slots);
                var ctor = CreateConstructor(opts.Type, slots, module);
                foreach (var method in impl) def.Methods.Add(method);

                def.Methods.Add(ctor);

                if (opts.IsPInvokeProxyEnabled)
                    def.CustomAttributes.Add(new CustomAttribute(
                        Utilities.GetReference(typeof(PInvokeProxyAttribute).GetConstructor(new Type[0]), module)));
                return def;
            }

            throw new ArgumentException
            (
                "The type to create an implementation of must either be an abstract class which derives from " +
                "NativeApiContainer, or an interface.",
                nameof(type)
            );
        }

        public static MethodDefinition CreateConstructor(Type type, int slots, ModuleDefinition module)
        {
            var ctor = new MethodDefinition(".ctor",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName, module.TypeSystem.Void);
            ctor.Parameters.Add(new ParameterDefinition(Utilities.GetReference(typeof(NativeLibrary), module)));
            var ctorIl = ctor.Body.GetILProcessor();
            ctorIl.Emit(OpCodes.Ldarg_0);
            ctorIl.Emit(OpCodes.Ldarg_1);
            ctorIl.Emit(OpCodes.Ldc_I4, slots);
            ctorIl.Emit(OpCodes.Call,
                (type.IsClass
                    ? Utilities.GetReference(type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, Type.DefaultBinder,
                        new[] {typeof(NativeLibrary), typeof(int)}, new ParameterModifier[0]), module)
                    : Utilities.GetReference(typeof(NativeApiContainer).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                        Type.DefaultBinder, new[] {typeof(NativeLibrary), typeof(int)}, new ParameterModifier[0]), module)) ??
                throw new ArgumentException("Type must preserve the (NativeLibrary, int) constructor", nameof(type)));
            ctorIl.Emit(OpCodes.Ret);
            return ctor;
        }

        public static IReadOnlyList<MethodDefinition> CreateImplementation(ref BuilderOptions opts, MethodInfo load,
            ModuleDefinition mod, out int slots)
        {
            slots = 0;
            var type = opts.Type;
            var mainAttr = type.GetCustomAttribute<NativeApiAttribute>();
            var methods = (type.IsInterface
                    ? new[] {type}
                        .Concat(
                            type.GetInterfaces()).SelectMany(i => i.GetMethods()).ToArray()
                    : type.GetMethods(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance))
                .Where(x => type.IsInterface || x.IsAbstract)
                .ToArray();
            var ret = new List<MethodDefinition>();
            for (var i = 0; i < methods.Length; i++)
            {
                var method = methods[i];
                var attr = method.GetCustomAttribute<NativeApiAttribute>();
                var ep = NativeApiAttribute.GetEntryPoint(attr, mainAttr, method.Name);
                var def = Utilities.CreateEmptyDefinition(method,
                    MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual |
                    MethodAttributes.HideBySig | MethodAttributes.NewSlot, mod);

                var il = def.Body.GetILProcessor();
                EmitParameters(il, method.GetParameters(), opts.ParameterMarshallers, out var paramTypes);
                EmitEntryPoint(il, Utilities.GetReference(load, mod), i, ep);
                EmitCalli(il, attr, mainAttr, def);
                EmitReturn(il, method.ReturnParameter, opts.ReturnTypeMarshallers, out var retType);
                ret.Add(def);
                if (opts.IsPInvokeProxyEnabled)
                    ret.AddRange(PInvokeGenerator.Generate(ep, paramTypes, retType,
                        NativeApiAttribute.GetCallingConvention(attr, mainAttr), mod, opts.PInvokeName));
                slots++;
            }

            return ret;

            void EmitParameters(ILProcessor il, ParameterInfo[] parameterInfo, IParameterMarshaller[] marshal,
                out Type[] finalTypes)
            {
                finalTypes = new Type[parameterInfo.Length];
                for (var i = 0; i < parameterInfo.Length; i++)
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
                            currentType = stage.Write(currentType, il, parameterInfo[i]);
                            changed = true;
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

            void EmitReturn(ILProcessor il, ParameterInfo parameterInfo, IReturnTypeMarshaller[] marshal,
                out Type finalType)
            {
                var currentType = parameterInfo.ParameterType;
                if (parameterInfo.ParameterType != typeof(void))
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