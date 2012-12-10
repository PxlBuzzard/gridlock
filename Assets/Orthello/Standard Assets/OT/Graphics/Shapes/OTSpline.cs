using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Spline shape
/// </summary>
[ExecuteInEditMode]
public class OTSpline : OTShape {
    /// <summary>
    /// Array of spline points (<seealso cref="OTSplinePoint"/>)
    /// </summary>
    [HideInInspector]
    public OTSplinePoint[] points = new OTSplinePoint[] { };
    /// <summary>
    /// Close or open spline
    /// </summary>
    public bool closed = false;
    
    public int _pointIndex = 0;
    
    public Vector2 _pointPosition = Vector2.zero;
    
    public OTSplinePoint.OTSplinePointType _pointType = OTSplinePoint.OTSplinePointType.Align;
    /// <summary>
    /// Color of control handles
    /// </summary>
    public Color handleColor = Color.black;
    /// <summary>
    /// Color of spline points
    /// </summary>
    public Color pointColor = Color.black;
    /// <summary>
    /// Color of selected point
    /// </summary>
    public Color selectedColor = Color.red;
    /// <summary>
    /// Spline resolution ( between 2 points )
    /// </summary>
    public int resolution = 25;

    bool pointsDirty = true;
    OTSplinePoint[] _points = new OTSplinePoint[] { };
    Vector2[] vPoints = new Vector2[] { };
    float[] distance = new float[] { };
	
	/// <summary>
	/// Gets the length of the spline in pixels/units
	/// </summary>
	public float length
	{
		get
		{
			float res = 0;
			for (int d=0; d<distance.Length; d++)
				res += distance[d];
			return res;
		}
	}
	
	/// <summary>
	/// Gets the duration to follow this spline with a specific speed.
	/// </summary>
	public override float DurationWithSpeed(float speed)
	{
		return (length / speed);
	}

    
	protected override Mesh GetLineMesh()
	{
		Vector2[] points = vPoints;
		if (points.Length<=1) 
			return null;
		
		Mesh mesh = new Mesh();
		
		Vector3[] verts = new Vector3[points.Length * 4 * lineSubdivide];
		Vector2[] uv = new Vector2[points.Length * 4 * lineSubdivide];
		int[] tris = new int[points.Length * 6 * lineSubdivide];
		
		Vector3 oldPosition = transform.position;
		transform.position = new Vector3(0,0,0);
		
		int pl = points.Length;
		if (!closed) pl -= 1;
		
		for (int pi=0; pi<pl; pi++)
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
        return "OTSpline";
    }

    Vector2[] GetPoints()
    {
        List<Vector2> points = new List<Vector2>();
        List<float> distance = new List<float>();
        for (int i = 0; i < this.points.Length - 1; i++)
            for (int r = 0; r < resolution; r++)
                points.Add(BezierPoint(this.points[i], this.points[i + 1], r * (1.0f / resolution)));

        if (closed)
        {
            for (int r = 0; r < resolution; r++)
                points.Add(BezierPoint(this.points[this.points.Length - 1], this.points[0], r * (1.0f / resolution)));
            points.Add(BezierPoint(this.points[0], this.points[1], 0));
        }
        else
            points.Add(BezierPoint(this.points[this.points.Length - 1], this.points[0], 0));

        float di = 0, tdi = 0;
        int pi = 0; int pii = 0;
        for (int p = 0; p < points.Count; p++)
        {
            if (pii == 0 && pi<this.points.Length)
            {
                this.points[pi].distance = tdi;
                pi++;
            }

            pii++;
            if (pii==resolution-1) 
                pii=0;

            if (p > 0)
                di = (points[p] - points[p - 1]).magnitude;
            distance.Add(di);
            tdi += di;
        }
        if (!closed)
            this.points[this.points.Length - 1].distance = tdi;

        pi = 0; pii = 0;
        for (int p = 0; p < points.Count; p++)
        {
            distance[p] = distance[p] / tdi;

            if (pii == 0 && pi<this.points.Length)
            {
                this.points[pi].perc = distance[p];
                pi++;
            }

            pii++;
            if (pii == resolution - 1)
                pii = 0;

            if (p > 0)
                distance[p] = distance[p] + distance[p - 1];
        }
        if (!closed)
            this.points[this.points.Length - 1].perc = 1;

        this.distance = distance.ToArray();

        pointsDirty = false;
        return points.ToArray();
    }

