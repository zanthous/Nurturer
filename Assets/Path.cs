
using UnityEngine;

public class Path : MonoBehaviour
{
    void Awake()
    {
        GetComponent<LineRenderer>().material.SetColor("_TintColor", new Color(1,0,0,.5f));
    }
}
