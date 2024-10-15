using System;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "OceanBubbleSettings",
        menuName = "Quantra/Core/World/OceanBubbleSettings")]
    public class OceanBubblesSettings : ScriptableObject
    {
        [Serializable]
        public class BubbleType
        {
            public GameObject Prefab;
            public float SpawnProb; // per ocean tile
        }
        
        public BubbleType[] BubblePrefabs;
        public int MaxBubblesPerTile;
        public bool UseObjectPool;
    }
}