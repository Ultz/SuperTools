using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using CustomAttributeNamedArgument = Mono.Cecil.CustomAttributeNamedArgument;
using EventAttributes = Mono.Cecil.EventAttributes;
using FieldAttributes = Mono.Cecil.FieldAttributes;
using GenericParameterAttributes = Mono.Cecil.GenericParameterAttributes;
using ICustomAttributeProvider = System.Reflection.ICustomAttributeProvider;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using MethodImplAttributes = Mono.Cecil.MethodImplAttributes;
using ParameterAttributes = Mono.Cecil.ParameterAttributes;
using PropertyAttributes = Mono.Cecil.PropertyAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;

namespace Ultz.SuperPack
{
    internal class MetadataMapper
    {
        private const BindingFlags AllDeclared = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
                                                 BindingFlags.Instance | BindingFlags.DeclaredOnly;

        private static readonly OpCode[] Opcodes = typeof(OpCodes)
            .GetFields(BindingFlags.Static | BindingFlags.Public)
            .Select(f => f.GetValue(null))
            .Cast<OpCode>()
            .ToArray();

        private readonly Assembly _assembly;

        private AssemblyDefinition _assemblyDefinition;
        private ModuleDefinition _moduleDefinition;

        private MetadataMapper(Assembly assembly)
        {
            _assembly = assembly;
        }

        public static AssemblyDefinition MapAssembly(Assembly assembly)
        {
            var mapper = new MetadataMapper(assembly);
            return mapper.Map();
        }

        private AssemblyDefinition Map()
        {
            _assemblyDefinition = AssemblyDefinitionFor(_assembly);
            _moduleDefinition = _assemblyDefinition.MainModule;

            foreach (var type in _assembly.GetTypes().Where(t => !t.IsNested))
                MapType(type);

            MapCustomAttributes(_assembly, _assemblyDefinition);
            MapCustomAttributes(_assembly.ManifestModule, _assemblyDefinition.MainModule);
            return _assemblyDefinition;
        }

        private void MapType(Type type, TypeDefinition declaringType = null)
        {
            var typeDefinition = TypeDefinitionFor(type, declaringType);

            foreach (var field in type.GetFields(AllDeclared))
                MapField(typeDefinition, field);

            foreach (var method in type.GetConstructors(AllDeclared).Cast<MethodBase>()
                .Concat(type.GetMethods(AllDeclared)))
                MapMethod(typeDefinition, method);

            foreach (var property in type.GetProperties(AllDeclared))
                MapProperty(property, PropertyDefinitionFor(property, typeDefinition));

            foreach (var evt in type.GetEvents(AllDeclared))
                MapEvent(evt, EventDefinitionFor(evt, typeDefinition));

            foreach (var iface in GetInterfaces(type))
                typeDefinition.Interfaces.Add(new InterfaceImplementation(CreateReference(iface, typeDefinition)));

            foreach (var nestedType in type.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic))
                MapType(nestedType, typeDefinition);

            MapCustomAttributes(type, typeDefinition);
        }

        private IEnumerable<Type> GetInterfaces(Type type)
        {
            if (type.BaseType == null)
                return type.GetInterfaces();

            return type.GetInterfaces().Except(type.BaseType.GetInterfaces());
        }

        private void MapMethod(TypeDefinition typeDefinition, MethodBase method)
        {
            var methodDefinition = MethodDefinitionFor(method, typeDefinition);

            MapCustomAttributes(method, methodDefinition);
            MapOverrides(method, methodDefinition);
            MapPInvokeInfo(method, methodDefinition);

            if (!ShouldMapBody(method, methodDefinition))
                return;

            MapMethodBody(method, methodDefinition);
        }

