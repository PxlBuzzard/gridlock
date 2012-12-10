using UnityEngine;
using System.Collections;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Alpha fade action
/// </summary>
/// <remarks>
/// This will only work if this action's target object has a meterial reference that supports
/// the alpha channel (like the default 'alpha' texture reference)
/// </remarks>
public class OTActionFade : OTActionTween {

    /// <summary>
    /// Fade action constructor
    /// </summary>
    /// <param name="fromValue">Alpha value to fade from</param>
    /// <param name="toValue">Alpha value to fade to</param>
    /// <param name="duration">Fade duration in seconds</param>
    /// <param name="easing">Fading easing function</param>
    public OTActionFade(float fromValue, float toValue, float duration, OTEase easing) :
        base("alpha", fromValue, toValue, duration, easing) 
    { 
    }

    
    public OTActionFade(float toValue, float duration, OTEase easing) :
        base("alpha", toValue, duration, easing)
    {
    }

}
