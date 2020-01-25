using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Ultz.SuperBind.Binders.Khronos;
using Ultz.SuperBind.Core;
using Ultz.SuperBind.Writers;
using Ultz.SuperBind.Writers.CSharp;

namespace Ultz.SuperBind.Tasks.Silk.NET
{
    public class GlBinderTask : Task
    {
        [Required] public ITaskItem[] Sources { get; set; }
        public ITaskItem[] TypeMaps { get; set; }

        public override bool Execute()
        {
            var processedTypeMaps = TypeMaps?.Select(x => File.ReadAllLines(x.ItemSpec).Select(y => y.Split('=')))
                                        .SelectMany(x => x.Select(y => new KeyValuePair<string, string>(y[0],
                                            string.Join("=", new ArraySegment<string>(y, 1, y.Length - 1)))))
                                        .ToDictionary(x => x.Key, x => x.Value) ?? new Dictionary<string, string>();
            foreach (var task in Sources)
            {
                var prefix = task.GetMetadata("Prefix");
                var @namespace = task.GetMetadata("Namespace");
                var postProcessor = task.GetMetadata("PostProcessor");
                var outputPath = task.GetMetadata("OutputPath");
                var className = task.GetMetadata("ClassName");
                var props = task.GetMetadata("PropsFiles");
                if (string.IsNullOrWhiteSpace(postProcessor))
                {
                    postProcessor = typeof(SilkPostProcessor).AssemblyQualifiedName;
                }

                var opts = new BinderOptions(task.ItemSpec, string.IsNullOrWhiteSpace(prefix) ? "gl" : prefix,
                    string.IsNullOrWhiteSpace(className)
                        ? string.IsNullOrWhiteSpace(prefix) ? "gl" : prefix
                        : className,
                    new string[0], new string[0], new PackageReference[0],
                    string.IsNullOrWhiteSpace(props) ? new string[0] : props.Split(';'),
                    string.IsNullOrWhiteSpace(@namespace) ? "Silk.NET" : @namespace,
                    x => ((IPostProcessor) Activator.CreateInstance(Type.GetType(postProcessor))).Apply(x));
                var glBinder = new GlBinder {TypeMap = processedTypeMaps};
                var projects = glBinder.GetProjects(opts);
                var writer = new CSharpWriter {RootPath = outputPath};
                writer.WriteItems(projects);
            }

            return true;
        }
    }
}