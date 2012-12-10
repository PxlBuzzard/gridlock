using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Action Tree to serialize and group your actions.
/// </summary>
public class OTActionTree {

    private string _name = "";
    /// <summary>
    /// Name of this tree
    /// </summary>
    public string name
    {
        get
        {
            return _name;
        }

    }

    
    public List<OTActionTreeElement> rootElements
    {
        get
        {
            return root;
        }
    }
	
	private List<OTActionTreeElement> root = new List<OTActionTreeElement>();
	private Dictionary<string, OTActionTreeElement> elements = new Dictionary<string,OTActionTreeElement>();
	
    /// <summary>
    /// Action tree constructor
    /// </summary>
    /// <param name="name">Name of the tree</param>
	public OTActionTree(string name)
	{
        this._name = name;
	}
    /// <summary>
    /// Action tree constructor
    /// </summary>
    public OTActionTree()
    {
    }

    
    public void SetName(string name)
    {
        this._name = name;
    }

	/// <summary>
	/// Adds a first tree element to the tree
	/// </summary>
	/// <param name="name">Name of this tree element's action</param>
	/// <param name="duration">Duration override</param>
	/// <returns>The added tree element</returns>
	public OTActionTreeElement Action(string name, float duration)
	{
		OTActionTreeElement el = new OTActionTreeElement(this, null, name, duration);
		root.Add(el);
		return el;
	}

    /// <summary>
    /// Finds a tree element with a specific name
    /// </summary>
    /// <param name="elementName">Name of the tree element to find</param>
    /// <returns>Tree element or null is none was found</returns>
    public OTActionTreeElement Find(string elementName)
    {
        elementName = elementName.ToLower();
        if (elements.ContainsKey(elementName))
            return elements[elementName];
        else
            return null;
    }

    
    public void AddElement(OTActionTreeElement el)
    {
        string name = el.name.ToLower();

        if (!elements.ContainsKey(name))
            elements.Add(name,el);
    }

    /// <summary>
    /// Adds a first tree element to the tree
    /// </summary>
    /// <param name="name">Name of this tree element's action</param>
    /// <returns>Added tree element</returns>
    public OTActionTreeElement Action(string name)
    {
        return Action(name, -1);
    }

    /// <summary>
    /// Adds a wait action to the start of this tree
    /// </summary>
    /// <param name="duration">Wait duration in seconds</param>
    /// <returns>Added tree element</returns>
    public OTActionTreeElement Wait(float duration)
    {
        return Action("Wait", duration);
    }
	
    /// <summary>
    /// Adds a tree element to the root of the tree
    /// </summary>
    /// <param name="el">Tree element to add</param>
	public void Root(OTActionTreeElement el)
	{
		root.Add(el);
	}
	
}
