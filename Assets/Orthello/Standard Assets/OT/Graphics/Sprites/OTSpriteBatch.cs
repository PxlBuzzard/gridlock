using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Batch multiple sprites into one big mesh with or without texture packing.
/// </summary>
public class OTSpriteBatch : OTObject
{
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
        public int triCount;
        public int subMesh;
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
	List<OTSprite> linked = new List<OTSprite>();

    // Use this for initialization
    new protected void Awake()		
    {
		passiveControl = true;
		base.Awake();
	}
    
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

	Color[] GetColors(SpriteData data)
	{
		Color[] co = new Color[] {};
		co = data.sprite.colors;		
		if (!data.sprite.visible)
		{
			for (int i=0; i<co.Length; i++)
				co[i] = Color.clear;
		}		
		return co;
	}
		
    Vector3[] GetVertices(SpriteData data)
    {
        Matrix4x4 matrix = new Matrix4x4();

		Vector3 scale = data.sprite.otTransform.lossyScale;
		matrix.SetTRS( data.sprite.otTransform.position - otTransform.position, data.sprite.otTransform.rotation, scale);
		
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
			System.Array.Resize<Vector2>(ref meshUV, count);
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
				
				if (!lookup.ContainsKey(sprite.GetInstanceID()))
				{
					sprite.onObjectChanged += SpriteChanged;
					sprite._iMsg = "batched";
				}
				
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
	
	void ResetRemovedSprites()
	{
        for (int s = 0; s < spriteData.Count; s++)
        {
            SpriteData data = spriteData[s];
			
			// check if we have removed this sprite using remove sprite
			if (data.sprite == null)
				continue;
			
            if (!sprites.Contains(data.sprite))
            {
				data.sprite._iMsg = "!batched";
				
                if (deactivateSprites)					
				{
#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
                    data.sprite.gameObject.active = true;
#else
                    data.sprite.gameObject.SetActive(true);
#endif
					
				}
                else
                    data.sprite.visible = true;
				data.sprite.onObjectChanged -= SpriteChanged;
                data.sprite.StartUp();
            }
        }		
	}

    bool linkedInitializing = false;
    protected override Mesh GetMesh()
    {		
        if (!Application.isPlaying)
            return null;
				
        sprites.Clear();
        // first check if all children are ready
        if (!ChildrenReady())
            return null;		
		
		if (linked.Count>0)
		{
			linkedInitializing = false;
			for (int i=0; i<linked.Count; i++)
				if (!linked[i].hasMesh || linked[i].uv == null)
				{
					linkedInitializing = true;
					return null;
				}
			sprites.AddRange(linked);		
		}
		
		if (sprites.Count==0)
		{
			ResetRemovedSprites();
			meshDirty = false;
			return new Mesh();
		}
		
        submeshMaterials.Clear();
        submeshTriangles.Clear();
        submeshes.Clear();
        gameObjects.Clear();
        packedUvs.Clear();
        texturesToPack.Clear();
				
		
#if UNITY_EDITOR		
		if (packTextures && Application.isEditor)
			CheckTexturesReadWrite();			
#endif
		
		ResetRemovedSprites();

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

                    string scId = sprite.materialReference;

                    if (sprite is OTFilledSprite)
                        scId += "|"+sprite.GetInstanceID();
					
					if (sprite.spriteContainer != null)
                        scId += "|"+sprite.spriteContainer.GetInstanceID();
                    else
                        if (sprite.image != null)
                            scId += "|" + sprite.image.GetInstanceID();

                    if (checkTintedAlpha && (sprite.materialReference == "tint" || sprite.materialReference=="alpha"))                   
                        scId += "|" + sprite.tintColor + "|" + sprite.alpha;

                    if (scId != "-1")
                    {
						data.subMesh = -1;
                        if (packTextures)
                        {
                            if (packedMaterial != null)
                                mat = new Material(packedMaterial);
                            if (mat == null)
							{
								
                                mat = new Material(sprite.otRenderer.material);
							}
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
								
								if (!(sprite is OTFilledSprite))
								{
	                                mat.mainTextureScale = Vector2.one;
	                                mat.mainTextureOffset = Vector2.zero;
								}
                                submeshMaterials.Add(mat);
                                submeshCount++;
                                submeshes.Add(scId);
                            }
                            else
                                subTriangles = submeshTriangles[scId];
							
							data.subMesh = submeshes.IndexOf(scId);
                        }
                    }
#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
                    data.active = sprite.gameObject.active;
                    if (deactivateSprites)
                        sprite.gameObject.active = false;
#else
                    data.active = sprite.gameObject.activeSelf;
                    if (deactivateSprites)
                        sprite.gameObject.SetActive(false);
#endif

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

					colors.AddRange(GetColors(data));
                    triangles.AddRange(tri);
                    subTriangles.AddRange(tri);
					data.triCount = tri.Length;
					
					Vector2[] _uv = GetUV(data,vts.Length);
					data.uvCount = _uv.Length;
					
                    uv.AddRange(_uv);
										
                    lookup.Add(data.sprite.GetInstanceID(), spriteData.Count);
                    spriteData.Add(data);

                    if (!deactivateSprites)
                        data.sprite._iMsg = "hide";
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
    }
	
