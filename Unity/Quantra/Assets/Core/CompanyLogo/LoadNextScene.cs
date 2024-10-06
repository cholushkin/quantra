using Gamelib;
using UnityEngine;

public class LoadNextScene : MonoBehaviour
{
    public string NextSceneName;
    public float Delay;
    void Start()
    {
        Invoke("LoadScene", Delay);
    }

    void LoadScene()
    {
        if (SceneLoader.Instance == null)
        {
            Debug.LogWarning("No SceneLoader.Instance. You need to run current scene with dependencies");
            return;
        }
        
        SceneLoader.Instance.Replace("Gameplay", "CompanyLogo", true);
    }
}
