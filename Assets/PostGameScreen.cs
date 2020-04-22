using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Added post game jam
public class PostGameScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        gameObject.SetActive(false);
        GameManager.GameOver += ShowPanel;
    }

    private void OnDestroy()
    {
        GameManager.GameOver -= ShowPanel;
    }

    private void ShowPanel(int timeTicks, int steps, int treesPlanted, int plantsPlanted, int energyDeposited, float highestDifficulty)
    {
        gameObject.SetActive(true);

        scoreText.text = "Post game stats: " + 
            "\nScore (total time ticks): " + timeTicks.ToString() +  
            "\nTotal steps: " + steps.ToString() + 
            "\nTrees planted: " + treesPlanted.ToString() + 
            "\nPlants planted: " + plantsPlanted.ToString() + 
            "\nEnergy deposited: " + energyDeposited.ToString() + 
            "\nHighest difficulty modifier: " + highestDifficulty.ToString();
    } 
}