    new void Start()
    {
        vPoints = GetPoints();
        base.Start();
    }

    void SavePoints()
    {
        _points = new OTSplinePoint[points.Length];
        for (int p = 0; p < points.Length; p++)
            _points[p] = points[p].Clone();
    }

    new void Update()
    {
        if (resolution < 1) resolution = 1;

        if (_points.Length != points.Length)
        {
            SavePoints();
            pointsDirty = true;
        }
        else
        {
            for (int p=0; p<points.Length; p++)
                if (!OTSplinePoint.Equals(points[p],_points[p]))
                {
                    SavePoints();
                    pointsDirty = true;
                    break;
                }
        }

        int vpl = (resolution * ((closed)?points.Length:points.Length-1)) +1;
        if (vPoints.Length != vpl || pointsDirty)
            vPoints = GetPoints();

        base.Update();
    }

    new void Awake()
    {
        base.Awake();

        if (!Application.isPlaying && points.Length==0)
        {
            transform.localScale = new Vector3(100, 100, 1);

            OTSplinePoint point1 = new OTSplinePoint();
            point1.pointType = OTSplinePoint.OTSplinePointType.Align;
            point1.position = new Vector2(-50, 0);
            point1.ctrl1 = new Vector2(0,-50);
            point1.ctrl2 = new Vector2(0, 50);

            OTSplinePoint point2 = new OTSplinePoint();
            point2.pointType = OTSplinePoint.OTSplinePointType.Align;
            point2.position = new Vector2(50, 0);
            point2.ctrl1 = new Vector2(0, 50);
            point2.ctrl2 = new Vector2(0, -50);

            points = new OTSplinePoint[] { point1, point2 };
            
        }
    }

    /// <summary>
    /// Get a bezier point at a specific position (0-1) between two spline points
    /// </summary>
    /// <param name="p1">Spline point 1</param>
    /// <param name="p2">Spline point 2</param>
    /// <param name="u">Position on spline</param>
    /// <returns>Vector2 point</returns>
    public Vector2 BezierPoint(OTSplinePoint p1, OTSplinePoint p2, float u)
    {
        Vector2 anchor1 = p1.position;
        Vector2 anchor2 = p2.position;
        Vector2 control1 = anchor1 + p1.ctrl2;
        Vector2 control2 = anchor2 + p2.ctrl1;
        return BezierPoint(anchor1, control1, anchor2, control2, u);
    }

    Vector2 BezierPoint(Vector2 anchor1, Vector2 control1, Vector2 anchor2, Vector2 control2, float u)
    {
        Vector2 pos = Vector2.zero;
        pos.x = Mathf.Pow(u, 3) * (anchor2.x + 3 * (control1.x - control2.x) - anchor1.x)
            + 3 * Mathf.Pow(u, 2) * (anchor1.x - 2 * control1.x + control2.x)
            + 3 * u * (control1.x - anchor1.x) + anchor1.x;

        pos.y = Mathf.Pow(u, 3) * (anchor2.y + 3 * (control1.y - control2.y) - anchor1.y)
            + 3 * Mathf.Pow(u, 2) * (anchor1.y - 2 * control1.y + control2.y)
            + 3 * u * (control1.y - anchor1.y) + anchor1.y;
        return pos;
    }

    Vector3 _p(Vector2 pos)
    {
        return new Vector3(pos.x, pos.y, transform.position.z);
    }

