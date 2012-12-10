using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class OTActionDebug : OTAction
{
    string msg = "";
    public OTActionDebug(string msg)
        : base("wait")
    {
        this.msg = msg;
    }
    protected override bool Execute()
    {
        Debug.Log(msg);
        return true;
    }
}
