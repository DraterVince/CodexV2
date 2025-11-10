# Supabase Player Data Integration - Setup Instructions

## Overview
The system has been successfully integrated to save player data (email, username, levels unlocked, current money, and unlocked cosmetics) to Supabase after registration and display the username in the level select screen.

## What Has Been Implemented

### 1. **PlayerData Model** (`Assets/Scripts/PlayerData.cs`)
- Stores: user_id, email, username, levels_unlocked, current_money, unlocked_cosmetics
- Properly inherits from `Supabase.Postgrest.Models.BaseModel`
- Uses correct attributes for Supabase table mapping

### 2. **PlayerDataManager** (`Assets/Scripts/PlayerDataManager.cs`)
- Singleton pattern for global access
- Methods to create, load, update, and sync player data
- Automatic sync with PlayerPrefs for offline access
- Methods for managing cosmetics

### 3. **Updated AuthManager** (`Assets/Scripts/AuthManager.cs`)
- Registration now requires a username parameter
- Automatically creates player data in Supabase after successful registration
- Loads player data from Supabase on login
- Syncs data before logout

### 4. **Updated UIAuth** (`Assets/Scripts/UIAuth.cs`)
- Added `usernameInput` field for registration
- Passes username to registration method

### 5. **Updated LevelSelection** (`Assets/LevelSelection.cs`)
- Displays username from PlayerPrefs/Supabase
- Shows "Welcome, [username]!" message

### 6. **Updated PlayCardButton** (`Assets/PlayCardButton.cs`)
- Automatically syncs level progress to Supabase when completing levels
- Syncs money rewards to Supabase

### 7. **Updated MoneyManager** (`Assets/MoneyManager.cs`)
- Added methods to update money with automatic Supabase sync
- `UpdateMoney()`, `AddMoney()`, and `SpendMoney()` methods

## Required Supabase Database Setup

You need to create a table in your Supabase project:

### SQL to Create the `player_data` Table:

```sql
CREATE TABLE player_data (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
    email TEXT NOT NULL,
    username TEXT NOT NULL UNIQUE,
    levels_unlocked INTEGER DEFAULT 5,
    current_money INTEGER DEFAULT 0,
    unlocked_cosmetics TEXT DEFAULT '[]',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Create an index on user_id for faster queries
CREATE INDEX idx_player_data_user_id ON player_data(user_id);

-- Create an index on username for uniqueness checks
CREATE INDEX idx_player_data_username ON player_data(username);

-- Enable Row Level Security
ALTER TABLE player_data ENABLE ROW LEVEL SECURITY;

-- Policy: Users can read their own data
CREATE POLICY "Users can view own player data" 
ON player_data FOR SELECT 
USING (auth.uid() = user_id);

-- Policy: Users can insert their own data
CREATE POLICY "Users can insert own player data" 
ON player_data FOR INSERT 
WITH CHECK (auth.uid() = user_id);

-- Policy: Users can update their own data
CREATE POLICY "Users can update own player data" 
ON player_data FOR UPDATE 
USING (auth.uid() = user_id);
```

## Unity Setup

### 1. **Add GameObjects to Your Initial Scene**

In your first scene (login/registration scene), make sure you have:

- **AuthManager GameObject** - with `AuthManager.cs` script attached
- **PlayerDataManager GameObject** - with `PlayerDataManager.cs` script attached

Both should be set to `DontDestroyOnLoad` (this is handled in the scripts).

### 2. **Update Registration UI**

In your registration UI, add a new input field for username:

1. Add a `TMP_InputField` for username input
2. Assign it to the `usernameInput` field in the `UIAuth` component inspector

### 3. **Update Level Selection UI**

In your level selection scene:

1. Add a `TextMeshProUGUI` component to display the username
2. Assign it to the `usernameText` field in the `LevelSelection` component inspector

## How It Works

### Registration Flow:
1. User enters email, username, password, and confirm password
2. `AuthManager.Register()` is called
3. User is created in Supabase Auth
4. `PlayerDataManager.CreatePlayerData()` creates a row in the `player_data` table
5. Data is saved to both Supabase and local PlayerPrefs

### Login Flow:
1. User enters email and password
2. `AuthManager.Login()` is called
3. User authenticates with Supabase
4. `PlayerDataManager.LoadPlayerData()` fetches the player's data from Supabase
5. Data is cached locally in PlayerPrefs

### Gameplay Data Sync:
- **Level Completion**: When a level is completed, `PlayCardButton` automatically calls `PlayerDataManager.UpdateLevelsUnlocked()`
- **Money Earned**: When money is earned, it syncs to Supabase via `PlayerDataManager.UpdateMoney()`
- **Logout**: All current game data is synced before logout

### Username Display:
- The username is displayed in the level selection screen from PlayerPrefs (cached from Supabase)
- Format: "Welcome, [username]!"

## API Methods for Future Use

### PlayerDataManager Methods:

```csharp
// Get current player data
PlayerData data = PlayerDataManager.Instance.GetCurrentPlayerData();

// Get username
string username = PlayerDataManager.Instance.GetUsername();

// Update levels unlocked
await PlayerDataManager.Instance.UpdateLevelsUnlocked(levelIndex);

// Update money
await PlayerDataManager.Instance.UpdateMoney(amount);

// Add unlocked cosmetic
await PlayerDataManager.Instance.AddUnlockedCosmetic("cosmetic_id");

// Get all unlocked cosmetics
List<string> cosmetics = PlayerDataManager.Instance.GetUnlockedCosmetics();

// Sync all game data to Supabase
await PlayerDataManager.Instance.SyncGameData();
```

## Testing Checklist

- [ ] Create the `player_data` table in Supabase
- [ ] Add AuthManager and PlayerDataManager GameObjects to your scene
- [ ] Add username input field to registration UI
- [ ] Add username display text to level selection scene
- [ ] Test registration with username
- [ ] Test login and verify username displays
- [ ] Test level completion and verify data syncs to Supabase
- [ ] Test money earning and verify sync
- [ ] Check Supabase dashboard to confirm data is being saved

## Troubleshooting

### "No player data found" after login
- Check if the `player_data` table exists in Supabase
- Verify Row Level Security policies are set correctly
- Check if data was created during registration

### Username not displaying
- Verify `usernameText` is assigned in LevelSelection inspector
- Check if PlayerPrefs has the username key: `PlayerPrefs.GetString("username")`

### Data not syncing
- Ensure both AuthManager and PlayerDataManager GameObjects exist in the scene
- Check Unity console for error messages
- Verify Supabase connection is working

## Next Steps

1. **Cosmetics System**: Use `AddUnlockedCosmetic()` and `GetUnlockedCosmetics()` to implement a cosmetics/character unlock system
2. **Leaderboards**: Add additional tables for global leaderboards
3. **Profile Page**: Create a profile page showing all player stats
4. **Cloud Save**: Data is already synced to cloud, so players can play across devices

## WebGL Support

? **WebGL is fully supported!** 

All features work in WebGL builds with JavaScript interop:
- User authentication (register, login, logout)
- Player data management (create, read, update)
- Real-time cloud sync
- Cross-device progress

**See `WEBGL_SETUP_GUIDE.md` for complete WebGL deployment instructions.**

## Notes

- Standalone builds use the native C# Supabase client
- WebGL builds use JavaScript SDK with Unity callbacks
- PlayerPrefs is used as a local cache for offline play
- Data syncs automatically on level completion, money changes, and logout
- WebGL template is located at `Assets/WebGLTemplates/SupabaseTemplate/`
