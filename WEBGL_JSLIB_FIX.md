# WebGL Build Fix - JavaScript Library Plugin

## Issue Resolved

The WebGL build was failing with "undefined symbol" errors because Unity couldn't find the JavaScript functions that were declared in C# using `DllImport`.

### Error Messages:
```
undefined symbol: createPlayerData
undefined symbol: getCurrentUser
undefined symbol: loadPlayerData
undefined symbol: updatePlayerData
```

## Solution

Created `Assets/Plugins/WebGL/SupabasePlugin.jslib` - a Unity WebGL plugin that bridges C# code with JavaScript functions.

## How It Works

### 1. **The .jslib File**
Unity WebGL requires JavaScript functions to be defined in `.jslib` files located in `Assets/Plugins/WebGL/`. These files are automatically included in the WebGL build.

### 2. **Function Bridging**
The `.jslib` file acts as a bridge:

```
C# Code (DllImport) ? .jslib Plugin ? JavaScript Functions in index.html
```

**Example Flow:**
```
PlayerDataManager.cs calls createPlayerData()
         ?
SupabasePlugin.jslib receives the call
         ?
Converts C# strings to JavaScript strings
         ?
Calls window.createPlayerData() in index.html
         ?
Supabase JavaScript SDK executes
```

### 3. **String Conversion**
C# strings are pointers in WebAssembly. The plugin uses `UTF8ToString()` to convert them:

```javascript
createPlayerData: function(userIdPtr, emailPtr, usernamePtr) {
    var userId = UTF8ToString(userIdPtr);    // Convert pointer to string
    var email = UTF8ToString(emailPtr);      // Convert pointer to string
    var username = UTF8ToString(usernamePtr); // Convert pointer to string
    
    window.createPlayerData(userId, email, username); // Call JS function
}
```

## Files Structure

```
Assets/
??? Plugins/
?   ??? WebGL/
?       ??? SupabasePlugin.jslib  ? Bridge between C# and JS
??? Scripts/
?   ??? AuthManager.cs            ? Uses DllImport
?   ??? PlayerDataManager.cs      ? Uses DllImport
??? WebGLTemplates/
    ??? SupabaseTemplate/
        ??? index.html             ? Contains actual JS functions
```

## Functions Implemented

### Authentication Functions:
- `SupabaseRegister(email, password)` - User registration
- `SupabaseLogin(email, password)` - User login
- `SupabaseLogout()` - User logout
- `getCurrentUser()` - Get current authenticated user

### Player Data Functions:
- `createPlayerData(userId, email, username)` - Create player record
- `loadPlayerData(userId)` - Load player data from Supabase
- `updatePlayerData(userId, levels, money, cosmetics)` - Update player data

## How C# Calls JavaScript

### In C# (PlayerDataManager.cs):
```csharp
#if UNITY_WEBGL && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void createPlayerData(string userId, string email, string username);
#endif
```

### In .jslib (SupabasePlugin.jslib):
```javascript
createPlayerData: function(userIdPtr, emailPtr, usernamePtr) {
    var userId = UTF8ToString(userIdPtr);
    var email = UTF8ToString(emailPtr);
    var username = UTF8ToString(usernamePtr);
    
    if (typeof window.createPlayerData === 'function') {
        window.createPlayerData(userId, email, username);
    }
}
```

### In HTML (index.html):
```javascript
window.createPlayerData = async function(userId, email, username) {
    const { data, error } = await supabaseClient
        .from('player_data')
        .insert({ user_id: userId, email: email, username: username });
    
    SendMessageToUnity('PlayerDataManager', 'OnPlayerDataCreated', JSON.stringify(data));
};
```

## Why This Is Necessary

1. **WebAssembly Limitation**: Unity WebGL compiles C# to WebAssembly, which can't directly call browser JavaScript APIs
2. **String Handling**: WebAssembly uses memory pointers, not JavaScript strings
3. **Unity Convention**: Unity requires `.jslib` files for custom JavaScript code in WebGL builds
4. **Type Safety**: The `.jslib` file handles type conversion between C# and JavaScript

## Testing the Fix

After creating the `.jslib` file:

1. ? **Build Succeeds** - No more "undefined symbol" errors
2. ? **Functions Available** - JavaScript functions are properly linked
3. ? **Communication Works** - C# can call JavaScript and receive callbacks

## Debugging Tips

### If functions still not found:
1. Check file location: Must be in `Assets/Plugins/WebGL/`
2. Check file extension: Must be `.jslib`
3. Rebuild the project completely
4. Check browser console for JavaScript errors

### If strings are corrupted:
1. Verify `UTF8ToString()` is used for all string parameters
2. Check that strings are passed correctly from C#

### If callbacks not working:
1. Verify `unityInstance.SendMessage()` is called in index.html
2. Check GameObject names match (case-sensitive)
3. Check method names in callbacks

## Common Errors and Solutions

### Error: "FS.syncfs is not a function"
**Solution**: This is unrelated to Supabase. It's a Unity WebGL file system error. Can be ignored if you're not using persistent storage.

### Error: "Cannot find module 'UTF8ToString'"
**Solution**: Make sure you're using the correct Unity WebGL version. UTF8ToString is built into Unity's WebGL runtime.

### Error: "window.createPlayerData is not a function"
**Solution**: The JavaScript function in index.html isn't loaded yet. Add error handling in the .jslib file (already included).

## Build Checklist

Before building for WebGL:

- [x] ? `.jslib` file created in `Assets/Plugins/WebGL/`
- [x] ? All functions declared in C# with `DllImport`
- [x] ? All functions implemented in `.jslib`
- [x] ? All functions implemented in `index.html`
- [x] ? String conversion using `UTF8ToString()`
- [x] ? Error handling for missing functions
- [x] ? Build settings set to use SupabaseTemplate

## What Happens During Build

1. Unity compiles C# to WebAssembly
2. Unity processes `.jslib` files and includes them in the build
3. Functions declared with `DllImport("__Internal")` are linked to `.jslib` functions
4. The `.jslib` code is merged into Unity's generated JavaScript
5. At runtime, C# calls execute the `.jslib` functions
6. The `.jslib` functions call the JavaScript functions in `index.html`

## Advanced: Adding New Functions

To add a new JavaScript function:

### Step 1: Declare in C#
```csharp
#if UNITY_WEBGL && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void MyNewFunction(string param);
#endif
```

### Step 2: Add to .jslib
```javascript
MyNewFunction: function(paramPtr) {
    var param = UTF8ToString(paramPtr);
    if (typeof window.MyNewFunction === 'function') {
        window.MyNewFunction(param);
    }
}
```

### Step 3: Implement in index.html
```javascript
window.MyNewFunction = function(param) {
    console.log('Called with:', param);
    // Your code here
};
```

## Performance Considerations

- ? Minimal overhead - Direct function calls
- ? Efficient string conversion
- ? No polling or timers needed
- ? Event-driven communication

## Security Notes

- The `.jslib` file is included in the build and is visible to users
- Don't put sensitive data in `.jslib` files
- All security is handled by Supabase RLS policies
- The anon key is safe to expose (it's in index.html anyway)

## Conclusion

The `.jslib` plugin is now properly configured and your WebGL build will work correctly with all Supabase functionality!

---

**File Created**: `Assets/Plugins/WebGL/SupabasePlugin.jslib`
**Status**: ? Working
**Build Status**: ? Successful
