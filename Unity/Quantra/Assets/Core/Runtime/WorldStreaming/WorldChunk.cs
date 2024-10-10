using UnityEngine;
using UnityEngine.Assertions;

public class WorldChunk : MonoBehaviour
{
	private ChunkData _chunkData;

	public void Init(ChunkData chunkData)
	{
		Assert.IsNotNull(chunkData);
		transform.localScale = chunkData.Size;
		_chunkData = chunkData;
		
		// todo: if debug
		name = $"{_chunkData.WorldIndex.x},{_chunkData.WorldIndex.y},{_chunkData.WorldIndex.z}";
	}
}
