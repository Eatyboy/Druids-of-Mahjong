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

    private static void GenerateConfigForType(Type runtimeType, string folder)
    {
        string configName = runtimeType.Name + "Config";
        string filePath = Path.Combine(folder, configName + ".cs");

        if (File.Exists(filePath))
        {
            Debug.Log($"Config already exists: {configName}. Deleting file.");
            File.Delete(filePath);
        }

        var fields = runtimeType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        var sb = new StringBuilder();

        sb.AppendLine("using UnityEngine;");
        sb.AppendLine();
        sb.AppendLine("[System.Serializable]");
        sb.AppendLine("public class " + configName + " : FlowerTileEffectConfig");
        sb.AppendLine("{");

        // Generate fields
        foreach (var field in fields)
        {
            sb.AppendLine($"    public {field.FieldType.FullName} {field.Name};");
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
