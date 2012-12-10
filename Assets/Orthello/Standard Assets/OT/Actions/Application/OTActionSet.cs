using UnityEngine;
using System.Collections;
using System.Reflection;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Set value action
/// </summary>
public class OTActionSet : OTAction
{

    
    protected object setValue;
    
    protected object value;

    new float duration;
    FieldInfo field = null;
    PropertyInfo prop = null;

    /// <summary>
    /// Set value action constructor
    /// </summary>
    /// <param name="name">Variable name</param>
    /// <param name="value">Valuye to set</param>
    public OTActionSet(string name, object value)
        : base(name)
    {
        this.setValue = value;
    }

    
    protected override void Initialize()
    {
        base.Initialize();

        if (owner!=null)
        {
            field = owner.GetType().GetField(name);
            if (field != null)
                value = field.GetValue(owner);
            else
            {
                prop = owner.GetType().GetProperty(name);
                if (prop != null)
                    value = prop.GetValue(owner, null);
            }

        }
    }

    
    protected override bool Execute()
    {
        if (value == null) return true;

        switch (value.GetType().Name.ToLower())
        {
            case "single": setValue = System.Convert.ToSingle(setValue); break;
            case "double": setValue = System.Convert.ToDouble(setValue); break;
            case "int": setValue = System.Convert.ToInt32(setValue); break;
        }

        if (field != null)
            field.SetValue(owner, setValue);
        else
            if (prop != null)
                prop.SetValue(owner, setValue, null);        
        return true;
    }

}
