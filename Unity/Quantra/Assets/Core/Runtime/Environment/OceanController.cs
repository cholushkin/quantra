using System.Collections.Generic;
using Core;
using GameLib;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OceanController : MonoBehaviour
{
    public OceanSettings OceanSettings;
    public WorldStreamingPointer StreamingPointer;

    // Pair(rounded position of the ocean tile surface instance, instance)
    private readonly Dictionary<Vector3, GameObject> _loadedSurfaces = new();
    private Camera _mainCamera;
    

    void Start()
    {
        // Cache the main camera for frustum calculations
        _mainCamera = Camera.main;
    }

    void Update()
    {
        Vector3 pointerPosition = StreamingPointer.transform.position;
        var oceanSettings = Environment.Instance.GetOceanSettingsSample(StreamingPointer.WorldSpatialIndex);

        LoadOceanTilesAroundPointer(pointerPosition);
        UnloadDistantOceanTiles();
    }

    private void LoadOceanTilesAroundPointer(Vector3 pointer)
    {
        // Calculate the base rounded position of the pointer once
        int baseX = Mathf.FloorToInt(pointer.x / OceanSettings.OceanTileSize);
        int baseZ = Mathf.FloorToInt(pointer.z / OceanSettings.OceanTileSize);
        
        // Loop through a horizontal range around the current pointer (in x and z axes) to load neighboring tiles
        for (int x = -OceanSettings.SpawnRange.x; x <= OceanSettings.SpawnRange.x; x++)
        {
            for (int z = -OceanSettings.SpawnRange.y; z <= OceanSettings.SpawnRange.y; z++)
            {
                var y = 0f;
                var roundedPos = new Vector3(
                    x * OceanSettings.OceanTileSize + baseX * OceanSettings.OceanTileSize,
                    y,
                    z * OceanSettings.OceanTileSize + baseZ * OceanSettings.OceanTileSize);

                // Only load the surface if it's not already loaded and is visible
                if (!_loadedSurfaces.ContainsKey(roundedPos) /*&& IsTileVisible(roundedPos)*/)
                {
                    SpawnSurface(roundedPos);
                }
            }
        }
    }

    private bool IsTileVisible(Vector3 tilePosition)
    {
        // Create a plane array for the camera's frustum
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(_mainCamera);
        // Check if the tile's bounding box is within the frustum
        Bounds tileBounds = new Bounds(tilePosition, new Vector3(OceanSettings.OceanTileSize, 1, OceanSettings.OceanTileSize));
        return GeometryUtility.TestPlanesAABB(frustumPlanes, tileBounds);
    }

    private void UnloadDistantOceanTiles()
    {
        List<Vector3> surfacesToUnload = new();
        var pointerPos = StreamingPointer.transform.position;

        foreach (var surface in _loadedSurfaces)
        {
            var distanceSqr = (surface.Key - pointerPos).sqrMagnitude; // Use sqrMagnitude for efficiency

            if (distanceSqr > OceanSettings.TileUnloadDistance * OceanSettings.TileUnloadDistance)
            {
                surfacesToUnload.Add(surface.Key);
            }
        }

        foreach (var chunkIndexToRemove in surfacesToUnload)
        {
            UnloadSurface(chunkIndexToRemove);
        }
    }

    private void SpawnSurface(Vector3 roundedPos)
    {
        var oceanTile = Instantiate(OceanSettings.OceanTilePrefab, transform);
        oceanTile.transform.position = roundedPos;
        oceanTile.transform.localScale = Vector3.one * OceanSettings.OceanTileSize;
        _loadedSurfaces.Add(roundedPos, oceanTile);
    }

    private void UnloadSurface(Vector3 roundedPos)
    {
        if (_loadedSurfaces.TryGetValue(roundedPos, out var surface))
        {
            Destroy(surface.gameObject);
            _loadedSurfaces.Remove(roundedPos);
        }
    }
}
