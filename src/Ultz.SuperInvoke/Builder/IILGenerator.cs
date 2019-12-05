﻿namespace Ultz.SuperInvoke.Builder
{
    public interface IILGenerator
    {
        void EmitPrologue(ILBuilder builder, PipelineMethod method);
        void EmitCall(ILBuilder builder, PipelineMethod method);
        void EmitEpilogue(ILBuilder builder, PipelineMethod method);
    }
}