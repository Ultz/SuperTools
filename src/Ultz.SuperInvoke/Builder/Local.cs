namespace Ultz.SuperInvoke.Builder
{
    public class Local
    {
        public TypeRef Type { get; set; }
        public virtual bool IsPinned
        {
            get => Type.IsPinned;
            set => Type.IsPinned = value;
        }
        public virtual bool IsValueType
        {
            get => Type.IsValueType;
            set => Type.IsValueType = value;
        }
        public bool IsByRef
        {
            get => Type.IsByReference;
            set => Type.IsByReference = value;
        }
    }
}