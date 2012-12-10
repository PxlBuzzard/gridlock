using UnityEngine;
using System.Collections;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Fade in action
/// </summary>
/// <remarks>
/// This will only work if this action's target object has a meterial reference that supports
/// the alpha channel (like the default 'alpha' texture reference)
/// </remarks>
public class OTActionFadeIn : OTActionFade {
    /// <summary>
    /// Fade in action constructor
    /// </summary>
    /// <param name="duration">Fade in duration in seconds</param>
    /// <param name="easing">Fade in easing function</param>
    public OTActionFadeIn(float duration, OTEase easing) :
        base( 1, duration, easing) 
    { 
    }
}
