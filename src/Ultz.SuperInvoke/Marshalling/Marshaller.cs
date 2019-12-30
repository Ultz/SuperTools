using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Ultz.SuperInvoke.Emit;

namespace Ultz.SuperInvoke.Marshalling
{
    public class Marshaller : Generator
    {
        private static readonly Generator BaseGenerator = new Generator();
        public IMarshaller[] Stages { get; set; }
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
            var firstStage = GetNextStage(ctx.OriginalMethod, -1, Stages, out var index);

            // If we don't have a first stage, we have no marshalling to do.
            // Just pass it to the regular generator :D
            if (firstStage is null)
            {
                BaseGenerator.GenerateMethod(ctx);
                return;
            }

            // Generate all methods, keeping track of the first diverged method.
            MethodBuilder entry;
            firstStage.Marshal(new MethodMarshalContext(ctx.DestinationMethod,
                entry = typeBuilder.DefineMethod(
                    ctx.OriginalMethod.Name,
                    MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig,
                    ctx.OriginalMethod.CallingConvention, ctx.OriginalMethod.ReturnType,
                    ctx.OriginalMethod.ReturnParameter?.GetRequiredCustomModifiers(),
                    ctx.OriginalMethod.ReturnParameter?.GetOptionalCustomModifiers(),
                    ctx.OriginalMethod.GetParameters().Select(x => x.ParameterType).ToArray(),
                    ctx.OriginalMethod.GetParameters().Select(x => x.GetRequiredCustomModifiers()).ToArray(),
                    ctx.OriginalMethod.GetParameters().Select(x => x.GetOptionalCustomModifiers()).ToArray()),
                EmitCall));

            // Emit the actual method using the default generator
            BaseGenerator.EmitPrologue(ctx); // pass all parameters as-is
            ctx.IL.Emit(OpCodes.Call, entry); // call the first marshalling wrapper
            BaseGenerator.EmitEpilogue(ctx); // call the default epilogue generator
            BaseGenerator.EmitReturn(ctx); // call the default return generator

            // A recursive local function for handling generating & calling the next wrapper from the previous wrapper.
            void EmitCall(MethodInfo wip, Type returnType, Type[] paramTypes, Type[] returnTypeReqModifiers,
                Type[] returnTypeOptModifiers, Type[][] requiredCustomModifiers,
                Type[][] optionalCustomModifiers, CustomAttributeBuilder[] rcas, CustomAttributeBuilder[][] pcas,
                ILGenerator il)
            {
                var nextStage = GetNextStage(wip, index, Stages, out index);

                // If no more stages are available, emit the native call and go home.
                if (nextStage is null)
                {
                    BaseGenerator.EmitEntryPoint(il, ctx.Slot, ctx.EntryPoint);
                    BaseGenerator.EmitNativeCall(il, ctx.Convention, wip, returnType, paramTypes);
                }
                else
                {
                    var call = nextStage.Marshal(new MethodMarshalContext(wip, typeBuilder.DefineMethod(
                        GetName(nextStage, ctx.OriginalMethod.Name, iteration++),
                        MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig,
                        wip.CallingConvention, returnType, returnTypeReqModifiers, returnTypeOptModifiers,
                        paramTypes, requiredCustomModifiers, optionalCustomModifiers), EmitCall));
                    il.Emit(OpCodes.Call, call);
                }
            }
        }

        private static string GetName(IMarshaller marshaller, string ogName, int iteration) =>
            $"{ogName}_{marshaller.GetType().Name}_{iteration}";

        private static IMarshaller GetNextStage(MethodInfo wip, int current, IReadOnlyList<IMarshaller> stages,
            out int index)
        {
            for (var i = current + 1; i < stages.Count; i++)
            {
                var stage = stages[i];
                if (stage.CanMarshal(wip))
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