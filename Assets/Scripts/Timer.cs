using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {
	
	private float currentTime;
	public bool isFinished;

	// Use this for initialization
	void Start () 
	{
		isFinished = false;
	}
	
	void Start(float countdownTime)
	{
		currentTime = countdownTime;	
	}
	
	// Update is called once per frame
	void Update () 
	{
		currentTime -= Time.deltaTime;
		
		if (currentTime <= 0)
		{
			isFinished = true;
		}
	}
}
