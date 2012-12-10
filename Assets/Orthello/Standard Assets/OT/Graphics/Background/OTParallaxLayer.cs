using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Layer of an OTParalaxBackground
/// </summary>
[System.Serializable]
public class OTParallaxLayer {

	/// <summary>
	/// The size of the layer.
	/// </summary>
	/// <remarks>
	/// In pixels (absolute) or percentage depending on the parallax background setting.
	/// The percentage is related to the parallax background world size.
	/// </remarks>
    public Vector2 layerSize;
	/// <summary>
	/// Drawing gizmos when true.
	/// </summary>
    public bool drawGizmo = true;
	
	public Vector2 position
    {
        get
        {
            return _pos;
        }
    }
	
    public Rect rect
    {
        get
        {
            return _rect;
        }
        set
        {
            _rect = value;
            _pos = new Vector2(_rect.xMin + _rect.width / 2, _rect.yMin + _rect.height / 2);
        }
    }

    Rect _rect = new Rect(0, 0, 0, 0);
    Vector2 _pos = Vector2.zero;

    List<OTObject> objects = new List<OTObject>();
    Vector2 topLeft = new Vector2(0,0);

    void MoveObjects(Vector2 delta)
    {
        for (int o = 0; o<objects.Count; o++)
            objects[o].position += delta;
    }
	
		
    public void Move(Vector2 topLeft)
    {
        if (!Vector2.Equals(this.topLeft,topLeft))
            MoveObjects(topLeft - this.topLeft);
    }


}