	List<OTObject> changedObjects = new List<OTObject>();
	void SpriteChanged(OTObject owner)
	{
		changedObjects.Add(owner);
	}

    void Build(SpriteData data)
    {

        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf != null && mf.mesh != null && mf.mesh.vertexCount>0)
        {
            int vi = data.verticeIdx;
            int vc = data.vertexCount;
            int ep = vi + vc;
			
			int ti = data.triangleIdx;
			int sti = data.subTriangleIdx;
			int tc = data.triCount;
			int tep = ti + tc;
			int step = sti + tc;
			
            data.position = data.sprite.position;						
            data.rotation = data.sprite.rotation;
            data.size = data.sprite.size;
            data.depth = data.sprite.depth;
            data.frameIndex = data.sprite.frameIndex;

            Vector2[] meshUvs = mf.mesh.uv;
            Color[]   meshColors = mf.mesh.colors;
			Vector3[] meshVs = mf.mesh.vertices;
			int[] 	  meshTri = mf.mesh.triangles;
			List<int[]> subTris = new List<int[]>();
			Material[] materials = otRenderer.materials;

			for (int i=0; i<mf.mesh.subMeshCount; i++)
				subTris.Add(mf.mesh.GetTriangles(i));
						
			// temporary clear triangles and sub triangles;			
						
			// adjust vertices
			Vector3[] spriteVs = GetVertices(data);			
            Vector3[] vs = new Vector3[meshVs.Length - vc + spriteVs.Length];
									
            System.Array.Copy(meshVs, 0, vs, 0, vi);
            System.Array.Copy(spriteVs, 0, vs, vi, spriteVs.Length);
            System.Array.Copy(meshVs, ep, vs, vi + spriteVs.Length, meshVs.Length - ep);

			int vDelta = spriteVs.Length - vc;
			

			if (vDelta!=0)
				mf.mesh.triangles = new int[] {};
			
            mf.mesh.vertices = vs;			
			data.vertexCount = spriteVs.Length;
			
			// adjust uv coordinates									
            Vector2[] spriteUvs = GetUV(data,spriteVs.Length);
			data.uvCount = spriteUvs.Length;
            Vector2[] us = new Vector2[vs.Length];
            System.Array.Copy(meshUvs, 0, us, 0, vi);
            System.Array.Copy(spriteUvs, 0, us, vi, spriteUvs.Length);
            System.Array.Copy(meshUvs, ep, us, vi + spriteUvs.Length, meshUvs.Length - ep);
			

			// check if vertexcount and triangles have changed
			if (vDelta!=0)
			{				
				// adjust triangles
	            int[] spriteTri = GetTriangles(data);
					
				// re-index all vertex indices
				for (int i=0; i<meshTri.Length; i++)
					if (meshTri[i]>=data.verticeIdx+vc)
						meshTri[i] += vDelta;
				
				// re-index vertex indices of the submeshes
				if (!packTextures)
					for (int i=subTris.Count-1; i>=0; i--)
					{				
						for (int ii=0; ii<subTris[i].Length; ii++)
							if (subTris[i][ii]>=data.verticeIdx+vc)
								subTris[i][ii] += vDelta;
					}
													
				// adjust vertex and uv index for this sprite in the batch
				for (int s=0; s < spriteData.Count; s++)
				{
					if (spriteData[s].sprite!=null)
					{
						if (spriteData[s].sprite!=data.sprite && spriteData[s].verticeIdx>=data.verticeIdx)
						{
							SpriteData d = spriteData[s];
							d.verticeIdx += vDelta;
							d.uvIdx += vDelta;
							spriteData[s] = d;
						}
					}
				}
							
				int tDelta = spriteTri.Length - data.triCount;
							
				// adjust triangles
				data.triCount = spriteTri.Length;
	            int[] tri = new int[meshTri.Length - tc + spriteTri.Length ];
	            System.Array.Copy(meshTri, 0, tri, 0, ti);
	            System.Array.Copy(spriteTri, 0, tri, ti, spriteTri.Length);
	            System.Array.Copy(meshTri, tep, tri, ti + spriteTri.Length, meshTri.Length - tep);
																		
				if (!packTextures)
				{
					int[] subMeshTri = subTris[data.subMesh];
		            int[] stri = new int[subMeshTri.Length - tc + spriteTri.Length ];
		            System.Array.Copy(subMeshTri, 0, stri, 0, sti);
		            System.Array.Copy(spriteTri, 0, stri, sti, spriteTri.Length);
		            System.Array.Copy(subMeshTri, step, stri, sti + spriteTri.Length, subMeshTri.Length - step);				
					subTris[data.subMesh] = stri;				
					
					// adjust submesh triangle index for this sprite in the batch
					for (int s=0; s < spriteData.Count; s++)
						if (spriteData[s].sprite!=null && spriteData[s].subMesh == data.subMesh && spriteData[s].sprite!=data.sprite && spriteData[s].subTriangleIdx>=data.subTriangleIdx)
						{
							SpriteData d = spriteData[s];				
							d.subTriangleIdx += tDelta;
							spriteData[s] = d;
						}				
				}
				
				// adjust triangle index for this sprite in the batch
				for (int s=0; s < spriteData.Count; s++)
					if (spriteData[s].sprite!=null && spriteData[s].sprite!=data.sprite && spriteData[s].triangleIdx>data.triangleIdx)
					{
						SpriteData d = spriteData[s];				
						d.triangleIdx += tDelta;
						spriteData[s] = d;
					}
				
				mf.mesh.triangles = tri;
				
			}
			
			
			
			// adjust colors
            Color[] spriteColors = GetColors(data);
			
			
            Color[] cs = new Color[meshColors.Length - vc + spriteColors.Length];
			
            System.Array.Copy(meshColors, 0, cs, 0, vi);
            System.Array.Copy(spriteColors, 0, cs, vi, spriteColors.Length);
            System.Array.Copy(meshColors, ep, cs, vi + spriteColors.Length, meshColors.Length - ep);								
			
			mf.mesh.colors = cs;									
			
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
			else
			{
				
				if (data.sprite is OTFilledSprite || (checkTintedAlpha && (data.sprite.materialReference == "tint" || data.sprite.materialReference == "alpha")))
				{
					materials[data.subMesh] = data.sprite.otRenderer.material;
					if (vDelta == 0)
						otRenderer.materials = materials;						
				}
				
				if (vDelta!=0)
				{
	                mf.mesh.subMeshCount = subTris.Count;
					for (int i=subTris.Count-1; i>=0; i--)
						mf.mesh.SetTriangles(subTris[i],i);					
					otRenderer.materials = materials;
				}
			}
			
            mf.mesh.uv = us;						
            mf.mesh.RecalculateBounds();
            mf.mesh.RecalculateNormals();
        }
    }

    /// <summary>
    /// Will remove a sprite from the SpriteBatch
    /// </summary>
    /// <param name="sprite">Sprite to remove</param>
    /// <param name="newParent">Move sprite to this parent</param>
    public void RemoveSprite(OTSprite sprite, GameObject newParent)
    {
        meshDirty = true;	

#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
        sprite.gameObject.SetActiveRecursively(true);
#else
        sprite.gameObject.SetActive(true);
#endif
		
		if (lookup.ContainsKey(sprite.GetInstanceID()))
		{
			spriteData[lookup[sprite.GetInstanceID()]] = new SpriteData();
        	lookup.Remove(sprite.GetInstanceID());			
		}			
        sprites.Remove(sprite);
		if (linked.Contains(sprite))
			linked.Remove(sprite);
		else
			sprite.otTransform.parent = newParent.transform;
		
		sprite.onObjectChanged -= SpriteChanged;
		sprite._iMsg = "show";
		sprite._iMsg = "!batched";
				
        sprite.StartUp();
    }

    /// <summary>
    /// Will remove a sprite from the SpriteBatch
    /// <param name="sprite">Sprite to remove</param>
    public void RemoveSprite(OTSprite sprite)
    {
        RemoveSprite(sprite, null);
    }

    /// <summary>
    /// Add a sprite to the SpriteBatch
    /// </summary>
    /// <param name="sprite">Sprite to add</param>
    /// <remarks>
    /// The sprite is added while it stays in its
    /// original scene hierarchy location. It will not be 
    /// moved as a child of the batch object.
    /// </remarks>
    public void AddSprite(OTSprite sprite)
    {
		if (linked.Contains(sprite))
			return;
		
        if (!lookup.ContainsKey(sprite.GetInstanceID()))
        {
			if (sprite.otTransform.parent!=transform)
				linked.Add(sprite);
			sprite.onObjectChanged += SpriteChanged;
			sprite._iMsg = "batched";
            meshDirty = true;
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

	public override void PassiveUpdate()
	{
		if (meshDirty || !ChildrenChecked || changedObjects.Count>0)
			Update();
	}
	
    // Update is called once per frame    
    new protected void Update()
    {
		if (!ChildrenChecked)
			meshDirty = true;

		while (changedObjects.Count>0)
		{
			int id = (changedObjects[0] as OTSprite).GetInstanceID();
			if (lookup.ContainsKey(id))
				Build(spriteData[lookup[id]]);
			changedObjects.RemoveAt(0);						
		}
		
		
        if (packTextures != _packTextures)
        {
            _packTextures = packTextures;			
            meshDirty = true;
        }
				
		if (!passive || meshDirty)
        	base.Update();
		
		if (linkedInitializing || !ChildrenChecked)
			meshDirty = true;
		
		
		if (depth!=0)
			depth = 0;
		
		if (!otTransform.position.Equals(Vector3.zero))
		{
			otTransform.position = Vector3.zero;
			position = Vector2.zero;
		}		
    }
	
	/*
	public void SpriteAfterMesh(OTObject o)
	{
		OTSprite sprite = (o as OTSprite);
		if (sprite!=null)
            meshDirty = true;
	}
	*/
	
}