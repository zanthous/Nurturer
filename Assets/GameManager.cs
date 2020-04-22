using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //int timeTicks, int steps, int treesPlanted, int plantsPlanted, int energyDeposited, float highestDifficulty
    public static Action<int, int, int, int, int, float> GameOver;
    public static Action RestartGame;

    private void Awake()
    {
    }

    private void OnDestroy()
    {
    }

    private void LoadGOScene()
    {
        //SceneManager.LoadScene(1);  
        //destroy player
        //attach movement script to camera
        //enable restart game command
        //display final statistics
    }

}
