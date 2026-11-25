using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class MemoryData
{
    public string id;
    public string title;
    public string photoPath;
    public float heading;
}

public class MemoryManager : MonoBehaviour
{
    public static MemoryManager Instance { get; private set; }

    public List<MemoryData> memories = new List<MemoryData>();
    private string _dataPath;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        _dataPath = Path.Combine(Application.persistentDataPath, "memories.json");
        LoadMemories();
    }

    public void AddMemory(string title, string photoPath, float heading)
    {
        MemoryData mem = new MemoryData
        {
            id = System.Guid.NewGuid().ToString(),
            title = title,
            photoPath = photoPath,
            heading = heading
        };
        memories.Add(mem);
        SaveMemories();
    }

    public void DeleteMemory(string id)
    {
        memories.RemoveAll(m => m.id == id);
        SaveMemories();
    }

    private void SaveMemories()
    {
        string json = JsonUtility.ToJson(new MemoryListWrapper { list = memories }, true);
        File.WriteAllText(_dataPath, json);
    }

    private void LoadMemories()
    {
        if (File.Exists(_dataPath))
        {
            string json = File.ReadAllText(_dataPath);
            MemoryListWrapper wrapper = JsonUtility.FromJson<MemoryListWrapper>(json);
            memories = wrapper.list ?? new List<MemoryData>();
        }
    }

    [System.Serializable]
    private class MemoryListWrapper { public List<MemoryData> list; }
}
