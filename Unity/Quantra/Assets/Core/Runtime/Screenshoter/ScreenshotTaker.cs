using UnityEngine;

public class ScreenshotTaker : MonoBehaviour
{
	public Camera camera; // Reference to the camera

	public void TakeScreenshot()
	{
		// Create a RenderTexture with the desired size
		int width = Mathf.RoundToInt(Screen.width);
		int height = Mathf.RoundToInt(Screen.height);
        
		RenderTexture renderTexture = new RenderTexture(width, height, 24);
		camera.targetTexture = renderTexture;
		camera.Render();

		RenderTexture.active = renderTexture;

		// Create a new Texture2D and read pixels from the RenderTexture
		Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
		screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		screenshot.Apply();

		// Reset the camera's target texture
		camera.targetTexture = null;
		RenderTexture.active = null;
		Destroy(renderTexture); // Cleanup

		// Save the screenshot to a file
		byte[] bytes = screenshot.EncodeToPNG();
		string filePath = $"{Application.persistentDataPath}/screenshot.png";
		System.IO.File.WriteAllBytes(filePath, bytes);
		Debug.Log($"Screenshot saved to: {filePath}");
        
		// Clean up
		Destroy(screenshot);
	}

	// For testing purposes, call TakeScreenshot in the Update method or through a button press
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.P)) // Press 'P' to take a screenshot
		{
			TakeScreenshot();
		}
	}
}