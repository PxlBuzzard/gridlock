using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


public class OTActionWait : OTAction
{
    
    public OTActionWait(float duration)
        : base("wait")
    {
        this.duration = duration;
    }

    
    protected override bool Execute()
    {
        if (time >= duration)
            return true;
        else
            return false;
    }

}
