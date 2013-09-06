using UnityEngine;
using System.Collections;
/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Orthello shape base class
/// </summary>
/// <remarks>
/// Shapes can be used as paths for sprite placement or movement around your scene.
/// 
/// <b><span style="color:red">IN PROGRESS</span></b> : We are working/thinking about also using the shapes to achieve shaped colliders
/// for user input, collision detection and physics (mesh colliders). Also generating
/// a shapes sprite mesh is under investigation. 
/// </remarks>
[ExecuteInEditMode]
public class OTShape: OTObject {

	/// <summary>
	/// Mesh mode enumeration
	/// </summary>
	/// <remarks>
	/// None = No mesh is created
	/// Line = the (out) Line for this mesh is created
	/// Fill = the mesh is a solid created shape
	/// </remarks>
	public enum MeshMode { None, Line } //, Fill, FilledOutline };
			
	// --------------------------------------------------------
	// public attributes (editor)
	// --------------------------------------------------------
		
    /// <summary>
    /// Show shape in scene editor
    /// </summary>
    public bool showShape = true;
	
    /// <summary>
    /// Color of shape in scene editor
    /// </summary>
    public Color lineColor = Color.white;
	
	public MeshMode _meshMode = MeshMode.None;
	public float _lineThickness = 1;
	public int  _lineSubdivide = 1;
	public Material  _lineMaterial = null;
			
	/// <summary>
	/// Gets the duration to follow this spline with a specific speed.
	/// </summary>
	public virtual float DurationWithSpeed(float speed)
	{
		return 0;
	}
		
	// --------------------------------------------------------
	// public attributes (getter/setter)
	// --------------------------------------------------------
    public bool inEdit
	{
		get
		{
			return _inEdit;
		}
		set
		{
			_inEdit = value;
		}
	}

	/// <summary>
	/// Gets the start position of this shape
	/// </summary>
	/// <value>
	/// The start position.
	/// </value>
    public Vector2 startPosition
    {
        get
        {
            return GetPosition(0);
        }
    }

	/// <summary>
	/// Gets the end position of this shape
	/// </summary>
	/// <value>
	/// The end position.
	/// </value>
    public Vector2 endPosition
    {
        get
        {
            return GetPosition(100);
        }
    }	
	
	/// <summary>
	/// Gets or sets the mesh mode.
	/// </summary>
	/// <value>
	/// The mesh mode.
	/// </value>
    public MeshMode meshMode
    {
        get
        {
            return _meshMode;
        }
		set
		{
			_meshMode = value;
			meshDirty = true;
			Update();
		}
    }
	
	public void MarkMeshDirty()
	{
		meshDirty = true;
	}
	
	/// <summary>
	/// Calculates intersection point between 2 lines
	/// </summary>
	/// <param name='start1'>
	/// Vector2 start point line 1
	/// </param>
	/// <param name='end1'>
	/// Vector2 end point line 1
	/// </param>
	/// <param name='start2'>
	/// Vector2 start point line 2
	/// </param>
	/// <param name='end2'>
	/// Vector2 end point line 1
	/// </param>
	/// <returns>
	/// Intersection point
	/// </returns>
    public Vector2 Intersect(Vector2 start1,Vector2 end1,Vector2 start2,Vector2 end2)
    {

        Vector2 offset1 = (end1 - start1) * 3;
        Vector2 offset2 = (end2 - start2) * 3;

        Vector2 dxy = new Vector2(-1,-1);
        Vector2 sdxy = new Vector2(-1,-1);
        float d = -1;
        for (float d1 = 0; d1 < 1; d1 += (float)1 / 2000)
        {
            float d2;
            // start1.x + d1 * offset1.x = start2.x + d2 * offset2.x
            d2 = ((start1.x + d1 * offset1.x) - start2.x) / offset2.x;
            if (d2>=0 && d2<=1)
                dxy.x = d2;
            // start1.y + d1 * offset1.y = start2.y + d2 * offset2.y
            d2 = ((start1.y + d1 * offset1.y) - start2.y) / offset2.y;
            if (d2 >= 0 && d2 <= 1)
                dxy.y = d2;

            if (dxy.y >= 0 && dxy.y <= 1 && dxy.x >= 0 && dxy.x <= 1)
            {
                if (d == -1) 
                {
                    d = d1;
                    sdxy = dxy;
                }
                else
                {
                    if (Mathf.Abs(dxy.x - dxy.y) < Mathf.Abs(sdxy.x - sdxy.y))
                    {
                        d = d1;
                        sdxy = dxy;
                    }
                }
            }
        }
        if (d!=-1)
            return start1 + (d * offset1);
        else
            return new Vector2(-1,-1);
    }
	
	
	/// <summary>
	/// Gets or sets the line thickness.
	/// </summary>
	/// <value>
	/// The line thickness.
	/// </value>
    public float lineThickness
    {
        get
        {
            return _lineThickness;
        }
		set
		{
			_lineThickness = value;
			meshDirty = true;
			Update();
		}
    }	

