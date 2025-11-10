using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Main menu controller for save/load UI
public class SaveLoadMenuUI : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject saveLoadPanel;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private GameObject newSavePanel;

    [Header("UI Elements")]
    [SerializeField] private Transform saveSlotContainer;
    [SerializeField] private GameObject saveSlotPrefab;
    [SerializeField] private Button newSaveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_InputField saveNameInput;
    [SerializeField] private TextMeshProUGUI confirmationText;
    [SerializeField] private TextMeshProUGUI modeText;

    [Header("Confirmation Buttons")]
    [SerializeField] private Button confirmYesButton;
    [SerializeField] private Button confirmNoButton;

    private List<SaveSlotUI> saveSlots = new List<SaveSlotUI>();
    private SaveFileInfo selectedSave;
    private bool isSaveMode = true;
    private System.Action<SaveFileInfo> pendingAction;

    private void Start()
    {
        // Setup button listeners
        if (newSaveButton) newSaveButton.onClick.AddListener(ShowNewSavePanel);
        if (loadButton) loadButton.onClick.AddListener(LoadSelectedSave);
        if (deleteButton) deleteButton.onClick.AddListener(ConfirmDeleteSave);
        if (backButton) backButton.onClick.AddListener(CloseMenu);
        if (confirmYesButton) confirmYesButton.onClick.AddListener(ExecutePendingAction);
        if (confirmNoButton) confirmNoButton.onClick.AddListener(HideConfirmationPanel);

        // Hide panels initially
        if (confirmationPanel) confirmationPanel.SetActive(false);
        if (newSavePanel) newSavePanel.SetActive(false);
        if (saveLoadPanel) saveLoadPanel.SetActive(false);
    }

    public void OpenSaveMenu()
    {
        isSaveMode = true;
        OpenMenu();
        if (modeText) modeText.text = "SAVE GAME";
        if (newSaveButton) newSaveButton.gameObject.SetActive(true);
        if (loadButton) loadButton.gameObject.SetActive(false);
    }

    public void OpenLoadMenu()
    {
        isSaveMode = false;
        OpenMenu();
        if (modeText) modeText.text = "LOAD GAME";
        if (newSaveButton) newSaveButton.gameObject.SetActive(false);
        if (loadButton) loadButton.gameObject.SetActive(true);
    }

    private void OpenMenu()
    {
        saveLoadPanel.SetActive(true);
        RefreshSaveSlots();
        Time.timeScale = 0f; // Pause game
    }

    private void CloseMenu()
    {
        saveLoadPanel.SetActive(false);
        Time.timeScale = 1f; // Resume game
    }

    private void RefreshSaveSlots()
    {
        // Clear existing slots
        foreach (var slot in saveSlots)
        {
            Destroy(slot.gameObject);
        }
        saveSlots.Clear();

        // Get all save files
        List<SaveFileInfo> saveFiles = SaveGameManager.Instance.GetAllSaveFiles();

        // Create UI slots for each save
        foreach (var saveFile in saveFiles)
        {
            GameObject slotObj = Instantiate(saveSlotPrefab, saveSlotContainer);
            SaveSlotUI slotUI = slotObj.GetComponent<SaveSlotUI>();

            if (slotUI != null)
            {
                slotUI.SetupSlot(saveFile, OnSaveSlotSelected);
                saveSlots.Add(slotUI);
            }
        }

        // Update button states
        UpdateButtonStates();
    }

    private void OnSaveSlotSelected(SaveFileInfo saveInfo)
    {
        selectedSave = saveInfo;

        // Update visual selection
        foreach (var slot in saveSlots)
        {
            slot.SetSelected(slot.SaveInfo == saveInfo);
        }

        UpdateButtonStates();

        // If in save mode and slot selected, ask to overwrite
        if (isSaveMode && selectedSave != null)
        {
            ConfirmOverwriteSave();
        }
    }

    private void UpdateButtonStates()
    {
        bool hasSelection = selectedSave != null;

        if (loadButton) loadButton.interactable = hasSelection && !isSaveMode;
        if (deleteButton) deleteButton.interactable = hasSelection;
    }

    private void ShowNewSavePanel()
    {
        newSavePanel.SetActive(true);
        saveNameInput.text = "Save_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        saveNameInput.Select();
    }

    public void CreateNewSave()
    {
        string saveName = saveNameInput.text;

        if (string.IsNullOrEmpty(saveName))
        {
            Debug.LogWarning("Save name cannot be empty!");
            return;
        }

        // Check if save already exists
        if (SaveGameManager.Instance.SaveFileExists(saveName + ".json"))
        {
            ShowConfirmation($"Save '{saveName}' already exists. Overwrite?", () =>
            {
                SaveGameManager.Instance.CreateNewSave(saveName);
                newSavePanel.SetActive(false);
                RefreshSaveSlots();
            });
        }
        else
        {
            SaveGameManager.Instance.CreateNewSave(saveName);
            newSavePanel.SetActive(false);
            RefreshSaveSlots();
        }
    }

    private void ConfirmOverwriteSave()
    {
        if (selectedSave == null) return;

        ShowConfirmation($"Overwrite save '{selectedSave.saveName}'?", () =>
        {
            SaveGameManager.Instance.SaveGame();
            RefreshSaveSlots();
        });
    }

    private void LoadSelectedSave()
    {
        if (selectedSave == null) return;

        ShowConfirmation($"Load save '{selectedSave.saveName}'? Unsaved progress will be lost.", () =>
        {
            if (SaveGameManager.Instance.LoadGame(selectedSave.fileName))
            {
                CloseMenu();
            }
        });
    }

    private void ConfirmDeleteSave()
    {
        if (selectedSave == null) return;

        ShowConfirmation($"Delete save '{selectedSave.saveName}'? This cannot be undone.", () =>
        {
            if (SaveGameManager.Instance.DeleteSaveFile(selectedSave.fileName))
            {
                selectedSave = null;
                RefreshSaveSlots();
            }
        });
    }

    private void ShowConfirmation(string message, System.Action onConfirm)
    {
        confirmationPanel.SetActive(true);
        confirmationText.text = message;
        pendingAction = (info) => onConfirm();
    }

    private void ExecutePendingAction()
    {
        pendingAction?.Invoke(selectedSave);
        HideConfirmationPanel();
    }

    private void HideConfirmationPanel()
    {
        confirmationPanel.SetActive(false);
        pendingAction = null;
    }
}

