using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Method call action
/// </summary>
/// <remarks>
/// The method that is to be called must be declared 'public'. Be sure to add the right parameters.
/// If no parameters are provided, the found method's parameters will be checked and owner (object) , name (String) and 
/// action (String = actionName ) parameters will be automaticly passed.
/// </remarks>
public class OTActionCall : OTAction
{
    Component component;
    object[] parameters;

    /// <summary>
    /// Method call action constructor
    /// </summary>
    /// <param name="name">Name of method to call</param>
    /// <param name="component">Script (Component) that has the method</param>
    /// <param name="parameters">Parameters to pass on</param>
    public OTActionCall(string name, Component component, object[] parameters)
        : base(name)
    {
        this.component = component;
        this.parameters = parameters;
    }
    
    public OTActionCall(string name, Component component)
        : base(name)
    {
        this.component = component;
        this.parameters = null;
    }

    object GetVar(string name)
    {
        if (component != null)
        {
            FieldInfo field = component.GetType().GetField(name);
            if (field != null)
            {
                return field.GetValue(component);
            }
            else
            {
                PropertyInfo prop = component.GetType().GetProperty(name);
                if (prop != null)
                    return prop.GetValue(component, null);
            }

        }
        return name;
    }

    bool inMethod = false;
    
    protected override bool Execute()
    {
        if (inMethod) return false;

        if (parameters!=null && parameters.Length > 0)
        {
            List<System.Type> types = new List<System.Type>();
            for (int p = 0; p < parameters.Length; p++)
            {
                if (parameters[p] is string)
                {
                    string s = parameters[p] as string;
                    if (s[0] == '@')
                    {
                        s = s.Substring(1, s.Length - 1);
                        object so = GetVar(s);
                        parameters[p] = so;
                    }
                }
                types.Add(parameters[p].GetType());
            }
            MethodInfo mi = null;
            try
            {
                mi = component.GetType().GetMethod(name, types.ToArray());
            }
            catch (System.Exception e)
            {
                Debug.LogError("Orthello Action : Error finding method " + name + " with provided parameters.");
                Debug.LogError(e.Message);
                return true;
            }

            try
            {
                if (mi != null)
                {
                    inMethod = true;
                    mi.Invoke(component, parameters);
                    inMethod = false;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Orthello Action : Error Calling " + name + " with provided parameters");
                Debug.LogError(e.Message);
            }
        }
        else
        {
            MethodInfo mi = null;
            try
            {
                mi = component.GetType().GetMethod(name);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Orthello Action : Method "+name+" could not be retrieved, try passing correct parameters.");
                Debug.LogError(e.Message);
                return true;
            }
            if (mi != null)
            {
                ParameterInfo[] api = mi.GetParameters();
                List<object> lo = new List<object>();
                if (api.Length > 0)
                {
                    for (int p = 0; p < api.Length; p++)
                    {
                        ParameterInfo pi = api[p];
                        if (pi.Name.ToLower() == "owner")
                            lo.Add(this.owner);
                        if (pi.Name.ToLower() == "name" && pi.ParameterType.Name == "String")
                        {
                            if (this.owner is OTObject)
                                lo.Add((this.owner as OTObject).name);
                            else
                                if (this.owner is GameObject)
                                    lo.Add((this.owner as GameObject).name);
                        }
                        if (pi.Name.ToLower() == "action" && pi.ParameterType.Name == "String")
                            lo.Add(this.ownerActionName);
                    }
                    inMethod = true;
                    mi.Invoke(component, lo.ToArray());
                    inMethod = false;
                }
                else
                {
                    inMethod = true;
                    mi.Invoke(component, null);
                    inMethod = false;
                }
            }
            else
            {
                Debug.LogWarning("Orthello Action : Method " + name + " not found! Check if it is a Public method.");
            }
        }
        return true;
    }

}