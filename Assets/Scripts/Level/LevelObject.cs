using System;
using UnityEngine;

namespace Level
{
    public enum LevelObjectType
    {
        Coin,
        Barrier,
        Jumpable,
        Rollable,
    } 

    [Serializable]
    public class LevelObject
    {
        public LevelObjectType Type;
        public GameObject Prefab;
    }
}
