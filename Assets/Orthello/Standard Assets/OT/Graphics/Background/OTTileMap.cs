using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System;
using System.Xml;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Use a multi layered tilemap in your game
/// </summary>
/// <remarks>
/// <a href="http://www.mapeditor.org" target="_blank" >TILED</a> map editor import is supported.
/// By setting custom properties to your tiles in TILED, you can auto-create colliders, set a tile display 
/// state (visible/invisible), set the tile's depth (vertexZ) and more...
/// 
/// Read more in our <a href="http://www.wyrmtale.com/orthello-pro/tilemaps">user manual</a>.
/// 
/// <span style="color:red"><b>NOTE</b></span> Rename the default TILED .tmx to .xml so that Unity can detect it as a TextAsset.
/// </remarks>
public class OTTileMap : OTObject
{
    //-----------------------------------------------------------------------------
    // Editor settings
    //-----------------------------------------------------------------------------

    
	//[HideInInspector]
	public bool reload = false;
    
    [HideInInspector]
    public Vector2 mapSize;
    
    [HideInInspector]
    public Vector2 mapTileSize;
	/// <summary>
	/// Array with tileset images.
	/// </summary>
    public Texture[] tileSetImages;
    /// <summary>
    /// The tile map XML Textasset
    /// </summary>
	public TextAsset tileMapXML;
	/// <summary>
	/// Generates colliders and trigger if true
	/// </summary>
	/// <remarks>
	/// To auto generate colliders when you import a tilemap from the TILED map editor, you will
	/// have to create custom properties on the tiles in the tileset. 
	/// 
	/// Only tiles that have a 'collider' property will be taken into account when generating colliders. The collider must
	/// be set to the value 'collider' (creates a static rigid body collider) or 'trigger' (creates
	/// a trigger collider). 
	/// </remarks>
	public bool generateColliders = true;
	/// <summary>
	/// Reduces bleeding.
	/// <remarks>
	/// By reducing bleeding, a tiny tiny fraction is taken of the tile so there 
	/// is less texel rounding related bleeding when an tile atlas consists of 
	/// tiles that are directly attached and thus have no padding between them
	/// on the tile atlas.
	/// </remarks>
	/// </summary>
	public bool reduceBleeding = true;
    
    [HideInInspector]
    public OTTileSet[] tileSets = new OTTileSet[] { };
    
    //[HideInInspector]
    public OTTileMapLayer[] layers = new OTTileMapLayer[] { };

    //-----------------------------------------------------------------------------
    // public attributes (get/set)
    //-----------------------------------------------------------------------------
    /// <summary>
    /// Scroll value x/y in pixels per second
    /// </summary>

    //-----------------------------------------------------------------------------
    // private and protected fields
    //-----------------------------------------------------------------------------

    TextAsset _tileMapXML;
    Dictionary<int, OTTileSet> tileSetLookup = new Dictionary<int, OTTileSet>();
    Material[] materials = new Material[] { };
    Texture[] _images = new Texture[] { };
	bool _generateColliders = true;
	bool _reduceBleeding = true;

    //-----------------------------------------------------------------------------
    // overridden subclass methods
    //-----------------------------------------------------------------------------
    
    protected override void CheckSettings()
    {
        base.CheckSettings();
    }

    
    protected override string GetTypeName()
    {
        return "TileMap";
    }
    
	
    protected override void Clean()
    {
        base.Clean();
    }

    //-----------------------------------------------------------------------------
    // class methods
    //-----------------------------------------------------------------------------

    
    protected override void Awake()
    {
        base.Awake();
    }

    void Clear()
    {
    }
	
    
    static public byte[] DecodeFrom64(string encodedData)
    {
        return System.Convert.FromBase64String(encodedData);
    }

