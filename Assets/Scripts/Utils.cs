using System.Collections.Generic;
using UnityEngine;

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
}
