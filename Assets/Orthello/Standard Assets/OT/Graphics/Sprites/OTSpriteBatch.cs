using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Batch multiple sprites into one big mesh with or without texture packing.
/// </summary>
public class OTSpriteBatch : OTObject
{
    /// <summary>
    /// Spritebatch change monitor is on/off
    /// </summary>
    public bool autoCheckSprites = false;
    /// <summary>
    /// Spritebatch change monitor frequency (in seconds)
    /// </summary>
    public float autoCheckFrequency = 0.5f;
    /// <summary>
    /// Use one packed texture.
    /// </summary>
    public bool packTextures = false;
    /// <summary>
    /// Maximum size of the packed texture
    /// </summary>
    public int maxAtlasSize = 2048;
    /// <summary>
    /// Material used when packing textures
    /// </summary>
    public Material packedMaterial = null;
    /// <summary>
    /// de-activate sprites
    /// </summary>
    public bool deactivateSprites = true;
    /// <summary>
    /// If true, the sprite's unique alpha and tint will generate a new sub-material
    /// </summary>
    public bool checkTintedAlpha = false;

    float autoCheckTime = 0;
    bool _packTextures = false;	
		
    /// <summary>
    /// The packed texture
    /// </summary>
    public Texture texture
    {
        get
        {
            if (packTextures)
                return packedTexture;
            else
                return null;
        }
    }

    struct SpriteData
    {
        public int index;
        public OTSprite sprite;
        public Vector2 position;
        public float rotation;
        public Vector2 size;
        public float depth;
        public int frameIndex;
        public int vertexCount;
        public int verticeIdx;
        public int triangleIdx;
        public int subTriangleIdx;
        public int uvIdx;
        public int uvCount;
        public Texture image;
        public OTContainer container;
        public MeshFilter mf;
        public int textureIdx;
        public bool active;
    }

    List<SpriteData> spriteData = new List<SpriteData>();
    List<GameObject> gameObjects = new List<GameObject>();
    List<OTSprite> sprites = new List<OTSprite>();
    Dictionary<string, List<int>> submeshTriangles = new Dictionary<string, List<int>>();
    List<Material> submeshMaterials = new List<Material>();
    List<string> submeshes = new List<string>();
    List<Rect> packedUvs = new List<Rect>();
    Texture2D packedTexture = null;
    List<Texture2D> texturesToPack = new List<Texture2D>();

    Dictionary<int, int> lookup = new Dictionary<int, int>();

    // Use this for initialization
    
    new protected void Start()
    {
        base.Start();
        gameObject.isStatic = true;
        _packTextures = packTextures;
        meshDirty = true;
    }

    
    protected override string GetTypeName()
    {
        return "OTSpriteBatch";
    }

	Color[] GetColors(SpriteData data, int count)
	{
		if (data.mf.mesh.colors.Length == 0)
		{
			Color[] co = new Color[] {};
			System.Array.Resize<Color>(ref co,count);
			for (int i=0; i<co.Length; i++)
				co[i] = new Color(1,1,1,1);
			return co;
		}
		else
			return data.mf.mesh.colors;
	}
	
    Vector3[] GetVertices(SpriteData data)
    {
        Matrix4x4 matrix = new Matrix4x4();
        matrix.SetTRS(
            new Vector3(data.position.x - position.x - data.sprite._offset.x, data.position.y - position.y - data.sprite._offset.y, data.depth),
            Quaternion.Euler(0, 0, data.rotation), new Vector3(data.size.x, data.size.y, 1));

        Vector3[] newVertices = data.mf.mesh.vertices;
        for (int v = 0; v < newVertices.Length; v++)
            newVertices[v] = matrix.MultiplyPoint3x4(newVertices[v]);

        return newVertices;
    }

    int[] GetTriangles(SpriteData data)
    {
        List<int> newTriangles = new List<int>();
        newTriangles.AddRange(data.mf.mesh.triangles);
        for (int t = 0; t < newTriangles.Count; t++)
            newTriangles[t] += data.verticeIdx;

        return newTriangles.ToArray();
    }

