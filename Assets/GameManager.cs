using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static Action GameOver;




    private void Awake()
    {
        GameOver += LoadGOScene;
    }

    private void OnDestroy()
    {
        GameOver -= LoadGOScene;
    }

    private void LoadGOScene()
    {
        SceneManager.LoadScene(1);  
    }
}