        private void MapPInvokeInfo(MethodBase method, MethodDefinition methodDefinition)
        {
            var attributes = method.GetCustomAttributes(typeof(DllImportAttribute), false);
            if (attributes.Length == 0)
                return;

            var import = (DllImportAttribute) attributes[0];
            var info = new PInvokeInfo(0, import.EntryPoint, ModuleReferenceFor(import.Value))
            {
                IsBestFitEnabled = import.BestFitMapping,
                IsThrowOnUnmappableCharEnabled = import.ThrowOnUnmappableChar,
                SupportsLastError = import.SetLastError,
                IsNoMangle = import.ExactSpelling
            };

            switch (import.CallingConvention)
            {
                case CallingConvention.Cdecl:
                    info.IsCallConvCdecl = true;
                    break;
                case CallingConvention.FastCall:
                    info.IsCallConvFastcall = true;
                    break;
                case CallingConvention.StdCall:
                    info.IsCallConvStdCall = true;
                    break;
                case CallingConvention.ThisCall:
                    info.IsCallConvThiscall = true;
                    break;
                case CallingConvention.Winapi:
                    info.IsCallConvWinapi = true;
                    break;
            }

            switch (import.CharSet)
            {
                case CharSet.Ansi:
                    info.IsCharSetAnsi = true;
                    break;
                case CharSet.Auto:
                    info.IsCharSetAuto = true;
                    break;
                case CharSet.None:
                    info.IsCharSetNotSpec = true;
                    break;
                case CharSet.Unicode:
                    info.IsCharSetUnicode = true;
                    break;
            }

            methodDefinition.PInvokeInfo = info;
        }

        private ModuleReference ModuleReferenceFor(string name)
        {
            foreach (var reference in _moduleDefinition.ModuleReferences)
                if (reference.Name == name)
                    return reference;

            var module = new ModuleReference(name);
            _moduleDefinition.ModuleReferences.Add(module);
            return module;
        }

        private static bool ShouldMapBody(MethodBase method, MethodDefinition methodDefinition)
        {
            return methodDefinition.HasBody && method.GetMethodBody() != null;
        }

        private void MapOverrides(MethodBase method, MethodDefinition methodDefinition)
        {
            var mi = method as MethodInfo;
            if (mi == null || !mi.IsVirtual)
                return;

            var type = method.DeclaringType;
            if (type == null || type.IsInterface)
                return;

            var overrides = type
                .GetInterfaces()
                .Select(type.GetInterfaceMap)
                .SelectMany(m =>
                    m.InterfaceMethods.Zip(m.TargetMethods, (im, tm) => new {InterfaceMethod = im, TargetMethod = tm}))
                .Where(p => p.TargetMethod.DeclaringType == type)
                .Where(p => p.InterfaceMethod.Name != p.TargetMethod.Name)
                .Where(p => p.TargetMethod == method)
                .Select(p => p.InterfaceMethod);

            foreach (var ov in overrides)
                methodDefinition.Overrides.Add(CreateReference(ov, methodDefinition).GetElementMethod());
        }

        private void MapField(TypeDefinition typeDefinition, FieldInfo field)
        {
            var fieldDefinition = FieldDefinitionFor(field, typeDefinition);

            if (fieldDefinition.HasDefault)
                fieldDefinition.Constant = field.GetRawConstantValue();

            if ((fieldDefinition.Attributes & FieldAttributes.HasFieldRVA) != 0)
                fieldDefinition.InitialValue = GetInitialValue(field);

            var attributes = field.GetCustomAttributes(typeof(FieldOffsetAttribute), false);
            if (attributes.Length > 0)
                fieldDefinition.Offset = ((FieldOffsetAttribute) attributes[0]).Value;

            MapCustomAttributes(field, fieldDefinition);
        }

        private static byte[] GetInitialValue(FieldInfo field)
        {
            if (!field.IsStatic)
                throw new NotSupportedException();

            var value = field.GetValue(null);
            if (value == null)
                return new byte [0];

            var type = value.GetType();
            if (!type.IsValueType || type.IsPrimitive)
                throw new NotSupportedException();

            return ToByteArray(value);
        }

