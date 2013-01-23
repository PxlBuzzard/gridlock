using UnityEngine;
using System.Collections;

/// <summary>
/// Creates a health that hovers over a player.
/// </summary>
public class healthBar : MonoBehaviour {
    
	#region Variables
	private const int BAR_HEIGHT = 8;
    public int maxHealth;
    public int currentHealth;
    private Vector3 screenPosition;
    private float healthBarLength;
    private float maxHealthBarLength;
    private GUIStyle maxStyle;
    private GUIStyle currentStyle;
	private GUIStyle killStyle;
	public OTAnimatingSprite parent;
	public bool showKills;
	#endregion
	
	/// <summary>
	/// Start this instance.
	/// </summary>
    void Start()
    {
        //initialize size of the bar
        maxHealthBarLength = 75;
		healthBarLength = maxHealthBarLength;
        maxStyle = new GUIStyle();
        currentStyle = new GUIStyle();
		killStyle = new GUIStyle();
       
        //create color fill
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.red);
        tex.Apply();
       
        //style fullsize health bar
        maxStyle.normal.background = tex;
        maxStyle.alignment = TextAnchor.MiddleCenter;
        maxStyle.normal.textColor = Color.white;
       
        //style remaining health bar
        Texture2D tex2 = new Texture2D(1, 1);
        tex2.SetPixel(0, 0, Color.green);
        tex2.Apply();
        currentStyle.normal.background = tex2;
		
		//style the kill counter
		killStyle.fontSize = 24;
		killStyle.alignment = TextAnchor.MiddleCenter;
		killStyle.normal.textColor = Color.white;
		
		//show kill count above the health bar if attached to a player
		showKills = (parent.GetComponent<playerUpdate>());
    }
    
    /// <summary>
    /// Draws the GUI.
    /// </summary>
    void OnGUI ()
    {
        //position of the health bar onscreen
    	screenPosition = Camera.main.WorldToScreenPoint(parent.transform.position);
    	screenPosition.y = Screen.height - screenPosition.y;
		
		//draw the health boxes
        GUI.Box(new Rect(screenPosition.x - (maxHealthBarLength / 2), screenPosition.y - 42, maxHealthBarLength, BAR_HEIGHT), "", maxStyle);
        GUI.Box(new Rect(screenPosition.x - (maxHealthBarLength / 2), screenPosition.y - 42, healthBarLength, BAR_HEIGHT), "", currentStyle);
		
		//draw kill count
		if (showKills)
		{
			GUI.Box(new Rect(screenPosition.x - 100, screenPosition.y - 85, 200, 50), "Kills: " + parent.GetComponent<playerUpdate>().killScore, killStyle);
		}
	}
    
	/// <summary>
	/// Update this instance.
	/// </summary>
    void Update ()
    {
           
    }
	
	/// <summary>
	/// Changes the opacity of the bar.
	/// </summary>
	/// <param name='diff'>
	/// Amount to change bar opacity to.
	/// </param>
	/// <param name='decrement'>
	/// Decrement from current opacity (true), or set new absolute opacity (false).
	/// </param>
	public void barOpacity (float diff, bool decrement)
	{
		//get the current colors
		Color fadeColor = currentStyle.normal.background.GetPixel(0, 0);
		Color fadeColor2 = maxStyle.normal.background.GetPixel(0, 0);
		Color fadeColor3 = killStyle.normal.textColor;
		
		//change the alpha
		if (decrement)
		{
			fadeColor.a -= diff;
			fadeColor2.a -= diff;
			fadeColor3.a -= diff;
		}
		else
		{
			fadeColor.a = diff;
			fadeColor2.a = diff;
			fadeColor3.a = diff;
		}
		
		//set new color
		currentStyle.normal.background.SetPixel(0, 0, fadeColor);
		maxStyle.normal.background.SetPixel(0, 0, fadeColor2);
		killStyle.normal.textColor = fadeColor3;
		
		//send update to graphics card
		currentStyle.normal.background.Apply();
		maxStyle.normal.background.Apply();
	}
    
	/// <summary>
	/// Adjusts the health bar.
	/// </summary>
	/// <param name='adj'>
	/// The width of the green bar.
	/// </param>
    public void AdjustCurrentHealth (int adj)
    {
        currentHealth = adj;
        
		//keep the bar correctly sized
        if(currentHealth < 1)
            currentHealth = 0;
        if(currentHealth > maxHealth)
            currentHealth = maxHealth;
        if(maxHealth < 1)
            maxHealth = 1;
           
        healthBarLength = (float)maxHealthBarLength * (currentHealth / (float)maxHealth);
    }
}