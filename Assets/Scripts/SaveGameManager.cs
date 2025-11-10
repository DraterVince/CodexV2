using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Data class that holds all game data you want to save
[Serializable]
public class SaveData
{
    public string saveName;
    public DateTime saveDate;
    public int playerLevel;
    public float playerHealth;

    instance = this;
        DontDestroyOnLoad(gameObject);

    // Create saves folder path
    savesFolderPath = Path.Combine(Application.persistentDataPath, "Saves");
        if (!Directory.Exists(savesFolderPath))
        {
            Directory.CreateDirectory(savesFolderPath);
        }

sessionStartTime = Time.time;
    }
    
    // Create a new save file
    public void CreateNewSave(string saveName = null)
{
    currentSaveData = new SaveData();

    if (!string.IsNullOrEmpty(saveName))
    {
        currentSaveData.saveName = saveName;
    }
    else
    {
        // Auto-generate save name with timestamp
        currentSaveData.saveName = "Save_" + DateTime.Now.ToString(
    public float playerMana;
public Vector3 playerPosition;
public string currentScene;
public int currency;
public List<string> unlockedItems;
public Dictionary<string, bool> gameFlags;
public float playTime;

// Constructor for new save
public SaveData()
{
    saveName = "New Save";
    saveDate = DateTime.Now;
    playerLevel = 1;
    playerHealth = 100f;
    playerMana = 50f;
    playerPosition = Vector3.zero;
    currentScene = "MainScene";
    currency = 0;
    unlockedItems = new List<string>();
    gameFlags = new Dictionary<string, bool>();
    playTime = 0f;
}
}

// Main SaveGameManager - attach this to a GameObject in your scene
public class SaveGameManager : MonoBehaviour
{
    private static SaveGameManager instance;
    public static SaveGameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SaveGameManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("SaveGameManager");
                    instance = go.AddComponent<SaveGameManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    private SaveData currentSaveData;
    private string savesFolderPath;
    private float sessionStartTime;

    public SaveData CurrentSaveData => currentSaveData;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        "yyyyMMdd_HHmmss");
    }

    // Initialize with default values
    currentSaveData.saveDate = DateTime.Now;
        currentSaveData.currentScene = SceneManager.GetActiveScene().name;
        
        // Save immediately
        SaveGame();

    Debug.Log($"New save created: {currentSaveData.saveName}");
    }

