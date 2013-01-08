using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
	
	#region Class Variables
	/// <summary>
	/// The name of the the weapon file. 
	/// </summary>
    public string theWeaponName;
	
	/// <summary>
	/// How much damage each bullet or melee swing does.
	/// </summary>
    public int damagePerAttack;
	
	/// <summary>
	/// The sound the weapon makes.
	/// </summary>
    //public SoundEffect attackSound;
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="Weapon"/> is a charged weapon.
	/// </summary>
	/// <value>
	/// <c>true</c> if it is a charge weapon; otherwise, <c>false</c>.
	/// </value>
    public bool isCharge
    {
        get { return IsCharge; }
    }
	protected bool IsCharge;
	
	/// <summary>
	/// How much ammo decreases per shot.
	/// </summary>
    protected int ammoIncrement;
	
	/// <summary>
	/// total ammo the gun can hold.
	/// </summary>
    public int MaxAmmo
    {
        get { return maxAmmo; }
    }
	protected int maxAmmo;
	
	/// <summary>
	/// The current ammo count.
	/// </summary>
    public int currentAmmo;
	
	/// <summary>
	/// The time it takes to fire consecutive attacks
	/// </summary>
    protected int timeBetweenAttack;
	
	/// <summary>
	/// The time last fired.
	/// </summary>
    protected float timeLastFired;
    #endregion

    public virtual void Shoot() { }

    public virtual string CreateWeaponString() { return ""; }
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
