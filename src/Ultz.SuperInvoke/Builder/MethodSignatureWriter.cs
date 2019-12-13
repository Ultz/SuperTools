using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Ultz.SuperInvoke.Builder
{
    public class MethodSignatureWriter
    {
        public static BlobHandle GetSignature(ImplMethod method, MetadataBuilder builder)
        {
            // Create method signature encoder
            var enc = new BlobEncoder(new BlobBuilder())
                .MethodSignature(
                    method.CallingConvention,
                    genericParameterCount: method.GenericArguments.Count,
                    isInstanceMethod: !method.IsStatic);

            // Add return type and parameters
            enc.Parameters(
                    method.Parameters.Count,
                    (retEnc) =>
                    {
                        if (method.ReturnType is null)
                        {
                            retEnc.Void();
                        }
                        else
                        {
                            method.ReturnType.Write(retEnc, builder);
                        }
                    },
                    (parEnc) =>
                    {
                        foreach (var par in method.Parameters)
                        {
                            par.Type.Write(parEnc.AddParameter(), builder);
                        }
                    }
                );
            
            // Get blob
            return builder.GetOrAddBlob(enc.Builder);
        }

        public static BlobHandle GetSignature(IList<Local> localVariables, MetadataBuilder builder)
        {
            var enc = new BlobEncoder(new BlobBuilder()).LocalVariableSignature(localVariables.Count);
            enc.AddRange(localVariables, builder);
            return builder.GetOrAddBlob(enc.Builder);
        }
    }
}