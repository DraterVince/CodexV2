# Complete Save/Load System Guide - 6 Slot System

## Overview
This system allows players to have up to 6 different game save slots, each with independent progress. Players can create new games, load existing saves, and delete saves.

## Files Created

### 1. **New AndLoadGameManager.cs** (Already exists at `Assets/NewAndLoadGameManager.cs`)
- Handles all save/load logic
- Manages 6 save slots
- Auto-save functionality
- Supabase sync support

### 2. **SaveLoadUI.cs** (Create at `Assets/Scripts/SaveLoadUI.cs`)
- Manages the save/load screen UI
- Creates and manages save slot UI elements
- Handles confirmation dialogs

### 3. **SaveLoadSlot.cs** (Create at `Assets/Scripts/SaveLoadSlot.cs`)
- Individual slot UI display
- Shows empty/filled slot states
- Handles button clicks for each slot

## Unity Setup

### Step 1: Create the Save/Load Scene

1. Create a new scene: **File > New Scene**
2. Save it as `SaveLoadScreen`

### Step 2: Create the UI Hierarchy

```
Canvas
??? SaveLoadUI (Empty GameObject with SaveLoadUI.cs)
??? SlotContainer (Vertical Layout Group)
?   ??? (Slots will be spawned here)
??? ConfirmationPanel
?   ??? Background (Image)
?   ??? MessageText (TextMeshProUGUI)
?   ??? YesButton (Button)
?   ??? NoButton (Button)
??? EventSystem
```

### Step 3: Create the Slot Prefab

1. Create new GameObject: **Right-click Hierarchy > Create Empty**
2. Name it `SaveSlot`
3. Add components and children:

```
SaveSlot (with SaveLoadSlot.cs component)
??? Background (Image)
??? SlotNumberText (TextMeshProUGUI) - "Slot X"
??? EmptySlotPanel
?   ??? EmptyText (TextMeshProUGUI) - "Empty Slot"
?   ??? NewGameButton (Button) - "New Game"
??? FilledSlotPanel
?   ??? UsernameText (TextMeshProUGUI)
?   ??? LevelText (TextMeshProUGUI) - "Level: X"
?   ??? MoneyText (TextMeshProUGUI) - "Money: X"
?   ??? LoadButton (Button) - "Load"
?   ??? DeleteButton (Button) - "Delete"
```

4. Drag `SaveSlot` to Project window to create prefab
5. Delete from Hierarchy

### Step 4: Setup SlotContainer

1. Select `SlotContainer` in Hierarchy
2. Add Component > Layout > Vertical Layout Group
3. Settings:
   - Child Alignment: Upper Center
   - Control Child Size: Width ?, Height ?
   - Child Force Expand: Width ?
   - Spacing: 10

4. Add Component > Layout > Content Size Fitter
   - Vertical Fit: Preferred Size

### Step 5: Assign References in SaveLoadUI

1. Select `SaveLoadUI` GameObject
2. In Inspector, assign:
   - **Slot Prefab**: Drag your SaveSlot prefab
   - **Slot Container**: Drag SlotContainer GameObject
   - **Confirmation Panel**: Drag ConfirmationPanel
   - **Confirmation Text**: Drag MessageText
   - **Confirm Yes Button**: Drag YesButton
   - **Confirm No Button**: Drag NoButton
   - **Max Slots**: 6

### Step 6: Setup Slot Prefab References

1. Select your SaveSlot prefab in Project
2. Assign references in SaveLoadSlot component:
   - **Slot Number Text**: SlotNumberText
   - **Username Text**: UsernameText (in FilledSlotPanel)
   - **Level Text**: LevelText (in FilledSlotPanel)
   - **Money Text**: MoneyText (in FilledSlotPanel)
   - **New Game Button**: NewGameButton (in EmptySlotPanel)
   - **Load Game Button**: LoadButton (in FilledSlotPanel)
   - **Delete Button**: DeleteButton (in FilledSlotPanel)
   - **Empty Slot Panel**: EmptySlotPanel
   - **Filled Slot Panel**: FilledSlotPanel

## Code Files

