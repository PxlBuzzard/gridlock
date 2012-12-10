using UnityEngine;
using System.Collections;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Fade out action
/// </summary>
/// <remarks>
/// This will only work if this action's target object has a meterial reference that supports
/// the alpha channel (like the default 'alpha' texture reference)
/// </remarks>
public class OTActionFadeOut : OTActionFade
{
    /// <summary>
    /// Fade out action constructor
    /// </summary>
    /// <param name="duration">Fade out duration in seconds</param>
    /// <param name="easing">Fade out easing function</param>
    public OTActionFadeOut(float duration, OTEase easing) :
        base( 0, duration, easing)
    {
    }


}