// Individual save slot UI component
public class SaveSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI saveNameText;
    [SerializeField] private TextMeshProUGUI saveDateText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI playTimeText;
    [SerializeField] private TextMeshProUGUI sceneText;
    [SerializeField] private Button selectButton;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

    private SaveFileInfo saveInfo;
    private System.Action<SaveFileInfo> onSelectCallback;

    public SaveFileInfo SaveInfo => saveInfo;

    public void SetupSlot(SaveFileInfo info, System.Action<SaveFileInfo> onSelect)
    {
        saveInfo = info;
        onSelectCallback = onSelect;

        // Update UI texts
        if (saveNameText) saveNameText.text = info.saveName;
        if (saveDateText) saveDateText.text = info.GetFormattedDate();
        if (levelText) levelText.text = $"Level {info.playerLevel}";
        if (playTimeText) playTimeText.text = info.GetFormattedPlayTime();
        if (sceneText) sceneText.text = info.sceneName;

        // Setup button
        if (selectButton)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onSelectCallback?.Invoke(saveInfo));
        }

        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        if (backgroundImage)
        {
            backgroundImage.color = selected ? selectedColor : normalColor;
        }
    }
}

// Example Player Controller that uses the save system
public class PlayerController : MonoBehaviour
{
    [SerializeField] private int playerLevel = 1;
    [SerializeField] private float health = 100f;
    [SerializeField] private float mana = 50f;
    [SerializeField] private int currency = 0;

    private void Start()
    {
        // Load player data if save exists
        LoadPlayerData();
    }

    private void LoadPlayerData()
    {
        var saveData = SaveGameManager.Instance.CurrentSaveData;
        if (saveData != null)
        {
            playerLevel = saveData.playerLevel;
            health = saveData.playerHealth;
            mana = saveData.playerMana;
            currency = saveData.currency;
            transform.position = saveData.playerPosition;
        }
    }

    public void SavePlayerData()
    {
        SaveGameManager.Instance.UpdatePlayerData(playerLevel, health, mana, transform.position);
        SaveGameManager.Instance.UpdateCurrency(currency);
    }

    // Call this before saving
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SavePlayerData();
            SaveGameManager.Instance.QuickSave();
        }
    }

    private void OnApplicationQuit()
    {
        SavePlayerData();
        SaveGameManager.Instance.QuickSave();
    }
}