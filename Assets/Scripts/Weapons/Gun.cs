using UnityEngine;
using System.Collections;

/// <summary>
/// A generic gun class.
/// </summary>
public class Gun : Weapon {
	
	#region Variables
    public string bulletSprite;
	public OTAnimatingSprite player;
	public OTAnimatingSprite gun;
	public OTSound gunShot;
    #endregion
	
	/// <summary>
	/// Create a gun.
	/// </summary>
	void Start () 
	{
		gunShot = new OTSound("gunShot");
		gunShot.Volume(.02f);
		
		for (int i = 0; i < gun.animation.framesets.Length; i++)
			gun.animation.framesets[i].container = gun.spriteContainer;
	}

    /// <summary>
    /// Generic pistol, should only be used for testing purposes.
    /// </summary>
    public Gun(bool ChargeGun)
    {
        IsCharge = ChargeGun;

        if (IsCharge)
        {
            theWeaponName = "ChargePistol";
            damagePerAttack = 10;
            ammoIncrement = 20;
            timeBetweenAttack = 200;
            maxAmmo = 100;
        }
        else
        {
            theWeaponName = "AmmoPistol";
            damagePerAttack = 10;
            ammoIncrement = 1;
            timeBetweenAttack = 200;
            maxAmmo = 20;
        }
        //attackSound = theContentManager.Load<SoundEffect>("Sounds/Weapons/Guns/Pistol");
        currentAmmo = maxAmmo;
        //bulletSprite = theContentManager.Load<Texture2D>("Sprites/Gun/Bullet/bullet");
    }

    /// <summary>
    /// Creates a custom gun.
    /// </summary>
    /// <param name="weaponName">name of the gun</param>
    /// <param name="dmgPerAtk">damage per shot</param>
    /// <param name="gunSound">firing sound</param>
    /// <param name="weaponWeight">Light, Medium, or Heavy</param>
    /// <param name="isChargeGun">overheat (true) or ammo (false)</param>
    /// <param name="maxAmmoOrCharge">Maximum ammo/charge capacity</param>
    /// <param name="chargeAmountOrBulletsFired">Charge increment or number of bullets to fire</param>
    /// <param name="timeBetweenShots">The time between consecutive attacks</param>
    /// <param name="theBulletSprite">file path of the bullet sprite being used</param>
    public Gun(string weaponName, int dmgPerAtk, 
		string gunSound, bool isChargeGun, int maxAmmoOrCharge,
        int chargeAmountOrBulletsFired, int timeBetweenShots, string theBulletSprite)
    {
        theWeaponName = weaponName;
        damagePerAttack = dmgPerAtk;
        //attackSound = theContentManager.Load<SoundEffect>(gunSound);
        IsCharge = isChargeGun;
        ammoIncrement = chargeAmountOrBulletsFired;
        maxAmmo = maxAmmoOrCharge;
        timeBetweenAttack = timeBetweenShots;
        //bulletSprite = theContentManager.Load<Texture2D>(theBulletSprite);
        currentAmmo = maxAmmo;
        //bulletPool = new ObjectPool<Bullet>(10, new Bullet(bulletSprite, bulletColor));
    }

    /// <summary>
    /// Handles generic shooting from a gun.
    /// </summary>
    public override void Shoot()
    {
        if (ShootCheck())
        {
            //decrement current ammo amount and reset shoot timer
            currentAmmo -= ammoIncrement;
            timeLastFired = Time.time;

            //get bullet from pool and add to active bullets
            //Bullet temp = bulletPool.Get();
            //temp.damage = damagePerAttack;
            //Vector2 playerPosition = new Vector2(player.Position.X + 32, player.Position.Y + 32);
            //temp.pIndex = player.playerIndex;
            //temp.Fire(player.facing, playerPosition);
            //activeBullets.Add(temp);
            //attackSound.Play(0.1f, 0.0f, 0.0f);
        }
    }

    /// <summary>
    /// Checks if the gun is able to fire.
    /// </summary>
    /// <returns>
    /// If the gun is allowed to shoot (true).
    /// </returns>
    private bool ShootCheck()
    {
        if (Time.time - timeLastFired > timeBetweenAttack)
        {
            if (isCharge)
                return (currentAmmo - ammoIncrement >= 0);
            else
                return (currentAmmo > 0);
        }
        return false;
    }

    /// <summary>
    /// Updates bullets if charge weapon.
    /// </summary>
    void FixedUpdate()
    {
        if (isCharge && currentAmmo < maxAmmo)
            currentAmmo++;
    }

    /// <summary>
    /// Create a string of the gun for saving/loading.
    /// </summary>
    /// <returns>a string holding a guns' variables</returns>
    public override string CreateWeaponString()
    {
        //string gunHash = theWeaponName + "_" + damagePerAttack + "_" + //bulletColor. + "_" + 
        //    //attackSound.Name + "_" + 
		//	isCharge + "_" + maxAmmo + "_" + 
        //    ammoIncrement + "_" + timeBetweenAttack + "_" + bulletSprite.Name;
        //return gunHash;
		return "";
    }

    ///// <summary>
    ///// Creates a new Gun from a string.
    ///// </summary>
    ///// <param name="weaponString">the weapon string</param>
    ///// <returns>a gun</returns>
    //public static Gun WeaponFromString(ContentManager theContentManager, string weaponString)
    //{
    //    string[] parse = weaponString.Split('_');
    //    Color aBulletColor = new Color();
    //    aBulletColor.PackedValue = Convert.ToUInt32(parse[2]);
    //    GunWeight weight = (GunWeight)Enum.Parse(typeof(GunWeight), parse[4]);

    //    return new Gun(theContentManager, parse[0], Convert.ToInt32(parse[1]), aBulletColor, parse[3],
    //        weight, Convert.ToBoolean(parse[5]), Convert.ToInt32(parse[6]), Convert.ToInt32(parse[7]),
    //        Convert.ToInt32(parse[8]), parse[9]);
    //}
}