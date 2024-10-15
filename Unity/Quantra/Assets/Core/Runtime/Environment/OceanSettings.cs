using System;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "OceanSettings",
        menuName = "Quantra/Core/World/OceanSettings")]
    public class OceanSettings : ScriptableObject
    {
        [Serializable]
        public class Sample
        {
            // parameters of the wave
        }

        public GameObject OceanTilePrefab;
        public Sample DefaultSample;
        public Vector2Int SpawnRange = new Vector2Int(16, 16);
        public Vector2Int OceanTileSize = new Vector2Int(20, 20);
        public float TileUnloadDistance = 400f;
    }
}