using System.IO;
using Ultz.SuperBind.Core;

namespace Ultz.SuperBind.Writers.CSharp
{
    public class CSharpWriter : IWriter
    {
        private ProjectWriter _writer;
        public string RootPath { get; set; }

        public void WriteProject(ProjectSpecification project)
        {
            var dir = Path.Combine(RootPath, project.Name);
            _writer.Write(project, dir);
        }
    }
}