using UnityEngine;
using System.Collections;
using System.Reflection;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Incremental rotate action
/// </summary>
public class OTActionRotateBy : OTActionTween
{
    float byValue;

    /// <summary>
    /// Incremental rotate action constructor
    /// </summary>
    /// <param name="byValue">Incremental rotate value</param>
    /// <param name="duration">Rotation duration</param>
    /// <param name="easing">Rotation easing function</param>
    public OTActionRotateBy(float byValue, float duration, OTEase easing)
        : base("rotation",0,duration,easing)
    {
        this.byValue = byValue;
    }

    
    protected override void Initialize()
    {
        base.Initialize();
        toValue = (float)fromValue + byValue;
    }
    

}
