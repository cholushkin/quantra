using NaughtyAttributes;
using UnityEngine;

public class ScreenshotTaker : MonoBehaviour
{
	[InfoBox("Don't forget to set game viewport to screenshot resolution")]
	public Camera Camera; // Reference to the camera

	public void TakeScreenshot(string directory, string filename)
	{
		// Create a RenderTexture with the desired size
		int width = Mathf.RoundToInt(Screen.width);
		int height = Mathf.RoundToInt(Screen.height);
        
		RenderTexture renderTexture = new RenderTexture(width, height, 24);
		Camera.targetTexture = renderTexture;
		Camera.Render();

		RenderTexture.active = renderTexture;

		// Create a new Texture2D and read pixels from the RenderTexture
		Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
		screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		screenshot.Apply();

		// Reset the camera's target texture
		Camera.targetTexture = null;
		RenderTexture.active = null;
		Destroy(renderTexture); // Cleanup

		// Save the screenshot to a file
		byte[] bytes = screenshot.EncodeToPNG();
		string filePath = $"{directory}/{filename}.png";
		System.IO.File.WriteAllBytes(filePath, bytes);
		Debug.Log($"Screenshot saved to: {filePath}");
        
		// Clean up
		Destroy(screenshot);
	}
}