using UnityEngine;
using System.Collections;
using System.Reflection;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Hide action
/// </summary>
public class OTActionHide : OTActionSet
{
    /// <summary>
    /// Hide action constructor
    /// </summary>
    public OTActionHide()
        : base("visible", false)
    {
    }
}
