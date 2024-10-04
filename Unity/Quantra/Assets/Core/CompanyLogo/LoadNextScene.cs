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
        SceneLoader.Instance.Replace("Gameplay", "CompanyLogo", true);
       
    }
}
