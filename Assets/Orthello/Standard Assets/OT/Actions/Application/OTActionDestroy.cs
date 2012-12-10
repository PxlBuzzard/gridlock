using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class OTActionDestroy : OTAction
{
    public OTActionDestroy()
        : base("OTDestroyObject")
    {
    }
    protected override bool Execute()
    {
        if (owner is OTObject)
        {
            OTActionController c = (owner as OTObject).Controller<OTActionController>() as OTActionController;
            if (c!=null)
                c.Stop();
            OT.DestroyObject(owner as OTObject);
        }
        else
            if (owner is GameObject)
                OT.DestroyObject(owner as GameObject);
        return true;
    }

}
