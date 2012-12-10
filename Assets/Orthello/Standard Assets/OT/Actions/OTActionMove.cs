using UnityEngine;
using System.Collections;
using System.Reflection;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Move action
/// </summary>
public class OTActionMove : OTActionTween
{
    /// <summary>
    /// Move action controller
    /// </summary>
    /// <param name="fromValue">Movement start position</param>
    /// <param name="toValue">Movement end position</param>
    /// <param name="duration">Movement duration</param>
    /// <param name="easing">Movement easing function</param>
    public OTActionMove(Vector2 fromValue, Vector2 toValue, float duration, OTEase easing)
        : base("position",fromValue,toValue,duration,easing)
    {
    }

    
    public OTActionMove(Vector2 toValue, float duration, OTEase easing)
        : base("position", toValue, duration, easing)
    {
    }
}
