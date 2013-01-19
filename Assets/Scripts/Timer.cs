using UnityEngine;
using System.Collections;

/// <summary>
/// A timer written for Unity.
/// </summary>
/// <author>
/// Daniel Jost
/// </author>
public class Timer {
	
	private float currentTime;
	public bool isFinished = false;
	public bool isRunning = false;

	/// <summary>
	/// Start this instance.
	/// </summary>
	public void Start () 
	{

	}
	
	/// <summary>
	/// Countdown timer.
	/// </summary>
	/// <param name='countdownTime'>
	/// Time to countdown from.
	/// </param>
	public void Countdown (float countdownTime)
	{
		currentTime = countdownTime;
		isFinished = false;
		isRunning = true;
	}
	
	/// <summary>
	/// Update the timer.
	/// </summary>
	/// <returns>
	/// If the timer is finished (true).
	/// </returns>
	public bool Update () 
	{
		if(isRunning)
		{
			currentTime -= Time.deltaTime;
			
			if (currentTime <= 0)
			{
				isRunning = false;
				isFinished = true;
				return isFinished;
			}
			return false;
		}
		
		return isFinished;
	}
}