        private static byte[] ToByteArray(object @struct)
        {
            var size = Marshal.SizeOf(@struct.GetType());
            var data = new byte [size];
            var ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(@struct, ptr, true);
            Marshal.Copy(ptr, data, 0, size);
            Marshal.FreeHGlobal(ptr);

            return data;
        }

        private void MapProperty(PropertyInfo property, PropertyDefinition propertyDefinition)
        {
            var typeDefinition = propertyDefinition.DeclaringType;

            var getter = property.GetGetMethod(true);
            if (getter != null)
            {
                propertyDefinition.GetMethod = typeDefinition.Methods.Single(m => m.Name == getter.Name);
                propertyDefinition.GetMethod.IsGetter = true;
            }

            var setter = property.GetSetMethod(true);
            if (setter != null)
            {
                propertyDefinition.SetMethod = typeDefinition.Methods.Single(m => m.Name == setter.Name);
                propertyDefinition.SetMethod.IsSetter = true;
            }

            MapCustomAttributes(property, propertyDefinition);
        }

        private PropertyDefinition PropertyDefinitionFor(PropertyInfo property, TypeDefinition declaringType)
        {
            var propertyDefinition = new PropertyDefinition(
                property.Name,
                (PropertyAttributes) property.Attributes,
                CreateReference(property.PropertyType, declaringType));

            declaringType.Properties.Add(propertyDefinition);

            return propertyDefinition;
        }

        private void MapEvent(EventInfo evt, EventDefinition eventDefinition)
        {
            var typeDefinition = eventDefinition.DeclaringType;

            var add = evt.GetAddMethod(true);
            if (add != null)
            {
                eventDefinition.AddMethod = typeDefinition.Methods.Single(m => m.Name == add.Name);
                eventDefinition.AddMethod.IsAddOn = true;
            }

            var remove = evt.GetRemoveMethod(true);
            if (remove != null)
            {
                eventDefinition.RemoveMethod = typeDefinition.Methods.Single(m => m.Name == remove.Name);
                eventDefinition.RemoveMethod.IsRemoveOn = true;
            }

            var raise = evt.GetRaiseMethod(true);
            if (raise != null)
            {
                eventDefinition.InvokeMethod = typeDefinition.Methods.Single(m => m.Name == raise.Name);
                eventDefinition.InvokeMethod.IsFire = true;
            }

            MapCustomAttributes(evt, eventDefinition);
        }

        private EventDefinition EventDefinitionFor(EventInfo evt, TypeDefinition declaringType)
        {
            var eventDefinition = new EventDefinition(
                evt.Name,
                (EventAttributes) evt.Attributes,
                CreateReference(evt.EventHandlerType, declaringType));

            declaringType.Events.Add(eventDefinition);

            return eventDefinition;
        }

        private void MapMethodBody(MethodBase method, MethodDefinition methodDefinition)
        {
            MapVariables(method, methodDefinition);
            MapInstructions(method, methodDefinition);
            MapExceptions(method, methodDefinition);
        }