    Vector2[] GetUV(SpriteData data, int count)
    {
        Vector2[] meshUV = new Vector2[]{};
				
        if (data.sprite.spriteContainer != null || data.sprite.image!=null)
        {
            // OTContainer.Frame frame = data.sprite.CurrentFrame();
			if (data.sprite.uv != null)
            	meshUV = data.sprite.uv.Clone() as Vector2[];
        }
        else
		{
            meshUV = new Vector2[] { };
			Array.Resize<Vector2>(ref meshUV, count);
		}
		
        return meshUV;

    }

	bool ChildrenChecked = false;
    bool ChildrenReady()
    {
		ChildrenChecked = false;
        for (int c = 0; c < otTransform.childCount; c++)
        {
            GameObject g = otTransform.GetChild(otTransform.childCount - 1 - c).gameObject;
            OTSprite sprite = g.GetComponent<OTSprite>();
            if (sprite != null)
            {
				if (sprite.uv==null)
					return false;
								
				if (sprite.isInvalid || sprite.passive)
					sprite.ForceUpdate();
										
                if (sprite.spriteContainer != null && !sprite.spriteContainer.isReady)
                    return false;								
				
                gameObjects.Add(g);
                sprites.Add(sprite);
            }
        }
		ChildrenChecked = true;
        return true;
    }

    bool ChildrenChanged()
    {
        if (otTransform.childCount != gameObjects.Count)
            return true;

        for (int c = 0; c < otTransform.childCount; c++)
            if (!gameObjects.Contains(otTransform.GetChild(c).gameObject))
                return true;

        return false;
    }