### SaveLoadUI.cs

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveLoadUI : MonoBehaviour
{
    [Header("Slot Prefab")]
    public GameObject slotPrefab;
    
    [Header("Slot Container")]
    public Transform slotContainer;
    
    [Header("Confirmation Panel")]
    public GameObject confirmationPanel;
    public TextMeshProUGUI confirmationText;
    public Button confirmYesButton;
    public Button confirmNoButton;
    
    [Header("Settings")]
    public int maxSlots = 6;
    
    private NewAndLoadGameManager saveLoadManager;
    private int pendingSlot = -1;
    private bool isNewGame = false;
    
    private void Start()
    {
        saveLoadManager = NewAndLoadGameManager.Instance;
        if (saveLoadManager == null)
        {
            GameObject managerObj = new GameObject("SaveLoadManager");
            saveLoadManager = managerObj.AddComponent<NewAndLoadGameManager>();
        }
        
        CreateSlots();
        
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
            
            if (confirmYesButton != null)
                confirmYesButton.onClick.AddListener(OnConfirmYes);
            
            if (confirmNoButton != null)
                confirmNoButton.onClick.AddListener(OnConfirmNo);
        }
    }
    
    private void CreateSlots()
    {
        if (slotContainer == null || slotPrefab == null)
        {
            Debug.LogError("Slot container or prefab not assigned!");
            return;
        }
        
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }
        
        for (int i = 1; i <= maxSlots; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotContainer);
            SaveLoadSlot slot = slotObj.GetComponent<SaveLoadSlot>();
            
            if (slot != null)
            {
                slot.Initialize(i, this);
                slot.UpdateDisplay();
            }
        }
    }
    
    public void OnNewGameClicked(int slotNumber)
    {
        if (!saveLoadManager.IsSlotEmpty(slotNumber))
        {
            pendingSlot = slotNumber;
            isNewGame = true;
            ShowConfirmation($"Slot {slotNumber} already has saved data. Overwrite?");
        }
        else
        {
            StartNewGame(slotNumber);
        }
    }
    
    public void OnLoadGameClicked(int slotNumber)
    {
        if (!saveLoadManager.IsSlotEmpty(slotNumber))
        {
            LoadGame(slotNumber);
        }
    }
    
    public void OnDeleteClicked(int slotNumber)
    {
        pendingSlot = slotNumber;
        isNewGame = false;
        ShowConfirmation($"Delete save data in Slot {slotNumber}?");
    }
    
    private void ShowConfirmation(string message)
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
            if (confirmationText != null)
                confirmationText.text = message;
        }
    }
    
    private void OnConfirmYes()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
        
        if (pendingSlot > 0)
        {
            if (isNewGame)
            {
                StartNewGame(pendingSlot);
            }
            else
            {
                DeleteSlot(pendingSlot);
            }
        }
        
        pendingSlot = -1;
    }
    
    private void OnConfirmNo()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
        
        pendingSlot = -1;
    }
    
    private void StartNewGame(int slotNumber)
    {
        saveLoadManager.NewGame(slotNumber);
        RefreshSlot(slotNumber);
        // Scene will be loaded by NewAndLoadGameManager
    }
    
    private void LoadGame(int slotNumber)
    {
        saveLoadManager.LoadGame(slotNumber);
        // Scene will be loaded by NewAndLoadGameManager
    }
    
    private void DeleteSlot(int slotNumber)
    {
        saveLoadManager.DeleteSlot(slotNumber);
        RefreshSlot(slotNumber);
    }
    
    private void RefreshSlot(int slotNumber)
    {
        foreach (Transform child in slotContainer)
        {
            SaveLoadSlot slot = child.GetComponent<SaveLoadSlot>();
            if (slot != null && slot.slotNumber == slotNumber)
            {
                slot.UpdateDisplay();
                break;
            }
        }
    }
}
```

### SaveLoadSlot.cs

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveLoadSlot : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI slotNumberText;
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI moneyText;
    public Button newGameButton;
    public Button loadGameButton;
    public Button deleteButton;
    public GameObject emptySlotPanel;
    public GameObject filledSlotPanel;
    
    [HideInInspector]
    public int slotNumber;
    
    private SaveLoadUI uiManager;
    private NewAndLoadGameManager saveLoadManager;
    
    public void Initialize(int slot, SaveLoadUI manager)
    {
        slotNumber = slot;
        uiManager = manager;
        saveLoadManager = NewAndLoadGameManager.Instance;
        
        if (newGameButton != null)
            newGameButton.onClick.AddListener(() => uiManager.OnNewGameClicked(slotNumber));
        
        if (loadGameButton != null)
            loadGameButton.onClick.AddListener(() => uiManager.OnLoadGameClicked(slotNumber));
        
        if (deleteButton != null)
            deleteButton.onClick.AddListener(() => uiManager.OnDeleteClicked(slotNumber));
    }
    
    public void UpdateDisplay()
    {
        if (slotNumberText != null)
            slotNumberText.text = "Slot " + slotNumber;
        
        var data = saveLoadManager.GetSlotData(slotNumber);
        
        if (data != null && !data.isEmpty)
        {
            ShowFilledSlot(data);
        }
        else
        {
            ShowEmptySlot();
        }
    }
    
    private void ShowFilledSlot(NewAndLoadGameManager.GameData data)
    {
        if (emptySlotPanel != null)
            emptySlotPanel.SetActive(false);
        
        if (filledSlotPanel != null)
            filledSlotPanel.SetActive(true);
        
        if (usernameText != null)
            usernameText.text = data.username;
        
        if (levelText != null)
            levelText.text = "Level: " + data.levelsUnlocked;
        
        if (moneyText != null)
            moneyText.text = "Money: " + data.currentMoney;
        
        if (loadGameButton != null)
            loadGameButton.interactable = true;
        
        if (deleteButton != null)
            deleteButton.interactable = true;
    }
    
    private void ShowEmptySlot()
    {
        if (emptySlotPanel != null)
            emptySlotPanel.SetActive(true);
        
        if (filledSlotPanel != null)
            filledSlotPanel.SetActive(false);
        
        if (loadGameButton != null)
            loadGameButton.interactable = false;
        
        if (deleteButton != null)
            deleteButton.interactable = false;
    }
}
```

