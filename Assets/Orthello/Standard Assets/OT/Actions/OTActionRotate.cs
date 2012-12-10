using UnityEngine;
using System.Collections;
using System.Reflection;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Rotate action
/// </summary>
public class OTActionRotate : OTActionTween
{
    /// <summary>
    /// Rotate action controller
    /// </summary>
    /// <param name="fromValue">Start angle</param>
    /// <param name="toValue">End angle</param>
    /// <param name="duration">Rotation duration</param>
    /// <param name="easing">Rotation easing function</param>
    public OTActionRotate(float fromValue, float toValue, float duration, OTEase easing)
        : base("rotation",fromValue,toValue,duration,easing)
    {
    }

    
    public OTActionRotate(float toValue, float duration, OTEase easing)
        : base("rotation", toValue, duration, easing)
    {
    }
}
