using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "HeightMapSettings", menuName = "Core/Landmass/HeightMapSettings")]
public class HeightMapSettings : UpdatableScriptableObject
{
    public NoiseSettings noiseSettings;
    public int Width; // x
    public int Depth; // z

    public bool useFalloff;

    public float heightMultiplier;
    public AnimationCurve heightCurve;

    public float minHeight
    {
        get { return heightMultiplier * heightCurve.Evaluate(0); }
    }

    public float maxHeight
    {
        get { return heightMultiplier * heightCurve.Evaluate(1); }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        noiseSettings.ValidateValues();
        base.OnValidate();
    }
#endif
}