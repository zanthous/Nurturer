using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PostGameControls : MonoBehaviour
{
    private bool postGame = false;

    [SerializeField] private GameObject postGamePanel;
    [SerializeField] private float cameraSpeed;
    private int minSize = 1;
    private int maxSize = 25;
    private int curSize = 5;
    private int zoomAmount = 2;

    private void Awake()
    {
        GameManager.GameOver += EnablePostGameControls;
    }

    private void OnDestroy()
    {
        GameManager.GameOver -= EnablePostGameControls;
    }

    private void Update()
    {
        if(!postGame)
            return;
        Vector2 offset = new Vector2();
        if(Input.GetKey(KeyCode.W))
        {
            offset += Vector2.up * Time.deltaTime * cameraSpeed;

        }
        if(Input.GetKey(KeyCode.A))
        {
            offset += Vector2.left * Time.deltaTime * cameraSpeed;
        }
        if(Input.GetKey(KeyCode.S))
        {
            offset += Vector2.down * Time.deltaTime * cameraSpeed;
        }
        if(Input.GetKey(KeyCode.D))
        {
            offset += Vector2.right * Time.deltaTime * cameraSpeed;
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            if(curSize > minSize)
                curSize -= zoomAmount;
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(curSize < maxSize)
                curSize += zoomAmount;
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            postGamePanel.SetActive(!postGamePanel.activeSelf);
        }
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneManager.LoadScene(0);
        }

        transform.position += new Vector3(offset.x, offset.y, 0);
        Camera.main.orthographicSize = curSize;

    }

    private void EnablePostGameControls(int a, int b, int c, int d, int e, float f)
    {
        postGame = true;
        var player = Camera.main.transform.parent.gameObject;
        Camera.main.transform.parent = transform;
        Destroy(player);
    }
}