## Usage in Your Game

### Auto-Save After Level Complete

In your `PlayCardButton.cs` or level completion code:

```csharp
if (enemyManager.counter == enemyManager.enemies.Count)
{
    // Level completed
    nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
    if (nextLevel > PlayerPrefs.GetInt("levelAt"))
    {
        PlayerPrefs.SetInt("levelAt", nextLevel);
    }
    
    // AUTO-SAVE
    if (NewAndLoadGameManager.Instance != null)
    {
        NewAndLoadGameManager.Instance.AutoSave();
    }
    
    YouWinScreen.SetActive(true);
}
```

### Auto-Save After Money Changes

In your `MoneyManager.cs`:

```csharp
public void UpdateMoney(int amount)
{
    moneyCount = amount;
    PlayerPrefs.SetInt("moneyCount", moneyCount);
    PlayerPrefs.Save();
    moneyText.text = moneyCount.ToString();
    
    // AUTO-SAVE
    if (NewAndLoadGameManager.Instance != null)
    {
        NewAndLoadGameManager.Instance.AutoSave();
    }
}
```

## Testing

1. **Run SaveLoadScreen**
2. Click "New Game" on Slot 1
3. Game should start with fresh data
4. Play a level, earn money
5. Go back to SaveLoadScreen
6. Slot 1 should show your progress
7. Try creating a new game in Slot 2
8. Both slots should maintain independent progress

## Features

? 6 independent save slots  
? Auto-save on level complete  
? Auto-save on money change  
? Empty/Filled slot states  
? Confirmation dialogs  
? Delete save functionality  
? Supabase sync support  
? Works with existing PlayerPrefs system  

## Troubleshooting

### Slots not showing up
- Check SlotContainer has Vertical Layout Group
- Check SlotPrefab is assigned in SaveLoadUI
- Check SaveLoadSlot component exists on prefab

### Can't click buttons
- Check EventSystem exists in scene
- Check buttons have Button component
- Check Canvas is set to Screen Space - Overlay

### Data not saving
- Check NewAndLoadGameManager exists (check for Instance in console)
- Check AutoSave() is called after important events
- Check PlayerPrefs keys match ("levelAt", "moneyCount", etc.)

---

**Your save/load system is now complete!** Players can have 6 different save files with independent progress.
