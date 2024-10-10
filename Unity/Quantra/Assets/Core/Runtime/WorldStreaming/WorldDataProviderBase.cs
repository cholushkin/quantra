using UnityEngine;


interface IWorldDataProvider
{
    ChunkData GetChunkData(Vector3Int worldIndex);
}

public class ChunkData
{
    public Vector3Int WorldIndex;
    public Vector3 Size;

    // todo: save data
}


public class WorldDataProviderBase : MonoBehaviour, IWorldDataProvider
{
    public ChunkData GetChunkData(Vector3Int worldIndex)
    {
        return new ChunkData { WorldIndex = worldIndex, Size = Vector3.one * 5f };
    }
}