        private void MapInstructions(MethodBase method, MethodDefinition methodDefinition)
        {
            var instructions = method.GetInstructions();

            foreach (var instruction in instructions)
            {
                var il = methodDefinition.Body.GetILProcessor();

                var op = OpCodeFor(instruction);

                switch (op.OperandType)
                {
                    case OperandType.InlineNone:
                        il.Emit(op);
                        break;
                    case OperandType.InlineMethod:
                        il.Emit(op, CreateReference((MethodBase) instruction.Operand, methodDefinition));
                        break;
                    case OperandType.InlineField:
                        il.Emit(op, CreateReference((FieldInfo) instruction.Operand, methodDefinition));
                        break;
                    case OperandType.InlineType:
                        il.Emit(op, CreateReference((Type) instruction.Operand, methodDefinition));
                        break;
                    case OperandType.InlineTok:
                        var member = (MemberInfo) instruction.Operand;
                        if (member is Type)
                            il.Emit(op, CreateReference((Type) instruction.Operand, methodDefinition));
                        else if (member is FieldInfo)
                            il.Emit(op, CreateReference((FieldInfo) instruction.Operand, methodDefinition));
                        else if (member is MethodBase)
                            il.Emit(op, CreateReference((MethodBase) instruction.Operand, methodDefinition));
                        else
                            throw new NotSupportedException();
                        break;
                    case OperandType.ShortInlineI:
                        if (op.Code == Code.Ldc_I4_S)
                            il.Emit(op, (sbyte) instruction.Operand);
                        else
                            il.Emit(op, (byte) instruction.Operand);
                        break;
                    case OperandType.InlineI:
                        il.Emit(op, (int) instruction.Operand);
                        break;
                    case OperandType.InlineI8:
                        il.Emit(op, (long) instruction.Operand);
                        break;
                    case OperandType.ShortInlineR:
                        il.Emit(op, (float) instruction.Operand);
                        break;
                    case OperandType.InlineR:
                        il.Emit(op, (double) instruction.Operand);
                        break;
                    case OperandType.ShortInlineVar:
                    case OperandType.InlineVar:
                        il.Emit(op, VariableFor(instruction, methodDefinition));
                        break;
                    case OperandType.ShortInlineArg:
                    case OperandType.InlineArg:
                        il.Emit(op, ParameterFor(instruction, methodDefinition));
                        break;
                    case OperandType.InlineString:
                        il.Emit(op, (string) instruction.Operand);
                        break;
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.InlineBrTarget:
                        il.Emit(op, Mono.Cecil.Cil.Instruction.Create(OpCodes.Nop));
                        break;
                    case OperandType.InlineSwitch:
                        il.Emit(op, new[] {Mono.Cecil.Cil.Instruction.Create(OpCodes.Nop)});
                        break;
                    case OperandType.InlineSig:
                        il.Emit(op,
                            MapCallSite((byte[]) instruction.Operand, method.Module,
                                method.DeclaringType.GetGenericArguments(), method.GetGenericArguments()));
                        break;
                    default:
                        throw new NotSupportedException(op.OperandType.ToString());
                }
            }

            foreach (var instruction in instructions)
            {
                var op = OpCodeFor(instruction);

                switch (op.OperandType)
                {
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.InlineBrTarget:
                        var br = OffsetToInstruction(instruction.Offset, instructions, methodDefinition);
                        var target = (Instruction) instruction.Operand;
                        if (target != null)
                            br.Operand = OffsetToInstruction(target.Offset, instructions, methodDefinition);

                        break;

                    case OperandType.InlineSwitch:
                        var @switch = OffsetToInstruction(instruction.Offset, instructions, methodDefinition);
                        @switch.Operand = ((Instruction[]) instruction.Operand)
                            .Select(i => OffsetToInstruction(i.Offset, instructions, methodDefinition)).ToArray();
                        break;
                }
            }
        }

        private unsafe CallSite MapCallSite(byte[] sig, Module module, Type[] typeGenArgs, Type[] methGenArgs)
        {
            var b = new ByteBuffer(sig);
            var convention = (MethodCallingConvention) b.ReadByte();
            var parameters = new Type[b.ReadCompressedUInt32()];
            var opIndex = -1;
            var isOptional = false;
            var returnType = ReadType(b, module, ref isOptional, typeGenArgs, methGenArgs);
            for (var i = 0; i < parameters.Length; i++)
            {
                parameters[i] = ReadType(b, module, ref isOptional, typeGenArgs, methGenArgs);
                if (isOptional && opIndex == -1)
                {
                    opIndex = i;
                }
            }

            var ret = new CallSite(CreateReference(returnType));
            for (var i = 0; i < parameters.Length; i++)
            {
                var t = parameters[i];
                var tr = CreateReference(t);
                if (i >= opIndex)
                {
                    tr = tr.MakeSentinelType();
                }

                ret.Parameters.Add(new ParameterDefinition(tr));
            }

            ret.CallingConvention = convention;
            return ret;
        }

