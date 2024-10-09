using UnityEngine;

public class WorldChunk : MonoBehaviour
{
	public Vector3Int WorldIndex;

	public void Init(Vector3Int worldIndex, Vector3 chunkSize)
	{
		transform.localScale = chunkSize;
		WorldIndex = worldIndex;
	}
}
