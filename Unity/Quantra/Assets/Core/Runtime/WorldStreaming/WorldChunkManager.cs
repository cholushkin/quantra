using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

public class WorldChunkManager : MonoBehaviour
{
    [Required]
    public WorldDataProviderBase WorldDataProvider;
        
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
    private Vector3Int _previousChunkSpatialIndex;  // Stores the previous world index
    private Vector3Int _currentChunkWrappedIndex;
    private Vector3Int _currentChunkSpatialIndex;
    
    void Start()
    {
        // Initialize previousWorldIndex to a different value from the initial world index to ensure proper first update
        _previousChunkSpatialIndex = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
    }
    
    void Update()
    {
        // Convert the streaming pointer's world position into a chunk index
        Vector3 pointerPosition = StreamingPointer.transform.position;
        _currentChunkSpatialIndex = PositionToWorldIndex(pointerPosition);
        _currentChunkWrappedIndex = WrapIndex(_currentChunkSpatialIndex);

        // Check if the world index has changed
        if (_currentChunkSpatialIndex != _previousChunkSpatialIndex)
        {
            // Update the world index in the streaming pointer
            StreamingPointer.WorldSpatialIndex = _currentChunkSpatialIndex;

            // Load chunks around the new current chunk
            LoadChunksAroundCurrentChunk();

            // Unload distant chunks
            UnloadDistantChunks();

            // Update previousWorldIndex to the new value
            _previousChunkSpatialIndex = _currentChunkSpatialIndex;
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


    // Loads all chunks within the defined range around the current chunk. This method ensures that all nearby chunks are instantiated and managed.
    private void LoadChunksAroundCurrentChunk()
    {
        // Loop through a cubic range around the current chunk (in x, y, and z axes) to load neighboring chunks
        for (int x = -chunkRange; x <= chunkRange; x++)
        {
            for (int y = -chunkRange; y <= chunkRange; y++)
            {
                for (int z = -chunkRange; z <= chunkRange; z++)
                {
                    Vector3Int chunkSpatialIndex = new Vector3Int(
                        _currentChunkSpatialIndex.x + x,
                        _currentChunkSpatialIndex.y + y,
                        _currentChunkSpatialIndex.z + z
                    );

                    // Ensure the chunk index wraps around correctly at the edges of the world
                    Vector3Int chunkWrappedIndex = WrapIndex(chunkSpatialIndex);
                    
                    // Get data by wrappedIndex world index
                    var chunkData = WorldDataProvider.GetChunkData(chunkWrappedIndex);
                    
                    // Only load the chunk if it's not already loaded
                    if (!loadedChunks.ContainsKey(chunkWrappedIndex))
                    {
                        SpawnChunk(chunkData, chunkSpatialIndex, chunkWrappedIndex);
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

        wrapped.x = ((newChunkIndex.x % WorldSize.x) + WorldSize.x) % WorldSize.x;  // Ensure x is in range [0, WorldSize.x)
        wrapped.y = newChunkIndex.y;  // No wrapping for height
        wrapped.z = ((newChunkIndex.z % WorldSize.z) + WorldSize.z) % WorldSize.z;  // Ensure z is in range [0, WorldSize.z)

        return wrapped;
    }

    
    private void SpawnChunk(ChunkData chunkData, Vector3Int spatialWorldIndex, Vector3Int wrappedChunkIndex)
    {
        // Calculate the world position where the chunk should be placed
        Vector3 chunkPosition = Vector3.Scale(spatialWorldIndex, ChunkSize);
        
        // Instantiate the chunk at the given scene position
        WorldChunk newChunk = Instantiate(ChunkPrefab, chunkPosition, Quaternion.identity);
        newChunk.Init(chunkData);  // Initialize the chunk with its index and size
        
        // Add the chunk to the dictionary of loaded chunks so we can manage it later
        loadedChunks.Add(wrappedChunkIndex, newChunk);
    }
 

    // Unloads chunks that are beyond the specified distance (UnloadChunkDistance) from the current chunk.
    // Uses chunk indices to calculate distance, ensuring performance is not dependent on world position.
    private void UnloadDistantChunks()
    {
        List<Vector3Int> chunksToUnload = new List<Vector3Int>();  // List to store chunks that need to be unloaded

        // Get the current chunk index where the streaming pointer is located
        Vector3Int pointerChunkIndex = StreamingPointer.WorldSpatialIndex;
        

        // Loop through all currently loaded chunks and calculate their distance from the current chunk
        foreach (var chunk in loadedChunks)
        {
            var chunkPos = chunk.Value.transform.position;
            var chunkWorldIndex = PositionToWorldIndex(chunkPos);
            
            // Calculate the Manhattan distance (sum of absolute differences) between the current chunk and each loaded chunk
            int distanceX = Mathf.Abs(pointerChunkIndex.x - chunkWorldIndex.x);
            int distanceY = Mathf.Abs(pointerChunkIndex.y - chunkWorldIndex.y);
            int distanceZ = Mathf.Abs(pointerChunkIndex.z - chunkWorldIndex.z);

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
        var pointerWorldPos = StreamingPointer.transform.position;
        var curPointerChunkIndex = PositionToWorldIndex(pointerWorldPos);
        Vector3 chunkCenter = Vector3.Scale(curPointerChunkIndex, ChunkSize) + ChunkSize / 2f;

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
