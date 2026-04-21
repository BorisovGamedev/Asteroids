using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassDiagramGenerator.Application.Abstractions;
using ClassDiagramGenerator.Domain.Models;

namespace ClassDiagramGenerator.Infrastructure.Export
{
    public sealed class PlantUmlExporter : IPlantUmlExporter
    {
        public string Export(IReadOnlyList<DiagramTypeModel> types, bool includeAssociations)
        {
            var sb = new StringBuilder();
            sb.AppendLine("@startuml");

            foreach (var type in types)
            {
                string typeKeyword = type.IsInterface ? "interface" : "class";
                string stereotype = !type.IsInterface && type.IsAbstract ? " <<abstract>>" : string.Empty;

                sb.AppendLine($"{typeKeyword} {type.Name}{stereotype} {{");

                foreach (var field in type.Fields)
                    sb.AppendLine($"    {field.Visibility} {field.Name} : {field.Type}");

                foreach (var property in type.Properties)
                    sb.AppendLine($"    {property.Visibility} {property.Name} : {property.Type} {{ get; set; }}");

                foreach (var method in type.Methods)
                {
                    string parameterList = string.Join(", ", method.Parameters.Select(p => $"{p.Name} : {p.Type}"));
                    sb.AppendLine($"    {method.Visibility} {method.Name}({parameterList}) : {method.ReturnType}");
                }

                sb.AppendLine("}");

                if (!string.IsNullOrWhiteSpace(type.Summary))
                    sb.AppendLine($"' {type.Name}: {type.Summary}");
            }

            foreach (var type in types)
            {
                if (!string.IsNullOrWhiteSpace(type.BaseType))
                    sb.AppendLine($"{StripGenerics(type.BaseType)} <|-- {type.Name}");

                foreach (string iface in type.Interfaces)
                    sb.AppendLine($"{StripGenerics(iface)} <|.. {type.Name}");
            }

            if (includeAssociations)
                AppendAssociations(sb, types);

            sb.AppendLine("@enduml");
            return sb.ToString();
        }

        private static void AppendAssociations(StringBuilder sb, IReadOnlyList<DiagramTypeModel> types)
        {
            var names = new HashSet<string>(types.Select(type => StripGenerics(type.Name)));
            var added = new HashSet<string>();

            foreach (var type in types)
            {
                string className = StripGenerics(type.Name);

                foreach (var field in type.Fields)
                {
                    string typeName = StripGenerics(field.Type);
                    if (names.Contains(typeName) && typeName != className && added.Add($"{className}-{typeName}-f"))
                        sb.AppendLine($"{className} --> {typeName} : field");
                }

                foreach (var property in type.Properties)
                {
                    string typeName = StripGenerics(property.Type);
                    if (names.Contains(typeName) && typeName != className && added.Add($"{className}-{typeName}-p"))
                        sb.AppendLine($"{className} --> {typeName} : property");
                }

                foreach (var method in type.Methods)
                {
                    foreach (var parameter in method.Parameters)
                    {
                        string typeName = StripGenerics(parameter.Type);
                        if (names.Contains(typeName) && typeName != className && added.Add($"{className}-{typeName}-a"))
                            sb.AppendLine($"{className} --> {typeName} : parameter");
                    }
                }
            }
        }

        private static string StripGenerics(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                return typeName;

            int index = typeName.IndexOf('<');
            return index >= 0 ? typeName.Substring(0, index) : typeName;
        }
    }
}