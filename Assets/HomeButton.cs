using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButton : MonoBehaviour
{
    public int nextLevel;
    public void LoadHome()
    {
        //nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
        //if (nextLevel > PlayerPrefs.GetInt("levelAt"))
        //{
        //    PlayerPrefs.SetInt("levelAt", nextLevel);
        //}
        SceneManager.LoadSceneAsync("LevelSelect");
    }
}