    Vector3[] GetVertices(OTTileSet ts, OTTileMapLayer layer, Vector2 pos)
    {
        Vector2 _meshsize_ = new Vector2(1.0f/mapSize.x, 1.0f/mapSize.y);
        Vector2 _pivotPoint = new Vector2((pos.x-1) * _meshsize_.x * -1 - _meshsize_.x / 2, (pos.y-1) * _meshsize_.y + _meshsize_.y / 2); 
		
        // Vector2 _pivotPoint = new Vector2((pos.x-1) * _meshsize_.x * -1, (pos.y-1) * _meshsize_.y);
        _pivotPoint.x += .5f;
        _pivotPoint.y -= .5f;

        float dx = (ts.tileSize.x / mapTileSize.x) - 1;
        float dy = (ts.tileSize.y / mapTileSize.y) - 1;
		
		Vector2 _offset = new Vector2((layer.offsetX / mapTileSize.x) * _meshsize_.x, -1 * layer.offsetY / mapTileSize.y * _meshsize_.y);
		
        return new Vector3[] { 
                new Vector3(((_meshsize_.x/2) * -1) - _pivotPoint.x + _offset.x, (_meshsize_.y/2) - _pivotPoint.y + (dy * _meshsize_.y) + _offset.y, layer.depth),
                new Vector3((_meshsize_.x/2) - _pivotPoint.x + (dx * _meshsize_.x) + _offset.x, (_meshsize_.y/2) - _pivotPoint.y + (dy * _meshsize_.y) + _offset.y, layer.depth),
                new Vector3((_meshsize_.x/2) - _pivotPoint.x + (dx * _meshsize_.x) + _offset.x, ((_meshsize_.y/2) * -1) - _pivotPoint.y + _offset.y, layer.depth),
                new Vector3(((_meshsize_.x/2) * -1) - _pivotPoint.x + _offset.x, ((_meshsize_.y/2) * -1) - _pivotPoint.y + _offset.y, layer.depth)
            };
    }

    int[] GetTriangles(int idx)
    {
        return new int[] { 
                0+idx,1+idx,2+idx,2+idx,3+idx,0+idx
            };
    }