        private unsafe Type ReadType(ByteBuffer b, Module module, ref bool isOptional, Type[] typeGenArgs,
            Type[] methodGenArgs)
        {
            var elementType = (ElementType) b.ReadByte();

            if (elementType == ElementType.Sentinel)
            {
                elementType = (ElementType) b.ReadByte();
                isOptional = true;
            }

            var ptrLevels = 0;
            var byRef = false;
            while (elementType == ElementType.Ptr || elementType == ElementType.ByRef)
            {
                if (elementType == ElementType.Ptr)
                {
                    ptrLevels += 1;
                }
                else if (elementType == ElementType.ByRef)
                {
                    byRef = true;
                }

                elementType = (ElementType) b.ReadByte();
            }

            if (!DecodeToken(elementType, out Type type))
                return elementType switch
                {
                    ElementType.Void => typeof(void),
                    ElementType.Boolean => typeof(bool),
                    ElementType.Char => typeof(char),
                    ElementType.SByte => typeof(sbyte),
                    ElementType.Byte => typeof(byte),
                    ElementType.Int16 => typeof(short),
                    ElementType.UInt16 => typeof(ushort),
                    ElementType.UInt32 => typeof(uint),
                    ElementType.Int32 => typeof(int),
                    ElementType.Int64 => typeof(long),
                    ElementType.UInt64 => typeof(ulong),
                    ElementType.Single => typeof(float),
                    ElementType.Double => typeof(double),
                    ElementType.String => typeof(string),
                    ElementType.TypedReference => typeof(TypedReference),
                    ElementType.IntPtr => typeof(IntPtr),
                    ElementType.UIntPtr => typeof(UIntPtr),
                    ElementType.Object => typeof(object),
                    _ => throw new InvalidOperationException(
                        $"{BitConverter.ToString(new[] {(byte) elementType})} is not a valid element type" +
                        "at this point.")
                };

            for (var i = 0; i < ptrLevels; i++)
            {
                type = type.MakePointerType();
            }

            if (byRef)
            {
                type = type.MakeByRefType();
            }

            return type;

            bool DecodeToken(ElementType e, out Type t)
            {
                if (e == ElementType.Class || e == ElementType.ValueType)
                {
                    try
                    {
                        t = module.ResolveType(b.ReadCompressedInt32() >> 2, typeGenArgs, methodGenArgs);
                        if (t != null)
                        {
                            return true;
                        }
                    }
                    catch
                    {
                        // do nothing
                    }
                }

                t = null;
                return false;
            }
        }

        private void MapVariables(MethodBase method, MethodDefinition methodDefinition)
        {
            var body = method.GetMethodBody();
            if (body == null)
                return;

            foreach (var variable in body.LocalVariables)
            {
                var variableType = CreateReference(variable.LocalType, methodDefinition);
                methodDefinition.Body.Variables.Add(
                    new VariableDefinition(variable.IsPinned ? new PinnedType(variableType) : variableType));
            }

            methodDefinition.Body.InitLocals = body.InitLocals;
        }

        private void MapExceptions(MethodBase method, MethodDefinition methodDefinition)
        {
            var body = method.GetMethodBody();
            if (body == null)
                return;

            var instructions = method.GetInstructions();

            foreach (var clause in body.ExceptionHandlingClauses)
            {
                var handler = new ExceptionHandler((ExceptionHandlerType) clause.Flags)
                {
                    TryStart = OffsetToInstruction(clause.TryOffset, instructions, methodDefinition),
                    TryEnd = OffsetToInstruction(clause.TryOffset + clause.TryLength, instructions, methodDefinition),
                    HandlerStart = OffsetToInstruction(clause.HandlerOffset, instructions, methodDefinition),
                    HandlerEnd = OffsetToInstruction(clause.HandlerOffset + clause.HandlerLength, instructions,
                        methodDefinition)
                };

                switch (handler.HandlerType)
                {
                    case ExceptionHandlerType.Catch:
                        handler.CatchType = CreateReference(clause.CatchType, methodDefinition);
                        break;
                    case ExceptionHandlerType.Filter:
                        handler.FilterStart = OffsetToInstruction(clause.FilterOffset, instructions, methodDefinition);
                        break;
                }

                methodDefinition.Body.ExceptionHandlers.Add(handler);
            }
        }

