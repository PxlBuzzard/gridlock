using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Action tree element
/// </summary>
public class OTActionTreeElement {


    /// <summary>
    /// Name of this tree element
    /// </summary>
    public string name
    {
        get
        {
            return _name;
        }
    }

    /// <summary>
    /// Duration in seconds of this tree element
    /// </summary>
    public float duration
    {
        get
        {
            return _duration;
        }
    }

    /// <summary>
    /// Action tree of this element
    /// </summary>
    public OTActionTree tree
    {
        get
        {
            return _tree;
        }
    }

    
    public List<OTActionTreeElement> children
    {
        get
        {
            return _children;
        }
    }

	private string _name = "";
	private float _duration = -1;
	
	private OTActionTreeElement parent = null;
	private List<OTActionTreeElement> _children = new List<OTActionTreeElement>();
	private OTActionTree _tree = null;
		
    /// <summary>
    /// Action tree element constructor
    /// </summary>
    /// <param name="tree">Tree of this element</param>
    /// <param name="parent">Parament tree element</param>
    /// <param name="name">Name of the action to use</param>
    /// <param name="duration">Duration of this action in seconds</param>
	public OTActionTreeElement(OTActionTree tree, OTActionTreeElement parent, string name, float duration)
	{
		_name = name;
		this._duration = duration;
		this.parent = parent;
		this._tree = tree;

        tree.AddElement(this);
	}
	
    /// <summary>
    /// Adds a parallel tree element
    /// </summary>
    /// <param name="name">Action name of this tree element</param>
    /// <param name="duration">Action duration in seconds</param>
    /// <returns>Added tree element</returns>
	public OTActionTreeElement And(string name, float duration)
	{
		OTActionTreeElement el = new OTActionTreeElement(_tree, parent, name, duration);
		if (parent == null)
			_tree.Root(el);
		else
			parent._children.Add(el);
		return el;
	}

    /// <summary>
    /// Adds a parallel tree element
    /// </summary>
    /// <param name="name">Action name of this tree element</param>
    /// <returns>Added tree element</returns>
	public OTActionTreeElement And(string name)
	{
		return And(name,-1);
	}

    /// <summary>
    /// Adds a serial tree element
    /// </summary>
    /// <param name="name">Action name of this tree element</param>
    /// <param name="duration">Action duration in seconds</param>
    /// <returns>Added tree element</returns>
    public OTActionTreeElement FollowedBy(string name, float duration)
	{
		OTActionTreeElement el = new OTActionTreeElement(_tree, this, name, duration);
		_children.Add(el);
		return el;
	}

    /// <summary>
    /// Adds a serial tree element
    /// </summary>
    /// <param name="name">Action name of this tree element</param>
    /// <returns>Added tree element</returns>
    public OTActionTreeElement FollowedBy(string name)
    {
        return FollowedBy(name, -1);
    }

    /// <summary>
    /// Adds a (serial) wait tree element
    /// </summary>
    /// <param name="duration">Wait duration</param>
    /// <returns>Added tree element</returns>
    public OTActionTreeElement Wait(float duration)
    {
        return FollowedBy("Wait", duration);
    }
    /// <summary>
    /// End the tree by destroying the owner object
    /// </summary>
    public OTActionTreeElement Destroy()
    {
        return FollowedBy("Destroy");
    }
		
}
