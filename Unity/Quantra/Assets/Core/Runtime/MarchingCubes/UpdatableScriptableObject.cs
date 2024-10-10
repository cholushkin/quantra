using UnityEngine;

public class UpdatableScriptableObject : ScriptableObject
{
    public event System.Action OnValuesUpdated;

    protected virtual void OnValidate()
    {
        NotifyOfUpdatedValues();
    }

    public void NotifyOfUpdatedValues()
    {
        if (OnValuesUpdated != null)
            OnValuesUpdated();
    }
}