    private static int SpritesByDepth(OTSprite x, OTSprite y)
    {
        if (x == null)
        {
            if (y == null)
                return 0;
            else
                return -1;
        }
        else
        {
            if (y == null)
                return 1;
            else
            {
                int retval = x.otTransform.position.z.CompareTo(y.otTransform.position.z);
                if (retval != 0)
                    return retval;
                else
                {
                    retval = x.position.y.CompareTo(y.position.y);
                    if (retval == 0)
                        retval = y.position.x.CompareTo(x.position.x);

                }
                return retval;
            }
        }
    }

    
    protected override Mesh GetMesh()
    {
        submeshMaterials.Clear();
        submeshTriangles.Clear();
        submeshes.Clear();
        gameObjects.Clear();
        sprites.Clear();
        packedUvs.Clear();
        texturesToPack.Clear();

        if (!Application.isPlaying)
            return null;

        // first check if all children are ready
        if (!ChildrenReady())
            return null;
		
#if UNITY_EDITOR		
		if (packTextures && Application.isEditor)
			CheckTexturesReadWrite();			
#endif
				
        for (int s = 0; s < spriteData.Count; s++)
        {
            SpriteData data = spriteData[s];
            if (!sprites.Contains(data.sprite))
            {
                if (deactivateSprites)
                    data.sprite.gameObject.active = true;
                else
                    data.sprite.visible = true;
                data.sprite.StartUp();
            }
        }

        if (sprites.Count == 0)
        {
            visible = false;
            meshDirty = false;
            return null;
        }

        Material mat = null;
        spriteData.Clear();
        lookup.Clear();

        List<GameObject> toDelete = new List<GameObject>();
		
		
        if (Application.isPlaying)
        {
            Mesh mesh = new Mesh();

            List<Vector3> vertices = new List<Vector3>();
            List<Color> colors = new List<Color>();
            List<int>triangles = new List<int>();
            List<int> subTriangles = new List<int>();
            List<Vector2> uv = new List<Vector2>();
            int submeshCount = 0;

            sprites.Sort(SpritesByDepth);
									
            for (int c = 0; c < sprites.Count; c++)
            {
                OTSprite sprite = sprites[sprites.Count - 1 - c];
                GameObject g = sprite.gameObject;

                MeshFilter mf = g.GetComponent<MeshFilter>();
                if (mf.mesh != null)
                {

                    SpriteData data = new SpriteData();

                    string scId = "-1";
                    if (sprite.spriteContainer != null)
                        scId = "" + sprite.spriteContainer.GetInstanceID();
                    else
                        if (sprite.image != null)
                            scId = "" + sprite.image.GetInstanceID();

                    if (checkTintedAlpha)
                        scId += "|" + sprite.tintColor + "|" + sprite.alpha;

                    if (scId != "-1")
                    {
                        if (packTextures)
                        {
                            if (packedMaterial != null)
                                mat = new Material(packedMaterial);
                            if (mat == null)
                                mat = new Material(sprite.otRenderer.material);
                            if (!texturesToPack.Contains(sprite.otRenderer.material.mainTexture as Texture2D))
                            {
                                texturesToPack.Add(sprite.otRenderer.material.mainTexture as Texture2D);
                                data.textureIdx = texturesToPack.Count - 1;
                            }
                            else
                                data.textureIdx = texturesToPack.IndexOf(sprite.otRenderer.material.mainTexture as Texture2D);
                        }
                        else
                        {
                            if (!submeshTriangles.ContainsKey(scId))
                            {
                                subTriangles = new List<int>();
                                submeshTriangles.Add(scId, subTriangles);
                                mat = new Material(sprite.otRenderer.material);
                                if (sprite.spriteContainer != null)
                                    mat.mainTexture = sprite.spriteContainer.GetTexture();
                                else
                                    mat.mainTexture = sprite.image;

                                mat.mainTextureScale = Vector2.one;
                                mat.mainTextureOffset = Vector2.zero;
                                submeshMaterials.Add(mat);
                                submeshCount++;
                                submeshes.Add(scId);
                            }
                            else
                                subTriangles = submeshTriangles[scId];
                        }
                    }
					
                    data.active = sprite.gameObject.active;
                    if (deactivateSprites)
                        sprite.gameObject.active = false;

                    data.index = spriteData.Count;
                    data.sprite = sprite;
                    data.mf = mf;
                    data.position = sprite.position;
                    data.rotation = sprite.rotation;
                    data.size = sprite.size;
                    data.depth = sprite.depth;
                    data.verticeIdx = vertices.Count;
                    data.triangleIdx = triangles.Count;
                    data.subTriangleIdx = subTriangles.Count;
                    data.uvIdx = uv.Count;
                    data.image = sprite.image;
                    data.container = sprite.spriteContainer;
                    data.frameIndex = sprite.frameIndex;
                    data.vertexCount = mf.mesh.vertexCount;
					
					Vector3[] vts = GetVertices(data);
                    vertices.AddRange(vts);
                    int[] tri = GetTriangles(data);

					colors.AddRange(GetColors(data, vts.Length ));
                    triangles.AddRange(tri);
                    subTriangles.AddRange(tri);
					
					Vector2[] _uv = GetUV(data,vts.Length);
					data.uvCount = _uv.Length;
					
                    uv.AddRange(_uv);
					
                    lookup.Add(data.sprite.GetInstanceID(), spriteData.Count);
                    spriteData.Add(data);

                    if (!deactivateSprites)
                        data.sprite.visible = false;
                }
            }

            while (toDelete.Count > 0)
            {
                DestroyImmediate(toDelete[0]);
                toDelete.RemoveAt(0);
            }

            mesh.vertices = vertices.ToArray();
			mesh.colors = colors.ToArray();
            mesh.triangles = triangles.ToArray();
            if (!packTextures && submeshCount > 1)
            {
                mesh.uv = uv.ToArray();
                mesh.subMeshCount = submeshCount;
                for (int i = submeshCount - 1; i >= 0; i--)
                    mesh.SetTriangles(submeshTriangles[submeshes[i]].ToArray(), i);
                otRenderer.materials = submeshMaterials.ToArray();
            }
            else
            {
                if (packTextures)
                {									
                    if (packedTexture == null)
                        packedTexture = new Texture2D(maxAtlasSize, maxAtlasSize);
                    packedUvs = new List<Rect>(packedTexture.PackTextures(texturesToPack.ToArray(), 0, maxAtlasSize, false));

                    for (int s = 0; s < spriteData.Count; s++)
                    {
                        SpriteData data = spriteData[s];
                        Rect uvBase = packedUvs[data.textureIdx];
                        for (int u = 0; u < data.uvCount; u++)
                        {
                            Vector2 v = uv[data.uvIdx + u];
                            uv[data.uvIdx + u] = new Vector2(uvBase.x + (uvBase.width * v.x), uvBase.y + (uvBase.height * v.y));
                        }
                    }
                }
					
                mesh.uv = uv.ToArray();
                mesh.subMeshCount = 1;
                otRenderer.materials = new Material[] { };
                otRenderer.material = mat;
                if (packTextures)
                    mat.mainTexture = packedTexture;
            }
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            visible = true;



            return mesh;
        }
        else
            return null;
    }

