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
public class StateMiner : MonoBehaviour
{
    public ChunkControllerBase ChunkController;
    private Dictionary<string, string> MinedStates = new Dictionary<string, string>(); // hash state
    private bool _newStatesMined = false;
    private bool _isMining = true;
    
    public void Awake()
    {
        if (enabled)
            DeserializeMinedState();
    }

    [Button]
    async UniTask MineLoop()
    {
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
    void StopMining()
    {
        _isMining = false;
    }

    public void TryMineState(ChunkControllerBase chunkController)
    {
        var (state, hash) = StateString(chunkController);
        if (!MinedStates.ContainsKey(hash))
        {
            MinedStates.Add(hash, state);
            _newStatesMined = true;
            Debug.Log($"New state has been mined {MinedStates.Count}");
        }
    }

    private (string hash, string data) StateString(ChunkControllerBase chunkController)
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
        return (GetSHA256Hash(strData), strData);
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
        return Path.Combine(Application.persistentDataPath, $"{ChunkController.name}_MinedStates.json");
    }

    // Serialize mined states to a JSON file
    private void SerializeMinedState()
    {
        if (!_newStatesMined)
            return;
        
        var saveFilePath = GetSaveFilePath();
        Debug.Log($"Serialize to {saveFilePath}");

        try
        {
            string jsonData = JsonUtility.ToJson(new SerializableDictionary(MinedStates), true);
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
                SerializableDictionary loadedData = JsonUtility.FromJson<SerializableDictionary>(jsonData);
                MinedStates = loadedData.ToDictionary();
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
                MinedStates.Clear(); // Clear the dictionary after deleting the file
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
}

// Helper class to make Dictionary serializable
[Serializable]
public class SerializableDictionary
{
    public List<string> keys = new List<string>();
    public List<string> values = new List<string>();

    public SerializableDictionary(Dictionary<string, string> dictionary)
    {
        foreach (var kvp in dictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public Dictionary<string, string> ToDictionary()
    {
        var dictionary = new Dictionary<string, string>();
        for (int i = 0; i < keys.Count; i++)
        {
            dictionary[keys[i]] = values[i];
        }
        return dictionary;
    }
}

#endif
