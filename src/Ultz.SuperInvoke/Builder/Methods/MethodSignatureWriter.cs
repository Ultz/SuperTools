using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Ultz.SuperInvoke.Builder
{
    public class MethodSignatureWriter
    {
        public BlobHandle GetSignature(ImplMethod method)
        {
            // Get parameters
            var parameters = methodBase.GetParameters();

            // Create method signature encoder
            var enc = new BlobEncoder(new BlobBuilder())
                .MethodSignature(
                    method.CallingConvention,
                    genericParameterCount: method.GenericArguments.Count,
                    isInstanceMethod: !method.IsStatic);

            // Add return type and parameters
            enc.Parameters(
                    parameters.Length,
                    (retEnc) =>
                    {
                        if (method.Ret)
                    },
                    (parEnc) =>
                    {
                        foreach (var par in parameters)
                        {
                            if (par.ParameterType.IsByRef)
                            {
                                parEnc.AddParameter().Type(true).FromSystemType(par.ParameterType.GetElementType(), this);
                            }
                            else
                            {
                                parEnc.AddParameter().Type(false).FromSystemType(par.ParameterType, this);
                            }
                        }
                    }
                );

            // Get blob
            return GetOrAddBlob(enc.Builder);
        }
    }
}