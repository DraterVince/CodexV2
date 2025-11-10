# Save/Load System Setup Guide - Multiple Save Slots

## Overview
Your game now supports **6 separate save slots** for different game progressions. Each slot can store:
- Username
- Levels unlocked
- Current money
- Unlocked cosmetics
- Last played date/time

## Files Created/Modified

### New Files:
1. **`Assets/Scripts/NewAndLoadGameManager.cs`** - Main save/load manager (Updated)
2. **`Assets/Scripts/SaveLoadUI.cs`** - UI controller for save/load menu
3. **`Assets/Scripts/SaveLoadSlot.cs`** - Individual slot display component

### Modified Files:
1. **`Assets/PlayCardButton.cs`** - Added auto-save when level completes

## Features

### ? Core Features
- **6 Save Slots** - Support for up to 6 different game progressions
- **New Game** - Start fresh in any empty slot
- **Load Game** - Continue from any saved slot
- **Delete Save** - Remove unwanted save data
- **Auto-Save** - Automatically saves progress after level completion
- **Manual Save** - Save anytime during gameplay
- **Supabase Sync** - Syncs with cloud when logged in
- **Local Storage** - Uses PlayerPrefs for offline play

### ? What Each Slot Stores
```csharp
- username (string)
- levelsUnlocked (int)
- currentMoney (int)
- unlockedCosmetics (string - JSON array)
- lastPlayed (string - datetime)
- isEmpty (bool)
```

## Unity Scene Setup

### Step 1: Add Save/Load Manager to Your Scene

1. **Create Empty GameObject**:
   - Right-click in Hierarchy
   - Create Empty
   - Name it: "NewAndLoadGameManager"

2. **Add Script**:
   - Select the GameObject
   - Add Component > Scripts > NewAndLoadGameManager
   - This persists across scenes (DontDestroyOnLoad)

### Step 2: Create Save/Load Menu Scene

1. **Create New Scene**:
   - File > New Scene
   - Save as "SaveLoadMenu"

2. **Add to Build Settings**:
   - File > Build Settings
   - Add Open Scenes
   - Make sure it's in the list

### Step 3: Create Save/Load UI

#### A. Create Canvas
```
Hierarchy:
??? Canvas
?   ??? SaveLoadUI (Empty GameObject)
?   ??? SlotContainer (Vertical Layout Group)
?   ??? BackButton
?   ??? ConfirmationDialog (Panel)
?       ??? ConfirmText (TextMeshProUGUI)
?       ??? YesButton
?       ??? NoButton
```

#### B. Setup SlotContainer
1. Add **Vertical Layout Group** component
2. Settings:
   - Spacing: 10
   - Child Alignment: Upper Center
   - Child Force Expand: Width = true

#### C. Create Slot Prefab

1. **Create Slot GameObject**:
```
SlotPrefab:
??? Background (Image)
??? SlotNumber (TextMeshProUGUI)
??? DataContainer
?   ??? Username (TextMeshProUGUI)
?   ??? Level (TextMeshProUGUI)
?   ??? Money (TextMeshProUGUI)
?   ??? LastPlayed (TextMeshProUGUI)
??? EmptySlotIndicator (TextMeshProUGUI) - "Empty Slot"
??? LoadButton (Button)
??? DeleteButton (Button)
```

2. **Add SaveLoadSlot Script**:
   - Select SlotPrefab
   - Add Component > SaveLoadSlot
   - Assign all UI references in Inspector

3. **Save as Prefab**:
   - Drag SlotPrefab to Project folder
   - Delete from scene

#### D. Setup SaveLoadUI Component

1. Select SaveLoadUI GameObject
2. Add Component > SaveLoadUI
3. Assign in Inspector:
   - **Slot Prefab**: Your saved SlotPrefab
   - **Slot Container**: The Vertical Layout Group GameObject
   - **Back Button**: Reference to back button
   - **Confirmation Dialog**: Reference to dialog panel
   - **Confirmation Text**: Reference to text
   - **Confirm Yes/No Buttons**: References to buttons

## Usage in Code

### Starting a New Game
```csharp
// Start new game in slot 1
NewAndLoadGameManager.Instance.NewGame(1);
```

### Loading a Game
```csharp
// Load game from slot 3
NewAndLoadGameManager.Instance.LoadGame(3);
```

### Saving Current Progress
```csharp
// Manual save
NewAndLoadGameManager.Instance.SaveCurrentGame();
```

### Auto-Save (Already Implemented)
```csharp
// Called automatically in PlayCardButton when level completes
NewAndLoadGameManager.Instance.AutoSave();
```

### Deleting a Slot
```csharp
// Delete slot 2
NewAndLoadGameManager.Instance.DeleteSlot(2);
```

### Checking if Slot is Empty
```csharp
bool isEmpty = NewAndLoadGameManager.Instance.IsSlotEmpty(1);
if (isEmpty)
{
    // Show "New Game" option
}
else
{
    // Show "Load Game" option
}
```

### Getting Slot Data for UI
```csharp
var data = NewAndLoadGameManager.Instance.GetSlotData(1);
if (data != null && !data.isEmpty)
{
    Debug.Log($"Slot 1: {data.username}, Level {data.levelsUnlocked}");
}
```

### Syncing with Supabase
```csharp
// Sync current slot with cloud
await NewAndLoadGameManager.Instance.SyncWithSupabase();
```

## Integration with Existing Systems

### Your Game Flow

1. **Main Menu** ? Player selects "Continue" or "New Game"
2. **Save/Load Menu** ? Shows 6 slots
3. **Player Selects Slot** ? Either starts new game or loads existing
4. **Game Plays** ? Progress auto-saves after each level
5. **Player Can Manually Save** ? Anytime from pause menu

