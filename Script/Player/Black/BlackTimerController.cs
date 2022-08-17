using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackTimerController : MonoBehaviour
{

    // Total limit time
	private float totalTime;

	// Limit time (minute)
	[SerializeField]
	private int minute;

	// Limit time (second)
	[SerializeField]
	private float seconds;

	// Previous time
	private float oldSeconds;

	private Text timerText;

	private Game gameScript;
 
	void Start ()
	{
		GameObject game = GameObject.FindGameObjectWithTag("GameController");
		gameScript = game.GetComponent<Game>();

		totalTime = minute * 60 + seconds;
		oldSeconds = 0f;
		timerText = GetComponentInChildren<Text>();
		timerText.text = minute.ToString("00") + ":" + ((int) seconds).ToString("00");
	}
 
	void Update () 
	{
		string player = gameScript.GetCurrentPlayer();

		if (player == "black" && gameScript.gameOver == false)
		{
			this.GetComponent<Text>().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

			// Stop timer
			if (totalTime <= 0f)
				return;

		    // Measure the total time
		    totalTime = minute * 60 + seconds;
		    totalTime -= Time.deltaTime;
 
		    // Resetting
		    minute = (int) totalTime / 60;
		    seconds = totalTime - minute * 60;
 
		    // Display the time
		    if((int)seconds != (int)oldSeconds) 
			    DisplayTime((int)seconds, (int)oldSeconds);

		    oldSeconds = seconds;
		    // Time Limit
		    if(totalTime <= 0f)
			    gameScript.GetComponent<Game>().Winner("white");
		}
		else
			this.GetComponent<Text>().color = new Color (1.0f, 1.0f, 1.0f, 0.25f);
	}

	private void DisplayTime(int seconds, int oldSeconds)
	{
		timerText.text = minute.ToString("00") + ":" + ((int) seconds).ToString("00");
	}
}
