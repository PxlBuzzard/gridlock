using UnityEngine;
using System.Collections;

public class Timer {
	
	private float currentTime;
	public bool isFinished;

	// Use this for initialization
	public void Start () 
	{
		isFinished = false;
	}
	
	public void Countdown(float countdownTime)
	{
		currentTime = countdownTime;
		isFinished = false;
	}
	
	// Update is called once per frame
	public bool Update () 
	{
		currentTime -= Time.deltaTime;
		
		if (currentTime <= 0)
		{
			return isFinished = true;
		}
		return false;
	}
}
