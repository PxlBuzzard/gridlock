using UnityEngine;
using System.Collections;
using System.Reflection;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Incremental size action
/// </summary>
public class OTActionSizeBy : OTActionTween
{
    Vector2 byValue;

    /// <summary>
    /// Incremental size action constructor
    /// </summary>
    /// <param name="byValue">Incremental size value</param>
    /// <param name="duration">Sizing duration</param>
    /// <param name="easing">Sizing easing function</param>
    public OTActionSizeBy(Vector2 byValue, float duration, OTEase easing)
        : base("size",Vector2.zero,duration,easing)
    {
        this.byValue = byValue;
    }


    
    protected override void Initialize()
    {
        base.Initialize();
        toValue = (Vector2)fromValue + byValue;
    }
    

}
