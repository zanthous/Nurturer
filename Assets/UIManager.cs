using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _flowerHealthText;
    private WorldBlossom flower;

    // Start is called before the first frame update
    void Start()
    {
        flower = FindObjectOfType<WorldBlossom>();
    }

    // Update is called once per frame
    void Update()
    {
        _flowerHealthText.text = "World blossom health: " + flower.Health.ToString();    
    }
}
