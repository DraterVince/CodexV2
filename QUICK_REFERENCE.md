# Quick Reference - Supabase Integration

## ?? Quick Start

### For Standalone (PC/Mac/Linux)
1. Add `AuthManager` and `PlayerDataManager` GameObjects to first scene
2. Create `player_data` table in Supabase (see SETUP_INSTRUCTIONS.md)
3. Add username input to registration UI
4. Build and run!

### For WebGL
1. Follow standalone steps above
2. Select **SupabaseTemplate** in Player Settings > WebGL Template
3. Build for WebGL
4. Deploy to web server (see WEBGL_SETUP_GUIDE.md)
5. Configure CORS in Supabase

## ?? Essential Code Snippets

### Get Username
```csharp
string username = PlayerDataManager.Instance.GetUsername();
```

### Update Level Progress
```csharp
await PlayerDataManager.Instance.UpdateLevelsUnlocked(levelIndex);
```

### Update Money
```csharp
await PlayerDataManager.Instance.UpdateMoney(newAmount);
```

### Add Cosmetic
```csharp
await PlayerDataManager.Instance.AddUnlockedCosmetic("cosmetic_id");
```

### Get Unlocked Cosmetics
```csharp
List<string> cosmetics = PlayerDataManager.Instance.GetUnlockedCosmetics();
```

### Manual Sync
```csharp
await PlayerDataManager.Instance.SyncGameData();
```

### Get Current Player Data
```csharp
PlayerData data = PlayerDataManager.Instance.GetCurrentPlayerData();
if (data != null)
{
    Debug.Log($"User: {data.username}");
    Debug.Log($"Levels: {data.levels_unlocked}");
    Debug.Log($"Money: {data.current_money}");
}
```

## ??? Supabase SQL Quick Commands

### View All Player Data
```sql
SELECT * FROM player_data;
```

### Find Player by Username
```sql
SELECT * FROM player_data WHERE username = 'PlayerName';
```

### Update Player Money
```sql
UPDATE player_data 
SET current_money = 1000 
WHERE username = 'PlayerName';
```

### Delete Player Data
```sql
DELETE FROM player_data WHERE username = 'PlayerName';
```

### View Top Players by Level
```sql
SELECT username, levels_unlocked 
FROM player_data 
ORDER BY levels_unlocked DESC 
LIMIT 10;
```

## ?? Common Tasks

### Add New Cosmetic Type

1. Update PlayerData model if needed
2. Use existing `unlocked_cosmetics` JSON array
3. Call `AddUnlockedCosmetic("cosmetic_id")`

### Add New Data Field

1. Add column to Supabase table:
```sql
ALTER TABLE player_data ADD COLUMN new_field INTEGER DEFAULT 0;
```

2. Update `PlayerData.cs`:
```csharp
[Column("new_field")]
public int new_field { get; set; }
```

3. Update any UI/logic that uses it

### Reset Player Progress

```csharp
if (PlayerDataManager.Instance.GetCurrentPlayerData() != null)
{
    var data = PlayerDataManager.Instance.GetCurrentPlayerData();
    data.levels_unlocked = 5;
    data.current_money = 0;
    data.unlocked_cosmetics = "[]";
    await PlayerDataManager.Instance.UpdatePlayerData(data);
}
```

## ?? Debug Checklist

### Data Not Saving?
- [ ] Check Unity console for errors
- [ ] Verify Supabase table exists
- [ ] Check RLS policies
- [ ] Verify AuthManager and PlayerDataManager exist in scene
- [ ] Check internet connection

### WebGL Not Working?
- [ ] Using web server (not file://)?
- [ ] CORS configured in Supabase?
- [ ] Check browser console (F12)
- [ ] Verify template selected in build settings
- [ ] Check Supabase credentials in index.html

### Username Not Showing?
- [ ] Check PlayerPrefs: `PlayerPrefs.GetString("username")`
- [ ] Verify user logged in
- [ ] Check if data loaded (console logs)
- [ ] Verify UI reference assigned

## ?? Build Comparison

| Feature | Standalone | WebGL |
|---------|-----------|-------|
| **Build Size** | ~50-100 MB | ~100-200 MB |
| **Load Time** | Fast | Moderate |
| **Distribution** | Download | Browser |
| **Updates** | Requires redownload | Instant |
| **Performance** | Best | Good |
| **Offline Play** | Yes (cached) | Yes (cached) |
| **Auto-sync** | Yes | Yes |

## ?? Best Practices

### Performance
- ? Sync data on important events (level complete, money change)
- ? Don't sync on every frame
- ? Use PlayerPrefs for quick local access
- ? Batch updates when possible

### Security
- ? Always use Row Level Security in Supabase
- ? Never expose service role key
- ? Validate all user input
- ? Use proper authentication

### User Experience
- ? Show loading indicators during sync
- ? Cache data locally with PlayerPrefs
- ? Handle offline gracefully
- ? Provide feedback for sync success/failure

## ?? File Structure

```
Assets/
??? Scripts/
?   ??? AuthManager.cs (Authentication)
?   ??? PlayerDataManager.cs (Data management)
?   ??? PlayerData.cs (Data model)
?   ??? UIAuth.cs (Login/Register UI)
??? WebGLTemplates/
?   ??? SupabaseTemplate/
?       ??? index.html (WebGL template)
?       ??? TemplateData/
?           ??? style.css (Styling)
??? LevelSelection.cs (Shows username)
??? MoneyManager.cs (Manages money)
??? PlayCardButton.cs (Game logic)
```

## ?? Important Links

- **Documentation**: SETUP_INSTRUCTIONS.md
- **WebGL Guide**: WEBGL_SETUP_GUIDE.md
- **Supabase Dashboard**: https://supabase.com/dashboard
- **Unity WebGL Docs**: https://docs.unity3d.com/Manual/webgl.html

## ?? Tips

- Save frequently but not constantly
- Test both standalone and WebGL builds
- Use version control for your Supabase schema
- Keep backups of your database
- Monitor Supabase usage in dashboard
- Test on multiple devices for sync verification

## ?? Need Help?

1. Check console logs (Unity + Browser)
2. Review documentation files
3. Test with minimal example
4. Check Supabase dashboard for data
5. Verify network calls in browser DevTools

---

**Files**: `SETUP_INSTRUCTIONS.md` | `WEBGL_SETUP_GUIDE.md` | This File
