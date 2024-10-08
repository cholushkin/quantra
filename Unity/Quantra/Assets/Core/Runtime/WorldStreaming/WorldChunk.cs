using UnityEngine;

public class WorldChunk : MonoBehaviour
{
	public Vector3Int WorldIndex;

	public void Init(Vector3Int worldIndex, Vector3Int chunkSize)
	{
		transform.localScale = chunkSize;
		WorldIndex = worldIndex;
	}
}
