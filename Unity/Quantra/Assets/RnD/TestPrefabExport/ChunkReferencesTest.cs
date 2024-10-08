using TowerGenerator;
using UnityEngine;


// ## Solution
// - We keep chunk prefabs as copy of model chunk prefab.
//   Unity treats Model Prefabs differently from regular Prefabs because their structure is determined
//   by the external FBX file and cannot be modified directly in Unity

// ## Tested scenarios
// 1. Scene overrides doesn't work. If you have chunk instance on the scene and you add some scripts to it in the scene without applying to the prefab
// on reexporting chunk you will lose them. The problem that it happens not in 100% cases, but you obviously can't rely on it
// 2. Same for the external ref to prefab instance objects. Inconsistency of surviving ref. For example
//    GameObject1 has ChunkReferencesTest with RefToRenderer == QuantumCat/BgPlane 
//    GameObject2 has ChunkReferencesTest with RefToRenderer == QuantumCat/HeadContainer/Ears/Ears.v0
//    GameObject1 will lose such ref, GameObject2 will not lose it 

// ## Workarounds
// - You dont keep references to chunk prefab from anywhere. Exception the root prefab instance reference(?)
// - All attached scripts and refs inside chunk should be only inside prefab itself

public class ChunkReferencesTest : MonoBehaviour
{
    [Tooltip("SAFE")]
    public GameObject RefToRootGameObjectOfPrefabInstance;
    [Tooltip("UNSAFE")]
    public ChunkControllerBase RefToControllerBase;
    [Tooltip("UNSAFE")]
    public Renderer RefToRenderer;
    [Tooltip("UNSAFE")]
    public Renderer RefToRenderer2;
    
    
    void Start()
    {
        
    }


}
