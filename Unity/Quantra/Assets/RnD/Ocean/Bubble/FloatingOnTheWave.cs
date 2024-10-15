using UnityEngine;

public class FloatingOnTheWave : MonoBehaviour
{
    public Material waveMaterial; // Reference to the material using the shader
    public float tileSize; // Tile size of the wave

    public float posxTile;
    public float poszTile;

    public float FloatScale = 10;
    void Update()
    {
        // // Get the wave properties from the material
        // float waveFrequencyX = waveMaterial.GetFloat("_WaveFrequencyX");
        // float waveFrequencyZ = waveMaterial.GetFloat("_WaveFrequencyZ");
        // float waveAmplitude = waveMaterial.GetFloat("_WaveAmplitude");
        // float speed = waveMaterial.GetFloat("_Speed");
        //
        // // Get the object's X and Z position
        // Vector3 pos = transform.position;
        //
        // // Normalize the X and Z positions based on tile size
        // float normalizedX = pos.x % tileSize;
        // float normalizedZ = pos.z % tileSize;
        //
        // posxTile = pos.x % tileSize;
        // poszTile = pos.z % tileSize;
        //
        // // Calculate time value to simulate wave movement
        // float time = Time.time;
        //
        // // Recreate the wave displacement calculation
        // float waveX = Mathf.Sin(normalizedX * waveFrequencyX * 2 * Mathf.PI + time * speed) * waveAmplitude;
        // float waveZ = Mathf.Sin(normalizedZ * waveFrequencyZ * 2 * Mathf.PI + time * speed) * waveAmplitude;
        //
        // // Set the new Y position based on wave calculations
        // pos.y += (waveX + waveZ) * FloatScale;
        //
        // // Update the object's position
        // transform.position = pos;
    }
}