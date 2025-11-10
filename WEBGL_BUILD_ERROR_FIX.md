# WebGL Build Error - "error on case 4 in buildwebglframework" Fix

## Problem
The error "error on case 4 in buildwebglframework console.error(str)" typically occurs when Unity's WebGL build encounters an initialization error. This is usually caused by:

1. **Missing Build Files** - Build folder doesn't contain all necessary files
2. **CORS Issues** - Files not served from a proper web server
3. **Memory Issues** - WebAssembly memory initialization problems
4. **Template Issues** - Missing error handling in the template

## Solutions

### Solution 1: Build Settings (Most Common)

1. **Open Build Settings**: File > Build Settings > WebGL
2. **Player Settings**: Click "Player Settings" button
3. **Publishing Settings**:
   - **Compression Format**: Set to "Disabled" for testing (or Gzip/Brotli for production)
   - **Decompression Fallback**: Enable this checkbox
   - **Data Caching**: Disable for testing

4. **Other Settings**:
   - **Exception Support**: Set to "Full" for better error messages
   - **Enable Exceptions**: Check this box

### Solution 2: Use Local Web Server

Never open index.html directly (file://). Always use a web server:

**Python (easiest):**
```bash
cd YourBuildFolder
python -m http.server 8000
```

**Node.js:**
```bash
cd YourBuildFolder
npx http-server -p 8000
```

Then open: `http://localhost:8000`

### Solution 3: Clean Build

1. Delete the entire Build folder
2. In Unity: File > Build Settings
3. Click "Clean" button (if available)
4. Close Unity
5. Delete these folders in your project:
   - `Library`
   - `Temp`
6. Reopen Unity
7. Rebuild for WebGL

### Solution 4: Check Unity Version

The error can occur with older Unity versions. Recommended:
- Unity 2020.3 LTS or newer
- Unity 2021.3 LTS (best for WebGL)
- Unity 2022.3 LTS

### Solution 5: Browser Console

Check the browser console (F12) for detailed errors:

1. Press F12 in your browser
2. Go to Console tab
3. Look for the actual error message
4. Common errors:
   - "Failed to fetch" = CORS issue (use web server)
   - "Out of memory" = Increase memory limit
   - "Undefined symbol" = Missing .jslib file
   - "Module not found" = Build files missing

### Solution 6: Template Fix

The template needs better error handling. Here's what to check:

**Check 1: Verify SupabasePlugin.jslib exists**
Location: `Assets/Plugins/WebGL/SupabasePlugin.jslib`

If missing, you need to recreate it.

**Check 2: Verify Template Files**
- `Assets/WebGLTemplates/SupabaseTemplate/index.html`
- `Assets/WebGLTemplates/SupabaseTemplate/TemplateData/style.css`

**Check 3: Build Settings Template**
1. File > Build Settings > WebGL
2. Player Settings
3. Resolution and Presentation
4. WebGL Template: Should be "SupabaseTemplate"

## Quick Diagnostic Steps

### Step 1: Test with Default Template First
1. Build Settings > WebGL
2. Player Settings > WebGL Template > Default
3. Build
4. If this works, the issue is with the custom template
5. If this fails, it's a build configuration issue

### Step 2: Check Build Output
After building, your Build folder should contain:
```
Build/
??? index.html
??? Build/
?   ??? YourGame.data.gz (or .br)
?   ??? YourGame.framework.js.gz (or .br)
?   ??? YourGame.loader.js
?   ??? YourGame.wasm.gz (or .br)
??? TemplateData/
?   ??? style.css
??? StreamingAssets/ (if you have any)
```

### Step 3: Browser Compatibility
Test in multiple browsers:
- Chrome (best for WebGL)
- Firefox
- Edge
- Safari (can have issues)

## Common Error Messages and Fixes

### "Cannot read property 'buffer' of undefined"
**Fix**: Build with compression disabled

### "Failed to instantiate wasm module"
**Fix**: Memory settings too high. Reduce in Player Settings > Publishing Settings > Memory Size

### "Uncaught ReferenceError: createUnityInstance is not defined"
**Fix**: Loader file failed to load. Check web server is running.

### "The code execution cannot proceed because UnityLoader.js was not found"
**Fix**: Old Unity version. Update to 2020.3 or newer.

## Testing Checklist

Before testing your build:

- [ ] Using a web server (not file://)
- [ ] All build files are present in Build folder
- [ ] Browser console open (F12) to see errors
- [ ] Tested with compression disabled first
- [ ] Template is set to SupabaseTemplate in Player Settings
- [ ] SupabasePlugin.jslib exists in Assets/Plugins/WebGL/
- [ ] Build is not from an error/warning-filled project

## Still Not Working?

If you're still getting errors, provide these details:

1. **Exact error message from browser console**
2. **Unity version**
3. **Build settings** (compression, exceptions, etc.)
4. **How you're running it** (web server? which one?)
5. **Browser and version**

## Working Build Settings (Recommended)

For a clean build that works:

**Player Settings > Publishing Settings:**
- Compression Format: Disabled (for testing) or Gzip (for production)
- Decompression Fallback: ? Enabled
- Data Caching: ? Disabled (for testing)

**Player Settings > Other Settings:**
- Exception Support: Full
- Enable Exceptions: ?
- Managed Stripping Level: Minimal
- Strip Engine Code: ? (for testing)

**Build Settings:**
- Development Build: ? (for testing)
- Autoconnect Profiler: ?
- Deep Profiling: ?

## Next Steps

1. Try Solution 1 (Build Settings) first
2. If that doesn't work, try Solution 3 (Clean Build)
3. Always use Solution 2 (Web Server) for testing
4. Check browser console for specific errors
5. Test with Default template to isolate the issue

---

**Most Common Fix**: Compression Format set to "Disabled" in Publishing Settings + using a proper web server instead of opening file directly.
