using UnityEngine;
using System.Collections.Generic;

public class WorldChunkManager : MonoBehaviour
{
    // Prefab for the chunk object to be instantiated when loading new chunks
    public WorldChunk ChunkPrefab;

    // Reference to the object that moves through the world and determines which chunks to load/unload
    public WorldStreamingPointer StreamingPointer;

    // The distance, in chunks, at which chunks will be unloaded if they are too far from the current chunk
    public int UnloadChunkDistance = 2;

    // The physical dimensions of a chunk in world units (e.g., meters). Each chunk has this width, height, and depth.
    public Vector3 ChunkSize = new Vector3(50, 60, 80);

    // The total size of the world in terms of chunk indices. Defines the wrap-around behavior at world edges.
    public Vector3Int WorldSize = new Vector3Int(50, 60, 80);

    // The range around the current chunk (in chunk indices) to keep loaded. Determines how many neighboring chunks to load.
    public int chunkRange = 1;

    // Dictionary to track loaded chunks by their chunk index. Allows quick lookup and management of loaded chunks.
    private Dictionary<Vector3Int, WorldChunk> loadedChunks = new Dictionary<Vector3Int, WorldChunk>();
    private Vector3Int previousWorldIndex;  // Stores the previous world index
    
    void Start()
    {
        // Initialize previousWorldIndex to a different value from the initial world index to ensure proper first update
        previousWorldIndex = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
    }
    
    void Update()
    {
        // Convert the streaming pointer's world position into a chunk index
        Vector3 pointerPosition = StreamingPointer.transform.position;
        Vector3Int currentWorldIndex = PositionToWorldIndex(pointerPosition);

        // Check if the world index has changed
        if (currentWorldIndex != previousWorldIndex)
        {
            // Update the world index in the streaming pointer
            StreamingPointer.WorldIndex = currentWorldIndex;

            // Load chunks around the new current chunk
            LoadChunksAroundCurrentChunk();

            // Unload distant chunks
            UnloadDistantChunks();

            // Update previousWorldIndex to the new value
            previousWorldIndex = currentWorldIndex;
        }
    }

    // Converts a world position (in terms of Unity's world space) into a chunk index by dividing the position by the chunk size.
    // This allows us to figure out which chunk a given world position belongs to.
    private Vector3Int PositionToWorldIndex(Vector3 position)
    {
        return new Vector3Int(
            Mathf.FloorToInt(position.x / ChunkSize.x),  // Divide x position by chunk width
            Mathf.FloorToInt(position.y / ChunkSize.y),  // Divide y position by chunk height
            Mathf.FloorToInt(position.z / ChunkSize.z)   // Divide z position by chunk depth
        );
    }

    // Retrieves both the current chunk's world position and its index (relative to the world grid)
    private (Vector3 pos, Vector3Int worldIndex) GetCurrentChunkPos()
    {
        var index = StreamingPointer.WorldIndex;  // Current chunk index based on pointer
        var worldPos = Vector3.Scale(StreamingPointer.WorldIndex, ChunkSize);  // Calculate world position by scaling the index with chunk size
        return (worldPos, index);
    }

    // Loads all chunks within the defined range around the current chunk. This method ensures that all nearby chunks are instantiated and managed.
    private void LoadChunksAroundCurrentChunk()
    {
        // Get the current chunk index where the streaming pointer is located
        Vector3Int currentChunkIndex = StreamingPointer.WorldIndex;

        // Loop through a cubic range around the current chunk (in x, y, and z axes) to load neighboring chunks
        for (int x = -chunkRange; x <= chunkRange; x++)
        {
            for (int y = -chunkRange; y <= chunkRange; y++)
            {
                for (int z = -chunkRange; z <= chunkRange; z++)
                {
                    // Calculate the index of a new chunk within the range
                    Vector3Int newChunkIndex = new Vector3Int(
                        currentChunkIndex.x + x,
                        currentChunkIndex.y + y,
                        currentChunkIndex.z + z
                    );

                    // Ensure the chunk index wraps around correctly at the edges of the world
                    var wrappedIndex = WrapIndex(newChunkIndex);
                    
                    // todo: get data by wrappedIndex world index
                    // todo: spawn chunk by new newChunkIndex

                    // Only load the chunk if it's not already loaded
                    if (!loadedChunks.ContainsKey(newChunkIndex))
                    {
                        // Calculate the world position where the chunk should be placed
                        Vector3 chunkPosition = Vector3.Scale(newChunkIndex, ChunkSize);

                        // Load the chunk at the calculated world position
                        LoadChunk(newChunkIndex, chunkPosition);
                    }
                }
            }
        }
    }

