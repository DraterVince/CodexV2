# WebGL Setup Guide - Complete Supabase Integration

## Overview
This guide explains how to set up and deploy your Unity game with full Supabase functionality for WebGL builds.

## What Works in WebGL

? **Authentication**
- User registration
- User login
- User logout
- Session management

? **Player Data Management**
- Create player data after registration
- Load player data on login
- Update levels unlocked
- Update current money
- Manage unlocked cosmetics
- Sync all game data to Supabase cloud

? **Local Storage**
- Data cached in browser localStorage via PlayerPrefs
- Works offline with cached data
- Auto-syncs when online

## Files Created

### 1. **JavaScript Plugin** (`Assets/Plugins/WebGL/SupabasePlugin.jslib`)
- **CRITICAL**: Required for WebGL build to work
- Bridges C# DllImport calls to JavaScript functions
- Handles string conversion between C# and JavaScript
- Without this file, WebGL build will fail with "undefined symbol" errors

### 2. **WebGL Template** (`Assets/WebGLTemplates/SupabaseTemplate/index.html`)
- Custom HTML template with Supabase JavaScript SDK
- All JavaScript interop functions for auth and player data
- Unity WebGL loader configuration

### 3. **Template Styles** (`Assets/WebGLTemplates/SupabaseTemplate/TemplateData/style.css`)
- Styling for the WebGL player
- Loading bar and UI elements

### 4. **Updated Scripts**
- `PlayerDataManager.cs` - Added WebGL support with JavaScript callbacks
- `AuthManager.cs` - Added WebGL callbacks for authentication
- `UIAuth.cs` - Stores username for WebGL registration flow

## Unity Build Settings

### Step 1: Select WebGL Template

1. Go to **File > Build Settings**
2. Select **WebGL** platform
3. Click **Player Settings**
4. Under **Resolution and Presentation** > **WebGL Template**
5. Select **SupabaseTemplate**

### Step 2: Configure Build Settings

1. In **Player Settings > Other Settings**:
   - Set **Color Space** to **Linear** (recommended)
   - Set **Auto Graphics API** (leave checked)
   
2. In **Publishing Settings**:
   - Set **Compression Format** to **Gzip** or **Brotli** (recommended)
   - Enable **Decompression Fallback** if needed

### Step 3: Build

1. Click **Build** in Build Settings
2. Choose a folder (e.g., `WebGLBuild`)
3. Wait for build to complete

## Deployment Options

### Option 1: Local Testing

1. After building, you cannot simply open `index.html` in a browser (CORS issues)
2. Use a local web server:

**Using Python:**
```bash
cd WebGLBuild
python -m http.server 8000
```

**Using Node.js:**
```bash
cd WebGLBuild
npx http-server -p 8000
```

3. Open browser to `http://localhost:8000`

### Option 2: Deploy to Hosting Service

#### A. **Netlify** (Recommended - Free)

1. Install Netlify CLI:
```bash
npm install -g netlify-cli
```

2. Deploy:
```bash
cd WebGLBuild
netlify deploy --prod
```

3. Follow prompts and your game will be live!

#### B. **GitHub Pages**

1. Create a new repository on GitHub
2. Push your WebGLBuild folder to the repository
3. Go to **Settings > Pages**
4. Select source branch and folder
5. Your game will be available at `https://yourusername.github.io/repository-name`

#### C. **Vercel**

1. Install Vercel CLI:
```bash
npm install -g vercel
```

2. Deploy:
```bash
cd WebGLBuild
vercel
```

#### D. **itch.io**

1. Zip your entire WebGLBuild folder
2. Go to itch.io and create a new project
3. Upload the ZIP file
4. Select "This file will be played in the browser"
5. Set **index.html** as the main file

### Option 3: Custom Server

Upload your WebGLBuild folder to any web hosting service that supports static files.

**Important:** Make sure your server supports:
- HTTPS (required for many WebGL features)
- Proper MIME types for `.data`, `.wasm`, `.js` files
- Compression (gzip/brotli)

## Required Supabase Configuration

### Enable CORS for Your Domain

In your Supabase project:

1. Go to **Settings > API**
2. Add your domain to **CORS Configuration**
3. Examples:
   - `http://localhost:8000` (for local testing)
   - `https://yourgame.netlify.app` (for Netlify)
   - `https://yourusername.github.io` (for GitHub Pages)

## How WebGL Version Works

### Registration Flow:
1. User enters email, username, password in Unity UI
2. `UIAuth.cs` stores username temporarily in PlayerPrefs
3. `AuthManager.Register()` calls JavaScript `SupabaseRegister()`
4. JavaScript authenticates with Supabase
5. JavaScript calls Unity callback `OnRegisterSuccess()`
6. Unity retrieves stored username and creates player data
7. JavaScript `createPlayerData()` saves to Supabase database
8. Data also saved to browser localStorage via PlayerPrefs

### Login Flow:
1. User enters email and password
2. `AuthManager.Login()` calls JavaScript `SupabaseLogin()`
3. JavaScript authenticates with Supabase
4. JavaScript calls Unity callback `OnLoginSuccess()`
5. Unity calls `PlayerDataManager.LoadPlayerData()`
6. JavaScript `loadPlayerData()` fetches from Supabase
7. JavaScript calls Unity callback `OnPlayerDataLoaded()`
8. Unity caches data in PlayerPrefs

