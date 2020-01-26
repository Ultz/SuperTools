using System;
using System.Collections.Generic;
using System.IO;
using Ultz.SuperBind.Core;

namespace Ultz.SuperBind.Writers.CSharp
{
    public class CSharpWriter : IWriter
    {
        private ProjectWriter _writer;
        public string RootPath { get; set; }
        public bool CreateProjectSubdirectory { get; set; } = true;
        private string _path;
        private string ActualRootPath =>
            _path ?? throw new InvalidOperationException("Writer has not been initialized");
        public void Initialize(ProjectSpecification spec)
        {
            if (!(_path is null))
            {
                throw new InvalidOperationException("Already initialized for another project.");
            }
            
            _path = CreateProjectSubdirectory ? Path.Combine(RootPath, spec.Name) : RootPath;
            
            if (Directory.Exists(Path.Combine(ActualRootPath, "Interfaces")))
            {
                Directory.Delete(Path.Combine(ActualRootPath, "Interfaces"), true);
            }

            if (Directory.Exists(Path.Combine(ActualRootPath, "Structs")))
            {
                Directory.Delete(Path.Combine(ActualRootPath, "Structs"), true);
            }

            if (Directory.Exists(Path.Combine(ActualRootPath, "Delegates")))
            {
                Directory.Delete(Path.Combine(ActualRootPath, "Delegates"), true);
            }

            if (Directory.Exists(Path.Combine(ActualRootPath, "Classes")))
            {
                Directory.Delete(Path.Combine(ActualRootPath, "Classes"), true);
            }

            if (Directory.Exists(Path.Combine(ActualRootPath, "Enums")))
            {
                Directory.Delete(Path.Combine(ActualRootPath, "Enums"), true);
            }

            Directory.CreateDirectory(Path.Combine(ActualRootPath, "Interfaces"));
            Directory.CreateDirectory(Path.Combine(ActualRootPath, "Structs"));
            Directory.CreateDirectory(Path.Combine(ActualRootPath, "Delegates"));
            Directory.CreateDirectory(Path.Combine(ActualRootPath, "Classes"));
            Directory.CreateDirectory(Path.Combine(ActualRootPath, "Enums"));
        }

        public void WriteProjectData(ProjectSpecification project)
        {
            var dir = Path.Combine(ActualRootPath, project.Name);
            _writer.Write(project, dir);
        }

        public void WriteInterface(InterfaceSpecification spec) =>
            InterfaceWriter.WriteInterface(new StreamWriter(Path.Combine(ActualRootPath, "Interfaces",
                $"{spec.Namespace}.{spec.Name}.gen.cs")), spec);


        public void WriteStruct(StructSpecification spec) =>
            StructWriter.WriteStruct(new StreamWriter(Path.Combine(ActualRootPath, "Structs",
                $"{spec.Namespace}.{spec.Name}.gen.cs")), spec);


        public void WriteDelegate(DelegateSpecification spec) =>
            DelegateWriter.WriteDelegate(new StreamWriter(Path.Combine(ActualRootPath, "Delegates",
                $"{spec.Namespace}.{spec.Name}.gen.cs")), spec);

        public void Reset()
        {
            _path = null;
        }


        public void WriteClass(ClassSpecification spec) =>
            ClassWriter.WriteClass(new StreamWriter(Path.Combine(ActualRootPath, "Classes",
                $"{spec.Namespace}.{spec.Name}.gen.cs")), spec);


        public void WriteEnum(EnumSpecification spec) =>
            EnumWriter.WriteEnum(new StreamWriter(Path.Combine(ActualRootPath, "Enums",
                $"{spec.Namespace}.{spec.Name}.gen.cs")), spec);
    }
}