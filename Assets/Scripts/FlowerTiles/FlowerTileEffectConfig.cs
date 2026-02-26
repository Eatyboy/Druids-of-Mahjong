using System;
using UnityEngine;

[System.Serializable]
public abstract class FlowerTileEffectConfig
{
    public abstract FlowerTileEffect CreateInstance();
}

[AttributeUsage(AttributeTargets.Field)]
public class ConfigurableAttribute : Attribute { } 