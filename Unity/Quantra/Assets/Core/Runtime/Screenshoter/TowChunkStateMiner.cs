#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameLib.Alg;
using TowerGenerator;
using UnityEngine;
using System.Security.Cryptography;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;

[Serializable]
public class TowChunkStateMiner : MonoBehaviour
{
    public class ChunkStates
    {
        [Serializable]
        public class State
        {
            public string Items; // Chunk active items
            public string Hash;
        }

        public string ChunkName;
        public List<State> States = new List<State>(); // List of mined states
    }

    public enum SaveDestination
    {
        PersistentStorage,
        Assets,
        DesignData
    }

    [Required] public ChunkControllerBase ChunkController;
    public ScreenshotTaker Screenshoter;
    public SaveDestination SaveDestinationType;
    public string OutputDirectory;
    public bool StartOnWake;
    
    private ChunkStates _chunkStates = new ChunkStates(); // Use ChunkStates instead of Dictionary
    private bool _newStatesMined = false;
    private bool _isMining = true;

    public void Awake()
    {
        if (enabled)
            DeserializeMinedState();
        if (StartOnWake)
            _ = MineLoop();
    }

    [Button]
    async UniTask MineLoop()
    {
        Debug.Log($"Start mining states for {ChunkController.name}");
        _isMining = true;
        while (_isMining)
        {
            ChunkController.SetConfiguration();
            TryMineState(ChunkController);
            await UniTask.DelayFrame(8);
        }
        SerializeMinedState();
    }

    [Button]
    void MineStep()
    {
        ChunkController.SetConfiguration();
        TryMineState(ChunkController);
    }

    [Button]
    void StopMining()
    {
        _isMining = false;
    }

    public void TryMineState(ChunkControllerBase chunkController)
    {
        var (stateData, hash) = StateString(chunkController);

        // Check if the state is already mined
        if (!_chunkStates.States.Exists(s => s.Hash == hash))
        {
            var newState = new ChunkStates.State { Items = stateData, Hash = hash };
            _chunkStates.States.Add(newState); // Add new state to ChunkStates
            _newStatesMined = true;
            
            if(Screenshoter)
                Screenshoter.TakeScreenshot(Path.GetDirectoryName(GetSaveFilePath()), newState.Hash);
            Debug.Log($"New state has been mined. Total mined states: {_chunkStates.States.Count}");
        }
    }

    private (string data, string hash) StateString(ChunkControllerBase chunkController)
    {
        List<string> debugNames = new List<string>();

        // Collect all debug names from the active children
        foreach (var child in chunkController.transform.TraverseBreadthFirst())
        {
            if (child.gameObject.activeInHierarchy)
            {
                debugNames.Add(child.GetDebugName());
            }
        }

        // Sort the collected names
        debugNames.Sort();

        // Build the final string using the sorted names
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var name in debugNames)
        {
            stringBuilder.AppendLine(name);
        }

        var strData = stringBuilder.ToString();
        return (strData, GetSHA256Hash(strData));
    }

    private static string GetSHA256Hash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }
    }

    private string GetSaveFilePath()
    {
        string fileName = $"{ChunkController.name}_MinedStates.json";

        switch (SaveDestinationType)
        {
            case SaveDestination.PersistentStorage:
                // Save to persistent storage
                return Path.Combine(Application.persistentDataPath, fileName);

            case SaveDestination.Assets:
                // Save within the Assets folder, using OutputDirectory
                if (string.IsNullOrEmpty(OutputDirectory))
                {
                    Debug.LogWarning("OutputDirectory is not specified. Saving to the root of Assets.");
                    return Path.Combine(Application.dataPath, fileName); // Default to root of Assets if not specified
                }
                else
                {
                    string assetsPath = Path.Combine(Application.dataPath, OutputDirectory);
                    Directory.CreateDirectory(assetsPath); // Ensure the directory exists
                    return Path.Combine(assetsPath, fileName);
                }

            case SaveDestination.DesignData:
                // Save to the root directory of the project (one level above Assets)
                string projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
                if (string.IsNullOrEmpty(projectRoot))
                {
                    Debug.LogError("Unable to determine the project root path.");
                    return null;
                }
                return Path.Combine(projectRoot, OutputDirectory, fileName);

            default:
                Debug.LogError("Unknown SaveDestinationType.");
                return null;
        }
    }

    // Serialize mined states to a JSON file
    private void SerializeMinedState()
    {
        if (!_newStatesMined)
            return;

        _chunkStates.ChunkName = ChunkController.name; // Set the chunk name before saving

        var saveFilePath = GetSaveFilePath();
        Debug.Log($"Serialize to {saveFilePath}");

        try
        {
            string jsonData = JsonUtility.ToJson(_chunkStates, true);
            File.WriteAllText(saveFilePath, jsonData);
            Debug.Log($"Mined states saved to {saveFilePath}");
            _newStatesMined = false; // Reset flag after saving
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error while saving mined states: {ex}");
        }
    }

    // Deserialize mined states from a JSON file
    private void DeserializeMinedState()
    {
        var saveFilePath = GetSaveFilePath();
        Debug.Log($"Deserialize from {saveFilePath}");

        if (File.Exists(saveFilePath))
        {
            try
            {
                string jsonData = File.ReadAllText(saveFilePath);
                _chunkStates = JsonUtility.FromJson<ChunkStates>(jsonData);
                Debug.Log("Mined states successfully loaded.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error while loading mined states: {ex}");
            }
        }
        else
        {
            Debug.Log("No mined states file found.");
        }
    }

    // Method to delete the serialized state file
    [Button]
    public void DeleteSerializedStateFile()
    {
        var saveFilePath = GetSaveFilePath();
        if (File.Exists(saveFilePath))
        {
            try
            {
                File.Delete(saveFilePath);
                _chunkStates.States.Clear(); // Clear the list after deleting the file
                _newStatesMined = false; // Reset the flag since we have no states now
                Debug.Log($"Mined states file deleted: {saveFilePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error while deleting mined states file: {ex}");
            }
        }
        else
        {
            Debug.Log("No mined states file found to delete.");
        }
    }
    
    // Called when the application quits or Play Mode is stopped in the editor
    private void OnApplicationQuit()
    {
        HandleOnStop();
    }

  

    private void HandleOnStop()
    {
        SerializeMinedState(); // Serialize the mined states before exiting
        Debug.Log("Mined states serialized upon stopping play mode or quitting the application.");
    }
}
#endif
