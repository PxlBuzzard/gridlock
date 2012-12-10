using UnityEngine;
using System.Collections;
using System.Reflection;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Value tweening action
/// </summary>
/// <destription>
/// Value can by of single, float, double, int , Color, Vector2 and Vector3 type.
/// </destription>
public class OTActionTween : OTAction
{

    
    protected object fromValue;
    
    protected object toValue;
    
    protected object value;

    OTEase easing;
    FieldInfo field = null;
    PropertyInfo prop = null;
    bool getFromValue = false;

    /// <summary>
    /// Value tween action constructor
    /// </summary>
    /// <param name="name">Variable name</param>
    /// <param name="fromValue">Starting value</param>
    /// <param name="toValue">Ending value</param>
    /// <param name="duration">Tween duration</param>
    /// <param name="easing">Tween easing function</param>
    public OTActionTween(string name, object fromValue, object toValue, float duration, OTEase easing)
        : base(name)
    {
        this.fromValue = fromValue;
        this.toValue = toValue;
        this._duration = duration;
        this.easing = easing;
    }

    
    public OTActionTween(string name, object toValue, float duration, OTEase easing)
        : base(name)
    {
        getFromValue = true;
        this.toValue = toValue;
        this._duration = duration;
        this.easing = easing;
    }

    
    protected override void Initialize()
    {
        base.Initialize();

        if (owner != null)
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

            if (value == null)
            {
                Debug.LogError("Orthello Action Tween variable [" + name + "] not found on " + (owner as OTObject).name + "!");
            }

        }
        else
        {
            Debug.LogError("Orthello Action Tween owner is null!");
        }


        if (getFromValue)
            fromValue = value;

    }

    
    protected override bool Execute()
    {
        if (value == null)
        {
            time = _duration;
            return true;
        }

        if (time > _duration) time = _duration;

        switch (value.GetType().Name.ToLower())
        {
            case "single":
                value = easing.ease(time, (float)fromValue, (float)toValue - (float)fromValue, _duration);
                break;
            case "double":
                value = easing.ease(time, (float)fromValue, (float)toValue - (float)fromValue, _duration);
                break;
            case "int":
                value = easing.ease(time, (int)fromValue, (int)toValue - (int)fromValue, _duration);
                break;
            case "vector2":
                Vector2 _toValue2 = (Vector2)toValue;
                Vector2 _fromValue2 = (Vector2)fromValue;
                Vector2 _value2 = (Vector2)value;

                if ((_toValue2 - _fromValue2).x!=0)
                    _value2.x = easing.ease(time, _fromValue2.x, (_toValue2 - _fromValue2).x, _duration);
                else
                    _value2.x = _fromValue2.x;

                if ((_toValue2 - _fromValue2).y != 0)
                    _value2.y = easing.ease(time, _fromValue2.y, (_toValue2 - _fromValue2).y, _duration);
                else
                    _value2.y = _fromValue2.y;

                value = _value2;
                break;
            case "vector3":
                Vector3 _toValue3 = (Vector3)toValue;
                Vector3 _fromValue3 = (Vector3)fromValue;
                Vector3 _value3 = (Vector3)value;

                if ((_toValue3 - _fromValue3).x != 0)
                    _value3.x = easing.ease(time, _fromValue3.x, (_toValue3 - _fromValue3).x, _duration);
                else
                    _value3.x = _fromValue3.x;

                if ((_toValue3 - _fromValue3).y != 0)
                    _value3.y = easing.ease(time, _fromValue3.y, (_toValue3 - _fromValue3).y, _duration);
                else
                    _value3.y = _fromValue3.y;

                if ((_toValue3 - _fromValue3).z != 0)
                    _value3.z = easing.ease(time, _fromValue3.z, (_toValue3 - _fromValue3).z, _duration);
                else
                    _value3.z = _fromValue3.z;

                value = _value3;
                break;
            case "color":
                Color _toColor = (Color)toValue;
                Color _fromColor = (Color)fromValue;

                float r = easing.ease(time, _fromColor.r, _toColor.r - _fromColor.r, _duration);
                float g = easing.ease(time, _fromColor.g, _toColor.g - _fromColor.g, _duration);
                float b = easing.ease(time, _fromColor.b, _toColor.b - _fromColor.b, _duration);
                float a = easing.ease(time, _fromColor.a, _toColor.a - _fromColor.a, _duration);

                value = new Color(r, g, b, a);
                break;
        }

        if (field != null)
            field.SetValue(owner, value);
        else
            if (prop != null)
                prop.SetValue(owner, value, null);
        
        if (time == _duration)
            return true;
        else
            return false;
    }

}