    // Ensures that a chunk's index wraps around when it reaches the world boundary. 
    // For example, if a chunk goes beyond the world size, it will reappear on the opposite side (looping world).
    private Vector3Int WrapIndex(Vector3Int newChunkIndex)
    {
        var wrapped = newChunkIndex;
        wrapped.x = newChunkIndex.x % WorldSize.x;  // Wrap x index if it exceeds world bounds
        wrapped.y = newChunkIndex.y % WorldSize.y;  // Wrap y index if it exceeds world bounds
        wrapped.z = newChunkIndex.z % WorldSize.z;  // Wrap z index if it exceeds world bounds
        return wrapped;
    }

    // Loads a single chunk at a given chunk index and world position. Instantiates the chunk prefab and adds it to the dictionary of loaded chunks.
    private void LoadChunk(Vector3Int chunkIndex, Vector3 chunkPosition)
    {
        // Instantiate the chunk at the given world position
        WorldChunk newChunk = Instantiate(ChunkPrefab, chunkPosition, Quaternion.identity);
        newChunk.Init(chunkIndex, ChunkSize);  // Initialize the chunk with its index and size

        // Add the chunk to the dictionary of loaded chunks so we can manage it later
        loadedChunks.Add(chunkIndex, newChunk);
    }

    // Unloads chunks that are beyond the specified distance (UnloadChunkDistance) from the current chunk.
    // Uses chunk indices to calculate distance, ensuring performance is not dependent on world position.
    private void UnloadDistantChunks()
    {
        List<Vector3Int> chunksToUnload = new List<Vector3Int>();  // List to store chunks that need to be unloaded

        // Get the current chunk index where the streaming pointer is located
        Vector3Int currentChunkIndex = StreamingPointer.WorldIndex;

        // Loop through all currently loaded chunks and calculate their distance from the current chunk
        foreach (var chunk in loadedChunks)
        {
            // Calculate the Manhattan distance (sum of absolute differences) between the current chunk and each loaded chunk
            int distanceX = Mathf.Abs(currentChunkIndex.x - chunk.Key.x);
            int distanceY = Mathf.Abs(currentChunkIndex.y - chunk.Key.y);
            int distanceZ = Mathf.Abs(currentChunkIndex.z - chunk.Key.z);

            // If any chunk is farther than the unload distance in any dimension, mark it for unloading
            if (distanceX > UnloadChunkDistance || distanceY > UnloadChunkDistance || distanceZ > UnloadChunkDistance)
            {
                chunksToUnload.Add(chunk.Key);  // Add the chunk to the list of chunks to unload
            }
        }

        // Unload each chunk that was marked for removal
        foreach (var chunkIndexToRemove in chunksToUnload)
        {
            UnloadChunk(chunkIndexToRemove);
        }
    }

    // Unloads a specific chunk by its chunk index, removing it from the world and memory.
    private void UnloadChunk(Vector3Int chunkIndex)
    {
        // If the chunk exists in the loadedChunks dictionary, proceed with unloading
        if (loadedChunks.ContainsKey(chunkIndex))
        {
            // Destroy the chunk's GameObject to remove it from the scene
            Destroy(loadedChunks[chunkIndex].gameObject);

            // Remove the chunk from the dictionary to keep the loaded chunks list accurate
            loadedChunks.Remove(chunkIndex);
        }
    }

    // Visualize important elements related to chunk management, such as the current chunk, its center, and the streaming pointer.
    private void OnDrawGizmos()
    {
        // Get the current chunk's position and world index
        var curChunkPos = GetCurrentChunkPos();

        // Calculate the world position of the current chunk's center
        Vector3 chunkCenter = Vector3.Scale(curChunkPos.worldIndex, ChunkSize) + ((Vector3)ChunkSize / 2f);

        // Draw a green sphere at the center of the current chunk
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(chunkCenter, 0.25f);

        // Draw a yellow wireframe around the current chunk to visualize its bounds
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(chunkCenter, ChunkSize);

        // Draw a red sphere at the position of the streaming pointer for visual debugging
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(StreamingPointer.transform.position, 0.5f);
    }
}