    /// <summary>
    /// Will (re)build the entire SpriteBatch mesh
    /// </summary>
    public void Build()
    {
        meshDirty = true;
		Passive();
    }

    void Build(SpriteData data)
    {

        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf != null && mf.mesh != null)
        {
            int vi = data.verticeIdx;
            int vc = data.vertexCount;
            int ep = vi + vc;
			

            data.position = data.sprite.position;						
            data.rotation = data.sprite.rotation;
            data.size = data.sprite.size;
            data.depth = data.sprite.depth;
            data.frameIndex = data.sprite.frameIndex;

            Vector2[] meshUvs = mf.mesh.uv;
			Vector3[] meshVs = mf.mesh.vertices;
            Vector3[] spriteVs = GetVertices(data);
			
            Vector3[] vs = new Vector3[meshVs.Length - vc + spriteVs.Length];
			
            Array.Copy(meshVs, 0, vs, 0, vi);
            Array.Copy(spriteVs, 0, vs, vi, spriteVs.Length);
            Array.Copy(meshVs, ep, vs, vi + spriteVs.Length, meshVs.Length - ep);

            mf.mesh.vertices = vs;
			data.vertexCount = spriteVs.Length;
									
            Vector2[] spriteUvs = GetUV(data,spriteVs.Length);
			data.uvCount = spriteUvs.Length;
            Vector2[] us = new Vector2[vs.Length];
            Array.Copy(meshUvs, 0, us, 0, vi);
            Array.Copy(spriteUvs, 0, us, vi, spriteUvs.Length);
            Array.Copy(meshUvs, ep, us, vi + spriteUvs.Length, meshUvs.Length - ep);
			
            spriteData[data.index] = data;
									
            if (packTextures)
            {
                Rect uvBase = packedUvs[data.textureIdx];
                for (int u = 0; u < data.uvCount; u++)
                {
                    Vector2 v = us[data.uvIdx + u];
                    us[data.uvIdx + u] = new Vector2(uvBase.x + (uvBase.width * v.x), uvBase.y + (uvBase.height * v.y));
                }
            }								
			
            mf.mesh.uv = us;
            mf.mesh.RecalculateBounds();
            mf.mesh.RecalculateNormals();
        }
    }

    void CheckSprites()
    {
        for (int s = 0; s < spriteData.Count; s++)
            CheckSprite(spriteData[s]);
    }

    bool CheckSprite(SpriteData data)
    {
        if (!Vector2.Equals(data.position, data.sprite.position) ||
            !Vector2.Equals(data.size, data.sprite.size) ||
            data.rotation != data.sprite.rotation ||
            data.frameIndex != data.sprite.frameIndex)
        {
            Build(data);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Will remove a sprite from the SpriteBatch
    /// </summary>
    /// <param name="sprite">Sprite to remove</param>
    /// <param name="newParent">Move sprite to this parent</param>
    public void RemoveSprite(OTSprite sprite, GameObject newParent)
    {
        meshDirty = true;
        if (newParent != null)
            sprite.otTransform.parent = newParent.transform;
        else
            sprite.otTransform.parent = null;

        sprite.gameObject.SetActiveRecursively(true);
        lookup.Remove(sprite.GetInstanceID());
        sprites.Remove(sprite);
        for (int s = 0; s < spriteData.Count; s++)
        {
            if (spriteData[s].sprite == sprite)
            {
                spriteData.Remove(spriteData[s]);
                break;
            }
        }
        sprite.StartUp();
		Passive();
    }

    /// <summary>
    /// Will remove a sprite from the SpriteBatch and move to root
    /// </summary>
    /// <param name="sprite">Sprite to remove</param>
    public void RemoveSprite(OTSprite sprite)
    {
        RemoveSprite(sprite, null);
    }

    /// <summary>
    /// Add a sprite to the SpriteBatch
    /// </summary>
    /// <param name="sprite">Sprite to add</param>
    public void AddSprite(OTSprite sprite)
    {
        if (!lookup.ContainsKey(sprite.GetInstanceID()) && sprite.otTransform.parent != transform)
        {
            sprite.otTransform.parent = transform;
            meshDirty = true;
			Passive();			
        }
    }

    /// <summary>
    /// Updates a sprite in the SpriteBatch
    /// </summary>
    /// <param name="sprite">Sprite to update</param>
    public void SetSprite(OTSprite sprite)
    {										
        if (lookup.ContainsKey(sprite.GetInstanceID()))
            CheckSprite(spriteData[lookup[sprite.GetInstanceID()]]);
        else
        {
            sprite.otTransform.parent = transform;
            meshDirty = true;
			Passive();			
        }
    }
	
	
#if UNITY_EDITOR
	static List<Texture2D> textureChecked = new List<Texture2D>();
	public void CheckTexturesReadWrite()
	{				
		for (int i=0; i<sprites.Count; i++)
			if (sprites[i].spriteContainer!=null)
			{
				Texture2D tx = sprites[i].spriteContainer.GetTexture() as Texture2D;
				if (!textureChecked.Contains(tx))
				{
					string pa = UnityEditor.AssetDatabase.GetAssetPath(tx);
					UnityEditor.TextureImporter timp = UnityEditor.AssetImporter.GetAtPath(pa) as UnityEditor.TextureImporter;			
					if (!timp.isReadable)
					{
						timp.isReadable = true;
						UnityEditor.AssetDatabase.ImportAsset(pa);
						textureChecked.Add(tx);
					}
					else
						textureChecked.Add(tx);
				}
			}
	}
#endif
	
	
    // Update is called once per frame    
    new protected void Update()
    {
		if (!ChildrenChecked || mesh == null)
			meshDirty = true;
		
        if (!meshDirty && autoCheckSprites)
        {
            autoCheckTime += Time.deltaTime;
            if (autoCheckTime >= autoCheckFrequency)
            {
                if (ChildrenChanged())
                {
                    for (int g = 0; g < gameObjects.Count; g++)
                    {
                        if (gameObjects[g].transform.parent != transform)
                        {
                            OTSprite sprite = gameObjects[g].GetComponent<OTSprite>();
                            if (sprite != null) sprite.visible = true;
                        }
                    }
                    meshDirty = true;
                }
                else
                    CheckSprites();
                autoCheckTime = 0;
            }
        }

        if (packTextures != _packTextures)
        {
            _packTextures = packTextures;			
            meshDirty = true;
        }
				
        base.Update();
    }
	
	public void SpriteAfterMesh(OTObject o)
	{
		OTSprite sprite = (o as OTSprite);
		if (sprite!=null)
		{
            meshDirty = true;
			/*
			
	        if (lookup.ContainsKey(sprite.GetInstanceID()))
            	Build(spriteData[lookup[sprite.GetInstanceID()]]);
	        else
	        {
	            sprite.transform.parent = transform;
	            meshDirty = true;
	        }
	        */
		}
	}
	
}