        private static Mono.Cecil.Cil.Instruction OffsetToInstruction(int offset, IList<Instruction> instructions,
            MethodDefinition methodDefinition)
        {
            var instruction = instructions.FirstOrDefault(i => i.Offset == offset);
            if (instruction == null)
                return null;

            return methodDefinition.Body.Instructions[instructions.IndexOf(instruction)];
        }

        private static AssemblyDefinition AssemblyDefinitionFor(Assembly assembly)
        {
            var name = assembly.GetName();

            var assemblyDefinition = AssemblyDefinition.CreateAssembly(
                new AssemblyNameDefinition(name.Name, name.Version),
                assembly.ManifestModule.Name, ModuleKind.Dll);

            assemblyDefinition.MainModule.Runtime = TargetRuntime.Net_4_0;
            return assemblyDefinition;
        }

        private MethodDefinition MethodDefinitionFor(MethodBase method, TypeDefinition declaringType)
        {
            var methodDefinition = new MethodDefinition(
                method.Name,
                (MethodAttributes) method.Attributes,
                _moduleDefinition.TypeSystem.Void);

            methodDefinition.ImplAttributes = (MethodImplAttributes) (int) method.GetMethodImplementationFlags();

            declaringType.Methods.Add(methodDefinition);

            var methodInfo = method as MethodInfo;

            if (methodInfo != null)
            {
                var genericParameters = methodInfo.GetGenericArguments();
                for (var i = 0; i < genericParameters.Length; i++)
                    methodDefinition.GenericParameters.Add(GenericParameterFor(genericParameters[i],
                        methodDefinition));

                for (var i = 0; i < genericParameters.Length; i++)
                    MapGenericParameterConstraints(genericParameters[i], methodDefinition.GenericParameters[i],
                        methodDefinition);
            }

            foreach (var parameter in method.GetParameters())
                MapParameter(methodDefinition, parameter);

            if (methodInfo != null)
                methodDefinition.ReturnType = CreateReference(methodInfo.ReturnType, methodDefinition);

            return methodDefinition;
        }

        private void MapParameter(MethodDefinition methodDefinition, ParameterInfo parameter)
        {
            var parameterDefinition = new ParameterDefinition(
                parameter.Name,
                (ParameterAttributes) parameter.Attributes,
                CreateReference(parameter.ParameterType, methodDefinition));

            MapCustomAttributes(parameter, parameterDefinition);

            methodDefinition.Parameters.Add(parameterDefinition);
        }

        private FieldDefinition FieldDefinitionFor(FieldInfo field, TypeDefinition declaringType)
        {
            var fieldDefinition = new FieldDefinition(
                field.Name,
                (FieldAttributes) field.Attributes,
                CreateReference(field.FieldType, declaringType));

            declaringType.Fields.Add(fieldDefinition);

            return fieldDefinition;
        }

