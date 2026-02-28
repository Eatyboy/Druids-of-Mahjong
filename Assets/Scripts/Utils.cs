using System;
using System.Collections.Generic;
using System.Linq;
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

    public static T GetRandomItemInArray<T>(T[] array) where T : class
    {
        return array[Random.Range(0, array.Length)];
    }

    public static T GetRandomItemInArray<T>(Array array) where T : class
    {
        return (T)array.GetValue(Random.Range(0, array.Length));
    }

    public static T GetRandomEnumValue<T>() where T : struct, Enum
    {
        Array values = Enum.GetValues(typeof(T));
        int randomIndex = Random.Range(0, values.Length);
        return (T)values.GetValue(randomIndex);
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

    public static Vector2 PointOnQuadraticBezierCurve2D(float t, Vector2 p1, Vector2 p2, Vector2 p3, float timeRange = 1.0f)
    {
        float newT = t / timeRange;
        return ((1.0f - (newT * newT)) * p1) + (2.0f * (1 - newT) * newT * p2) + ((newT * newT) * p3);
    }

    public static Vector2 SlopeOnQuadraticBezierCurve2D(float t, Vector2 p1, Vector2 p2, Vector2 p3, float timeRange = 1.0f)
    {
        float newT = t / timeRange;
        return (2.0f * (1 - newT) * (p2 - p1)) + (2.0f * newT * (p3 - p2));
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

    public static float ExpEaseIn(float t)
    {
        return t == 0.0f ? 0.0f : Mathf.Pow(2.0f, 10.0f * t - 10.0f);
    }

    public static float ExpEaseIn(float t, float intensity)
    {
        return t == 0.0f ? 0.0f : (Mathf.Exp(intensity * t) - 1.0f) / (Mathf.Exp(intensity) - 1.0f);
    }

    public static bool AreDisjoint<T>(IEnumerable<T> a, IEnumerable<T> b)
    {
        var set = new HashSet<T>(a);
        return !b.Any(set.Contains);
    }
    public static List<List<T>> GetAllKCombinations<T>(List<List<T>> items, int k)
    {
        var result = new List<List<T>>();
        int n = items.Count;
        if (k > n) return result;

        int[] indices = new int[k];
        for (int i = 0; i < k; i++) indices[i] = i;

        while (true)
        {
            var combo = new List<T>(k);
            for (int i = 0; i < k; i++)
            {
                combo.AddRange(items[indices[i]]);
            }
            result.Add(combo);

            int t;
            for (t = k - 1; t >= 0 && indices[t] == n - k + t; t--);
            if (t < 0) break;

            indices[t]++;
            for (int i = t + 1; i < k; i++)
            {
                indices[i] = indices[i - 1] + 1;
            }
        }

        return result;
    }
}
