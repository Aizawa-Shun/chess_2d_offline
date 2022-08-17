using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonManager : MonoBehaviour
{
    GameObject buttonObject;
    GameObject uiManager;

    GameObject resignButton;
    GameObject drawButton;


    void Awake()
    {
        resignButton = GameObject.Find("ResignButton");
        drawButton = GameObject.Find("DrawButton");
        uiManager = GameObject.FindGameObjectWithTag("UIManager");
    }

    public void ResignButton()
    {
        buttonObject = GameObject.FindGameObjectWithTag("GameController");
        Game sc = buttonObject.GetComponent<Game>();

        Debug.Log("resign");

        if (sc.GetCurrentPlayer() == "white")
        {
            sc.Winner("black");
            Debug.Log("blackWin");
        }
        else if (sc.GetCurrentPlayer() == "black")
        {
            sc.Winner("white");
            Debug.Log("whiteWin");
        } 
    }

    public void DrawButton()
    {
        Game sc = buttonObject.GetComponent<Game>();
        sc.Draw();
    }

  
}
