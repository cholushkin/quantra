using System.Collections.Generic;
using Core;
using UnityEngine;

public class OceanController : MonoBehaviour
{
    public OceanSettings OceanSettings;
    public WorldStreamingPointer StreamingPointer;

    // pair(rounded position of the ocean tile surface instance, instance)
    private readonly Dictionary<Vector3, GameObject> _loadedSurfaces = new();


    void Update()
    {
        Vector3 pointerPosition = StreamingPointer.transform.position;
        var oceanSettings = Environment.Instance.GetOceanSettingsSample(StreamingPointer.WorldSpatialIndex);

        LoadOceanTilesAroundPointer(pointerPosition);
        UnloadDistantOceanTiles();
    }

    private void LoadOceanTilesAroundPointer(Vector3 pointer)
    {
        // Loop through a horizontal range around the current pointer (in x and z axes) to load neighboring tiles
        for (int x = -OceanSettings.SpawnRange.x; x <= OceanSettings.SpawnRange.x; x++)
        for (int z = -OceanSettings.SpawnRange.y; z <= OceanSettings.SpawnRange.y; z++)
        {
            var y = 0f;
            var roundedPos = new Vector3(x + (int)pointer.x, y, z + (int)pointer.z);

            // Only load the surface if it's not already loaded
            if (!_loadedSurfaces.ContainsKey(roundedPos))
            {
                SpawnSurface(roundedPos);
            }
        }
    }

    private void UnloadDistantOceanTiles()
    {
        List<Vector3> surfacesToUnload = new();
        var pointerPos = StreamingPointer.transform.position;

        foreach (var surface in _loadedSurfaces)
        {
            var distance = Vector3.Distance(surface.Key, pointerPos);

            if (distance > OceanSettings.TileUnloadDistance )
                surfacesToUnload.Add(surface.Key);
        }

        foreach (var chunkIndexToRemove in surfacesToUnload)
        {
            UnloadSurface(chunkIndexToRemove);
        }
    }
    
    private void SpawnSurface(Vector3 roundedPos)
    {
        var oceanTile = Instantiate(OceanSettings.OceanTilePrefab, roundedPos, Quaternion.identity);
        _loadedSurfaces.Add(roundedPos, oceanTile);
    }
    
    private void UnloadSurface(Vector3 roundedPos)
    {
        if (_loadedSurfaces.ContainsKey(roundedPos))
        {
            Destroy(_loadedSurfaces[roundedPos].gameObject);
            _loadedSurfaces.Remove(roundedPos);
        }
    }
}