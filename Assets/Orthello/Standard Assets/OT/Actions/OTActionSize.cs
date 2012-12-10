using UnityEngine;
using System.Collections;
using System.Reflection;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Size action
/// </summary>
public class OTActionSize : OTActionTween
{
    /// <summary>
    /// Size action controller
    /// </summary>
    /// <param name="fromValue">Start size</param>
    /// <param name="toValue">End size</param>
    /// <param name="duration">Sizing duration</param>
    /// <param name="easing">Sizing easing function</param>
    public OTActionSize(Vector2 fromValue, Vector2 toValue, float duration, OTEase easing)
        : base("size",fromValue,toValue,duration,easing)
    {
    }

    
    public OTActionSize(Vector2 toValue, float duration, OTEase easing)
        : base("size", toValue, duration, easing)
    {
    }
}
