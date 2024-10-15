using System;
using GameLib.Alg;
using GameLib.ColorScheme;
using NaughtyAttributes;
using UnityEngine;

namespace Core
{
    public class Environment : Singleton<Environment>
    {
        [Header("Settings")]
        // -------------------
        public OceanSettings OceanSettings;

        public OceanBubblesSettings OceanBubbleSettings;


        [Header("References")]
        // -------------------
        [Required]
        public WorldStreamingPointer StreamingPointer;

        [Required] public GameObject FloorCollisionSurface;


        #region Logic

        private void Start()
        {
        }

        void Update()
        {
            // Process environment
            // Process ocean
            // Process chunks
        }

        // todo: On world chunk enter 
        // todo: On world chunk quit

        #endregion


        #region API

        public ColorScheme GetColorScheme(Vector2Int worldChunkSampleIndex)
        {
            throw new NotImplementedException();
        }

        public OceanSettings.Sample GetOceanSettingsSample(Vector3Int worldChunkSampleIndex)
        {
            // todo: sample using noise map, or some function
            return OceanSettings.DefaultSample;
        }

        #endregion
    }
}