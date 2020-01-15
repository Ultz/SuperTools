using System.IO;
using Ultz.SuperBind.Core;

namespace Ultz.SuperBind.Writers.CSharp
{
    internal class ProjectWriter
    {
        public void Write(ProjectSpecification proj, string dir)
        {
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }

            Directory.CreateDirectory(dir);
            
            var csproj = new StreamWriter(Path.Combine(dir, proj.Name + ".csproj"));
            csproj.WriteLine("<Project>");
            csproj.WriteLine("  <PropertyGroup>");
            if (proj.TargetFrameworks?.Length == 1)
            {
                csproj.WriteLine($"    <TargetFramework>{proj.TargetFrameworks[0]}</TargetFramework>");
            }
            else if (proj.TargetFrameworks?.Length > 1)
            {
                csproj.WriteLine($"    <TargetFrameworks>{string.Join(";", proj.TargetFrameworks)}</TargetFrameworks>");
            }
            
            csproj.WriteLine("  </ProjectGroup>");
            csproj.WriteLine("  <ProjectGroup>");
            foreach (var prop in proj.Properties)
            {
                csproj.WriteLine($"    {prop}");
            }
            csproj.WriteLine("  </ProjectGroup>");
            csproj.WriteLine("  <ItemGroup>");
            foreach (var assemblyReference in proj.AssemblyReferences)
            {
                csproj.WriteLine(
                    $"    <Reference Include=\"{assemblyReference.Name}\" HintPath=\"{assemblyReference.Path}\" />");
            }

            foreach (var packageReference in proj.PackageReferences)
            {
                csproj.WriteLine(
                    $"    <PackageReference Include=\"{packageReference.Name} Version=\"{packageReference.Version}\" />");
            }
            
            foreach (var projectReference in proj.ProjectReferences)
            {
                csproj.WriteLine($"    <ProjectReference Include=\"{projectReference.Path}\" />");
            }
            csproj.WriteLine("  </ItemGroup>");
            csproj.WriteLine("  <ItemGroup>");
            foreach (var item in proj.Items)
            {
                csproj.WriteLine($"    {item}");
            }
            csproj.WriteLine("  </ItemGroup>");
            
            foreach (var prop in proj.PropFiles)
            {
                csproj.WriteLine($"  <Import Project=\"{prop.Path}\" />");
            }
            csproj.WriteLine("</Project>");

            string classDir, interfaceDir, structDir, delegateDir;
            Directory.CreateDirectory(classDir = Path.Combine(dir, "Classes"));
            Directory.CreateDirectory(interfaceDir = Path.Combine(dir, "Interfaces"));
            Directory.CreateDirectory(structDir = Path.Combine(dir, "Structs"));
            Directory.CreateDirectory(delegateDir = Path.Combine(dir, "Delegates"));
        }
    }
}