    Vector2[] GetUV(int tile)
    {
        OTTileSet ts = tileSetLookup[tile];

        int ty = (int)Mathf.Floor((float)(tile-ts.firstGid) / ts.tilesXY.x);
        int tx = (tile-ts.firstGid+1) - (int)((float)ty * ts.tilesXY.x) - 1;
				
        float ux = (1f / ts.imageSize.x);
        float uy = (1f / ts.imageSize.y);
        float usx = ux *  ts.tileSize.x;
        float usy = uy *  ts.tileSize.y;
		
        // float utx = (ux * tx);
        float utx = (ux * ts.margin)+(tx * usx);
		if (tx>0)utx+=(tx * ts.spacing * ux);
		
        float uty = (uy * ts.margin)+(ty * usy);
		if (ty>0)uty+=(ty * ts.spacing * uy);
		
		// create a tiny fraction (uv size / 25 )
		// that will be removed from the UV coords
		// to reduce bleeding.
		int dv = 25;
        float dx = usx / dv;
        float dy = usy / dv;
		if (!reduceBleeding)
		{
			dx = 0; dy = 0;
		}
      
		return new Vector2[] { 
            new Vector2(utx + dx,1 - uty - dy ), new Vector2(utx + usx - dx,1 - uty - dy), 
            new Vector2(utx + usx - dx ,1- uty - usy + dy), new Vector2(utx + dx,1 - uty - usy + dy) 
        };
    }


    
    protected override Mesh GetMesh()
    {
        if (layers.Length == 0 || mapSize.Equals(Vector2.zero))
            return null;
				
        Mesh mesh = new Mesh();

        for (int i = 0; i < tileSets.Length; i++)
            tileSets[i].idx = i;
 
        if (materials.Length > 0)
        {
            for (int i = 0; i < materials.Length; i++)
                DestroyImmediate(materials[i]);
        }

        List<int> subTriangles;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();

        Dictionary<string, List<int>> subTrianglesLookup = new Dictionary<string, List<int>>();

        System.Array.Resize<Material>(ref materials, tileSets.Length);
        System.Array.Resize<Texture>(ref _images, tileSets.Length);
        for (int i = 0; i < tileSets.Length; i++)
        {
            Material mat = new Material(OT.materialTransparent);
            _images[i] = tileSets[i].image;
            if (tileSets[i].image != null)
                mat.mainTexture = tileSets[i].image;
            subTrianglesLookup.Add(tileSets[i].name, new List<int>());
            materials[i] = mat;
        }


        int idx = 0;
        List<OTTileSet> usedSets = new List<OTTileSet>();
        for (int l = 0; l < layers.Length; l++)
        {
            OTTileMapLayer layer = layers[l];
            if (layer.included)
            {								
                int px = 1;
                int py = 1;
                for (int t = 0; t < layer.tiles.Length; t++)
                {					
                    int tile = layer.tiles[t];
                    if (tile > 0)
                    {																		
						if (tileSetLookup.ContainsKey(tile))
						{
                        	OTTileSet ts = tileSetLookup[tile];
	                        if (!usedSets.Contains(ts))
	                            usedSets.Add(ts);
	                        subTriangles = subTrianglesLookup[ts.name];                   
	                        vertices.AddRange(GetVertices(ts,layer, new Vector2(px,py)));
	                        int[] tri = GetTriangles(idx);
	                        idx += 4;
	                        triangles.AddRange(tri);
	                        subTriangles.AddRange(tri);
	                        uv.AddRange(GetUV(tile));
						}
                    }
                    px += 1;
                    if (px > mapSize.x)
                    {
                        px = 1;
                        py += 1;
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();

        if (usedSets.Count > 1)
        {
            Material[] usedMaterials = new Material[usedSets.Count];

            mesh.subMeshCount = usedSets.Count;
            renderer.materials = new Material[usedSets.Count];
            mesh.subMeshCount = usedSets.Count;
            for (int i = usedSets.Count - 1; i >= 0; i--)
            {
                subTriangles = subTrianglesLookup[usedSets[i].name];
                mesh.SetTriangles(subTriangles.ToArray(), i);
                usedMaterials[i] = materials[usedSets[i].idx];
            }

            renderer.sharedMaterials = usedMaterials;
        }
        else
        {
			if (usedSets.Count>0)
			{
	            mesh.subMeshCount = 1;
	            //renderer.sharedMaterials = new Material[] { };
	            renderer.sharedMaterial = materials[usedSets[0].idx];
			}
        }

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }
	
	
	public int[] GetTiles(Rect rect, OTTileSet tileset, OTTileMapLayer layer, bool checkTileSet)
	{				
		int[] tiles = new int[(int)rect.width * (int)rect.height];
		int idx = 0;
		if (layer == null)
		{
			for (int l=0; l<layers.Length; l++)
			{
				idx = 0;
				int[] ltiles = GetTiles(rect, tileset, layers[l], checkTileSet);
				for (int x = (int)rect.xMin; x<(int)rect.xMax; x++)
					for (int y = (int)rect.yMin; y<(int)rect.yMax; y++)
				{
					if (ltiles[idx]>=0)
						tiles[idx] = ltiles[idx];
					idx++;
				}								
			}
			return tiles;
		}
				
		if (layer!=null && checkTileSet)
		{
			for (int x = (int)rect.xMin; x<(int)rect.xMax; x++)
				for (int y = (int)rect.yMin; y<(int)rect.yMax; y++)
			{
				if (x>=0 && x<layer.layerSize.x && y>=0 && y<layer.layerSize.y)
				{
					int tile = layer.tiles[(y * (int)layer.layerSize.x) + x];
					if (tileSetLookup.ContainsKey(tile) && tileSetLookup[tile] == tileset)
						tiles[idx++] = layer.tiles[(y * (int)layer.layerSize.x) + x] - tileset.firstGid;						
					else
						tiles[idx++] = -1;
				}
			}
		}
		else
		{
			for (int x = (int)rect.xMin; x<(int)rect.xMax; x++)
				for (int y = (int)rect.yMin; y<(int)rect.yMax; y++)
			{
				if (x>=0 && x<layer.layerSize.x && y>=0 && y<layer.layerSize.y)
					tiles[idx++]= layer.tiles[(y * (int)layer.layerSize.x) + x] - tileset.firstGid;						
				else
					tiles[idx++] = -1;
			}
		}
				
		return tiles;
	}
	
	/// <summary>
	/// Gets a tileset based on the provided image
	/// </summary>
	public OTTileSet TileSetFromImage(Texture image)
	{
		for (int t = 0; t<tileSets.Length; t++)
		{
			if (tileSets[t].image == image)
				return tileSets[t];
		}
		return null;
	}
	
	/// <summary>
	/// Gets a layer from its name
	/// </summary>
	public OTTileMapLayer LayerByName(string layer)
	{
		string llayer = layer.ToLower();
		for (int l=0; l<layers.Length; l++)
		{
			if (layers[l].name.ToLower() == llayer)
				return layers[l];
		}
		return null;
	}
	
	string XPathLower(string propName)
	{
		return "translate(@"+propName+", 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')";
	}
	
	string TileProp(XmlNode tileNode, string propName)
	{
		
		
		XmlNode propNode = tileNode.SelectSingleNode("properties/property["+XPathLower("name")+"='"+propName.ToLower()+"']");
		if (propNode!=null)
			return AtS(propNode,"value");
		else
			return "";
	}

    string AtS(XmlNode n, string attrib)
    {
        try
        {
            XmlAttribute a = n.Attributes[attrib];
            if (a != null && a.Value!=null)
                return a.Value;

        }
        catch (System.Exception)
        {
            return "";
        }
        return "";       
    }

    int AtI(XmlNode n, string attrib)
    {
        try
        {
            return Convert.ToInt32(AtS(n, attrib));
        }
        catch (System.Exception)
        {
            return 0;
        }
    }

    bool HasProp(XmlNodeList props, string propName)
    {
        for (int i = 0; i < props.Count; i++)
        {
            try
            {
                if (props[i].Attributes["name"].Value == propName)
                    return true;
            }
            catch (System.Exception)
            {
            }
        }
        return false;
    }
    string GetPropS(XmlNodeList props, string propName)
    {
        for (int i = 0; i < props.Count; i++)
        {
            try
            {
                if (props[i].Attributes["name"].Value == propName)
                    return props[i].Attributes["value"].Value;
            }
            catch (System.Exception)
            {
            }
        }
        return "";
    }
    int GetPropI(XmlNodeList props, string propName)
    {
        string sv = GetPropS(props, propName);
        if (sv != "")
            return Convert.ToInt32(sv);
        else
            return 0;
    }

    void LoadTileMap()
    {
		RemoveColliders();
		List <OTTile>allTiles = new List<OTTile>();
		Dictionary<int , OTTile>lookupTile = new Dictionary<int, OTTile>();
		
        if (tileMapXML == null)
        {
            if (tileSets == null)
                tileSets = new OTTileSet[] { };
            return;
        }

        XmlDocument xd = new XmlDocument();
        try
        {
            xd.LoadXml(tileMapXML.text);
        }
        catch(System.Exception)
        {
            Debug.LogError("TileMap XML - invalid XML!");
            return;
        }

        if (xd.DocumentElement == null)
        {
            Debug.LogError("TileMap XML - invalid XML!");
            return;
        }

        if (xd.DocumentElement.Name != "map")
        {
            Debug.LogError("TileMap XML - No Tiled Tilemap found!");
            return;
        }

        mapSize = new Vector2(AtI(xd.DocumentElement, "width"), AtI(xd.DocumentElement, "height"));
        mapTileSize = new Vector2(AtI(xd.DocumentElement, "tilewidth"), AtI(xd.DocumentElement, "tileheight"));

        XmlNodeList xmlTileSets = xd.DocumentElement.SelectNodes("tileset");
        if (xmlTileSets.Count == 0)
        {
            Debug.LogError("TileMap XML - No Tilesets found!");
            return;
        }

        XmlNodeList xmlLayers = xd.DocumentElement.SelectNodes("layer");
        if (xmlLayers.Count == 0)
        {
            Debug.LogError("TileMap XML - No Layers found!");
            return;
        }

        if (tileSets == null)
            tileSets = new OTTileSet[] { };

        tileSetLookup.Clear();
        System.Array.Resize<OTTileSet>(ref tileSets, xmlTileSets.Count);
        for (int i = 0; i < xmlTileSets.Count; i++)
        {
            if (tileSets[i] == null) 
                tileSets[i] = new OTTileSet();

            OTTileSet ts = tileSets[i];
            XmlNode n = xmlTileSets[i];
            ts.name = AtS(n, "name");
            ts.firstGid = AtI(n, "firstgid");
            ts.margin = AtI(n, "margin");
            ts.spacing = AtI(n, "spacing");
            ts.tileSize = new Vector2(AtI(n, "tilewidth"), AtI(n, "tileheight"));

            XmlNode im = n.SelectSingleNode("image");
            string source = AtS(im, "source");

            if (tileSetImages.Length > 0)
            {
                for (int ii = 0; ii < tileSetImages.Length; ii++)
                    if (source.IndexOf(tileSetImages[ii].name) >= 0)
                        ts.image = tileSetImages[ii];
            }

            ts.imageSize = new Vector2(AtI(im, "width"), AtI(im, "height"));
            ts.tilesXY = new Vector2(Mathf.Round((ts.imageSize.x-(ts.margin*2)) / (ts.tileSize.x+ts.spacing)), 
				Mathf.Round((ts.imageSize.y-(ts.margin*2)) / (ts.tileSize.y+ts.spacing)));
            int tileCount = (int)(ts.tilesXY.x * ts.tilesXY.y);
			
			int idx = 0;			
            for (int ii = ts.firstGid; ii < ts.firstGid + tileCount; ii++)
			{
				if (!tileSetLookup.ContainsKey(ii))
                	tileSetLookup.Add(ii, ts);

				OTTile tile = new OTTile();
				XmlNode tileNode = n.SelectSingleNode("tile[@id='"+idx+"']");
				tile.index = idx++;											
				if (tileNode!=null)
				{
					tile.collider = TileProp(tileNode,"collider");
					tile.name = TileProp(tileNode,"name");
					tile.display = true;
					string sDisplay = TileProp(tileNode,"display").ToLower();
					if (sDisplay=="none" || sDisplay=="false" || sDisplay=="hidden" || sDisplay=="0")						
						tile.display = false;
					try
					{
						tile.height = System.Convert.ToInt16(TileProp(tileNode,"height"));
					}
					catch(System.Exception)
					{
						tile.height = 0;
					}
					if (tile.height == 0) tile.height = (int)mapTileSize.x;
					if (tile.height == 0) tile.height = 10;
					tile.gid = ii;
					if (tile.name=="") tile.name = "tileGUID"+ii;
					allTiles.Add(tile);
					lookupTile.Add(ii,tile);
				}				
			}		
        }

        if (layers == null)
            layers = new OTTileMapLayer[] { };

        System.Array.Resize<OTTileMapLayer>(ref layers, xmlLayers.Count);
        for (int i = 0; i < xmlLayers.Count; i++)
        {
            if (layers[i] == null)
                layers[i] = new OTTileMapLayer();

            OTTileMapLayer l = layers[i];
            XmlNode n = xmlLayers[i];
            l.name = AtS(n, "name");
            l.depth = 0 - i;
            l.layerSize = new Vector2(AtI(n, "width"), AtI(n, "height"));
            int tileCount = (int)(l.layerSize.x * l.layerSize.y);

            if (l.tiles.Length!=tileCount)
                System.Array.Resize<int>(ref l.tiles, tileCount);

            try
            {
                try
                {
                    XmlNodeList props = n.SelectSingleNode("properties").SelectNodes("property");
                    if (HasProp(props, "depth"))
                        l.depth = GetPropI(props, "depth");
					if (HasProp(props, "offset-x"))
						l.offsetX = GetPropI(props, "offset-x");
					if (HasProp(props, "offset-x"))
						l.offsetY = GetPropI(props, "offset-y");
                }
                catch (System.Exception)
                {
                }

                XmlNodeList tiles = n.SelectSingleNode("data").SelectNodes("tile");
                if (tiles.Count != tileCount)
                    Debug.LogWarning("TileMap XML - Invalid number of tiles " + tiles.Count+" on layer "+l.name+", size "+l.layerSize.x+" x "+l.layerSize.y+
                    " , so expected "+tileCount+" tiles.");

                for (int li = 0; li < tileCount; li++)
                    l.tiles[li] = 0;

                for (int li = 0; li < tiles.Count; li++)
				{
					int gid = AtI(tiles[li],"gid");
					if (lookupTile.ContainsKey(gid))
					{
						if (lookupTile[gid].display)
                    		l.tiles[li] = AtI(tiles[li],"gid");
						else
							l.tiles[li] = 0;
					}
					else					
                    	l.tiles[li] = AtI(tiles[li],"gid");
				}
            }
            catch (System.Exception)
            {
                Debug.LogError("TileMap XML - Could not load tiles from layer " + l.name);
            }
        }

        meshDirty = true;
        isDirty = true;

        size = new Vector2(mapSize.x * mapTileSize.x, mapSize.y * mapTileSize.y);

		if (generateColliders)
			CreateColliders(allTiles);

    }
	
	void RemoveColliders()
	{
		Transform colliders = transform.FindChild("colliders");
		if (colliders != null)
			DestroyImmediate(colliders.gameObject);
	}
		
	void CreateTileColliders(OTTile tile)
	{
		// create tilemap for collider creation
		int[] tiles = new int[] {};
        for (int i = 0; i < layers.Length; i++)
        {
            OTTileMapLayer l = layers[i];
			if (i==0)
			{
				tiles = l.tiles.Clone() as int[];
				for (int t=0; t<tiles.Length; t++)
					if (tiles[t]!=tile.gid) tiles[t] = 0;
			}
			else				
			{
				for (int t=0; t<l.tiles.Length; t++)
					if (l.tiles[t]==tile.gid) tiles[t] = tile.gid;
			}								
		}
	

		Vector2 pos = new Vector2(0,0);
		while (!pos.Equals(mapSize))
		{
			if (tiles[(int)((pos.y * mapSize.x)+pos.x)] == tile.gid)
			{
				Vector2 old = pos;
				Vector2 col = Vector2.one;				
				while (tiles[(int)((pos.y * mapSize.x)+pos.x)]==tile.gid)
				{
					if (pos.y==old.y)
					{
						if (pos.x - old.x + 1 >= col.x && pos.y - old.y +1 >= col.y)
							col = new Vector2(pos.x - old.x + 1, pos.y - old.y +1);
					}
					else
					{
						if (pos.x - old.x + 1 > col.x) 
							break;
						else
							col = new Vector2(col.x, pos.y - old.y +1);
					}
					pos.x++;
					if (pos.x==mapSize.x)
					{
						pos.x = old.x;
						pos.y++;
						if (pos.y==mapSize.y)
							break;						
					}
					else
					{
						if (tiles[(int)((pos.y * mapSize.x)+pos.x)]!=tile.gid)
						{							
							if (col.y > 1 && (pos.x - old.x + 1 < col.x))
							{
								--col.y;
								break;
							}
														
							pos.x = old.x;							
							pos.y++;
							if (pos.y==mapSize.y)
								break;						
							
						}
					}															
				}				
				pos = old;

				for (int dx = 0; dx < col.x; dx++)
					for (int dy = 0; dy < col.y; dy++)
						tiles[(int)(((pos.y + dy) * mapSize.x)+(pos.x+dx))] = 0;				
				
				Transform colliders = transform.FindChild("colliders");
				if (colliders == null)
				{
					GameObject g = new GameObject();
					g.name = "colliders";
					colliders = g.transform;
					colliders.parent = transform;	
					colliders.localScale = Vector3.one;
					colliders.localPosition = Vector3.zero;
				}				
				Transform tileColliders = colliders.FindChild(tile.name);
				if (tileColliders == null)
				{
					GameObject g = new GameObject();
					g.name = tile.name;
					tileColliders = g.transform;
					tileColliders.parent = colliders;										
					tileColliders.localScale = Vector3.one;
					tileColliders.localPosition = Vector3.zero;
				}
				
				GameObject go = new GameObject();
				if (tile.collider.ToLower() != "collider")
					go.name = "tilemapTrigger";
				else
					go.name = "tilemapCollider";
				
				OTTileSet ts = tileSetLookup[tile.gid];
				

				col.x *= ts.tileSize.x / mapTileSize.x;
				float fy = ts.tileSize.y / mapTileSize.y;
				col.y *= fy;				
				
				if (OT.world == OT.World.WorldTopDown2D)
				{
					go.transform.localScale = new Vector3(col.x * mapTileSize.x, tile.height, col.y * mapTileSize.y);
					go.transform.position = transform.position - 
						new Vector3((mapSize.x * mapTileSize.x)/2,tile.height/3,  (-mapSize.y * mapTileSize.y)/2);					
					go.transform.position += new Vector3((pos.x * mapTileSize.x) + mapTileSize.x/2,0,  -pos.y * mapTileSize.y - (mapTileSize.y/2));
					go.transform.position += new Vector3((col.x * mapTileSize.x)/2 - mapTileSize.x/2,0, (-col.y * mapTileSize.y)/2 + mapTileSize.y/2);
					if (fy>1)
					  go.transform.position += new Vector3(0, 0, mapTileSize.y * (fy-1));
				}
				else
				{
					go.transform.localScale = new Vector3(col.x * mapTileSize.x, col.y * mapTileSize.y, tile.height);
					go.transform.position = transform.position - 
						new Vector3((mapSize.x * mapTileSize.x)/2, (-mapSize.y * mapTileSize.y)/2, tile.height/3);					
					go.transform.position += new Vector3((pos.x * mapTileSize.x) + mapTileSize.x/2, -pos.y * mapTileSize.y - (mapTileSize.y/2),0);
					go.transform.position += new Vector3((col.x * mapTileSize.x)/2 - mapTileSize.x/2, (-col.y * mapTileSize.y)/2 + mapTileSize.y/2,0);
					if (fy>1)
					  go.transform.position += new Vector3(0, mapTileSize.y * (fy-1),0);					
				}
					
				go.transform.parent = tileColliders;								
				BoxCollider box = go.AddComponent<BoxCollider>() as BoxCollider;
				
				if (tile.collider.ToLower() == "collider")
				{
					Rigidbody rb = go.AddComponent<Rigidbody>() as Rigidbody;
					// rb.useGravity = false;
					rb.isKinematic = true;
				}
				else
					box.isTrigger = true;
				
			}
			else
			{
				pos.x++;
				if (pos.x==mapSize.x)
				{
					pos.x = 0;
					pos.y++;
					if (pos.y==mapSize.y)
						break;						
				}
			}			
		}						
		
	}
	
	void CreateColliders(List<OTTile> allTiles)
	{
		while (allTiles.Count>0)
		{
			OTTile tile = allTiles[0];
			if (tile.collider!="")
				CreateTileColliders(tile);
			allTiles.Remove(tile);
		}
	}

    new void Start()
    {        
        base.Start();
        _tileMapXML = tileMapXML;
        meshDirty = false;

        if (tileSets.Length > 0)
        {
            for (int i = 0; i < tileSets.Length; i++)
            {
                OTTileSet ts = tileSets[i];
                int tileCount = (int)(ts.tilesXY.x * ts.tilesXY.y);
                for (int ii = ts.firstGid; ii < ts.firstGid + tileCount; ii++)
                    tileSetLookup.Add(ii, ts);
            }
        }


    }
    // Update is called once per frame
    new void Update()
    {		
		bool doLoad = false;
        if (_tileMapXML != tileMapXML || reload)
        {
			doLoad = true;
            _tileMapXML = tileMapXML;
			reload = false;
        }
		
		if (!Application.isPlaying && (_generateColliders!=generateColliders || _reduceBleeding!=reduceBleeding))
        {
			doLoad = true;
            _generateColliders=generateColliders;
			_reduceBleeding = reduceBleeding;
        }
		
		if (doLoad)
		{
            LoadTileMap();
		}
		
        base.Update();
    }

	/// <summary>
	/// Rebuilds the tilemap
	/// </summary>
    public void Rebuild()
    {
        meshDirty = true;
        isDirty = true;
        Update();
    }
	
	/// <summary>
	/// Reloads and builds the tilemap
	/// </summary>
    public void Reload()
    {
        LoadTileMap();
        base.Update();
    }
}

public class OTTile
{

	public bool display;

	public int index;

	public int gid;

    public string collider;

    public int height;

    public string name;
}


/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// An OTTileMap tileset
/// </summary>
/// <remarks>
/// A tileset contains information about multiple tiles that are contained in a tile texture image.
/// </remarks>
[System.Serializable]
public class OTTileSet
{
	/// <summary>
	/// Name of the tileset
	/// </summary>
    public string name;
	/// <summary>
	/// Texture image of this tileset
	/// </summary>
    public Texture image;
	/// <summary>
	/// Dimensions of a tile in this tileset
	/// </summary>
    public Vector2 tileSize;
	/// <summary>
	/// Dimensions of the image of this tileset
	/// </summary>
    public Vector2 imageSize;
	/// <summary>
	/// Number of tile columns/rows
	/// </summary>
    public Vector2 tilesXY;
	/// <summary>
	/// tile id (index) of the first tile
	/// </summary>
    public int firstGid;
	/// <summary>
	/// The margin around the tile images
	/// </summary>
	public int margin;
	/// <summary>
	/// The spacing between the images
	/// </summary>
	public int spacing;
	
    public int idx;
}

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : OTTileMap layer.
/// </summary>
/// <remarks>
/// This object contains information about which  tiles are used in this OTTileMap layer.
/// </remarks>
[System.Serializable]
public class OTTileMapLayer
{
	/// <summary>
	/// The name of the layer
	/// </summary>
    public string name;
	/// <summary>
	/// The size of the layer in tiles
	/// </summary>
    public Vector2 layerSize;
	/// <summary>
	/// Array with tiles (gid) indexes
	/// </summary>
    public int[] tiles = new int[] { };
	/// <summary>
	/// The depth of this layer
	/// </summary>
    public int depth;
	/// <summary>
	/// This layer will be included when true (default)
	/// </summary>
    public bool included = true;  
	/// <summary>
	/// This layers offset distance
	/// </summary>
	public float offsetX;
	public float offsetY;
}