        private TypeDefinition TypeDefinitionFor(Type type, TypeDefinition declaringType)
        {
            var typeDefinition = new TypeDefinition(
                type.IsNested ? "" : type.Namespace,
                type.Name,
                (TypeAttributes) type.Attributes,
                _assemblyDefinition.MainModule.TypeSystem.Object);

            if (declaringType == null)
                _assemblyDefinition.MainModule.Types.Add(typeDefinition);
            else
                declaringType.NestedTypes.Add(typeDefinition);

            var genericParameters = type.GetGenericArguments();
            for (var i = 0; i < genericParameters.Length; i++)
                typeDefinition.GenericParameters.Add(GenericParameterFor(genericParameters[i], typeDefinition));

            for (var i = 0; i < genericParameters.Length; i++)
                MapGenericParameterConstraints(genericParameters[i], typeDefinition.GenericParameters[i],
                    typeDefinition);

            typeDefinition.BaseType = type.BaseType != null
                ? CreateReference(type.BaseType, typeDefinition)
                : null;

            var layout = type.StructLayoutAttribute;

            if (layout != null && layout.Value != LayoutKind.Auto)
            {
                typeDefinition.PackingSize = (short) layout.Pack;
                typeDefinition.ClassSize = layout.Size;
            }

            return typeDefinition;
        }

        private static GenericParameter GenericParameterFor(Type genericParameter, IGenericParameterProvider owner)
        {
            return new GenericParameter(genericParameter.Name, owner)
            {
                Attributes = (GenericParameterAttributes) (int) genericParameter.GenericParameterAttributes
            };
        }

        private void MapGenericParameterConstraints(Type genericParameter, GenericParameter gp,
            IGenericParameterProvider owner)
        {
            foreach (var constraint in genericParameter.GetGenericParameterConstraints())
                gp.Constraints.Add(new GenericParameterConstraint(
                    owner.GenericParameterType == GenericParameterType.Type
                        ? CreateReference(constraint, (TypeReference) owner)
                        : CreateReference(constraint, (MethodReference) owner)));
        }

        private static ParameterDefinition ParameterFor(Instruction instruction, MethodDefinition method)
        {
            var parameter = (ParameterInfo) instruction.Operand;
            return method.Parameters[parameter.Position];
        }

        private static VariableDefinition VariableFor(Instruction instruction, MethodDefinition method)
        {
            var local = (LocalVariableInfo) instruction.Operand;
            return method.Body.Variables[local.LocalIndex];
        }

        private static OpCode OpCodeFor(Instruction instruction)
        {
            foreach (var opcode in Opcodes)
                if (opcode.Value == instruction.OpCode.Value)
                    return opcode;

            throw new NotSupportedException("OpCode not found: " + instruction.OpCode.Name);
        }

        private static AssemblyName _nsName = Assembly.Load("netstandard").GetName();
        private static AssemblyNameReference _nsNameCecil = new AssemblyNameReference(_nsName.Name, _nsName.Version);
        private bool _nsAdded = false;
#if NETSTANDARD2_1
        private bool IsNs(string name) => Private.NSMap.NSMap.IsNetStandard(name);
#else
        private bool IsNs(string name) => false;
#endif

        private Type GetElementTypeAbsolute(Type type)
        {
            var ret = type;
            if (ret.IsByRef)
            {
                ret = ret.GetElementType();
            }

            while (ret.IsArray)
            {
                ret = ret.GetElementType();
            }

            while (ret.IsPointer)
            {
                ret = ret.GetElementType();
            }

            return ret;
        }
        private TypeReference CreateReference(Type type)
        {
            return MapReference(_moduleDefinition.ImportReference(type));
        }

        private TypeReference CreateReference(Type type, TypeReference context)
        {
            return MapReference(_moduleDefinition.ImportReference(type, context));
        }

        private TypeReference CreateReference(Type type, MethodReference context)
        {
            return MapReference(_moduleDefinition.ImportReference(type, context));
        }

        private FieldReference CreateReference(FieldInfo field, MethodReference context)
        {
            var reference = _moduleDefinition.ImportReference(field, context);
            MapReference(reference.DeclaringType);
            MapReference(reference.FieldType);
            return reference;
        }