    public int lineSubdivide
    {
        get
        {
            return _lineSubdivide;
        }
		set
		{
			if (value>=1)
			{
				_lineSubdivide = value;
				meshDirty = true;
				Update();
			}
		}
    }	
	
	/// <summary>
	/// Gets or sets the line material.
	/// </summary>
	/// <value>
	/// The line material.
	/// </value>
    public Material lineMaterial
    {
        get
        {
            return _lineMaterial;
        }
		set
		{
			_lineMaterial = value;
			isDirty = true;
			Update();
		}
    }	
	
	
	// --------------------------------------------------------
	// private and protected attributes 
	// --------------------------------------------------------
	
	private bool _inEdit = false;
	private MeshMode _meshMode_ = MeshMode.None;
	private float _lineThickness_ = 1;
	private Material _lineMaterial_ = null;
	private int _lineSubdivide_ = 1;

	// --------------------------------------------------------
	// public methods
	// --------------------------------------------------------
	
	/// <summary>
	/// Gets the position of this shape at a specific path percentage
	/// </summary>
	/// <returns>
	/// The position as a 2D coordinate
	/// </returns>
	/// <param name='perc'>
	/// Position path percentage
	/// </param>
    public virtual Vector2 GetPosition(float perc)
    {
        return Vector2.zero;
    }

    /// <summary>
    /// Sets this shape as a path for an OTObject
    /// </summary>
    /// <param name="o">Orthello object</param>
    /// <param name="duration">Movement Duration</param>
    /// <param name="looping">Object will be looping on shape</param>
    /// <param name="addRotation">Add aditional rotation</param>
    /// <returns>A path controller for this object.</returns>
    public OTPathController IsPathFor(OTObject o, float duration, bool looping, float addRotation)
    {
        OTPathController moving = new OTPathController(o, "movingpath", this, duration);
        moving.looping = looping;
        moving.addRotation = addRotation;
        OTController c = o.Controller(typeof(OTPathController));
        if (c != null)
            o.RemoveController(c);
        o.AddController(moving);
        return moving;
    }
    /// <summary>
    /// Sets this shape as a path for a game object 
    /// </summary>
    /// <param name="g">Game object</param>
    /// <param name="duration">Movement Duration</param>
    /// <param name="looping">Object will be looping on shape</param>
    /// <param name="addRotation">Add aditional rotation</param>
    /// <returns>A path controller for this object.</returns>
    public OTPathController IsPathFor(GameObject g, float duration, bool looping, float addRotation)
    {
        OTObject o = g.GetComponent<OTObject>();
        if (o != null)
            return IsPathFor(o,duration,looping,addRotation);
        OTPathController moving = new OTPathController(o, "movingpath-" + g.name, this, duration);
        moving.looping = looping;
        moving.addRotation = addRotation;
        OTController c = OT.Controller(typeof(OTPathController), "movingpath-" + g.name);
        if (c != null)
            OT.RemoveController(c);
        OT.AddController(moving);
        return moving;
    }
	
	
	// --------------------------------------------------------
	// private and protected nethods 
	// --------------------------------------------------------
    
    protected override string GetTypeName()
    {
        return "OTShape";
    }

       
    protected virtual void DrawShape()
    {
    }
	
	protected virtual Mesh GetLineMesh()
	{
		return null;
	}
	
	protected override Mesh GetMesh()
	{
		if (meshMode == MeshMode.Line)
			return GetLineMesh();				
		
		return null;
	}

	
	
	protected override void CheckDirty()
	{
		if (_lineSubdivide<=0) _lineSubdivide = 1;
		
		if (_meshMode_!=_meshMode || 
			_lineThickness_!=_lineThickness || 
			_lineSubdivide_!=_lineSubdivide ||
			_lineMaterial_ != _lineMaterial )
			meshDirty = true;
	}

	void SetAttr()
	{
		_meshMode_ = _meshMode;
		_lineThickness_ = _lineThickness;
		_lineMaterial_ = _lineMaterial;
		_lineSubdivide_ = _lineSubdivide;
	}
	
	protected override void Clean()
	{
		SetAttr();
		if (meshMode != MeshMode.None)
			renderer.sharedMaterial = lineMaterial;
		
#if UNITY_EDITOR			
		if (!Application.isPlaying)
			UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
#endif				
		
	}
	
	new protected void Awake()
	{
		base.Awake();
		SetAttr();
	}
	    
    protected void OnDrawGizmos()
    {
        if (showShape && !inEdit)
        {
            Gizmos.color = lineColor;
            DrawShape();
        }
    }


}


class OTTriangulator
{
	public static int[] Triangulate(OTShape shape)
	{
		return new int[]{};
	}
}
