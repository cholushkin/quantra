using UnityEngine;
using System.Collections.Generic;

public class WorldChunkManager : MonoBehaviour
{
    public WorldChunk ChunkPrefab; // Prefab of the chunk to instantiate
    public WorldStreamingPointer StreamingPointer;
    //public float LoadDistance = 150f; // Distance from the frame within which chunks will be loaded
    public float UnloadDistance = 200f; // Distance after which chunks will be unloaded
    public Vector3Int ChunkSize = new Vector3Int(50, 60, 80); // Size of each chunk
    public Vector3 WorldSize = new Vector3(5000, 6000, 8000); // World's size

    private Dictionary<Vector3Int, WorldChunk> loadedChunks = new Dictionary<Vector3Int, WorldChunk>(); // Keeps track of loaded chunks

    void Update()
    {
        // Update the world index based on the pointer position
        Vector3 pointerPosition = StreamingPointer.transform.position;
        StreamingPointer.WorldIndex = PositionToWorldIndex(pointerPosition);
    
        //LoadChunksAroundCurrentChunk();
        //
        // // Unload distant chunks
        // UnloadDistantChunks(Frame.WorldIndex);
    }
    
    // Convert world position to world index
    private Vector3Int PositionToWorldIndex(Vector3 position)
    {
        return new Vector3Int(
            Mathf.FloorToInt(position.x / ChunkSize.x),
            Mathf.FloorToInt(position.y / ChunkSize.y),
            Mathf.FloorToInt(position.z / ChunkSize.z)
        );
    }

