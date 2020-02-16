using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Ultz.SuperInvoke.Emit;

namespace Ultz.SuperInvoke.InteropServices
{
    public class Marshaller : Generator
    {
        private static readonly Generator BaseGenerator = new Generator();

        public static IMarshaller[] DefaultStages { get; } =
        {
            new PinObjectMarshaller(), // at the top so that the object is pinned when it reaches everything else
            new DelegateMarshaller(), // upper to get rid of delegates early.
            new StringMarshaller(),
            new RefMarshaller(),
            new BoolMarshaller(),
            new MergeMarshaller(),
            new SpanParameterMarshaller(),
        };

        public IMarshaller[] Stages { get; set; } = DefaultStages;
        public override void GenerateMethod(in MethodGenerationContext ctx) => Marshal(ctx);

        private void Marshal(MethodGenerationContext ctx)
        {
            // Snippet from Dylan's notepad (omg look at this dude still using a paper notepad in 2019)
            //
            // Main
            // |------> 0 --|      i.e. Generate the next wrapper when the previous wrapper
            //      |-- 1 <-|           needs to emit its native call.
            //      |-> 2 --|
            //      |-- 3 <-|
            //      |------------> Native

            // Get the first marshalling stage
            var iteration = 0;
            var typeBuilder = (TypeBuilder) ctx.DestinationMethod.DeclaringType;
            var firstStage = GetNextStage(new ParameterMarshalContext(ctx.OriginalMethod.ReturnParameter),
                ctx.OriginalMethod.GetParameters().Select(x => new ParameterMarshalContext(x)).ToArray(), -1, Stages,
                out var index);

            // If we don't have a first stage, we have no marshalling to do.
            // Just pass it to the regular generator :D
            if (firstStage is null)
            {
                BaseGenerator.GenerateMethod(ctx);
                return;
            }

            // Define the first marshalling wrapper
            var entry = typeBuilder.DefineMethod(
                GetName(firstStage, ctx.OriginalMethod.Name, iteration++),
                MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig,
                ctx.OriginalMethod.CallingConvention, ctx.OriginalMethod.ReturnType,
                ctx.OriginalMethod.ReturnParameter.GetRequiredCustomModifiers(),
                ctx.OriginalMethod.ReturnParameter.GetOptionalCustomModifiers(),
                ctx.OriginalMethod.GetParameters().Select(x => x.ParameterType).ToArray(),
                ctx.OriginalMethod.GetParameters().Select(x => x.GetRequiredCustomModifiers()).ToArray(),
                ctx.OriginalMethod.GetParameters().Select(x => x.GetOptionalCustomModifiers()).ToArray());
            MarshalUtils.CopyGenericTypes(ctx.OriginalMethod, entry);
            entry.SetImplementationFlags(MethodImplAttributes.AggressiveInlining | (MethodImplAttributes) 512);
            ctx.DestinationMethod.SetImplementationFlags(MethodImplAttributes.AggressiveInlining |
                                                         (MethodImplAttributes) 512);

            // Generate the marshalling wrappers (the first one will call EmitCall, which will then gen the next, etc.)
            firstStage.Marshal(new MethodMarshalContext(ctx.DestinationMethod, ctx.Slot, entry,
                new ParameterMarshalContext(ctx.OriginalMethod.ReturnParameter),
                ctx.OriginalMethod.GetParameters().Select(x => new ParameterMarshalContext(x)).ToArray(), EmitCall
            ));

            // Emit the actual method using the default generator
            ctx.IL.Emit(OpCodes.Ldarg_0);
            BaseGenerator.EmitPrologue(ctx); // pass all parameters as-is
            ctx.IL.Emit(OpCodes.Call, entry); // call the first marshalling wrapper
            BaseGenerator.EmitEpilogue(ctx); // call the default epilogue generator
            BaseGenerator.EmitReturn(ctx); // call the default return generator

            // A recursive local function for handling generating & calling the next wrapper from the previous wrapper.
            void EmitCall(MethodBuilder wip, ParameterMarshalContext ret, ParameterMarshalContext[] parameters,
                ILGenerator il)
            {
                var nextStage = GetNextStage(ret, parameters, index, Stages, out index);

                // If no more stages are available, emit the native call and go home.
                if (nextStage is null)
                {
                    NativeReturnType = ret.Type;
                    NativeParameterTypes = parameters.Select(x => x.Type).ToArray();
                    var terminator = typeBuilder.DefineMethod(
                        GetName(null, ctx.OriginalMethod.Name, iteration++),
                        MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig,
                        wip.CallingConvention, ret.Type, ret.RequiredModifiers, ret.OptionalModifiers,
                        parameters.Select(x => x.Type).ToArray(),
                        parameters.Select(x => x.RequiredModifiers).ToArray(),
                        parameters.Select(x => x.OptionalModifiers).ToArray());
                    var til = terminator.GetILGenerator();
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        til.Emit(OpCodes.Ldarg, i + 1);
                    }
                    
                    BaseGenerator.EmitEntryPoint(til, ctx.Slot, ctx.EntryPoint);
                    BaseGenerator.EmitNativeCall(til, ctx.Convention, ctx.OriginalMethod, ret.Type,
                        parameters.Select(x => x.Type).ToArray());
                    til.Emit(OpCodes.Ret);
                    terminator.SetImplementationFlags(MethodImplAttributes.AggressiveInlining | (MethodImplAttributes)512);
                    il.Emit(OpCodes.Call, terminator);
                }
                else
                {
                    var call = typeBuilder.DefineMethod(
                        GetName(nextStage, ctx.OriginalMethod.Name, iteration++),
                        MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig,
                        wip.CallingConvention, ret.Type, ret.RequiredModifiers, ret.OptionalModifiers,
                        parameters.Select(x => x.Type).ToArray(),
                        parameters.Select(x => x.RequiredModifiers).ToArray(),
                        parameters.Select(x => x.OptionalModifiers).ToArray());
                    MarshalUtils.CopyGenericTypes(ctx.OriginalMethod, call);
                    call = nextStage.Marshal(new MethodMarshalContext(wip, ctx.Slot, call, ret, parameters,
                        EmitCall));
                    call.SetImplementationFlags(MethodImplAttributes.AggressiveInlining | (MethodImplAttributes)512);
                    il.Emit(OpCodes.Call, call);
                }
            }
        }

        private static string GetName(IMarshaller marshaller, string ogName, int iteration) =>
            $"{ogName}_{marshaller?.GetType().Name ?? "Terminator"}_{iteration}";

        private static IMarshaller GetNextStage(in ParameterMarshalContext ret, ParameterMarshalContext[] parameters,
            int current, IReadOnlyList<IMarshaller> stages,
            out int index)
        {
            for (var i = current + 1; i < stages.Count; i++)
            {
                var stage = stages[i];
                if (stage.CanMarshal(ret, parameters))
                {
                    index = i;
                    return stage;
                }
            }

            index = stages.Count - 1;
            return null;
        }
    }
}