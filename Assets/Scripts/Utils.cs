using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// Container for various utility functions
/// </summary>
public static class Utils
{
    public static T GetRandomItemInList<T>(List<T> list) where T : class
    {
        return list[Random.Range(0, list.Count)];
    }

    public static void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = Random.Range(0, n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    } 

    public static void QuitGame()
    {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    /// <summary>
    /// Extracts the method name from compiler-generated lambda names like "<Foo>b__0"
    /// </summary>
    /// <param name="del"></param>
    /// <returns></returns>
    public static string GetReadableMethodName(Delegate del)
    {
        string raw = del.Method.Name;

        int start = raw.IndexOf('<');
        int end = raw.IndexOf('>');

        if (start >= 0 && end > start)
            return raw.Substring(start + 1, end - start - 1);

        return raw;
    }
}
