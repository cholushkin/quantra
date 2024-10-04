using UnityEngine;

public class CameraFitToMesh : MonoBehaviour
{
	public Camera mainCamera;  // Reference to the camera
	public Renderer targetRenderer;  // The mesh renderer of the object

	void Start()
	{
		FitToScreen();
	}

	void FitToScreen()
	{
		// Get the bounds of the mesh
		Bounds bounds = targetRenderer.bounds;
        
		// Calculate the height of the mesh in world space
		float meshHeight = bounds.size.y;

		// Get the camera's distance to the object along the Z axis
		float distanceToCamera = Mathf.Abs(mainCamera.transform.position.z - targetRenderer.transform.position.z);

		// Adjust the orthographic size based on the mesh's height
		mainCamera.orthographicSize = meshHeight / 2f;
	}
}