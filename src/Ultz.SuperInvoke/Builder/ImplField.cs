using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Ultz.SuperInvoke.Builder
{
    public class ImplField
    {
        public MetadataBuilder _mb;

        public ImplField(MetadataBuilder mb)
        {
            _mb = mb;
        }
        
        public FieldAttributes Attributes { get; set; }
        public string Name { get; set; }
        public TypeRef Type { get; set; }

        internal BlobHandle Write()
        {
            var ret = new BlobEncoder(new BlobBuilder());
            Type.Write(ret.FieldSignature(), _mb);
            return _mb.GetOrAddBlob(ret.Builder);
        }
    }
}