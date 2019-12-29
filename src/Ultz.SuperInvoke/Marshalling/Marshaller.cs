using System;
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
            var typeBuilder = (TypeBuilder)ctx.DestinationMethod.DeclaringType;
            var firstStage = GetNextStage(ctx.OriginalMethod, -1, Stages, out var index);

            // If we don't have a first stage, we have no marshalling to do.
            // Just pass it to the regular generator :D
            if (firstStage is null)
            {
                BaseGenerator.GenerateMethod(ctx);
                return;
            }

            // Generate all methods, keeping track of the first diverged method.
            var entry = firstStage.Marshal(new MethodMarshalContext(ctx.OriginalMethod,
                GetName(firstStage, ctx.OriginalMethod.Name, iteration++), typeBuilder, EmitCall));

            BaseGenerator.EmitPrologue(ctx);
            ctx.IL.Emit(OpCodes.Call, entry);
            BaseGenerator.EmitEpilogue(ctx);
            BaseGenerator.EmitReturn(ctx);

            void EmitCall(MethodInfo wip, Type returnType, Type[] paramTypes, ILGenerator il)
            {
                var nextStage = GetNextStage(wip, index, Stages, out index);
                if (nextStage is null)
                {
                    BaseGenerator.EmitEntryPoint(il, ctx.Slot, ctx.EntryPoint);
                    BaseGenerator.EmitNativeCall(il, ctx.Convention, wip, returnType, paramTypes);
                }
                else
                {
                    il.Emit(OpCodes.Call,
                        nextStage.Marshal(new MethodMarshalContext(wip,
                            GetName(nextStage, ctx.OriginalMethod.Name, iteration++), typeBuilder, EmitCall)));
                }
            }
        }

        private static string GetName(IMarshaller marshaller, string ogName, int iteration) =>
            $"{ogName}_{marshaller.GetType().Name}_{iteration}";

        private static IMarshaller GetNextStage(MethodInfo wip, int current, IMarshaller[] stages, out int index)
        {
            for (var i = current + 1; i < stages.Length; i++)
            {
                var stage = stages[i];
                if (stage.CanMarshal(wip))
                {
                    index = i;
                    return stage;
                }
            }

            index = stages.Length - 1;
            return null;
        }
    }
}
