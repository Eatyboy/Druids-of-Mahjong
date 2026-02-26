using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

public class EffectConfigScriptGenerator : EditorWindow
{
    [MenuItem("Tools/Flower Tiles/Generate Config Scripts (With Fields)")]
    public static void GenerateConfigs()
    {
        var runtimeTypes = GetEffectTypes();

        if (!runtimeTypes.Any())
        {
            Debug.LogWarning("No FlowerTileEffect subclasses found.");
            return;
        }

        string folder = "Assets/Scripts/FlowerTiles/Configs";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        foreach (var type in runtimeTypes)
        {
            GenerateConfigForType(type, folder);
        }

        AssetDatabase.Refresh();
        Debug.Log($"Generated config scripts for {runtimeTypes.Count} effect(s).");
    }

    private static List<Type> GetEffectTypes()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                typeof(FlowerTileEffect).IsAssignableFrom(t))
            .ToList();
    }

    private static string GetTypeName(Type type)
    {
        if (type.IsGenericType)
        {
            var genericType = type.GetGenericTypeDefinition();
            var genericArgs = type.GetGenericArguments();

            string typeName = genericType.Name;
            int backtickIndex = typeName.IndexOf('`');
            if (backtickIndex > 0)
                typeName = typeName.Substring(0, backtickIndex);

            string args = string.Join(", ", genericArgs.Select(GetTypeName));
            return $"{typeName}<{args}>";
        }

        if (type.IsArray)
        {
            return $"{GetTypeName(type.GetElementType())}[]";
        }

        // Handle common primitive aliases
        if (type == typeof(int)) return "int";
        if (type == typeof(float)) return "float";
        if (type == typeof(bool)) return "bool";
        if (type == typeof(string)) return "string";
        if (type == typeof(double)) return "double";
        if (type == typeof(object)) return "object";

        return type.Name;
    }
    private static string GetValueLiteral(object value, Type type)
    {
        if (value == null)
            return "null";

        if (type == typeof(int))
            return value.ToString();

        if (type == typeof(float))
            return ((float)value).ToString("0.0######") + "f";

        if (type == typeof(double))
            return ((double)value).ToString("0.0######");

        if (type == typeof(bool))
            return value.ToString().ToLower();

        if (type == typeof(string))
            return $"\"{value}\"";

        if (type.IsEnum)
            return $"{type.Name}.{value}";

        // Skip complex types (Queue, List, etc)
        return null;
    }

    private static void GenerateConfigForType(Type runtimeType, string folder)
    {
        string configName = runtimeType.Name + "Config";
        string filePath = Path.Combine(folder, configName + ".cs");

        if (File.Exists(filePath))
        {
            Debug.Log($"Config already exists: {configName}. Deleting file.");
            File.Delete(filePath);
        }

        var fields = runtimeType
            .GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Where(f => f.GetCustomAttribute<ConfigurableAttribute>() != null)
            .ToArray();

        var tempInstance = Activator.CreateInstance(runtimeType);

        var sb = new StringBuilder();

        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine();
        sb.AppendLine("[System.Serializable]");
        sb.AppendLine("public class " + configName + " : FlowerTileEffectConfig");
        sb.AppendLine("{");

        foreach (var field in fields)
        {
            object value = field.GetValue(tempInstance);
            string defaultValue = GetValueLiteral(value, field.FieldType);

            if (defaultValue != null)
                sb.AppendLine($"    public {GetTypeName(field.FieldType)} {field.Name} = {defaultValue};");
            else
                sb.AppendLine($"    public {GetTypeName(field.FieldType)} {field.Name};");
        }

        sb.AppendLine();
        sb.AppendLine("    public override FlowerTileEffect CreateInstance()");
        sb.AppendLine("    {");
        sb.AppendLine("        var rt = new " + runtimeType.Name + "();");

        // Copy fields into runtime
        foreach (var field in fields)
        {
            sb.AppendLine($"        rt.{field.Name} = {field.Name};");
        }

        sb.AppendLine("        return rt;");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        sb.AppendLine();

        File.WriteAllText(filePath, sb.ToString());

        Debug.Log($"Generated config script: {filePath}");
    }
}
