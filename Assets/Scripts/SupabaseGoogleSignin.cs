using System.Collections;
using System.Net;
using Supabase;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;
using TMPro;
using UnityEngine.Scripting;

public class SupabaseGoogleSignin : MonoBehaviour
{
    
    private async Task<Supabase.Client> InitializeSupabase()
    {
        string supabaseUrl = "https://bpjyqsfggliwehnqcbhy.supabase.co";
        string supabaseKey = "sb_secret_V2gujE3Apz-YnlkG=60qbw_IBzx1Uoo";

        var clientOptions = new Supabase.SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true,
        };
    }

}
