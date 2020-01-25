using System.Collections.Generic;
using System.IO;
using Ultz.SuperBind.Core;

namespace Ultz.SuperBind.Writers.CSharp
{
    public class CSharpWriter : IWriter
    {
        private ProjectWriter _writer;
        public string RootPath { get; set; }

        public void Initialize()
        {
            if (!Directory.Exists(Path.Combine(RootPath, "Interfaces")))
            {
                Directory.CreateDirectory(Path.Combine(RootPath, "Interfaces"));
            }

            if (!Directory.Exists(Path.Combine(RootPath, "Structs")))
            {
                Directory.CreateDirectory(Path.Combine(RootPath, "Structs"));
            }

            if (!Directory.Exists(Path.Combine(RootPath, "Delegates")))
            {
                Directory.CreateDirectory(Path.Combine(RootPath, "Delegates"));
            }

            if (!Directory.Exists(Path.Combine(RootPath, "Classes")))
            {
                Directory.CreateDirectory(Path.Combine(RootPath, "Classes"));
            }
        }

        public void WriteProjectData(ProjectSpecification project)
        {
            var dir = Path.Combine(RootPath, project.Name);
            Initialize();
            _writer.Write(project, dir);
        }

        public void WriteInterface(InterfaceSpecification spec) =>
            InterfaceWriter.WriteInterface(new StreamWriter(Path.Combine(RootPath, "Interfaces",
                $"{spec.Namespace}.{spec.Name}.gen.cs")), spec);


        public void WriteStruct(StructSpecification spec) =>
            StructWriter.WriteStruct(new StreamWriter(Path.Combine(RootPath, "Structs",
                $"{spec.Namespace}.{spec.Name}.gen.cs")), spec);


        public void WriteDelegate(DelegateSpecification spec) =>
            DelegateWriter.WriteDelegate(new StreamWriter(Path.Combine(RootPath, "Delegates",
                $"{spec.Namespace}.{spec.Name}.gen.cs")), spec);


        public void WriteClass(ClassSpecification spec) =>
            ClassWriter.WriteClass(new StreamWriter(Path.Combine(RootPath, "Classes",
                $"{spec.Namespace}.{spec.Name}.gen.cs")), spec);
    }
}