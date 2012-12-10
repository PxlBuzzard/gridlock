using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Parallax Background Component
/// </summary>
[ExecuteInEditMode]
public class OTParallaxBackground : MonoBehaviour
{
	/// <summary>
	/// Parallax layer size type.
	/// </summary>
    public enum ParallaxLayerSizeType 
	{ 	
		/// <summary>
		/// Absolute size in pixels
		/// </summary>
		Absolute, 
		/// <summary>
		/// Size is specified in percentage
		/// </summary>
		Percentage 
	};
	/// <summary>
	/// World dimension for the parallax background
	/// </summary>
    public Rect world = new Rect(-2000, -400, 4000, 800);
    /// <summary>
    /// View size for the parallax background
    /// </summary>
	public Vector2 view = new Vector2(1024, 768);
	/// <summary>
	/// Array with the parallax layers.
	/// </summary>
    public OTParallaxLayer[] depthLayers = new OTParallaxLayer[] { };
    /// <summary>
    /// Parallax layer size type
    /// </summary>
	public ParallaxLayerSizeType sizeType = ParallaxLayerSizeType.Percentage;
	
	/// <summary>
	/// Drawing gizmos when true
	/// </summary>
    public bool drawGizmos = false;
    /// <summary>
    /// The color of the gizmos.
    /// </summary>
	public Color gizmosColor = new Color(0.9f, 0.7f, 0.6f);
	
	
    [HideInInspector]
    public bool prefabBroken = false;	
		
    List<GameObject> layerObjects = new List<GameObject>();

    Vector2 worldCenter;
    Vector2 viewPos = new Vector2(0, 0);

    void SetLayerObjects()
    {
        layerObjects.Clear();
        for (int l = 0; l < depthLayers.Length; l++)
        {
            Transform t = transform.FindChild("layer-" + l);
            if (t == null)
            {
                GameObject g = new GameObject();
                g.name = "layer-" + l;
                g.transform.parent = transform;
            }
            else
                layerObjects.Add(t.gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        if (!OT.isValid)
            return;

        viewPos = OT.view.position;

        if (OT.view.worldBounds.width == 0 || OT.view.worldBounds.height == 0)
            OT.view.worldBounds = world;

        worldCenter = new Vector2(world.xMin + (world.width / 2), world.yMin + (world.height / 2));
        SetLayerObjects();
        SetLayers();
    }

    void SetLayers()
    {
        for (int l = 0; l < depthLayers.Length; l++)
        {
            OTParallaxLayer layer = depthLayers[l];
            float sx = layer.layerSize.x;
            float sy = layer.layerSize.y;

            if (sizeType == ParallaxLayerSizeType.Percentage)
            {
                sx = (view.x / 100) * sx;
                sy = (view.y / 100) * sy;
            }


            Gizmos.color = gizmosColor;

            float px = 0;
            if (sx > view.x)
            {
                float pp = OT.view.position.x - (world.xMin + (view.x / 2));
                px = world.x + (Mathf.Clamp(pp / (world.width - view.x), 0f, 1f) * (world.width - sx));
            }
            else
                px = OT.view.position.x - sx / 2;

            float py = 0;
            if (sy > view.y)
            {
                float pp = OT.view.position.y - (world.yMin + (view.y / 2));
                py = world.y + (Mathf.Clamp(pp / (world.height - view.y), 0f, 1f) * (world.height - sy));
            }
            else
                py = OT.view.position.y - sy / 2;

            layer.rect = new Rect(px, py, sx, sy);
            layerObjects[l].transform.position = layer.position;
        }
    }

    // Update is called once per frame
    void Update()
    {				
        if (Application.isEditor)
        {
            if (!Vector3.Equals(transform.position, (Vector3)worldCenter))
                transform.position = worldCenter;

            if (!Application.isPlaying)
                worldCenter = new Vector2(world.xMin + (world.width / 2), world.yMin + (world.height / 2));

            if (layerObjects.Count != depthLayers.Length)
                SetLayerObjects();											
        }
        if (!Vector2.Equals(viewPos, OT.view.position))
        {
            viewPos = OT.view.position;
            SetLayers();
        }
    }

    void DrawRect(Rect r, Color c)
    {
        Gizmos.color = c;
        Gizmos.DrawLine(new Vector3(r.xMin, r.yMin, 900), new Vector3(r.xMax, r.yMin, 900));
        Gizmos.DrawLine(new Vector3(r.xMin, r.yMin, 900), new Vector3(r.xMin, r.yMax, 900));
        Gizmos.DrawLine(new Vector3(r.xMax, r.yMin, 900), new Vector3(r.xMax, r.yMax, 900));
        Gizmos.DrawLine(new Vector3(r.xMin, r.yMax, 900), new Vector3(r.xMax, r.yMax, 900));
    }

    Color Darker(Color c, float perc)
    {
        return new Color(c.r * perc, c.g * perc, c.b * perc);
    }
	
	
    protected void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            DrawRect(world, Darker(gizmosColor, 0.5f));
            DrawRect(new Rect(OT.view.position.x - view.x / 2, OT.view.position.y - view.y / 2, view.x, view.y), Darker(gizmosColor, 0.5f));
            for (int l = 0; l < depthLayers.Length; l++)
            {
                OTParallaxLayer layer = depthLayers[l];
                if (layer.drawGizmo)
                    DrawRect(layer.rect, gizmosColor);
            }
        }
    }


}
