using UnityEngine;

public static class HeightMapGenerator
{
    public static HeightMap GenerateHeightMap(HeightMapSettings settings, Vector2 sampleCentre)
    {
        float[,] values = Noise.GenerateNoiseMap(settings.Width, settings.Depth, settings.noiseSettings, sampleCentre);

        AnimationCurve heightCurve = new AnimationCurve(settings.heightCurve.keys);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int i = 0; i < settings.Width; i++)
        {
            for (int j = 0; j < settings.Depth; j++)
            {
                values[i, j] *= heightCurve.Evaluate(values[i, j]) * settings.heightMultiplier;

                if (values[i, j] > maxValue)
                {
                    maxValue = values[i, j];
                }

                if (values[i, j] < minValue)
                {
                    minValue = values[i, j];
                }
            }
        }

        return new HeightMap(values, minValue, maxValue);
    }
}

public struct HeightMap
{
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;

    public HeightMap(float[,] values, float minValue, float maxValue)
    {
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}