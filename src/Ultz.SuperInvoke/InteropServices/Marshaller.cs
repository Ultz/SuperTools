using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Ultz.SuperInvoke.Emit;

namespace Ultz.SuperInvoke.InteropServices
{
    public class Marshaller : Generator
    {
        private static readonly Generator BaseGenerator = new Generator();

        public static IMarshaller[] DefaultStages { get; } =
        {
            new PinObjectMarshaller(),
            new BoolMarshaller()
        };

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
                ctx.OriginalMethod.GetParameters().Select(x => x.ParameterType).ToArray());

            // Generate the marshalling wrappers (the first one will call EmitCall, which will then gen the next, etc.)
            firstStage.Marshal(new MethodMarshalContext(ctx.DestinationMethod, ctx.Slot, entry,
                CreateMarshalContext(ctx.OriginalMethod.ReturnParameter),
                ctx.OriginalMethod.GetParameters().Select(CreateMarshalContext).ToArray(), EmitCall
            ));

            // Emit the actual method using the default generator
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
                    BaseGenerator.EmitEntryPoint(il, ctx.Slot, ctx.EntryPoint);
                    BaseGenerator.EmitNativeCall(il, ctx.Convention, wip, ret.Type,
                        parameters.Select(x => x.Type).ToArray());
                }
                else
                {
                    var call = nextStage.Marshal(new MethodMarshalContext(wip, ctx.Slot, typeBuilder.DefineMethod(
                            GetName(nextStage, ctx.OriginalMethod.Name, iteration++),
                            MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig,
                            wip.CallingConvention, ret.Type, parameters.Select(x => x.Type).ToArray()), ret, parameters,
                        EmitCall));
                    il.Emit(OpCodes.Call, call);
                }
            }

            ParameterMarshalContext CreateMarshalContext(ParameterInfo inf) => new ParameterMarshalContext(
                inf.ParameterType,
                inf.CloneAttributes(),
                inf.GetCustomAttributesData().ToArray());
        }

        private static string GetName(IMarshaller marshaller, string ogName, int iteration) =>
            $"{ogName}_{marshaller.GetType().Name}_{iteration}";

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