        private MethodReference CreateReference(MethodBase method, MethodReference context)
        {
            var reference = _moduleDefinition.ImportReference(method, context);
            MapReference(reference.GetElementMethod().DeclaringType);
            MapReference(reference.ReturnType);
            MapGenericArguments(reference);

            foreach (var parameter in reference.Parameters)
                MapReference(parameter.ParameterType);

            return reference;
        }

        private void MapGenericArguments(MemberReference reference)
        {
            var instance = reference as IGenericInstance;
            if (instance == null)
                return;

            foreach (var arg in instance.GenericArguments)
                MapReference(arg);
        }

        private TypeReference MapReference(TypeReference type)
        {
            if (type.IsGenericParameter)
                return type;

            if (type.IsPointer || type.IsByReference || type.IsPinned || type.IsArray)
            {
                MapElementType(type);
                return type;
            }

            if (type.IsGenericInstance)
            {
                MapGenericArguments(type);
                MapElementType(type);
                return type;
            }

            if (type.Scope.MetadataScopeType != MetadataScopeType.AssemblyNameReference)
                return type;

            var reference = (AssemblyNameReference) type.Scope;
            if (reference.FullName != _assemblyDefinition.FullName)
            {
                if (IsNs(GetReflectionName(type)))
                {
                    if (!_nsAdded)
                    {
                        _nsAdded = true;
                        _moduleDefinition.AssemblyReferences.Add(_nsNameCecil);
                    }

                    type.Scope = _nsNameCecil;
                    return type;
                }

                return type;
            }

            type.GetElementType().Scope = _moduleDefinition;
            _moduleDefinition.AssemblyReferences.Remove(reference);
            return type;
        }
        
        private static string GetReflectionName(TypeReference type)
        {
            if (type.IsGenericInstance)
            {
                var genericInstance = (GenericInstanceType)type;
                return
                    $"{genericInstance.Namespace}.{type.Name}[{string.Join(",", genericInstance.GenericArguments.Select(p => GetReflectionName(p)).ToArray())}]";
            }
            return type.FullName;
        }
        

        private void MapElementType(TypeReference type)
        {
            MapReference(((TypeSpecification) type).ElementType);
        }

        private void MapCustomAttributes(ICustomAttributeProvider provider,
            Mono.Cecil.ICustomAttributeProvider targetProvider)
        {
            var method = provider.GetType().GetMethod("GetCustomAttributesData");
            if (method == null)
                throw new NotSupportedException("No method GetCustomAttributesData for type " +
                                                provider.GetType().FullName);

            var customAttributesData = (IList<CustomAttributeData>) method.Invoke(provider, new object[0]);

            foreach (var customAttributeData in customAttributesData)
            {
                var customAttribute = new CustomAttribute(CreateReference(customAttributeData.Constructor, null));

                foreach (var argument in customAttributeData.ConstructorArguments)
                    customAttribute.ConstructorArguments.Add(CustomAttributeArgumentFor(argument));

                foreach (var namedArgument in customAttributeData.NamedArguments)
                {
                    var argument = new CustomAttributeNamedArgument(namedArgument.MemberInfo.Name,
                        CustomAttributeArgumentFor(namedArgument.TypedValue));
                    if (namedArgument.MemberInfo is PropertyInfo)
                        customAttribute.Properties.Add(argument);
                    else if (namedArgument.MemberInfo is FieldInfo)
                        customAttribute.Fields.Add(argument);
                    else
                        throw new NotSupportedException();
                }

                targetProvider.CustomAttributes.Add(customAttribute);
            }
        }

        private CustomAttributeArgument CustomAttributeArgumentFor(CustomAttributeTypedArgument argument)
        {
            return new CustomAttributeArgument(
                CreateReference(argument.ArgumentType),
                MapCustomAttributeValue(argument));
        }

        private object MapCustomAttributeValue(CustomAttributeTypedArgument argument)
        {
            var value = argument.Value;
            var type = value as Type;
            if (type != null)
                return CreateReference(type);

            return value;
        }
    }
}