// Save current game state
public void SaveGame(int slotIndex = -1)
{
    if (currentSaveData == null)
    {
        Debug.LogWarning("No save data to save!");
        return;
    }

    // Update save time
    currentSaveData.saveDate = DateTime.Now;
    currentSaveData.playTime += Time.time - sessionStartTime;
    sessionStartTime = Time.time;

    // Update current scene
    currentSaveData.currentScene = SceneManager.GetActiveScene().name;

    // Determine filename
    string fileName;
    if (slotIndex >= 0)
    {
        fileName = $"save_slot_{slotIndex}.json";
    }
    else
    {
        fileName = $"{currentSaveData.saveName}.json";
    }

    string savePath = Path.Combine(savesFolderPath, fileName);

    try
    {
        string json = JsonUtility.ToJson(currentSaveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Game saved to: {savePath}");
    }
    catch (Exception e)
    {
        Debug.LogError($"Failed to save game: {e.Message}");
    }
}

// Load game from file
public bool LoadGame(string saveFileName)
{
    string savePath = Path.Combine(savesFolderPath, saveFileName);

    if (!File.Exists(savePath))
    {
        Debug.LogError($"Save file not found: {savePath}");
        return false;
    }

    try
    {
        string json = File.ReadAllText(savePath);
        currentSaveData = JsonUtility.FromJson<SaveData>(json);
        sessionStartTime = Time.time;

        Debug.Log($"Game loaded from: {savePath}");

        // Load the scene if different
        if (SceneManager.GetActiveScene().name != currentSaveData.currentScene)
        {
            SceneManager.LoadScene(currentSaveData.currentScene);
        }

        return true;
    }
    catch (Exception e)
    {
        Debug.LogError($"Failed to load game: {e.Message}");
        return false;
    }
}

// Load game from slot
public bool LoadGameFromSlot(int slotIndex)
{
    string fileName = $"save_slot_{slotIndex}.json";
    return LoadGame(fileName);
}

// Get all available save files
public List<SaveFileInfo> GetAllSaveFiles()
{
    List<SaveFileInfo> saveFiles = new List<SaveFileInfo>();

    if (!Directory.Exists(savesFolderPath))
    {
        return saveFiles;
    }

    string[] files = Directory.GetFiles(savesFolderPath, "*.json");

    foreach (string file in files)
    {
        try
        {
            string json = File.ReadAllText(file);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            SaveFileInfo info = new SaveFileInfo
            {
                fileName = Path.GetFileName(file),
                saveName = data.saveName,
                saveDate = data.saveDate,
                playerLevel = data.playerLevel,
                playTime = data.playTime,
                sceneName = data.currentScene
            };

            saveFiles.Add(info);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Could not read save file {file}: {e.Message}");
        }
    }

    // Sort by date (newest first)
    saveFiles.Sort((a, b) => b.saveDate.CompareTo(a.saveDate));

    return saveFiles;
}

// Delete a save file
public bool DeleteSaveFile(string fileName)
{
    string savePath = Path.Combine(savesFolderPath, fileName);

    if (!File.Exists(savePath))
    {
        Debug.LogError($"Save file not found: {savePath}");
        return false;
    }

    try
    {
        File.Delete(savePath);
        Debug.Log($"Save file deleted: {fileName}");
        return true;
    }
    catch (Exception e)
    {
        Debug.LogError($"Failed to delete save file: {e.Message}");
        return false;
    }
}

// Quick save (overwrites current save)
public void QuickSave()
{
    if (currentSaveData == null)
    {
        CreateNewSave("QuickSave");
    }
    else
    {
        SaveGame();
    }
}

// Quick load (loads most recent save)
public bool QuickLoad()
{
    var saves = GetAllSaveFiles();
    if (saves.Count > 0)
    {
        return LoadGame(saves[0].fileName);
    }

    Debug.LogWarning("No save files found for quick load!");
    return false;
}

// Check if save file exists
public bool SaveFileExists(string fileName)
{
    string savePath = Path.Combine(savesFolderPath, fileName);
    return File.Exists(savePath);
}

// Auto-save functionality
public void EnableAutoSave(float intervalInSeconds = 300f)
{
    InvokeRepeating(nameof(AutoSave), intervalInSeconds, intervalInSeconds);
}

public void DisableAutoSave()
{
    CancelInvoke(nameof(AutoSave));
}

private void AutoSave()
{
    if (currentSaveData != null)
    {
        string originalName = currentSaveData.saveName;
        currentSaveData.saveName = "AutoSave";
        SaveGame();
        currentSaveData.saveName = originalName;
        Debug.Log("Auto-save completed");
    }
}

// Update specific save data fields
public void UpdatePlayerData(int level, float health, float mana, Vector3 position)
{
    if (currentSaveData != null)
    {
        currentSaveData.playerLevel = level;
        currentSaveData.playerHealth = health;
        currentSaveData.playerMana = mana;
        currentSaveData.playerPosition = position;
    }
}

public void UpdateCurrency(int amount)
{
    if (currentSaveData != null)
    {
        currentSaveData.currency = amount;
    }
}

public void AddUnlockedItem(string itemId)
{
    if (currentSaveData != null && !currentSaveData.unlockedItems.Contains(itemId))
    {
        currentSaveData.unlockedItems.Add(itemId);
    }
}

public void SetGameFlag(string flagName, bool value)
{
    if (currentSaveData != null)
    {
        currentSaveData.gameFlags[flagName] = value;
    }
}

public bool GetGameFlag(string flagName)
{
    if (currentSaveData != null && currentSaveData.gameFlags.ContainsKey(flagName))
    {
        return currentSaveData.gameFlags[flagName];
    }
    return false;
}
}

// Helper class for save file information
[Serializable]
public class SaveFileInfo
{
    public string fileName;
    public string saveName;
    public DateTime saveDate;
    public int playerLevel;
        return string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
    }
    public float playTime;
    public string sceneName;


    public string GetFormattedPlayTime()
{
    TimeSpan time = TimeSpan.FromSeconds(playTime);


    public string GetFormattedDate()
{
    return saveDate.ToString("yyyy-MM-dd HH:mm:ss");
}
}