using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using Ultz.SuperBind.Binders.Khronos;
using Ultz.SuperBind.Core;
using Ultz.SuperBind.Tasks.Silk.NET;
using Ultz.SuperBind.Writers;
using Ultz.SuperBind.Writers.CSharp;

namespace BinderTest
{
    class Program
    {
        [Required] public static string[] Sources { get; set; }
        public static string[] TypeMaps { get; set; }

        public static void Main(string[] args)
        {
            var xml = File.OpenWrite("gl.xml");
            new HttpClient()
                .GetStreamAsync("https://raw.githubusercontent.com/KhronosGroup/OpenGL-Registry/master/xml/gl.xml")
                .GetAwaiter().GetResult().CopyTo(xml);
            xml.Flush();
            xml.Dispose();
            Sources = new[]{"gl.xml"};
            TypeMaps = new string[0];
            Execute();
        }

        public static bool Execute()
        {
            var processedTypeMaps = TypeMaps?.Select(x => File.ReadAllLines(x).Select(y => y.Split('=')))
                                        .SelectMany(x => x.Select(y => new KeyValuePair<string, string>(y[0],
                                            string.Join("=", new ArraySegment<string>(y, 1, y.Length - 1)))))
                                        .ToDictionary(x => x.Key, x => x.Value) ?? new Dictionary<string, string>();
            foreach (var task in Sources)
            {
                var postProcessor = typeof(SilkPostProcessor).AssemblyQualifiedName;

                var opts = new BinderOptions(task, "gl", "GL",
                    new string[0], new string[0], new PackageReference[0], new string[0],
                    "Silk.NET.OpenGL",
                    x => ((IPostProcessor) Activator.CreateInstance(Type.GetType(postProcessor))).Apply(x));
                var glBinder = new GlBinder {TypeMap = processedTypeMaps};
                var projects = glBinder.GetProjects(opts);
                var writer = new CSharpWriter {RootPath = "Test"};
                writer.WriteItems(projects);
            }

            return true;
        }
    }
}