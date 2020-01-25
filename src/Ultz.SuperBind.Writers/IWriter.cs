using System.Collections.Generic;
using Ultz.SuperBind.Core;

namespace Ultz.SuperBind.Writers
{
    public interface IWriter
    {
        void Initialize();
        void WriteProjectData(ProjectSpecification project);
        void WriteInterface(InterfaceSpecification spec);
        void WriteStruct(StructSpecification spec);
        void WriteClass(ClassSpecification spec);
        void WriteDelegate(DelegateSpecification spec);
    }
}