### Adding Manual Save to Pause Menu

```csharp
public class PauseMenu : MonoBehaviour
{
    public void OnSaveClicked()
    {
        NewAndLoadGameManager.Instance.SaveCurrentGame();
        Debug.Log("Game saved!");
        // Show confirmation message
    }
}
```

### Adding to Main Menu

```csharp
public class MainMenu : MonoBehaviour
{
    public void OnContinueClicked()
    {
        // Go to save/load menu
        SceneManager.LoadScene("SaveLoadMenu");
    }
    
    public void OnNewGameClicked()
    {
        // Also go to save/load menu
        // Let player choose which slot
        SceneManager.LoadScene("SaveLoadMenu");
    }
}
```

## Supabase Integration

The system automatically syncs with Supabase when:
- Level is completed (auto-save)
- Player manually saves
- Player calls `SyncWithSupabase()`

### Loading from Supabase to a Slot
```csharp
// Load cloud data into slot 1
await NewAndLoadGameManager.Instance.LoadFromSupabase(1);
```

## Data Flow

### New Game Flow:
```
1. Player clicks slot ? NewGame(slot)
2. Creates GameData with defaults
3. Saves to PlayerPrefs with slot key
4. Sets as CurrentSlot
5. Loads game scene
```

### Load Game Flow:
```
1. Player clicks slot ? LoadGame(slot)
2. Loads GameData from PlayerPrefs
3. Applies data to PlayerPrefs (current game state)
4. Sets as CurrentSlot
5. Loads game scene
```

### Auto-Save Flow:
```
1. Level completes
2. AutoSave() called
3. Gets current PlayerPrefs data
4. Saves to current slot
5. Optionally syncs to Supabase
```

## Example UI Layout

```
????????????????????????????????????????
?          SAVE / LOAD GAME           ?
????????????????????????????????????????
?  Slot 1                         ?   ?
?  Player1 | Level 10 | 500 coins     ?
?  Last Played: 2025-01-15 14:30       ?
?  [Load]                              ?
????????????????????????????????????????
?  Slot 2                         ?   ?
?  CodeMaster | Level 5 | 200 coins   ?
?  Last Played: 2025-01-14 10:15       ?
?  [Load]                              ?
????????????????????????????????????????
?  Slot 3                              ?
?  [Empty Slot - Start New Game]       ?
?  [New Game]                          ?
????????????????????????????????????????
?              [Back]                  ?
????????????????????????????????????????
```

## API Reference

### NewAndLoadGameManager

| Method | Description |
|--------|-------------|
| `NewGame(int slot)` | Start new game in slot |
| `LoadGame(int slot)` | Load game from slot |
| `SaveCurrentGame()` | Save to current slot |
| `DeleteSlot(int slot)` | Delete slot data |
| `IsSlotEmpty(int slot)` | Check if slot has data |
| `GetSlotData(int slot)` | Get slot info for UI |
| `GetAllSlots()` | Get all slots info |
| `AutoSave()` | Quick save to current slot |
| `SyncWithSupabase()` | Sync with cloud |
| `LoadFromSupabase(int slot)` | Load cloud ? slot |

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `CurrentSlot` | int | Active save slot (1-6) |
| `Instance` | static | Singleton instance |

## Best Practices

1. **Always use a slot** - Don't let CurrentSlot be 0
2. **Auto-save often** - After important events
3. **Show save confirmations** - Let player know data was saved
4. **Handle errors gracefully** - Check if Instance exists
5. **Test slot limits** - Don't exceed 6 slots
6. **Clear old saves** - Give players option to delete

## Testing Checklist

- [ ] Can create new game in empty slot
- [ ] Can load game from filled slot
- [ ] Slot data displays correctly
- [ ] Can delete save data
- [ ] Auto-save works after level completion
- [ ] Manual save works
- [ ] Multiple slots work independently
- [ ] Last played time updates
- [ ] Money and level sync correctly
- [ ] Supabase sync works (if logged in)
- [ ] Works offline with PlayerPrefs
- [ ] Scene transitions work properly

## Troubleshooting

### Slot not saving
- Check if NewAndLoadGameManager exists in scene
- Verify CurrentSlot is set (> 0)
- Check Unity console for errors

### Data not loading
- Verify slot has data (not empty)
- Check PlayerPrefs keys: `SaveSlot_1` through `SaveSlot_6`
- Try deleting PlayerPrefs and starting fresh

### Auto-save not working
- Ensure NewAndLoadGameManager.Instance is not null
- Verify CurrentSlot is set
- Check if AutoSave() is being called

### UI not showing slots
- Verify SlotPrefab is assigned
- Check SlotContainer is assigned
- Ensure SaveLoadSlot script is on prefab

## Advanced Features (Optional)

### Cloud-Only Save
```csharp
// Save only to Supabase, not local
public async Task SaveToCloudOnly(int slot)
{
    if (PlayerDataManager.Instance != null)
    {
        await PlayerDataManager.Instance.SyncGameData();
    }
}
```

### Import/Export Saves
```csharp
// Export slot data as JSON string
string json = JsonUtility.ToJson(GetSlotData(1));

// Import from JSON
GameData data = JsonUtility.FromJson<GameData>(json);
SaveToSlot(1, data);
```

---

**Status**: ? Complete and Ready to Use
**Files**: 3 new scripts + 1 modified
**Slots**: 6 maximum
**Storage**: PlayerPrefs (local) + Supabase (cloud)
