using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Ultz.SuperInvoke.Builder
{
    internal class MethodBodyStreamWriter
    {
        // Adapted from: https://github.com/dotnet/corefx/blob/772a2486f2dd29f3a0401427a26da23e845a6e59/src/System.Reflection.Metadata/src/System/Reflection/Metadata/Ecma335/Encoding/MethodBodyStreamEncoder.cs#L222-L272
        //
        internal static int SerializeHeader(BlobBuilder il, int codeSize, int maxStack,
            MethodBodyAttributes attributes, StandaloneSignatureHandle localVariablesSignature,
            bool hasDynamicStackAllocation)
        {
            const int tinyFormat = 2;
            const int fatFormat = 3;
            const int moreSections = 8;
            const byte initLocalsC = 0x10;

            var initLocals = (attributes & MethodBodyAttributes.InitLocals) != 0;

            var isTiny = codeSize < 64 &&
                         maxStack <= 8 &&
                         localVariablesSignature.IsNil && (!hasDynamicStackAllocation || !initLocals);

            int offset;
            if (isTiny)
            {
                offset = il.Count;
                il.WriteByte((byte) ((codeSize << 2) | tinyFormat));
            }
            else
            {
                il.Align(4);

                offset = il.Count;

                ushort flags = (3 << 12) | fatFormat;

                if (initLocals)
                {
                    flags |= initLocalsC;
                }

                il.WriteUInt16((ushort) ((int) attributes | flags));
                il.WriteUInt16((ushort) maxStack);
                il.WriteInt32(codeSize);
                il.WriteInt32(
                    localVariablesSignature.IsNil ? 0 : MetadataTokens.GetToken(localVariablesSignature));
            }

            return offset;
        }
    }
}
