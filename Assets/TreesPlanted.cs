using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TreesPlanted : MonoBehaviour
{
    private TextMeshProUGUI text;
    private LevelData data;
    // Start is called before the first frame update
    void Awake()
    {
        data = FindObjectOfType<LevelData>();
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Trees planted " + data.GetObjectsOfType(ObjectType.Tree).Count.ToString() + " \nDifficulty level: " + data.flower.Divisions.ToString();
    }
}
