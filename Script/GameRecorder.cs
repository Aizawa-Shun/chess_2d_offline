using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRecorder : MonoBehaviour
{
    public GameObject canvas;
    public GameObject text;
    public GameObject turn;

    public int turnNumber;
    public string file;
    public int rank;


    private void Start()
    {
        GameObject record = (GameObject)Instantiate(text);
        GameObject num = (GameObject)Instantiate(turn);

        // White
        record = (GameObject)Instantiate(text);
        record.transform.SetParent(canvas.transform, false); 
        record.transform.localPosition = new Vector3(-100, -20, 0);
        record.transform.GetComponent<Text>().text = file + rank;

        // Black
        record = (GameObject)Instantiate(text);
        record.transform.SetParent(canvas.transform, false); 
        record.transform.localPosition = new Vector3(20, -20, 0);
        record.transform.GetComponent<Text>().text = file + rank;

        // Turn
        num = (GameObject)Instantiate(turn);
        num.transform.SetParent(canvas.transform, false); 
        num.transform.localPosition = new Vector3(-120, -20, 0);
        num.transform.GetComponent<Text>().text = turnNumber + ".";
    }        

    private void Update()
    {
    }
}