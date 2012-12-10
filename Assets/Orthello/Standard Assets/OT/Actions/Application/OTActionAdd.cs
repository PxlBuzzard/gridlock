using UnityEngine;
using System.Collections;
using System.Reflection;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Add value action
/// </summary>
public class OTActionAdd : OTAction
{

    
    protected object addValue;
    
    protected object value;

    new float duration;
    FieldInfo field = null;
    PropertyInfo prop = null;

    /// <summary>
    /// Add value action constructor
    /// </summary>
    /// <param name="name">Variable name</param>
    /// <param name="value">Valuye to add</param>
    public OTActionAdd(string name, object value)
        : base(name)
    {
        this.addValue = value;
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
            case "single": addValue = System.Convert.ToSingle(addValue); break;
            case "double": addValue = System.Convert.ToDouble(addValue); break;
            case "int": addValue = System.Convert.ToInt32(addValue); break;
        }

        switch (value.GetType().Name.ToLower())
        {
            case "single": value = (float)value + (float)addValue; break;
            case "double": value = (double)value + (double)addValue; break;
            case "int": value = (int)value + (int)addValue; break;
            case "vector2": value = (Vector2)value + (Vector2)addValue; break;
            case "vector3": value = (Vector3)value + (Vector3)addValue; break;
            case "color": value = new Color(
                    ((Color)value).r + ((Color)addValue).r,
                    ((Color)value).g + ((Color)addValue).g,
                    ((Color)value).b + ((Color)addValue).b,
                    ((Color)value).a + ((Color)addValue).a);
                break;
        }

        if (field != null)
            field.SetValue(owner, value);
        else
            if (prop != null)
                prop.SetValue(owner, value, null);        
        return true;
    }

}
