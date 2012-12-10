using UnityEngine;
using System.Collections;
using System.Reflection;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Incremental movement action
/// </summary>
public class OTActionMoveBy : OTActionTween
{
    Vector2 byValue;

    /// <summary>
    /// Incremental movement action constructor
    /// </summary>
    /// <param name="byValue">Incremental movement value</param>
    /// <param name="duration">Movement duration</param>
    /// <param name="easing">Movement easing function</param>
    public OTActionMoveBy(Vector2 byValue, float duration, OTEase easing)
        : base("position",Vector2.zero,duration,easing)
    {
        this.byValue = byValue;
    }

    
    protected override void Initialize()
    {
        base.Initialize();
        toValue = (Vector2)fromValue + byValue;
    }
    

}