    void DrawBezier(Vector2 point1, Vector2 ctrl1, Vector2 point2, Vector2 ctrl2)
    {
        Vector2 lp = Vector2.zero;
        Gizmos.color = lineColor;
        //loop through 100 steps of the curve
        for (float u = 0; u <= 1; u += (float)1 / resolution)
        {
            Vector2 pos = BezierPoint(point1, ctrl1, point2, ctrl2, u);
            if (u == 0)
                Gizmos.DrawLine(_p(point1), _p(pos));
            else
                Gizmos.DrawLine(_p(lp), _p(pos));
            lp = pos;
        }
        //Let the curve end on the second anchorPoint     
        Gizmos.DrawLine(_p(lp), _p(point2));
    }

    
    protected override void DrawShape()
    {
        float scale = Camera.current.orthographicSize / 100;

        Vector2 pos = transform.position;
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector2 anchor1 = pos + points[i].position;
            Vector2 anchor2 = pos + points[i+1].position;
            Vector2 control1 = anchor1 + points[i].ctrl2;
            Vector2 control2 = anchor2 + points[i+1].ctrl1;
            DrawBezier(anchor1, control1, anchor2, control2);
        }
        
        if (closed)
        {
            Vector2 anchor1 = pos + points[points.Length-1].position;
            Vector2 anchor2 = pos + points[0].position;
            Vector2 control1 = anchor1 + points[points.Length-1].ctrl2;
            Vector2 control2 = anchor2 + points[0].ctrl1;
            DrawBezier(anchor1, control1, anchor2, control2);
        }

        Gizmos.color = pointColor;
        for (int i = 0; i < points.Length; i++)
        {
            Vector2 anchor1 = pos + points[i].position;
            Gizmos.DrawSphere(_p(anchor1), 2f * scale);
        }


    }

    
    public override Vector2 GetPosition(float perc)
    {
        for (int i = 0; i < vPoints.Length; i++)
        {
            if (perc <= distance[i] )
            {
                if (perc == distance[i])
                    return position + vPoints[i];
                else
                {
                    Vector2 p2 = vPoints[i];
                    Vector2 p1 = vPoints[i - 1];

                    float pp = perc - distance[i - 1];
                    float pd = distance[i] - distance[i - 1];

                    return position + (p1 + ((p2 - p1) * (pp / pd)));
                }
            }
        }
        return position + vPoints[vPoints.Length-1];
    }


    
    new protected void OnDrawGizmos()
    {
        if (resolution < 1) resolution = 1;
        base.OnDrawGizmos();
    }

}

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : A point on a spline
/// </summary>
[System.Serializable]
public class OTSplinePoint
{
    /// <summary>
    /// Point type enumeration
    /// </summary>
    public enum OTSplinePointType { 
        /// <summary>
        /// Aligning controls
        /// </summary>
        Align, 
        /// <summary>
        /// Free controls
        /// </summary>
        Free 
    };
    /// <summary>
    /// Point type
    /// </summary>
    public OTSplinePointType pointType;
    /// <summary>
    /// Point position
    /// </summary>
    /// <remarks>
    /// The position is relative to the <seealso cref="OTSpline"/>'s position
    /// </remarks>
    public Vector2 position;
    /// <summary>
    /// Control handle 1
    /// </summary>
    /// <remarks>
    /// The control handle position is relative to the <seealso cref="OTSpline"/>'s position
    /// </remarks>
    public Vector2 ctrl1;
    /// <summary>
    /// Control handle 2
    /// </summary>
    /// <remarks>
    /// The control handle position is relative to the <seealso cref="OTSpline"/>'s position
    /// </remarks>
    public Vector2 ctrl2;
    /// <summary>
    /// Distance of this point on spline
    /// </summary>
    public float distance;
    /// <summary>
    /// location of this point on spline in percentage (0-1)
    /// </summary>
    public float perc;

    /// <summary>
    /// Checks if 2 spline points are equal
    /// </summary>
    /// <param name="p1">Point 1</param>
    /// <param name="p2">Point 2</param>
    /// <returns>True if equal</returns>
    public static bool Equals(OTSplinePoint p1, OTSplinePoint p2)
    {
        return (Vector2.Equals(p1.position,p2.position) &&
            Vector2.Equals(p1.ctrl1,p2.ctrl1) &&
            Vector2.Equals(p1.ctrl2,p2.ctrl2));
    }

    /// <summary>
    /// Clones a spline point
    /// </summary>
    /// <returns>Cloned spline point</returns>
    public OTSplinePoint Clone()
    {
        OTSplinePoint p = new OTSplinePoint();
        p.pointType = pointType;
        p.position = position;
        p.ctrl1 = ctrl1;
        p.ctrl2 = ctrl2;
        return p;
    }
}