### Gameplay Sync:
- Level completion ? `UpdatePlayerData()` ? JavaScript ? Supabase
- Money changes ? `UpdatePlayerData()` ? JavaScript ? Supabase
- Cosmetics unlock ? `UpdatePlayerData()` ? JavaScript ? Supabase
- All synced in real-time!

## Testing Your WebGL Build

### 1. Test Registration
- [ ] Register a new user with username
- [ ] Check browser console for "Registration successful"
- [ ] Verify user appears in Supabase Auth dashboard
- [ ] Verify player_data row created in Supabase

### 2. Test Login
- [ ] Login with registered credentials
- [ ] Check console for "Login successful"
- [ ] Verify username displays in level select
- [ ] Check that PlayerPrefs cached data

### 3. Test Gameplay Sync
- [ ] Complete a level
- [ ] Check Supabase dashboard - levels_unlocked should update
- [ ] Earn money
- [ ] Check Supabase dashboard - current_money should update

### 4. Test Cross-Device
- [ ] Login on Device A
- [ ] Play and make progress
- [ ] Login on Device B with same credentials
- [ ] Verify progress synced correctly

## Troubleshooting

### "undefined symbol" errors during build
**Error**: `undefined symbol: createPlayerData` or similar
**Solution**: The `.jslib` file is missing or not in the correct location
- Verify `Assets/Plugins/WebGL/SupabasePlugin.jslib` exists
- Rebuild the project completely
- See `WEBGL_JSLIB_FIX.md` for detailed explanation

### "Supabase is not defined"
- Make sure the Supabase CDN script is loading in index.html
- Check browser console for network errors
- Verify you're using HTTPS or localhost

### "CORS policy error"
- Add your domain to Supabase CORS settings
- Ensure you're using a web server (not file://)

### "Player data not syncing"
- Open browser console (F12)
- Check for JavaScript errors
- Verify Supabase credentials in index.html
- Check network tab to see if API calls are being made

### "Username not showing"
- Check if PlayerPrefs has the username: Open console and check localStorage
- Verify OnPlayerDataLoaded callback is being triggered
- Check Unity console for errors

### Build Size Issues
- WebGL builds are large (50-200MB typical)
- Enable **Brotli** compression in Publishing Settings
- Use **Code Stripping** in Player Settings
- Consider **IL2CPP** backend for smaller builds

## Performance Optimization

### For Better Loading Times:
1. Use **Brotli** compression (best)
2. Enable **Code Stripping** (Medium/High)
3. Use **IL2CPP** scripting backend
4. Optimize textures and assets
5. Use sprite atlases

### For Better Runtime Performance:
1. Limit draw calls
2. Use object pooling
3. Optimize physics calculations
4. Reduce transparency/alpha blending

## Security Notes

?? **Important Security Considerations:**

1. **API Keys in Client**: The Supabase anon key is visible in the HTML. This is OK because:
   - Row Level Security (RLS) protects your data
   - Users can only access their own data
   - All operations are validated server-side

2. **Never expose Service Role Key**: Only use the anon (public) key in WebGL builds

3. **Row Level Security**: Ensure all RLS policies are properly configured in Supabase

## Advanced: Customizing the Template

### Changing Supabase Credentials
Edit `Assets/WebGLTemplates/SupabaseTemplate/index.html`:

```javascript
const SUPABASE_URL = 'YOUR_SUPABASE_URL';
const SUPABASE_ANON_KEY = 'YOUR_ANON_KEY';
```

### Adding Custom Styling
Edit `Assets/WebGLTemplates/SupabaseTemplate/TemplateData/style.css`

### Adding Analytics
Add analytics scripts to the `<head>` section of index.html:

```html
<!-- Google Analytics -->
<script async src="https://www.googletagmanager.com/gtag/js?id=YOUR-GA-ID"></script>
```

## Differences from Standalone Builds

| Feature | Standalone (PC/Mac) | WebGL |
|---------|-------------------|--------|
| Supabase Client | C# Native | JavaScript SDK |
| Authentication | Async C# | JavaScript Interop |
| Data Storage | PlayerPrefs + Supabase | LocalStorage + Supabase |
| Performance | Better | Good |
| File Size | Smaller | Larger |
| Distribution | Download | Instant Play |

## Next Steps

1. ? Build for WebGL using SupabaseTemplate
2. ? Test locally with a web server
3. ? Configure Supabase CORS
4. ? Deploy to hosting service
5. ? Test all features online
6. ? Share your game!

## Support

If you encounter issues:
1. Check browser console for errors (F12)
2. Check Unity console for errors
3. Verify Supabase dashboard for data
4. Test with different browsers
5. Check network tab for API calls

## Resources

- [Unity WebGL Documentation](https://docs.unity3d.com/Manual/webgl.html)
- [Supabase JavaScript Documentation](https://supabase.com/docs/reference/javascript)
- [WebGL Best Practices](https://docs.unity3d.com/Manual/webgl-performance.html)

---

**Congratulations!** Your game now has full cloud save functionality in both standalone and WebGL builds! ????