    private (Vector3 pos, Vector3Int worldIndex) GetCurrentChunkPos()
    {
        var index = StreamingPointer.WorldIndex;
        var worldPos = Vector3.Scale(StreamingPointer.WorldIndex, ChunkSize);
        return (worldPos, index);
    }
    
    
    // private HashSet<Vector3Int> loadedChunks = new HashSet<Vector3Int>(); // Track loaded chunks
    // public int chunkRange = 2; // How many chunks to load around the current chunk
    //
    // private void LoadChunksAroundCurrentChunk()
    // {
    //     // Get the current chunk's world index
    //     Vector3Int currentChunkIndex = StreamingPointer.WorldIndex;
    //
    //     // Loop through chunks within the defined range in x, y, and z axes
    //     for (int x = -chunkRange; x <= chunkRange; x++)
    //     {
    //         for (int y = -chunkRange; y <= chunkRange; y++)
    //         {
    //             for (int z = -chunkRange; z <= chunkRange; z++)
    //             {
    //                 // Calculate the new chunk's index, with wrapping for x and y
    //                 Vector3Int newChunkIndex = new Vector3Int(
    //                     (currentChunkIndex.x + x + Mathf.FloorToInt(WorldSize.x / ChunkSize.x)) % Mathf.FloorToInt(WorldSize.x / ChunkSize.x),
    //                     (currentChunkIndex.y + y + Mathf.FloorToInt(WorldSize.y / ChunkSize.y)) % Mathf.FloorToInt(WorldSize.y / ChunkSize.y),
    //                     Mathf.Clamp(currentChunkIndex.z + z, 0, Mathf.FloorToInt(WorldSize.z / ChunkSize.z) - 1) // No wrapping for z
    //                 );
    //
    //                 // Check if this chunk is already loaded
    //                 if (!loadedChunks.Contains(newChunkIndex))
    //                 {
    //                     // If not loaded, calculate its world position
    //                     Vector3 chunkPosition = Vector3.Scale(newChunkIndex, ChunkSize);
    //
    //                     // Load the chunk
    //                     LoadChunk(newChunkIndex, chunkPosition);
    //                 }
    //             }
    //         }
    //     }
    // }

// Helper method to load a single chunk
    private void LoadChunk(Vector3Int chunkIndex, Vector3 chunkPosition)
    {
        WorldChunk newChunk = Instantiate(ChunkPrefab, chunkPosition, Quaternion.identity);
        newChunk.Init(chunkIndex, ChunkSize);

        // Add the chunk to the list of loaded chunks
        //loadedChunks.Add(chunkIndex);
    }

    
    //
    // // Load chunks around the frame based on the world index
    // private void LoadChunksAroundPointer(Vector3Int chunkIndex)
    // {
    //     int range = Mathf.CeilToInt(LoadDistance / ChunkSize.magnitude);
    //     
    //     for (int x = -range; x <= range; x++)
    //     {
    //         for (int y = -range; y <= range; y++)
    //         {
    //             // Wrap x and y coordinates based on world size
    //             Vector3Int newChunkIndex = new Vector3Int(
    //                 (chunkIndex.x + x + Mathf.FloorToInt(WorldSize.x / ChunkSize.x)) % Mathf.FloorToInt(WorldSize.x / ChunkSize.x),
    //                 Mathf.Clamp(chunkIndex.y + y, 0, Mathf.FloorToInt(WorldSize.y / ChunkSize.y) - 1), // Ensure y stays within bounds
    //                 chunkIndex.z // Z does not wrap
    //             );
    //
    //             Vector3 worldPosition = new Vector3(
    //                 newChunkIndex.x * ChunkSize.x,
    //                 newChunkIndex.y * ChunkSize.y,
    //                 newChunkIndex.z * ChunkSize.z
    //             );
    //
    //             // Load the chunk if not already loaded
    //             if (!loadedChunks.ContainsKey(newChunkIndex))
    //             {
    //                 if (Vector3.Distance(Frame.transform.position, worldPosition) <= LoadDistance)
    //                 {
    //                     LoadChunk(newChunkIndex, worldPosition);
    //                 }
    //             }
    //         }
    //     }
    // }
    //
    // // Load a single chunk
    // private void LoadChunk(Vector3Int chunkIndex, Vector3 position)
    // {
    //     WorldChunk newChunk = Instantiate(ChunkPrefab, position, Quaternion.identity);
    //     newChunk.Init(chunkIndex, ChunkSize);
    //     loadedChunks.Add(chunkIndex, newChunk);
    // }
    //
    // // Unload chunks that are too far away from the frame
    // private void UnloadDistantChunks(Vector3Int chunkIndex)
    // {
    //     List<Vector3Int> chunksToUnload = new List<Vector3Int>();
    //
    //     foreach (var chunk in loadedChunks)
    //     {
    //         if (Vector3.Distance(Frame.transform.position, chunk.Value.transform.position) > UnloadDistance)
    //         {
    //             chunksToUnload.Add(chunk.Key);
    //         }
    //     }
    //
    //     foreach (var chunkPosToRemove in chunksToUnload)
    //     {
    //         UnloadChunk(chunkPosToRemove);
    //     }
    // }
    //
    // // Unload a single chunk
    // private void UnloadChunk(Vector3Int chunkIndex)
    // {
    //     if (loadedChunks.ContainsKey(chunkIndex))
    //     {
    //         Destroy(loadedChunks[chunkIndex].gameObject);
    //         loadedChunks.Remove(chunkIndex);
    //     }
    // }


    private void OnDrawGizmos()
    {
        // Draw Streaming Pointer related gizmos
        {
            // Get the current chunk position and world index
            var curChunkPos = GetCurrentChunkPos();

            // Calculate the world position of the current chunk's center
            Vector3 chunkCenter = Vector3.Scale(curChunkPos.worldIndex, ChunkSize) + ((Vector3)ChunkSize / 2f);


            // Draw the center of the current chunk
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(chunkCenter, 0.5f);

            // Draw the wireframe of the current chunk (yellow)
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(chunkCenter, ChunkSize);

            // Draw the streaming pointer
            Gizmos.color = Color.red; // Adjust to a different color for better visibility
            Gizmos.DrawSphere(StreamingPointer.transform.position, 0.5f);
        }
    }
}
