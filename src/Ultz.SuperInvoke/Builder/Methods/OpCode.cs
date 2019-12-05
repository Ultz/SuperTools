using System;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using static Ultz.SuperInvoke.Builder.ControlFlowKind;
using static Ultz.SuperInvoke.Builder.InputBehaviorKind;
using static Ultz.SuperInvoke.Builder.OutputBehaviorKind;
using static System.Reflection.Emit.OperandType;

namespace Ultz.SuperInvoke.Builder
{
    public readonly struct OpCode : IEquatable<OpCode>
    {
        private OpCode(ILOpCode value, string name, InputBehaviorKind inputBehavior, OutputBehaviorKind outputBehavior,
            OperandType operandKind, int size, int encoding, ControlFlowKind controlFlow)
        {
            Value = value;
            Name = name;
            InputBehavior = inputBehavior;
            OutputBehavior = outputBehavior;
            OperandKind = operandKind;
            Size = size;
            Encoding = encoding;
            ControlFlow = controlFlow;
        }

        public static OpCode Nop { get; } = Create(ILOpCode.Nop);
        public static OpCode Break { get; } = Create(ILOpCode.Break);
        public static OpCode Ldarg_0 { get; } = Create(ILOpCode.Ldarg_0);
        public static OpCode Ldarg_1 { get; } = Create(ILOpCode.Ldarg_1);
        public static OpCode Ldarg_2 { get; } = Create(ILOpCode.Ldarg_2);
        public static OpCode Ldarg_3 { get; } = Create(ILOpCode.Ldarg_3);
        public static OpCode Ldloc_0 { get; } = Create(ILOpCode.Ldloc_0);
        public static OpCode Ldloc_1 { get; } = Create(ILOpCode.Ldloc_1);
        public static OpCode Ldloc_2 { get; } = Create(ILOpCode.Ldloc_2);
        public static OpCode Ldloc_3 { get; } = Create(ILOpCode.Ldloc_3);
        public static OpCode Stloc_0 { get; } = Create(ILOpCode.Stloc_0);
        public static OpCode Stloc_1 { get; } = Create(ILOpCode.Stloc_1);
        public static OpCode Stloc_2 { get; } = Create(ILOpCode.Stloc_2);
        public static OpCode Stloc_3 { get; } = Create(ILOpCode.Stloc_3);
        public static OpCode Ldarg_s { get; } = Create(ILOpCode.Ldarg_s);
        public static OpCode Ldarga_s { get; } = Create(ILOpCode.Ldarga_s);
        public static OpCode Starg_s { get; } = Create(ILOpCode.Starg_s);
        public static OpCode Ldloc_s { get; } = Create(ILOpCode.Ldloc_s);
        public static OpCode Ldloca_s { get; } = Create(ILOpCode.Ldloca_s);
        public static OpCode Stloc_s { get; } = Create(ILOpCode.Stloc_s);
        public static OpCode Ldnull { get; } = Create(ILOpCode.Ldnull);
        public static OpCode Ldc_i4_m1 { get; } = Create(ILOpCode.Ldc_i4_m1);
        public static OpCode Ldc_i4_0 { get; } = Create(ILOpCode.Ldc_i4_0);
        public static OpCode Ldc_i4_1 { get; } = Create(ILOpCode.Ldc_i4_1);
        public static OpCode Ldc_i4_2 { get; } = Create(ILOpCode.Ldc_i4_2);
        public static OpCode Ldc_i4_3 { get; } = Create(ILOpCode.Ldc_i4_3);
        public static OpCode Ldc_i4_4 { get; } = Create(ILOpCode.Ldc_i4_4);
        public static OpCode Ldc_i4_5 { get; } = Create(ILOpCode.Ldc_i4_5);
        public static OpCode Ldc_i4_6 { get; } = Create(ILOpCode.Ldc_i4_6);
        public static OpCode Ldc_i4_7 { get; } = Create(ILOpCode.Ldc_i4_7);
        public static OpCode Ldc_i4_8 { get; } = Create(ILOpCode.Ldc_i4_8);
        public static OpCode Ldc_i4_s { get; } = Create(ILOpCode.Ldc_i4_s);
        public static OpCode Ldc_i4 { get; } = Create(ILOpCode.Ldc_i4);
        public static OpCode Ldc_i8 { get; } = Create(ILOpCode.Ldc_i8);
        public static OpCode Ldc_r4 { get; } = Create(ILOpCode.Ldc_r4);
        public static OpCode Ldc_r8 { get; } = Create(ILOpCode.Ldc_r8);
        public static OpCode Dup { get; } = Create(ILOpCode.Dup);
        public static OpCode Pop { get; } = Create(ILOpCode.Pop);
        public static OpCode Jmp { get; } = Create(ILOpCode.Jmp);
        public static OpCode Call { get; } = Create(ILOpCode.Call);
        public static OpCode Calli { get; } = Create(ILOpCode.Calli);
        public static OpCode Ret { get; } = Create(ILOpCode.Ret);
        public static OpCode Br_s { get; } = Create(ILOpCode.Br_s);
        public static OpCode Brfalse_s { get; } = Create(ILOpCode.Brfalse_s);
        public static OpCode Brtrue_s { get; } = Create(ILOpCode.Brtrue_s);
        public static OpCode Beq_s { get; } = Create(ILOpCode.Beq_s);
        public static OpCode Bge_s { get; } = Create(ILOpCode.Bge_s);
        public static OpCode Bgt_s { get; } = Create(ILOpCode.Bgt_s);
        public static OpCode Ble_s { get; } = Create(ILOpCode.Ble_s);
        public static OpCode Blt_s { get; } = Create(ILOpCode.Blt_s);
        public static OpCode Bne_un_s { get; } = Create(ILOpCode.Bne_un_s);
        public static OpCode Bge_un_s { get; } = Create(ILOpCode.Bge_un_s);
        public static OpCode Bgt_un_s { get; } = Create(ILOpCode.Bgt_un_s);
        public static OpCode Ble_un_s { get; } = Create(ILOpCode.Ble_un_s);
        public static OpCode Blt_un_s { get; } = Create(ILOpCode.Blt_un_s);
        public static OpCode Br { get; } = Create(ILOpCode.Br);
        public static OpCode Brfalse { get; } = Create(ILOpCode.Brfalse);
        public static OpCode Brtrue { get; } = Create(ILOpCode.Brtrue);
        public static OpCode Beq { get; } = Create(ILOpCode.Beq);
        public static OpCode Bge { get; } = Create(ILOpCode.Bge);
        public static OpCode Bgt { get; } = Create(ILOpCode.Bgt);
        public static OpCode Ble { get; } = Create(ILOpCode.Ble);
        public static OpCode Blt { get; } = Create(ILOpCode.Blt);
        public static OpCode Bne_un { get; } = Create(ILOpCode.Bne_un);
        public static OpCode Bge_un { get; } = Create(ILOpCode.Bge_un);
        public static OpCode Bgt_un { get; } = Create(ILOpCode.Bgt_un);
        public static OpCode Ble_un { get; } = Create(ILOpCode.Ble_un);
        public static OpCode Blt_un { get; } = Create(ILOpCode.Blt_un);
        public static OpCode Switch { get; } = Create(ILOpCode.Switch);
        public static OpCode Ldind_i1 { get; } = Create(ILOpCode.Ldind_i1);
        public static OpCode Ldind_u1 { get; } = Create(ILOpCode.Ldind_u1);
        public static OpCode Ldind_i2 { get; } = Create(ILOpCode.Ldind_i2);
        public static OpCode Ldind_u2 { get; } = Create(ILOpCode.Ldind_u2);
        public static OpCode Ldind_i4 { get; } = Create(ILOpCode.Ldind_i4);
        public static OpCode Ldind_u4 { get; } = Create(ILOpCode.Ldind_u4);
        public static OpCode Ldind_i8 { get; } = Create(ILOpCode.Ldind_i8);
        public static OpCode Ldind_i { get; } = Create(ILOpCode.Ldind_i);
        public static OpCode Ldind_r4 { get; } = Create(ILOpCode.Ldind_r4);
        public static OpCode Ldind_r8 { get; } = Create(ILOpCode.Ldind_r8);
        public static OpCode Ldind_ref { get; } = Create(ILOpCode.Ldind_ref);
        public static OpCode Stind_ref { get; } = Create(ILOpCode.Stind_ref);
        public static OpCode Stind_i1 { get; } = Create(ILOpCode.Stind_i1);
        public static OpCode Stind_i2 { get; } = Create(ILOpCode.Stind_i2);
        public static OpCode Stind_i4 { get; } = Create(ILOpCode.Stind_i4);
        public static OpCode Stind_i8 { get; } = Create(ILOpCode.Stind_i8);
        public static OpCode Stind_r4 { get; } = Create(ILOpCode.Stind_r4);
        public static OpCode Stind_r8 { get; } = Create(ILOpCode.Stind_r8);
        public static OpCode Add { get; } = Create(ILOpCode.Add);
        public static OpCode Sub { get; } = Create(ILOpCode.Sub);
        public static OpCode Mul { get; } = Create(ILOpCode.Mul);
        public static OpCode Div { get; } = Create(ILOpCode.Div);
        public static OpCode Div_un { get; } = Create(ILOpCode.Div_un);
        public static OpCode Rem { get; } = Create(ILOpCode.Rem);
        public static OpCode Rem_un { get; } = Create(ILOpCode.Rem_un);
        public static OpCode And { get; } = Create(ILOpCode.And);
        public static OpCode Or { get; } = Create(ILOpCode.Or);
        public static OpCode Xor { get; } = Create(ILOpCode.Xor);
        public static OpCode Shl { get; } = Create(ILOpCode.Shl);
        public static OpCode Shr { get; } = Create(ILOpCode.Shr);
        public static OpCode Shr_un { get; } = Create(ILOpCode.Shr_un);
        public static OpCode Neg { get; } = Create(ILOpCode.Neg);
        public static OpCode Not { get; } = Create(ILOpCode.Not);
        public static OpCode Conv_i1 { get; } = Create(ILOpCode.Conv_i1);
        public static OpCode Conv_i2 { get; } = Create(ILOpCode.Conv_i2);
        public static OpCode Conv_i4 { get; } = Create(ILOpCode.Conv_i4);
        public static OpCode Conv_i8 { get; } = Create(ILOpCode.Conv_i8);
        public static OpCode Conv_r4 { get; } = Create(ILOpCode.Conv_r4);
        public static OpCode Conv_r8 { get; } = Create(ILOpCode.Conv_r8);
        public static OpCode Conv_u4 { get; } = Create(ILOpCode.Conv_u4);
        public static OpCode Conv_u8 { get; } = Create(ILOpCode.Conv_u8);
        public static OpCode Callvirt { get; } = Create(ILOpCode.Callvirt);
        public static OpCode Cpobj { get; } = Create(ILOpCode.Cpobj);
        public static OpCode Ldobj { get; } = Create(ILOpCode.Ldobj);
        public static OpCode Ldstr { get; } = Create(ILOpCode.Ldstr);
        public static OpCode Newobj { get; } = Create(ILOpCode.Newobj);
        public static OpCode Castclass { get; } = Create(ILOpCode.Castclass);
        public static OpCode Isinst { get; } = Create(ILOpCode.Isinst);
        public static OpCode Conv_r_un { get; } = Create(ILOpCode.Conv_r_un);
        public static OpCode Unbox { get; } = Create(ILOpCode.Unbox);
        public static OpCode Throw { get; } = Create(ILOpCode.Throw);
        public static OpCode Ldfld { get; } = Create(ILOpCode.Ldfld);
        public static OpCode Ldflda { get; } = Create(ILOpCode.Ldflda);
        public static OpCode Stfld { get; } = Create(ILOpCode.Stfld);
        public static OpCode Ldsfld { get; } = Create(ILOpCode.Ldsfld);
        public static OpCode Ldsflda { get; } = Create(ILOpCode.Ldsflda);
        public static OpCode Stsfld { get; } = Create(ILOpCode.Stsfld);
        public static OpCode Stobj { get; } = Create(ILOpCode.Stobj);
        public static OpCode Conv_ovf_i1_un { get; } = Create(ILOpCode.Conv_ovf_i1_un);
        public static OpCode Conv_ovf_i2_un { get; } = Create(ILOpCode.Conv_ovf_i2_un);
        public static OpCode Conv_ovf_i4_un { get; } = Create(ILOpCode.Conv_ovf_i4_un);
        public static OpCode Conv_ovf_i8_un { get; } = Create(ILOpCode.Conv_ovf_i8_un);
        public static OpCode Conv_ovf_u1_un { get; } = Create(ILOpCode.Conv_ovf_u1_un);
        public static OpCode Conv_ovf_u2_un { get; } = Create(ILOpCode.Conv_ovf_u2_un);
        public static OpCode Conv_ovf_u4_un { get; } = Create(ILOpCode.Conv_ovf_u4_un);
        public static OpCode Conv_ovf_u8_un { get; } = Create(ILOpCode.Conv_ovf_u8_un);
        public static OpCode Conv_ovf_i_un { get; } = Create(ILOpCode.Conv_ovf_i_un);
        public static OpCode Conv_ovf_u_un { get; } = Create(ILOpCode.Conv_ovf_u_un);
        public static OpCode Box { get; } = Create(ILOpCode.Box);
        public static OpCode Newarr { get; } = Create(ILOpCode.Newarr);
        public static OpCode Ldlen { get; } = Create(ILOpCode.Ldlen);
        public static OpCode Ldelema { get; } = Create(ILOpCode.Ldelema);
        public static OpCode Ldelem_i1 { get; } = Create(ILOpCode.Ldelem_i1);
        public static OpCode Ldelem_u1 { get; } = Create(ILOpCode.Ldelem_u1);
        public static OpCode Ldelem_i2 { get; } = Create(ILOpCode.Ldelem_i2);
        public static OpCode Ldelem_u2 { get; } = Create(ILOpCode.Ldelem_u2);
        public static OpCode Ldelem_i4 { get; } = Create(ILOpCode.Ldelem_i4);
        public static OpCode Ldelem_u4 { get; } = Create(ILOpCode.Ldelem_u4);
        public static OpCode Ldelem_i8 { get; } = Create(ILOpCode.Ldelem_i8);
        public static OpCode Ldelem_i { get; } = Create(ILOpCode.Ldelem_i);
        public static OpCode Ldelem_r4 { get; } = Create(ILOpCode.Ldelem_r4);
        public static OpCode Ldelem_r8 { get; } = Create(ILOpCode.Ldelem_r8);
        public static OpCode Ldelem_ref { get; } = Create(ILOpCode.Ldelem_ref);
        public static OpCode Stelem_i { get; } = Create(ILOpCode.Stelem_i);
        public static OpCode Stelem_i1 { get; } = Create(ILOpCode.Stelem_i1);
        public static OpCode Stelem_i2 { get; } = Create(ILOpCode.Stelem_i2);
        public static OpCode Stelem_i4 { get; } = Create(ILOpCode.Stelem_i4);
        public static OpCode Stelem_i8 { get; } = Create(ILOpCode.Stelem_i8);
        public static OpCode Stelem_r4 { get; } = Create(ILOpCode.Stelem_r4);
        public static OpCode Stelem_r8 { get; } = Create(ILOpCode.Stelem_r8);
        public static OpCode Stelem_ref { get; } = Create(ILOpCode.Stelem_ref);
        public static OpCode Ldelem { get; } = Create(ILOpCode.Ldelem);
        public static OpCode Stelem { get; } = Create(ILOpCode.Stelem);
        public static OpCode Unbox_any { get; } = Create(ILOpCode.Unbox_any);
        public static OpCode Conv_ovf_i1 { get; } = Create(ILOpCode.Conv_ovf_i1);
        public static OpCode Conv_ovf_u1 { get; } = Create(ILOpCode.Conv_ovf_u1);
        public static OpCode Conv_ovf_i2 { get; } = Create(ILOpCode.Conv_ovf_i2);
        public static OpCode Conv_ovf_u2 { get; } = Create(ILOpCode.Conv_ovf_u2);
        public static OpCode Conv_ovf_i4 { get; } = Create(ILOpCode.Conv_ovf_i4);
        public static OpCode Conv_ovf_u4 { get; } = Create(ILOpCode.Conv_ovf_u4);
        public static OpCode Conv_ovf_i8 { get; } = Create(ILOpCode.Conv_ovf_i8);
        public static OpCode Conv_ovf_u8 { get; } = Create(ILOpCode.Conv_ovf_u8);
        public static OpCode Refanyval { get; } = Create(ILOpCode.Refanyval);
        public static OpCode Ckfinite { get; } = Create(ILOpCode.Ckfinite);
        public static OpCode Mkrefany { get; } = Create(ILOpCode.Mkrefany);
        public static OpCode Ldtoken { get; } = Create(ILOpCode.Ldtoken);
        public static OpCode Conv_u2 { get; } = Create(ILOpCode.Conv_u2);
        public static OpCode Conv_u1 { get; } = Create(ILOpCode.Conv_u1);
        public static OpCode Conv_i { get; } = Create(ILOpCode.Conv_i);
        public static OpCode Conv_ovf_i { get; } = Create(ILOpCode.Conv_ovf_i);
        public static OpCode Conv_ovf_u { get; } = Create(ILOpCode.Conv_ovf_u);
        public static OpCode Add_ovf { get; } = Create(ILOpCode.Add_ovf);
        public static OpCode Add_ovf_un { get; } = Create(ILOpCode.Add_ovf_un);
        public static OpCode Mul_ovf { get; } = Create(ILOpCode.Mul_ovf);
        public static OpCode Mul_ovf_un { get; } = Create(ILOpCode.Mul_ovf_un);
        public static OpCode Sub_ovf { get; } = Create(ILOpCode.Sub_ovf);
        public static OpCode Sub_ovf_un { get; } = Create(ILOpCode.Sub_ovf_un);
        public static OpCode Endfinally { get; } = Create(ILOpCode.Endfinally);
        public static OpCode Leave { get; } = Create(ILOpCode.Leave);
        public static OpCode Leave_s { get; } = Create(ILOpCode.Leave_s);
        public static OpCode Stind_i { get; } = Create(ILOpCode.Stind_i);
        public static OpCode Conv_u { get; } = Create(ILOpCode.Conv_u);
        public static OpCode Arglist { get; } = Create(ILOpCode.Arglist);
        public static OpCode Ceq { get; } = Create(ILOpCode.Ceq);
        public static OpCode Cgt { get; } = Create(ILOpCode.Cgt);
        public static OpCode Cgt_un { get; } = Create(ILOpCode.Cgt_un);
        public static OpCode Clt { get; } = Create(ILOpCode.Clt);
        public static OpCode Clt_un { get; } = Create(ILOpCode.Clt_un);
        public static OpCode Ldftn { get; } = Create(ILOpCode.Ldftn);
        public static OpCode Ldvirtftn { get; } = Create(ILOpCode.Ldvirtftn);
        public static OpCode Ldarg { get; } = Create(ILOpCode.Ldarg);
        public static OpCode Ldarga { get; } = Create(ILOpCode.Ldarga);
        public static OpCode Starg { get; } = Create(ILOpCode.Starg);
        public static OpCode Ldloc { get; } = Create(ILOpCode.Ldloc);
        public static OpCode Ldloca { get; } = Create(ILOpCode.Ldloca);
        public static OpCode Stloc { get; } = Create(ILOpCode.Stloc);
        public static OpCode Localloc { get; } = Create(ILOpCode.Localloc);
        public static OpCode Endfilter { get; } = Create(ILOpCode.Endfilter);
        public static OpCode Unaligned { get; } = Create(ILOpCode.Unaligned);
        public static OpCode Volatile { get; } = Create(ILOpCode.Volatile);
        public static OpCode Tail { get; } = Create(ILOpCode.Tail);
        public static OpCode Initobj { get; } = Create(ILOpCode.Initobj);
        public static OpCode Constrained { get; } = Create(ILOpCode.Constrained);
        public static OpCode Cpblk { get; } = Create(ILOpCode.Cpblk);
        public static OpCode Initblk { get; } = Create(ILOpCode.Initblk);
        public static OpCode Rethrow { get; } = Create(ILOpCode.Rethrow);
        public static OpCode Sizeof { get; } = Create(ILOpCode.Sizeof);
        public static OpCode Refanytype { get; } = Create(ILOpCode.Refanytype);
        public static OpCode Readonly { get; } = Create(ILOpCode.Readonly);

        public ControlFlowKind ControlFlow { get; }

        public int Encoding { get; }

        public int Size { get; }

        public InputBehaviorKind InputBehavior { get; }

        public ILOpCode Value { get; }

        public string Name { get; }

        public OperandType OperandKind { get; }

        public OutputBehaviorKind OutputBehavior { get; }

        public static bool operator ==(OpCode left, OpCode right) => left.Value == right.Value;

        public static bool operator !=(OpCode left, OpCode right) => left.Value != right.Value;

        public static OpCode Create(ILOpCode kind) => kind switch
        {
            ILOpCode.Nop => new OpCode(ILOpCode.Nop, "nop", Pop0, Push0, InlineNone, 1, 0x0000, Next),
            ILOpCode.Break => new OpCode(ILOpCode.Break, "break", Pop0, Push0, InlineNone, 1, 0x0001,
                ControlFlowKind.Break),
            ILOpCode.Ldarg_0 => new OpCode(ILOpCode.Ldarg_0, "ldarg.0", Pop0, Push1, InlineNone, 1, 0x0002, Next),
            ILOpCode.Ldarg_1 => new OpCode(ILOpCode.Ldarg_1, "ldarg.1", Pop0, Push1, InlineNone, 1, 0x0003, Next),
            ILOpCode.Ldarg_2 => new OpCode(ILOpCode.Ldarg_2, "ldarg.2", Pop0, Push1, InlineNone, 1, 0x0004, Next),
            ILOpCode.Ldarg_3 => new OpCode(ILOpCode.Ldarg_3, "ldarg.3", Pop0, Push1, InlineNone, 1, 0x0005, Next),
            ILOpCode.Ldloc_0 => new OpCode(ILOpCode.Ldloc_0, "ldloc.0", Pop0, Push1, InlineNone, 1, 0x0006, Next),
            ILOpCode.Ldloc_1 => new OpCode(ILOpCode.Ldloc_1, "ldloc.1", Pop0, Push1, InlineNone, 1, 0x0007, Next),
            ILOpCode.Ldloc_2 => new OpCode(ILOpCode.Ldloc_2, "ldloc.2", Pop0, Push1, InlineNone, 1, 0x0008, Next),
            ILOpCode.Ldloc_3 => new OpCode(ILOpCode.Ldloc_3, "ldloc.3", Pop0, Push1, InlineNone, 1, 0x0009, Next),
            ILOpCode.Stloc_0 => new OpCode(ILOpCode.Stloc_0, "stloc.0", Pop1, Push0, InlineNone, 1, 0x000A, Next),
            ILOpCode.Stloc_1 => new OpCode(ILOpCode.Stloc_1, "stloc.1", Pop1, Push0, InlineNone, 1, 0x000B, Next),
            ILOpCode.Stloc_2 => new OpCode(ILOpCode.Stloc_2, "stloc.2", Pop1, Push0, InlineNone, 1, 0x000C, Next),
            ILOpCode.Stloc_3 => new OpCode(ILOpCode.Stloc_3, "stloc.3", Pop1, Push0, InlineNone, 1, 0x000D, Next),
            ILOpCode.Ldarg_s => new OpCode(ILOpCode.Ldarg_s, "ldarg.s", Pop0, Push1, ShortInlineVar, 1, 0x000E, Next),
            ILOpCode.Ldarga_s => new OpCode(ILOpCode.Ldarga_s, "ldarga.s", Pop0, PushI, ShortInlineVar, 1, 0x000F,
                Next),
            ILOpCode.Starg_s => new OpCode(ILOpCode.Starg_s, "starg.s", Pop1, Push0, ShortInlineVar, 1, 0x0010, Next),
            ILOpCode.Ldloc_s => new OpCode(ILOpCode.Ldloc_s, "ldloc.s", Pop0, Push1, ShortInlineVar, 1, 0x0011, Next),
            ILOpCode.Ldloca_s => new OpCode(ILOpCode.Ldloca_s, "ldloca.s", Pop0, PushI, ShortInlineVar, 1, 0x0012,
                Next),
            ILOpCode.Stloc_s => new OpCode(ILOpCode.Stloc_s, "stloc.s", Pop1, Push0, ShortInlineVar, 1, 0x0013, Next),
            ILOpCode.Ldnull => new OpCode(ILOpCode.Ldnull, "ldnull", Pop0, PushRef, InlineNone, 1, 0x0014, Next),
            ILOpCode.Ldc_i4_m1 => new OpCode(ILOpCode.Ldc_i4_m1, "ldc.i4.m1", Pop0, PushI, InlineNone, 1, 0x0015, Next),
            ILOpCode.Ldc_i4_0 => new OpCode(ILOpCode.Ldc_i4_0, "ldc.i4.0", Pop0, PushI, InlineNone, 1, 0x0016, Next),
            ILOpCode.Ldc_i4_1 => new OpCode(ILOpCode.Ldc_i4_1, "ldc.i4.1", Pop0, PushI, InlineNone, 1, 0x0017, Next),
            ILOpCode.Ldc_i4_2 => new OpCode(ILOpCode.Ldc_i4_2, "ldc.i4.2", Pop0, PushI, InlineNone, 1, 0x0018, Next),
            ILOpCode.Ldc_i4_3 => new OpCode(ILOpCode.Ldc_i4_3, "ldc.i4.3", Pop0, PushI, InlineNone, 1, 0x0019, Next),
            ILOpCode.Ldc_i4_4 => new OpCode(ILOpCode.Ldc_i4_4, "ldc.i4.4", Pop0, PushI, InlineNone, 1, 0x001A, Next),
            ILOpCode.Ldc_i4_5 => new OpCode(ILOpCode.Ldc_i4_5, "ldc.i4.5", Pop0, PushI, InlineNone, 1, 0x001B, Next),
            ILOpCode.Ldc_i4_6 => new OpCode(ILOpCode.Ldc_i4_6, "ldc.i4.6", Pop0, PushI, InlineNone, 1, 0x001C, Next),
            ILOpCode.Ldc_i4_7 => new OpCode(ILOpCode.Ldc_i4_7, "ldc.i4.7", Pop0, PushI, InlineNone, 1, 0x001D, Next),
            ILOpCode.Ldc_i4_8 => new OpCode(ILOpCode.Ldc_i4_8, "ldc.i4.8", Pop0, PushI, InlineNone, 1, 0x001E, Next),
            ILOpCode.Ldc_i4_s => new OpCode(ILOpCode.Ldc_i4_s, "ldc.i4.s", Pop0, PushI, ShortInlineI, 1, 0x001F, Next),
            ILOpCode.Ldc_i4 => new OpCode(ILOpCode.Ldc_i4, "ldc.i4", Pop0, PushI, InlineI, 1, 0x0020, Next),
            ILOpCode.Ldc_i8 => new OpCode(ILOpCode.Ldc_i8, "ldc.i8", Pop0, PushI8, InlineI8, 1, 0x0021, Next),
            ILOpCode.Ldc_r4 => new OpCode(ILOpCode.Ldc_r4, "ldc.r4", Pop0, PushR4, ShortInlineR, 1, 0x0022, Next),
            ILOpCode.Ldc_r8 => new OpCode(ILOpCode.Ldc_r8, "ldc.r8", Pop0, PushR8, InlineR, 1, 0x0023, Next),
            // unused: 0x0024,
            ILOpCode.Dup => new OpCode(ILOpCode.Dup, "dup", Pop1, Push1_Push1, InlineNone, 1, 0x0025, Next),
            ILOpCode.Pop => new OpCode(ILOpCode.Pop, "pop", Pop1, Push0, InlineNone, 1, 0x0026, Next),
            ILOpCode.Jmp => new OpCode(ILOpCode.Jmp, "jmp", Pop0, Push0, InlineMethod, 1, 0x0027, ControlFlowKind.Call),
            ILOpCode.Call => new OpCode(ILOpCode.Call, "call", VarPop, VarPush, InlineMethod, 1, 0x0028,
                ControlFlowKind.Call),
            ILOpCode.Calli => new OpCode(ILOpCode.Calli, "calli", VarPop, VarPush, InlineSig, 1, 0x0029,
                ControlFlowKind.Call),
            ILOpCode.Ret => new OpCode(ILOpCode.Ret, "ret", VarPop, Push0, InlineNone, 1, 0x002A, Return),
            ILOpCode.Br_s => new OpCode(ILOpCode.Br_s, "br.s", Pop0, Push0, ShortInlineBrTarget, 1, 0x002B, Branch),
            ILOpCode.Brfalse_s => new OpCode(ILOpCode.Brfalse_s, "brfalse.s", PopI, Push0, ShortInlineBrTarget, 1,
                0x002C, Cond_branch),
            ILOpCode.Brtrue_s => new OpCode(ILOpCode.Brtrue_s, "brtrue.s", PopI, Push0, ShortInlineBrTarget, 1, 0x002D,
                Cond_branch),
            ILOpCode.Beq_s => new OpCode(ILOpCode.Beq_s, "beq.s", Pop1_Pop1, Push0, ShortInlineBrTarget, 1, 0x002E,
                Cond_branch),
            ILOpCode.Bge_s => new OpCode(ILOpCode.Bge_s, "bge.s", Pop1_Pop1, Push0, ShortInlineBrTarget, 1, 0x002F,
                Cond_branch),
            ILOpCode.Bgt_s => new OpCode(ILOpCode.Bgt_s, "bgt.s", Pop1_Pop1, Push0, ShortInlineBrTarget, 1, 0x0030,
                Cond_branch),
            ILOpCode.Ble_s => new OpCode(ILOpCode.Ble_s, "ble.s", Pop1_Pop1, Push0, ShortInlineBrTarget, 1, 0x0031,
                Cond_branch),
            ILOpCode.Blt_s => new OpCode(ILOpCode.Blt_s, "blt.s", Pop1_Pop1, Push0, ShortInlineBrTarget, 1, 0x0032,
                Cond_branch),
            ILOpCode.Bne_un_s => new OpCode(ILOpCode.Bne_un_s, "bne.un.s", Pop1_Pop1, Push0, ShortInlineBrTarget, 1,
                0x0033, Cond_branch),
            ILOpCode.Bge_un_s => new OpCode(ILOpCode.Bge_un_s, "bge.un.s", Pop1_Pop1, Push0, ShortInlineBrTarget, 1,
                0x0034, Cond_branch),
            ILOpCode.Bgt_un_s => new OpCode(ILOpCode.Bgt_un_s, "bgt.un.s", Pop1_Pop1, Push0, ShortInlineBrTarget, 1,
                0x0035, Cond_branch),
            ILOpCode.Ble_un_s => new OpCode(ILOpCode.Ble_un_s, "ble.un.s", Pop1_Pop1, Push0, ShortInlineBrTarget, 1,
                0x0036, Cond_branch),
            ILOpCode.Blt_un_s => new OpCode(ILOpCode.Blt_un_s, "blt.un.s", Pop1_Pop1, Push0, ShortInlineBrTarget, 1,
                0x0037, Cond_branch),
            ILOpCode.Br => new OpCode(ILOpCode.Br, "br", Pop0, Push0, InlineBrTarget, 1, 0x0038, Branch),
            ILOpCode.Brfalse => new OpCode(ILOpCode.Brfalse, "brfalse", PopI, Push0, InlineBrTarget, 1, 0x0039,
                Cond_branch),
            ILOpCode.Brtrue => new OpCode(ILOpCode.Brtrue, "brtrue", PopI, Push0, InlineBrTarget, 1, 0x003A,
                Cond_branch),
            ILOpCode.Beq => new OpCode(ILOpCode.Beq, "beq", Pop1_Pop1, Push0, InlineBrTarget, 1, 0x003B, Cond_branch),
            ILOpCode.Bge => new OpCode(ILOpCode.Bge, "bge", Pop1_Pop1, Push0, InlineBrTarget, 1, 0x003C, Cond_branch),
            ILOpCode.Bgt => new OpCode(ILOpCode.Bgt, "bgt", Pop1_Pop1, Push0, InlineBrTarget, 1, 0x003D, Cond_branch),
            ILOpCode.Ble => new OpCode(ILOpCode.Ble, "ble", Pop1_Pop1, Push0, InlineBrTarget, 1, 0x003E, Cond_branch),
            ILOpCode.Blt => new OpCode(ILOpCode.Blt, "blt", Pop1_Pop1, Push0, InlineBrTarget, 1, 0x003F, Cond_branch),
            ILOpCode.Bne_un => new OpCode(ILOpCode.Bne_un, "bne.un", Pop1_Pop1, Push0, InlineBrTarget, 1, 0x0040,
                Cond_branch),
            ILOpCode.Bge_un => new OpCode(ILOpCode.Bge_un, "bge.un", Pop1_Pop1, Push0, InlineBrTarget, 1, 0x0041,
                Cond_branch),
            ILOpCode.Bgt_un => new OpCode(ILOpCode.Bgt_un, "bgt.un", Pop1_Pop1, Push0, InlineBrTarget, 1, 0x0042,
                Cond_branch),
            ILOpCode.Ble_un => new OpCode(ILOpCode.Ble_un, "ble.un", Pop1_Pop1, Push0, InlineBrTarget, 1, 0x0043,
                Cond_branch),
            ILOpCode.Blt_un => new OpCode(ILOpCode.Blt_un, "blt.un", Pop1_Pop1, Push0, InlineBrTarget, 1, 0x0044,
                Cond_branch),
            ILOpCode.Switch => new OpCode(ILOpCode.Switch, "switch", PopI, Push0, InlineSwitch, 1, 0x0045, Cond_branch),
            ILOpCode.Ldind_i1 => new OpCode(ILOpCode.Ldind_i1, "ldind.i1", PopI, PushI, InlineNone, 1, 0x0046, Next),
            ILOpCode.Ldind_u1 => new OpCode(ILOpCode.Ldind_u1, "ldind.u1", PopI, PushI, InlineNone, 1, 0x0047, Next),
            ILOpCode.Ldind_i2 => new OpCode(ILOpCode.Ldind_i2, "ldind.i2", PopI, PushI, InlineNone, 1, 0x0048, Next),
            ILOpCode.Ldind_u2 => new OpCode(ILOpCode.Ldind_u2, "ldind.u2", PopI, PushI, InlineNone, 1, 0x0049, Next),
            ILOpCode.Ldind_i4 => new OpCode(ILOpCode.Ldind_i4, "ldind.i4", PopI, PushI, InlineNone, 1, 0x004A, Next),
            ILOpCode.Ldind_u4 => new OpCode(ILOpCode.Ldind_u4, "ldind.u4", PopI, PushI, InlineNone, 1, 0x004B, Next),
            ILOpCode.Ldind_i8 => new OpCode(ILOpCode.Ldind_i8, "ldind.i8", PopI, PushI8, InlineNone, 1, 0x004C, Next),
            ILOpCode.Ldind_i => new OpCode(ILOpCode.Ldind_i, "ldind.i", PopI, PushI, InlineNone, 1, 0x004D, Next),
            ILOpCode.Ldind_r4 => new OpCode(ILOpCode.Ldind_r4, "ldind.r4", PopI, PushR4, InlineNone, 1, 0x004E, Next),
            ILOpCode.Ldind_r8 => new OpCode(ILOpCode.Ldind_r8, "ldind.r8", PopI, PushR8, InlineNone, 1, 0x004F, Next),
            ILOpCode.Ldind_ref => new OpCode(ILOpCode.Ldind_ref, "ldind.ref", PopI, PushRef, InlineNone, 1, 0x0050,
                Next),
            ILOpCode.Stind_ref => new OpCode(ILOpCode.Stind_ref, "stind.ref", PopI_PopI, Push0, InlineNone, 1, 0x0051,
                Next),
            ILOpCode.Stind_i1 => new OpCode(ILOpCode.Stind_i1, "stind.i1", PopI_PopI, Push0, InlineNone, 1, 0x0052,
                Next),
            ILOpCode.Stind_i2 => new OpCode(ILOpCode.Stind_i2, "stind.i2", PopI_PopI, Push0, InlineNone, 1, 0x0053,
                Next),
            ILOpCode.Stind_i4 => new OpCode(ILOpCode.Stind_i4, "stind.i4", PopI_PopI, Push0, InlineNone, 1, 0x0054,
                Next),
            ILOpCode.Stind_i8 => new OpCode(ILOpCode.Stind_i8, "stind.i8", PopI_PopI8, Push0, InlineNone, 1, 0x0055,
                Next),
            ILOpCode.Stind_r4 => new OpCode(ILOpCode.Stind_r4, "stind.r4", PopI_PopR4, Push0, InlineNone, 1, 0x0056,
                Next),
            ILOpCode.Stind_r8 => new OpCode(ILOpCode.Stind_r8, "stind.r8", PopI_PopR8, Push0, InlineNone, 1, 0x0057,
                Next),
            ILOpCode.Add => new OpCode(ILOpCode.Add, "add", Pop1_Pop1, Push1, InlineNone, 1, 0x0058, Next),
            ILOpCode.Sub => new OpCode(ILOpCode.Sub, "sub", Pop1_Pop1, Push1, InlineNone, 1, 0x0059, Next),
            ILOpCode.Mul => new OpCode(ILOpCode.Mul, "mul", Pop1_Pop1, Push1, InlineNone, 1, 0x005A, Next),
            ILOpCode.Div => new OpCode(ILOpCode.Div, "div", Pop1_Pop1, Push1, InlineNone, 1, 0x005B, Next),
            ILOpCode.Div_un => new OpCode(ILOpCode.Div_un, "div.un", Pop1_Pop1, Push1, InlineNone, 1, 0x005C, Next),
            ILOpCode.Rem => new OpCode(ILOpCode.Rem, "rem", Pop1_Pop1, Push1, InlineNone, 1, 0x005D, Next),
            ILOpCode.Rem_un => new OpCode(ILOpCode.Rem_un, "rem.un", Pop1_Pop1, Push1, InlineNone, 1, 0x005E, Next),
            ILOpCode.And => new OpCode(ILOpCode.And, "and", Pop1_Pop1, Push1, InlineNone, 1, 0x005F, Next),
            ILOpCode.Or => new OpCode(ILOpCode.Or, "or", Pop1_Pop1, Push1, InlineNone, 1, 0x0060, Next),
            ILOpCode.Xor => new OpCode(ILOpCode.Xor, "xor", Pop1_Pop1, Push1, InlineNone, 1, 0x0061, Next),
            ILOpCode.Shl => new OpCode(ILOpCode.Shl, "shl", Pop1_Pop1, Push1, InlineNone, 1, 0x0062, Next),
            ILOpCode.Shr => new OpCode(ILOpCode.Shr, "shr", Pop1_Pop1, Push1, InlineNone, 1, 0x0063, Next),
            ILOpCode.Shr_un => new OpCode(ILOpCode.Shr_un, "shr.un", Pop1_Pop1, Push1, InlineNone, 1, 0x0064, Next),
            ILOpCode.Neg => new OpCode(ILOpCode.Neg, "neg", Pop1, Push1, InlineNone, 1, 0x0065, Next),
            ILOpCode.Not => new OpCode(ILOpCode.Not, "not", Pop1, Push1, InlineNone, 1, 0x0066, Next),
            ILOpCode.Conv_i1 => new OpCode(ILOpCode.Conv_i1, "conv.i1", Pop1, PushI, InlineNone, 1, 0x0067, Next),
            ILOpCode.Conv_i2 => new OpCode(ILOpCode.Conv_i2, "conv.i2", Pop1, PushI, InlineNone, 1, 0x0068, Next),
            ILOpCode.Conv_i4 => new OpCode(ILOpCode.Conv_i4, "conv.i4", Pop1, PushI, InlineNone, 1, 0x0069, Next),
            ILOpCode.Conv_i8 => new OpCode(ILOpCode.Conv_i8, "conv.i8", Pop1, PushI8, InlineNone, 1, 0x006A, Next),
            ILOpCode.Conv_r4 => new OpCode(ILOpCode.Conv_r4, "conv.r4", Pop1, PushR4, InlineNone, 1, 0x006B, Next),
            ILOpCode.Conv_r8 => new OpCode(ILOpCode.Conv_r8, "conv.r8", Pop1, PushR8, InlineNone, 1, 0x006C, Next),
            ILOpCode.Conv_u4 => new OpCode(ILOpCode.Conv_u4, "conv.u4", Pop1, PushI, InlineNone, 1, 0x006D, Next),
            ILOpCode.Conv_u8 => new OpCode(ILOpCode.Conv_u8, "conv.u8", Pop1, PushI8, InlineNone, 1, 0x006E, Next),
            ILOpCode.Callvirt => new OpCode(ILOpCode.Callvirt, "callvirt", VarPop, VarPush, InlineMethod, 1, 0x006F,
                ControlFlowKind.Call),
            ILOpCode.Cpobj => new OpCode(ILOpCode.Cpobj, "cpobj", PopI_PopI, Push0, InlineType, 1, 0x0070, Next),
            ILOpCode.Ldobj => new OpCode(ILOpCode.Ldobj, "ldobj", PopI, Push1, InlineType, 1, 0x0071, Next),
            ILOpCode.Ldstr => new OpCode(ILOpCode.Ldstr, "ldstr", Pop0, PushRef, InlineString, 1, 0x0072, Next),
            ILOpCode.Newobj => new OpCode(ILOpCode.Newobj, "newobj", VarPop, PushRef, InlineMethod, 1, 0x0073,
                ControlFlowKind.Call),
            ILOpCode.Castclass => new OpCode(ILOpCode.Castclass, "castclass", PopRef, PushRef, InlineType, 1, 0x0074,
                Next),
            ILOpCode.Isinst => new OpCode(ILOpCode.Isinst, "isinst", PopRef, PushI, InlineType, 1, 0x0075, Next),
            ILOpCode.Conv_r_un => new OpCode(ILOpCode.Conv_r_un, "conv.r.un", Pop1, PushR8, InlineNone, 1, 0x0076,
                Next),
            // unused: 0x0077,
            // unused: 0x0078,
            ILOpCode.Unbox => new OpCode(ILOpCode.Unbox, "unbox", PopRef, PushI, InlineType, 1, 0x0079, Next),
            ILOpCode.Throw => new OpCode(ILOpCode.Throw, "throw", PopRef, Push0, InlineNone, 1, 0x007A,
                ControlFlowKind.Throw),
            ILOpCode.Ldfld => new OpCode(ILOpCode.Ldfld, "ldfld", PopRef, Push1, InlineField, 1, 0x007B, Next),
            ILOpCode.Ldflda => new OpCode(ILOpCode.Ldflda, "ldflda", PopRef, PushI, InlineField, 1, 0x007C, Next),
            ILOpCode.Stfld => new OpCode(ILOpCode.Stfld, "stfld", PopRef_Pop1, Push0, InlineField, 1, 0x007D, Next),
            ILOpCode.Ldsfld => new OpCode(ILOpCode.Ldsfld, "ldsfld", Pop0, Push1, InlineField, 1, 0x007E, Next),
            ILOpCode.Ldsflda => new OpCode(ILOpCode.Ldsflda, "ldsflda", Pop0, PushI, InlineField, 1, 0x007F, Next),
            ILOpCode.Stsfld => new OpCode(ILOpCode.Stsfld, "stsfld", Pop1, Push0, InlineField, 1, 0x0080, Next),
            ILOpCode.Stobj => new OpCode(ILOpCode.Stobj, "stobj", PopI_Pop1, Push0, InlineType, 1, 0x0081, Next),
            ILOpCode.Conv_ovf_i1_un => new OpCode(ILOpCode.Conv_ovf_i1_un, "conv.ovf.i1.un", Pop1, PushI, InlineNone, 1,
                0x0082, Next),
            ILOpCode.Conv_ovf_i2_un => new OpCode(ILOpCode.Conv_ovf_i2_un, "conv.ovf.i2.un", Pop1, PushI, InlineNone, 1,
                0x0083, Next),
            ILOpCode.Conv_ovf_i4_un => new OpCode(ILOpCode.Conv_ovf_i4_un, "conv.ovf.i4.un", Pop1, PushI, InlineNone, 1,
                0x0084, Next),
            ILOpCode.Conv_ovf_i8_un => new OpCode(ILOpCode.Conv_ovf_i8_un, "conv.ovf.i8.un", Pop1, PushI8, InlineNone,
                1, 0x0085, Next),
            ILOpCode.Conv_ovf_u1_un => new OpCode(ILOpCode.Conv_ovf_u1_un, "conv.ovf.u1.un", Pop1, PushI, InlineNone, 1,
                0x0086, Next),
            ILOpCode.Conv_ovf_u2_un => new OpCode(ILOpCode.Conv_ovf_u2_un, "conv.ovf.u2.un", Pop1, PushI, InlineNone, 1,
                0x0087, Next),
            ILOpCode.Conv_ovf_u4_un => new OpCode(ILOpCode.Conv_ovf_u4_un, "conv.ovf.u4.un", Pop1, PushI, InlineNone, 1,
                0x0088, Next),
            ILOpCode.Conv_ovf_u8_un => new OpCode(ILOpCode.Conv_ovf_u8_un, "conv.ovf.u8.un", Pop1, PushI8, InlineNone,
                1, 0x0089, Next),
            ILOpCode.Conv_ovf_i_un => new OpCode(ILOpCode.Conv_ovf_i_un, "conv.ovf.i.un", Pop1, PushI, InlineNone, 1,
                0x008A, Next),
            ILOpCode.Conv_ovf_u_un => new OpCode(ILOpCode.Conv_ovf_u_un, "conv.ovf.u.un", Pop1, PushI, InlineNone, 1,
                0x008B, Next),
            ILOpCode.Box => new OpCode(ILOpCode.Box, "box", Pop1, PushRef, InlineType, 1, 0x008C, Next),
            ILOpCode.Newarr => new OpCode(ILOpCode.Newarr, "newarr", PopI, PushRef, InlineType, 1, 0x008D, Next),
            ILOpCode.Ldlen => new OpCode(ILOpCode.Ldlen, "ldlen", PopRef, PushI, InlineNone, 1, 0x008E, Next),
            ILOpCode.Ldelema => new OpCode(ILOpCode.Ldelema, "ldelema", PopRef_PopI, PushI, InlineType, 1, 0x008F,
                Next),
            ILOpCode.Ldelem_i1 => new OpCode(ILOpCode.Ldelem_i1, "ldelem.i1", PopRef_PopI, PushI, InlineNone, 1, 0x0090,
                Next),
            ILOpCode.Ldelem_u1 => new OpCode(ILOpCode.Ldelem_u1, "ldelem.u1", PopRef_PopI, PushI, InlineNone, 1, 0x0091,
                Next),
            ILOpCode.Ldelem_i2 => new OpCode(ILOpCode.Ldelem_i2, "ldelem.i2", PopRef_PopI, PushI, InlineNone, 1, 0x0092,
                Next),
            ILOpCode.Ldelem_u2 => new OpCode(ILOpCode.Ldelem_u2, "ldelem.u2", PopRef_PopI, PushI, InlineNone, 1, 0x0093,
                Next),
            ILOpCode.Ldelem_i4 => new OpCode(ILOpCode.Ldelem_i4, "ldelem.i4", PopRef_PopI, PushI, InlineNone, 1, 0x0094,
                Next),
            ILOpCode.Ldelem_u4 => new OpCode(ILOpCode.Ldelem_u4, "ldelem.u4", PopRef_PopI, PushI, InlineNone, 1, 0x0095,
                Next),
            ILOpCode.Ldelem_i8 => new OpCode(ILOpCode.Ldelem_i8, "ldelem.i8", PopRef_PopI, PushI8, InlineNone, 1,
                0x0096, Next),
            ILOpCode.Ldelem_i => new OpCode(ILOpCode.Ldelem_i, "ldelem.i", PopRef_PopI, PushI, InlineNone, 1, 0x0097,
                Next),
            ILOpCode.Ldelem_r4 => new OpCode(ILOpCode.Ldelem_r4, "ldelem.r4", PopRef_PopI, PushR4, InlineNone, 1,
                0x0098, Next),
            ILOpCode.Ldelem_r8 => new OpCode(ILOpCode.Ldelem_r8, "ldelem.r8", PopRef_PopI, PushR8, InlineNone, 1,
                0x0099, Next),
            ILOpCode.Ldelem_ref => new OpCode(ILOpCode.Ldelem_ref, "ldelem.ref", PopRef_PopI, PushRef, InlineNone, 1,
                0x009A, Next),
            ILOpCode.Stelem_i => new OpCode(ILOpCode.Stelem_i, "stelem.i", PopRef_PopI_PopI, Push0, InlineNone, 1,
                0x009B, Next),
            ILOpCode.Stelem_i1 => new OpCode(ILOpCode.Stelem_i1, "stelem.i1", PopRef_PopI_PopI, Push0, InlineNone, 1,
                0x009C, Next),
            ILOpCode.Stelem_i2 => new OpCode(ILOpCode.Stelem_i2, "stelem.i2", PopRef_PopI_PopI, Push0, InlineNone, 1,
                0x009D, Next),
            ILOpCode.Stelem_i4 => new OpCode(ILOpCode.Stelem_i4, "stelem.i4", PopRef_PopI_PopI, Push0, InlineNone, 1,
                0x009E, Next),
            ILOpCode.Stelem_i8 => new OpCode(ILOpCode.Stelem_i8, "stelem.i8", PopRef_PopI_PopI8, Push0, InlineNone, 1,
                0x009F, Next),
            ILOpCode.Stelem_r4 => new OpCode(ILOpCode.Stelem_r4, "stelem.r4", PopRef_PopI_PopR4, Push0, InlineNone, 1,
                0x00A0, Next),
            ILOpCode.Stelem_r8 => new OpCode(ILOpCode.Stelem_r8, "stelem.r8", PopRef_PopI_PopR8, Push0, InlineNone, 1,
                0x00A1, Next),
            ILOpCode.Stelem_ref => new OpCode(ILOpCode.Stelem_ref, "stelem.ref", PopRef_PopI_PopRef, Push0, InlineNone,
                1, 0x00A2, Next),
            ILOpCode.Ldelem => new OpCode(ILOpCode.Ldelem, "ldelem", PopRef_PopI, Push1, InlineType, 1, 0x00A3, Next),
            ILOpCode.Stelem => new OpCode(ILOpCode.Stelem, "stelem", PopRef_PopI_Pop1, Push0, InlineType, 1, 0x00A4,
                Next),
            ILOpCode.Unbox_any => new OpCode(ILOpCode.Unbox_any, "unbox.any", PopRef, Push1, InlineType, 1, 0x00A5,
                Next),
            // unused: 0x00A6,
            // unused: 0x00A7,
            // unused: 0x00A8,
            // unused: 0x00A9,
            // unused: 0x00AA,
            // unused: 0x00AB,
            // unused: 0x00AC,
            // unused: 0x00AD,
            // unused: 0x00AE,
            // unused: 0x00AF,
            // unused: 0x00B0,
            // unused: 0x00B1,
            // unused: 0x00B2,
            ILOpCode.Conv_ovf_i1 => new OpCode(ILOpCode.Conv_ovf_i1, "conv.ovf.i1", Pop1, PushI, InlineNone, 1, 0x00B3,
                Next),
            ILOpCode.Conv_ovf_u1 => new OpCode(ILOpCode.Conv_ovf_u1, "conv.ovf.u1", Pop1, PushI, InlineNone, 1, 0x00B4,
                Next),
            ILOpCode.Conv_ovf_i2 => new OpCode(ILOpCode.Conv_ovf_i2, "conv.ovf.i2", Pop1, PushI, InlineNone, 1, 0x00B5,
                Next),
            ILOpCode.Conv_ovf_u2 => new OpCode(ILOpCode.Conv_ovf_u2, "conv.ovf.u2", Pop1, PushI, InlineNone, 1, 0x00B6,
                Next),
            ILOpCode.Conv_ovf_i4 => new OpCode(ILOpCode.Conv_ovf_i4, "conv.ovf.i4", Pop1, PushI, InlineNone, 1, 0x00B7,
                Next),
            ILOpCode.Conv_ovf_u4 => new OpCode(ILOpCode.Conv_ovf_u4, "conv.ovf.u4", Pop1, PushI, InlineNone, 1, 0x00B8,
                Next),
            ILOpCode.Conv_ovf_i8 => new OpCode(ILOpCode.Conv_ovf_i8, "conv.ovf.i8", Pop1, PushI8, InlineNone, 1, 0x00B9,
                Next),
            ILOpCode.Conv_ovf_u8 => new OpCode(ILOpCode.Conv_ovf_u8, "conv.ovf.u8", Pop1, PushI8, InlineNone, 1, 0x00BA,
                Next),
            // unused: 0x00BB,
            // unused: 0x00BC,
            // unused: 0x00BD,
            // unused: 0x00BE,
            // unused: 0x00BF,
            // unused: 0x00C0,
            // unused: 0x00C1,
            ILOpCode.Refanyval => new OpCode(ILOpCode.Refanyval, "refanyval", Pop1, PushI, InlineType, 1, 0x00C2, Next),
            ILOpCode.Ckfinite => new OpCode(ILOpCode.Ckfinite, "ckfinite", Pop1, PushR8, InlineNone, 1, 0x00C3, Next),
            // unused: 0x00C4,
            // unused: 0x00C5,
            ILOpCode.Mkrefany => new OpCode(ILOpCode.Mkrefany, "mkrefany", PopI, Push1, InlineType, 1, 0x00C6, Next),
            // unused: 0x00C7,
            // unused: 0x00C8,
            // unused: 0x00C9,
            // unused: 0x00CA,
            // unused: 0x00CB,
            // unused: 0x00CC,
            // unused: 0x00CD,
            // unused: 0x00CE,
            // unused: 0x00CF,
            ILOpCode.Ldtoken => new OpCode(ILOpCode.Ldtoken, "ldtoken", Pop0, PushI, InlineTok, 1, 0x00D0, Next),
            ILOpCode.Conv_u2 => new OpCode(ILOpCode.Conv_u2, "conv.u2", Pop1, PushI, InlineNone, 1, 0x00D1, Next),
            ILOpCode.Conv_u1 => new OpCode(ILOpCode.Conv_u1, "conv.u1", Pop1, PushI, InlineNone, 1, 0x00D2, Next),
            ILOpCode.Conv_i => new OpCode(ILOpCode.Conv_i, "conv.i", Pop1, PushI, InlineNone, 1, 0x00D3, Next),
            ILOpCode.Conv_ovf_i => new OpCode(ILOpCode.Conv_ovf_i, "conv.ovf.i", Pop1, PushI, InlineNone, 1, 0x00D4,
                Next),
            ILOpCode.Conv_ovf_u => new OpCode(ILOpCode.Conv_ovf_u, "conv.ovf.u", Pop1, PushI, InlineNone, 1, 0x00D5,
                Next),
            ILOpCode.Add_ovf => new OpCode(ILOpCode.Add_ovf, "add.ovf", Pop1_Pop1, Push1, InlineNone, 1, 0x00D6, Next),
            ILOpCode.Add_ovf_un => new OpCode(ILOpCode.Add_ovf_un, "add.ovf.un", Pop1_Pop1, Push1, InlineNone, 1,
                0x00D7, Next),
            ILOpCode.Mul_ovf => new OpCode(ILOpCode.Mul_ovf, "mul.ovf", Pop1_Pop1, Push1, InlineNone, 1, 0x00D8, Next),
            ILOpCode.Mul_ovf_un => new OpCode(ILOpCode.Mul_ovf_un, "mul.ovf.un", Pop1_Pop1, Push1, InlineNone, 1,
                0x00D9, Next),
            ILOpCode.Sub_ovf => new OpCode(ILOpCode.Sub_ovf, "sub.ovf", Pop1_Pop1, Push1, InlineNone, 1, 0x00DA, Next),
            ILOpCode.Sub_ovf_un => new OpCode(ILOpCode.Sub_ovf_un, "sub.ovf.un", Pop1_Pop1, Push1, InlineNone, 1,
                0x00DB, Next),
            ILOpCode.Endfinally => new OpCode(ILOpCode.Endfinally, "endfinally", Pop0, Push0, InlineNone, 1, 0x00DC,
                Return),
            ILOpCode.Leave => new OpCode(ILOpCode.Leave, "leave", Pop0, Push0, InlineBrTarget, 1, 0x00DD, Branch),
            ILOpCode.Leave_s => new OpCode(ILOpCode.Leave_s, "leave.s", Pop0, Push0, ShortInlineBrTarget, 1, 0x00DE,
                Branch),
            ILOpCode.Stind_i => new OpCode(ILOpCode.Stind_i, "stind.i", PopI_PopI, Push0, InlineNone, 1, 0x00DF, Next),
            ILOpCode.Conv_u => new OpCode(ILOpCode.Conv_u, "conv.u", Pop1, PushI, InlineNone, 1, 0x00E0, Next),
            // unused: 0x00E1,
            // unused: 0x00E2,
            // unused: 0x00E3,
            // unused: 0x00E4,
            // unused: 0x00E5,
            // unused: 0x00E6,
            // unused: 0x00E7,
            // unused: 0x00E8,
            // unused: 0x00E9,
            // unused: 0x00EA,
            // unused: 0x00EB,
            // unused: 0x00EC,
            // unused: 0x00ED,
            // unused: 0x00EE,
            // unused: 0x00EF,
            // unused: 0x00F0,
            // unused: 0x00F1,
            // unused: 0x00F2,
            // unused: 0x00F3,
            // unused: 0x00F4,
            // unused: 0x00F5,
            // unused: 0x00F6,
            // unused: 0x00F7,
            // prefix: 0x00F8,
            // prefix: 0x00F9,
            // prefix: 0x00FA,
            // prefix: 0x00FB,
            // prefix: 0x00FC,
            // prefix: 0x00FD,
            // prefix: 0x00FE,
            ILOpCode.Arglist => new OpCode(ILOpCode.Arglist, "arglist", Pop0, PushI, InlineNone, 2, 0xFE00, Next),
            ILOpCode.Ceq => new OpCode(ILOpCode.Ceq, "ceq", Pop1_Pop1, PushI, InlineNone, 2, 0xFE01, Next),
            ILOpCode.Cgt => new OpCode(ILOpCode.Cgt, "cgt", Pop1_Pop1, PushI, InlineNone, 2, 0xFE02, Next),
            ILOpCode.Cgt_un => new OpCode(ILOpCode.Cgt_un, "cgt.un", Pop1_Pop1, PushI, InlineNone, 2, 0xFE03, Next),
            ILOpCode.Clt => new OpCode(ILOpCode.Clt, "clt", Pop1_Pop1, PushI, InlineNone, 2, 0xFE04, Next),
            ILOpCode.Clt_un => new OpCode(ILOpCode.Clt_un, "clt.un", Pop1_Pop1, PushI, InlineNone, 2, 0xFE05, Next),
            ILOpCode.Ldftn => new OpCode(ILOpCode.Ldftn, "ldftn", Pop0, PushI, InlineMethod, 2, 0xFE06, Next),
            ILOpCode.Ldvirtftn => new OpCode(ILOpCode.Ldvirtftn, "ldvirtftn", PopRef, PushI, InlineMethod, 2, 0xFE07,
                Next),
            // unused: 0xFE08,
            ILOpCode.Ldarg => new OpCode(ILOpCode.Ldarg, "ldarg", Pop0, Push1, InlineVar, 2, 0xFE09, Next),
            ILOpCode.Ldarga => new OpCode(ILOpCode.Ldarga, "ldarga", Pop0, PushI, InlineVar, 2, 0xFE0A, Next),
            ILOpCode.Starg => new OpCode(ILOpCode.Starg, "starg", Pop1, Push0, InlineVar, 2, 0xFE0B, Next),
            ILOpCode.Ldloc => new OpCode(ILOpCode.Ldloc, "ldloc", Pop0, Push1, InlineVar, 2, 0xFE0C, Next),
            ILOpCode.Ldloca => new OpCode(ILOpCode.Ldloca, "ldloca", Pop0, PushI, InlineVar, 2, 0xFE0D, Next),
            ILOpCode.Stloc => new OpCode(ILOpCode.Stloc, "stloc", Pop1, Push0, InlineVar, 2, 0xFE0E, Next),
            ILOpCode.Localloc => new OpCode(ILOpCode.Localloc, "localloc", PopI, PushI, InlineNone, 2, 0xFE0F, Next),
            // unused: 0xFE10,
            ILOpCode.Endfilter => new OpCode(ILOpCode.Endfilter, "endfilter", PopI, Push0, InlineNone, 2, 0xFE11,
                Return),
            ILOpCode.Unaligned => new OpCode(ILOpCode.Unaligned, "unaligned.", Pop0, Push0, ShortInlineI, 2, 0xFE12,
                Meta),
            ILOpCode.Volatile => new OpCode(ILOpCode.Volatile, "volatile.", Pop0, Push0, InlineNone, 2, 0xFE13, Meta),
            ILOpCode.Tail => new OpCode(ILOpCode.Tail, "tail.", Pop0, Push0, InlineNone, 2, 0xFE14, Meta),
            ILOpCode.Initobj => new OpCode(ILOpCode.Initobj, "initobj", PopI, Push0, InlineType, 2, 0xFE15, Next),
            ILOpCode.Constrained => new OpCode(ILOpCode.Constrained, "constrained.", Pop0, Push0, InlineType, 2, 0xFE16,
                Meta),
            ILOpCode.Cpblk => new OpCode(ILOpCode.Cpblk, "cpblk", PopI_PopI_PopI, Push0, InlineNone, 2, 0xFE17, Next),
            ILOpCode.Initblk => new OpCode(ILOpCode.Initblk, "initblk", PopI_PopI_PopI, Push0, InlineNone, 2, 0xFE18,
                Next),
            // prefix: 0xFE19
            ILOpCode.Rethrow => new OpCode(ILOpCode.Rethrow, "rethrow", Pop0, Push0, InlineNone, 2, 0xFE1A,
                ControlFlowKind.Throw),
            // unused: 0xFE1B,
            ILOpCode.Sizeof => new OpCode(ILOpCode.Sizeof, "sizeof", Pop0, PushI, InlineType, 2, 0xFE1C, Next),
            ILOpCode.Refanytype => new OpCode(ILOpCode.Refanytype, "refanytype", Pop1, PushI, InlineNone, 2, 0xFE1D,
                Next),
            ILOpCode.Readonly => new OpCode(ILOpCode.Readonly, "readonly.", Pop0, Push0, InlineNone, 2, 0xFE1E, Meta),
            // unused: 0xFE1F,
            // unused: 0xFE20,
            // unused: 0xFE21,
            // unused: 0xFE22,
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };

        public override bool Equals(object? obj) => (obj is OpCode other) && Equals(other);

        public bool Equals(OpCode other) => this == other;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Name;
    }
}