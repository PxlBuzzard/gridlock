using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// OTShape object for multi corner shapes like: circle, rectangle, triangle etc.
/// </summary>
/// <description>
/// The resolution of this OTShape object is used to determine the number or corners. Create a rectangle 
/// by setting it to 4, or create a triangle by setting it to 3.
/// </description>
[ExecuteInEditMode]
public class OTCircle : OTShape
{


	// --------------------------------------------------------
	// public attributes (editor)
	// --------------------------------------------------------
	
    public float _radius = 50;
    /// <summary>
    /// Shape resolution
    /// </summary>
    /// <remarks>
    /// a resolution of 3 will give you a triangle shape, a resolution of 4 will generate a square.
    /// </remarks>
    public int resolution = 25;
    /// <summary>
    /// Always use a perfect circle
    /// </summary>
    /// <remarks>
    /// If this is set to true, the resolution is auto adjusted so that a prefect circle shape
    /// is used. In addition, when using this circle as a path, the location on the shape is
    /// calculated each time to match the exact circle coordinates instead of pre-generated
    /// resolution coordinates.
    /// </remarks>
    public bool perfectCircle = false;

	// --------------------------------------------------------
	// public properties (getter/setter)
	// --------------------------------------------------------
	
    /// <summary>
    /// Radius of the circle shape
    /// </summary>
    public float radius
    {
        get
        {
            return _radius;
        }
        set
        {
            _radius = value;
            size = new Vector2(radius * 2, radius * 2);
        }
    }
	
	
	/// <summary>
	/// Gets the duration to follow this spline with a specific speed.
	/// </summary>
	public override float DurationWithSpeed(float speed)
	{
		return (( 2 * Mathf.PI * radius) / speed);
	}
	
	
	// --------------------------------------------------------
	// private attributes
	// --------------------------------------------------------
	
    private float _radius_ = 25;
    private Vector2[] points = new Vector2[] { };


	// --------------------------------------------------------
	// public methods
	// --------------------------------------------------------
	
    /// <summary>
    /// Get coordinate of the circle at a certain position.
    /// </summary>
    /// <param name="perc">position percentage from start (0) to end (100)</param>
    /// <returns>Position 2d coordinate</returns>
    public override Vector2 GetPosition(float perc)
    {
        return GetPosition(perc, false);
    }
	
	// --------------------------------------------------------
	// private protected methods
	// --------------------------------------------------------
	
	
	protected override Mesh GetLineMesh()
	{
		if (points.Length<=1) 
			return null;
		
		Mesh mesh = new Mesh();
		
		Vector3[] verts = new Vector3[points.Length * 4 * lineSubdivide];
		Vector2[] uv = new Vector2[points.Length * 4 * lineSubdivide];
		int[] tris = new int[points.Length * 6 * lineSubdivide];
		
		Vector3 oldPosition = transform.position;
		transform.position = new Vector3(0,0,0);
		
		for (int pi=0; pi<points.Length; pi++)
		{							
			Vector2 p1=((points[pi]-position)/_size.x);								
			Vector2 p2=((points[0]-position)/_size.x);
			if (pi+1<points.Length)			
				p2=((points[pi+1]-position)/_size.x);

			
			Vector2 vd1 = p1.normalized/_size.x;
			Vector2 pl1 = p1 -(vd1 * .5f * lineThickness);
			Vector2 pr1 = p1 +(vd1 * .5f * lineThickness);

			Vector2 vd2 = p2.normalized/_size.x;
			Vector2 pl2 = p2 -(vd2 * .5f * lineThickness);
			Vector2 pr2 = p2 +(vd2 * .5f * lineThickness);
			
			Vector2 pld = (pl2-pl1)/lineSubdivide;
			Vector2 prd = (pr2-pr1)/lineSubdivide;
			
			Matrix4x4 m = new Matrix4x4();
			m.SetTRS(Vector3.zero,Quaternion.Euler(0,0,90), Vector3.one);
																		
			for (int s=0; s<lineSubdivide; s++)
			{																													
				int baseIndex = pi*(4*lineSubdivide)+(s*4);
							
				verts[baseIndex] = pl1 + (s * pld);				
				verts[baseIndex+1] = pr1 + (s * prd);
				verts[baseIndex+2] = pl1 + ((s+1) * pld);
				verts[baseIndex+3] = pr1 + ((s+1) * prd);
								
				uv[baseIndex] = new Vector2(0,0);
				uv[baseIndex+1] = new Vector2(1,0);
				uv[baseIndex+2] = new Vector2(0,1);
				uv[baseIndex+3] = new Vector2(1,1);
				
				new int[] { baseIndex, baseIndex+2, baseIndex+1, baseIndex+2, baseIndex+3, baseIndex+1 }.CopyTo(tris,(pi*6*lineSubdivide)+(s * 6));							
												
			}			
										
		}
		
		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.uv = uv;
		
		transform.position = oldPosition;
		
		return mesh;
	}
		
