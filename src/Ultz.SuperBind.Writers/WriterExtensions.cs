using System.Collections.Generic;
using Ultz.SuperBind.Core;

namespace Ultz.SuperBind.Writers
{
    public static class WriterExtensions
    {
        public static void WriteItems(this IWriter writer, ProjectSpecification[] specs)
        {
            foreach (var spec in specs)
            {
                writer.WriteItems(spec);
            }
        }

        public static void WriteProject(this IWriter writer, ProjectSpecification spec, bool init = true)
        {
            if (init){writer.Initialize(spec);}
            writer.WriteProjectData(spec);
            writer.WriteItems(spec, false);
            if (init){writer.Reset();}
        }

        public static void WriteItems(this IWriter writer, ProjectSpecification spec, bool init = true)
        {
            if (init){writer.Initialize(spec);}
            writer.WriteClasses(spec.Classes);
            writer.WriteInterfaces(spec.Interfaces);
            writer.WriteStructs(spec.Structs);
            writer.WriteDelegates(spec.Delegates);
            writer.WriteEnums(spec.Enums);
            if (init){writer.Reset();}
        }

        public static void WriteInterfaces(this IWriter writer, IEnumerable<InterfaceSpecification> spec)
        {
            foreach (var s in spec)
            {
                writer.WriteInterface(s);
            }
        }

        public static void WriteStructs(this IWriter writer, IEnumerable<StructSpecification> spec)
        {
            foreach (var s in spec)
            {
                writer.WriteStruct(s);
            }
        }

        public static void WriteClasses(this IWriter writer, IEnumerable<ClassSpecification> spec)
        {
            foreach (var s in spec)
            {
                writer.WriteClass(s);
            }
        }

        public static void WriteEnums(this IWriter writer, IEnumerable<EnumSpecification> spec)
        {
            foreach (var s in spec)
            {
                writer.WriteEnum(s);
            }
        }

        public static void WriteDelegates(this IWriter writer, IEnumerable<DelegateSpecification> spec)
        {
            foreach (var s in spec)
            {
                writer.WriteDelegate(s);
            }
        }
    }
}