    protected override string GetTypeName()
    {
        return "OTCircle";
    }
	
    private Vector2 GetPosition(float perc, bool perfect)
    {
        if (perfectCircle || perfect)
        {
            Vector2 v = new Vector3(radius * -1, 0, 0);
            Matrix4x4 m = new Matrix4x4();
            m.SetTRS(Vector3.zero, Quaternion.Euler(0, 0, rotation - ((perc * 360))), Vector3.one);
            v = m.MultiplyPoint3x4(v);
            return position + v;
        }
        else
        {
            if (points.Length == 0) return
                GetPosition(perc, true);

            float pp = 1.0f / points.Length;
            int pf = (int)Mathf.Floor(perc / pp);

            Vector2 p1 = points[pf];
            Vector2 p2 = points[0];
            if (pf < points.Length - 1)
                p2 = points[pf + 1];
            Vector2 v = p2 - p1;
            float pd = ((perc - (pf * pp)) / pp);

            v = position + (p1 + (v * pd));
            Matrix4x4 m = new Matrix4x4();
            m.SetTRS(Vector3.zero, Quaternion.Euler(0, 0, rotation - ((perc * 360))), Vector3.one);
            v = m.MultiplyPoint3x4(v);
            return v;
        }
    }		
	
    protected override void DrawShape()
    {
        if (perfectCircle)
        {
            if (radius < 10) resolution = 25;
            else
                if (radius < 100) resolution = 50;
                else
                    if (radius < 500) resolution = 100;
                    else
                        resolution = 250; for (float p = 0; p < 1; p += 1.0f / resolution)
            {
                float p2 = (p + 1.0f / resolution);
                if (p2 >= 100) p2 -= 100;
                Gizmos.DrawLine(GetPosition(p), GetPosition(p2));
            }
            Gizmos.DrawSphere(GetPosition(0), radius / 20);
        }
        else
        {
            for (int r = 0; r < resolution - 1; r++)
                Gizmos.DrawLine(position + points[r], position + points[r + 1]);
            Gizmos.DrawLine(position + points[resolution - 1], position + points[0]);
            Gizmos.DrawSphere(position + points[0], radius / 20);
        }
    }

    
    new protected void Start()
    {
        base.Start();
        _radius_ = _radius;
        points = GetPoints();
    }

    Vector2[] GetPoints()
    {
        List<Vector2> points = new List<Vector2>();
        for (float p = 0; p < 1; p += 1.0f / resolution)
            points.Add(GetPosition(p, true) - position);
        return points.ToArray();
    }

    
    new protected void Update()
    {
        if (resolution < 3) resolution = 3;
        if (radius < 1) radius = 1;

        if (_radius_ != _radius || rotation != _rotation || meshDirty)
        {
			meshDirty = true;
            _radius_ = _radius;
            size = new Vector2(radius * 2, radius * 2);
            points = GetPoints();
        }
        else
        {
            if (size.x != radius * 2)
			{
				meshDirty = true;
                radius = (size.x / 2);
#if UNITY_EDITOR			
				if (!Application.isPlaying)
					UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
#endif				
			}
            else
                if (size.y != radius * 2)
                    radius = (size.y / 2);
            if (points.Length != resolution || meshDirty)
			{
				meshDirty = true;
                points = GetPoints();
			}
        }

        base.Update();
        Vector2 nsize = new Vector2(radius * 2, radius * 2);
        if (!Vector2.Equals(size, nsize))
            size = nsize;
    }



}
