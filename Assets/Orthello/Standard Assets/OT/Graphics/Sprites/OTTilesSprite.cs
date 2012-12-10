using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class OTTilesSprite : OTSprite
{
	
	/// <summary>
	/// The tile count in this tiles sprite
	/// </summary>
	public int tileCount
	{
		get
		{
			return tileMatrix.x * tileMatrix.y;
		}
	}
	

	/// <summary>
	/// Gets the tile under the mouse pointer
	/// </summary>
	public IVector2 mouseTile
	{
		get
		{
			return TileFromWorldPoint(OT.view.camera.ScreenToWorldPoint(Input.mousePosition));
		}		
	}
	
	public IVector2 hitTile
	{
		get
		{
			return TileFromLocalPoint(hitPoint);
		}
	}
			
	public IVector2 _tileMatrix = new IVector2(6,3);
	IVector2 _tileMatrix_;
	/// <summary>
	/// Gets or sets the tile matrix.
	/// </summary>
	public IVector2 tileMatrix
	{
		get
		{
			return _tileMatrix;
		}
		set
		{
			_tileMatrix = value;
			DimTiles();
			meshDirty = true;
		}
	}

	public IVector2 _tileSize = new IVector2(50,50);
	IVector2 _tileSize_;
	/// <summary>
	/// Gets or sets the size of the tile.
	/// </summary>
	public IVector2 tileSize
	{
		get
		{
			return _tileSize;
		}
		set
		{
			_tileSize = value;
			meshDirty = true;
		}
	}
		
	int[][] _tiles;
	/// <summary>
	/// Gets or sets the tiles.
	/// </summary>
	public int[][] tiles {
		get
		{
			return _tiles;
		}
		set
		{
			_tiles = value;
			DimTiles();
			Repaint();
		}
	}
	
	/// <summary>
	/// Gets all tiles.
	/// </summary>
	public IVector2[] allTiles
	{
		get
		{
			int idx = 0;
			IVector2[] tiles = new IVector2[tileCount];
			Loop( delegate(IVector2 tile)
			{
				tiles[idx++] = tile;	
			});
			return tiles;
		}
	}
	
	/// <summary>
	/// Gets all filled tiles.
	/// </summary>
	public IVector2[] allFilledTiles
	{
		get
		{
			int idx = 0;
			IVector2[] tiles = new IVector2[tileCount];
			Loop( delegate(IVector2 tile)
			{
				if (this.tiles[tile.x][tile.y] >=0)
					tiles[idx++] = tile;	
			});
			System.Array.Resize<IVector2>(ref tiles,idx);
			return tiles;
		}
	}
	
	/// <summary>
	/// Gets all empty tiles.
	/// </summary>
	public IVector2[] allEmptyTiles
	{
		get
		{
			int idx = 0;
			IVector2[] tiles = new IVector2[tileCount];
			Loop( delegate(IVector2 tile)
			{
				if (this.tiles[tile.x][tile.y] == -1)
					tiles[idx++] = tile;	
			});
			System.Array.Resize<IVector2>(ref tiles,idx);
			return tiles;
		}
	}
	
	
	object[][] _objects;
	/// <summary>
	/// Gets or sets the objects.
	/// </summary>
	public object [][] objects {
		get
		{
			return _objects;
		}
		set
		{
			_objects = value;
			DimTiles();
		}
	}
	
	float[][] _alphas;
	/// <summary>
	/// Gets or sets the alpha values of the objects
	/// </summary>
	public float[][] alphas {
		get
		{
			return _alphas;
		}
		set
		{
			_alphas = value;
			DimTiles();
		}
	}
		
	bool _showRandomTiles = true;
	public bool showRandomTiles = true;

	// ------------------------------------------------------------------------------------------------------------------------------	
	// public methods
	// ------------------------------------------------------------------------------------------------------------------------------	
	
	/// <summary>
	/// Scroll the tiles with a certain delta (d)
	/// </summary>
	public void Scroll(IVector2 d, bool keepTiles)
	{
		if (d.Equals(IVector2.zero))
			return;
		
		Color[] colors = mesh.colors;
		if (d.y!=0)
		{
			// scrolling up or down
			int dy = Mathf.Abs(d.y);
			int[] tmpTiles = new int[dy * tileMatrix.x];
			object[] tmpObjects = new object[dy * tileMatrix.x];
			float[] tmpAlpha = new float[dy * tileMatrix.x];	
			Color[] tmpColors = new Color[dy * tileMatrix.x * 4];
			
			int idx = 0;
			int cidx = 0;
			
			int yv,yt,yd;
			
			if (keepTiles)
			{
				// scroll up
				yv = tileMatrix.y-1; yt = tileMatrix.y-dy; yd = -1;
				if (d.y<0)
				{
					// scroll down
					yv = 0; yt = dy; yd = 1;
				}
				
				
				for (int y=yv; (d.y<0)?(y<yt):(y>=yt); y+=yd)
					for (int x=0; x<tileMatrix.x; x++)
					{
						tmpTiles[idx] = tiles[x][y];
						tmpAlpha[idx] = alphas[x][y];
						tmpObjects[idx++] = objects[x][y];
						int vidx = ((x * tileMatrix.y) + y) * 4;				
						tmpColors[cidx++] = colors[vidx];
						tmpColors[cidx++] = colors[vidx+1];
						tmpColors[cidx++] = colors[vidx+2];
						tmpColors[cidx++] = colors[vidx+3];
					}
			}
	
			// scroll up
			yv = tileMatrix.y-1-dy; yt = 0; yd = -1;			
			if (d.y<0)
			{
				// scroll down
				yv = dy; yt = tileMatrix.y; yd = 1;
			}
						
			for (int y=yv; (d.y<0)?(y<yt):(y>=yt); y+=yd)
				for (int x=0; x<tileMatrix.x; x++)
				{
					tiles[x][y+d.y] = tiles[x][y];
					objects[x][y+d.y] = objects[x][y];
					alphas[x][y+d.y] = alphas[x][y];
					int vidxt = ((x * tileMatrix.y) + y + d.y) * 4;				
					int vidxf = ((x * tileMatrix.y) + y ) * 4;				
					colors[vidxt] = colors[vidxf];
					colors[vidxt+1] = colors[vidxf+1];
					colors[vidxt+2] = colors[vidxf+2];
					colors[vidxt+3] = colors[vidxf+3];
				}
	
			if (keepTiles)
			{				
				idx = 0;
				cidx = 0;
		
				// scroll up
				yv = dy-1; yt = 0; yd = -1;			
				if (d.y<0)
				{
					// scroll down
					yv = tileMatrix.y-dy; yt = tileMatrix.y; yd = 1;
				}				
				for (int y=yv; (d.y<0)?(y<yt):(y>=yt); y+=yd)
					for (int x=0; x<tileMatrix.x; x++)
					{
						tiles[x][y] = tmpTiles[idx];
						alphas[x][y] = tmpAlpha[idx];
						objects[x][y] = tmpObjects[idx++];
						int vidx = ((x * tileMatrix.y) + y) * 4;				
						colors[vidx] = tmpColors[cidx++];
						colors[vidx+1] = tmpColors[cidx++];
						colors[vidx+2] = tmpColors[cidx++];
						colors[vidx+3] = tmpColors[cidx++];
					}
			}
			else
			{
				if (d.y<0)
					Clear(TilesBlock(new Rect(1,tileMatrix.y-(dy-1),tileMatrix.x,dy)));
				else
					Clear(TilesBlock(new Rect(1,1,tileMatrix.x,dy)));
			}
			
		}

		if (d.x!=0)
		{
			// scrolling up or down
			int dx = Mathf.Abs(d.x);
			int[] tmpTiles = new int[dx * tileMatrix.y];
			object[] tmpObjects = new object[dx * tileMatrix.y];
			float[] tmpAlpha = new float[dx * tileMatrix.y];	
			Color[] tmpColors = new Color[dx * tileMatrix.y * 4];
			
			int idx = 0;
			int cidx = 0;
						
			int xv,xt,xd;
			
			
			if (keepTiles)
			{
				// scroll right
				xv = tileMatrix.x-1; xt = tileMatrix.x-dx; xd = -1;
				if (d.x<0)
				{
					// scroll down
					xv = 0; xt = dx; xd = 1;
				}
				
				
				for (int y=0; y<tileMatrix.y; y++)
					for (int x=xv; (d.x<0)?(x<xt):(x>=xt); x+=xd)
					{
						tmpTiles[idx] = tiles[x][y];
						tmpAlpha[idx] = alphas[x][y];
						tmpObjects[idx++] = objects[x][y];
						int vidx = ((x * tileMatrix.y) + y) * 4;				
						tmpColors[cidx++] = colors[vidx];
						tmpColors[cidx++] = colors[vidx+1];
						tmpColors[cidx++] = colors[vidx+2];
						tmpColors[cidx++] = colors[vidx+3];
					}
			}
	
			// scroll up
			xv = tileMatrix.x-1-dx; xt = 0; xd = -1;			
			if (d.x<0)
			{
				// scroll down
				xv = dx; xt = tileMatrix.x; xd = 1;
			}
						
			for (int y=0; y<tileMatrix.y; y++)
				for (int x=xv; (d.x<0)?(x<xt):(x>=xt); x+=xd)
				{
					tiles[x+d.x][y] = tiles[x][y];
					alphas[x+d.x][y] = alphas[x][y];
					objects[x+d.x][y] = objects[x][y];
					int vidxt = (((x + d.x) * tileMatrix.y) + y) * 4;				
					int vidxf = ((x * tileMatrix.y) + y ) * 4;				
					colors[vidxt] = colors[vidxf];
					colors[vidxt+1] = colors[vidxf+1];
					colors[vidxt+2] = colors[vidxf+2];
					colors[vidxt+3] = colors[vidxf+3];
				}
	
			
			if (keepTiles)
			{
				idx = 0;
				cidx = 0;
		
				// scroll up
				xv = dx-1; xt = 0; xd = -1;			
				if (d.x<0)
				{
					// scroll down
					xv = tileMatrix.x-dx; xt = tileMatrix.x; xd = 1;
				}
				
				for (int y=0; y<tileMatrix.y; y++)
					for (int x=xv; (d.x<0)?(x<xt):(x>=xt); x+=xd)
					{
						tiles[x][y] = tmpTiles[idx];
						alphas[x][y] = tmpAlpha[idx];
						objects[x][y] = tmpObjects[idx++];
						int vidx = ((x * tileMatrix.y) + y) * 4;				
						colors[vidx] = tmpColors[cidx++];
						colors[vidx+1] = tmpColors[cidx++];
						colors[vidx+2] = tmpColors[cidx++];
						colors[vidx+3] = tmpColors[cidx++];
					}
			}
			else
			{
				if (d.x<0)
					Clear(TilesBlock(new Rect(tileMatrix.x-(dx-1),1,dx,tileMatrix.y)));
				else
					Clear(TilesBlock(new Rect(1,1,dx,tileMatrix.y)));
			}
				
		}
		
		mesh.colors = colors;
		Repaint();
	}
	
	/// <summary>
	/// Gets local position of a tile
	/// </summary>
	public Vector2 TilesPosition(IVector2 tile)
	{
		return new Vector2((tile.x * tileSize.x) + (tileSize.x / 2), (tile.y * tileSize.y) + (tileSize.y / 2));
	}
	/// <summary>
	/// Gets local position of a tile
	/// </summary>
	public Vector2 LocalPosition(IVector2 tile)
	{
		return TilesToLocalPoint(TilesPosition(tile));
	}
	/// <summary>
	/// Gets local position of a tile
	/// </summary>
	public Vector2 WorldPosition(IVector2 tile)
	{
		return otTransform.localToWorldMatrix.MultiplyPoint3x4(TilesToLocalPoint(TilesPosition(tile)));
	}

	/// <summary>
	/// Translates the current local point to a point relative to the bottom left corner
	/// </summary>
	public Vector2 TilesToLocalPoint(Vector2 point)
	{
		Bounds  b = otCollider.bounds;
		Vector2 p = point;
		
		p.y *= otTransform.localScale.y;
		p.x *= otTransform.localScale.x;
		p -= new Vector2((b.extents.x*2) * (pivotPoint.x + 0.5f), (b.extents.y *2) *  (pivotPoint.y + 0.5f));
		return p;		
	}
	
	
	/// <summary>
	/// Translates the current local point to a point relative to the bottom left corner
	/// </summary>
	public Vector2 LocalToTilesPoint(Vector2 point)
	{
		Bounds  b = otCollider.bounds;
		Vector2 p = point;
		
		p += new Vector2((b.extents.x*2) * (pivotPoint.x + 0.5f), (b.extents.y *2) *  (pivotPoint.y + 0.5f));
		p.x /= otTransform.localScale.x;
		p.y /= otTransform.localScale.y;
		return p;		
	}
	
	/// <summary>
	/// Get tile from a point on the tiles sprite
	/// </summary>
	public IVector2 TileFromTilesPoint(Vector2 p)
	{
		int x = Mathf.FloorToInt(p.x / tileSize.x);
		int y = Mathf.FloorToInt(p.y / tileSize.y);
		
		if (x<0 || x>=tileMatrix.x || y<0 || y>=tileMatrix.y)
			return null;
		else
			return new IVector2(x,y);
	}
	
	/// <summary>
	/// Get tile from a local point on the tiles sprite
	/// </summary>
	public IVector2 TileFromLocalPoint(Vector2 p)
	{
		return TileFromTilesPoint(LocalToTilesPoint(p));
	}
	
	/// <summary>
	/// Get tile from a world point related to the tiles sprite
	/// </summary>
	public IVector2 TileFromWorldPoint(Vector2 p)
	{
		return TileFromLocalPoint(otTransform.worldToLocalMatrix.MultiplyPoint3x4(p));
	}

	/// <summary>
	/// Get the tiles from a specific row (y - starts at 1)
	/// </summary>
	public IVector2[] TilesY(int y)
	{
		int idx = 0;
		IVector2[] tiles = new IVector2[tileCount];
		LoopY(Mathf.Clamp(y,0,_tileMatrix.y-1), delegate(IVector2 tile)
		{
			tiles[idx++] = tile;	
		});
		System.Array.Resize<IVector2>(ref tiles,idx);		
		return tiles;
	}
	
	/// <summary>
	/// Get the tiles from a specific column (x - starts at 1)
	/// </summary>
	public IVector2[] TilesX(int x)
	{
		int idx = 0;
		IVector2[] tiles = new IVector2[tileCount];
		LoopX(Mathf.Clamp(x,0,_tileMatrix.x-1), delegate(IVector2 tile)
		{
			tiles[idx++] = tile;	
		});
		System.Array.Resize<IVector2>(ref tiles,idx);		
		return tiles;
	}
	
	/// <summary>
	/// Get the tiles from a specific block (x,y,w,h - starts at 1,1)
	/// </summary>
	public IVector2[] TilesBlock(Rect r)
	{
		int idx = 0;
		IVector2[] tiles = new IVector2[tileCount];
		Loop(delegate(IVector2 tile)
		{
			tiles[idx++] = tile;	
		}, r);
		System.Array.Resize<IVector2>(ref tiles,idx);		
		return tiles;		
	}

	/// <summary>
	/// sets the alpha value of a number of tiles;
	/// </summary>
	public void Alpha(IVector2[] tiles, float value)
	{
		for (int t=0; t<tiles.Length; t++)
			alphas[tiles[t].x][tiles[t].y] = value;
		Repaint(tiles);
	}
				
	/// <summary>
	/// sets the color tint of number of tiles
	/// </summary>
	public void Tint(IVector2[] tiles, Color color)		
	{
		try
		{
			Color[] colors = mesh.colors;						
			for (int t=0; t<tiles.Length; t++)
			{
				IVector2 tile = tiles[t];
				int vidx = ((tile.x * tileMatrix.y) + tile.y) * 4;	
				color.a = alphas[tile.x][tile.y];
				colors[vidx + 0] = color;
				colors[vidx + 1] = color;
				colors[vidx + 2] = color;
				colors[vidx + 3] = color;
			}			
			mesh.colors = colors;						
		}
		catch(System.Exception)
		{
		}
	}
	
	/// <summary>
	/// sets the color tint of a tile
	/// </summary>
	public Color Tint(IVector2 tile, Color color)		
	{
		try
		{
			Color[] colors = mesh.colors;						
			int vidx = ((tile.x * tileMatrix.y) + tile.y) * 4;
			if (!color.Equals(Color.clear))
			{
				color.a = alphas[tile.x][tile.y];
				colors[vidx + 0] = color;
				colors[vidx + 1] = color;
				colors[vidx + 2] = color;
				colors[vidx + 3] = color;
				mesh.colors = colors;						
			}			
			return colors[vidx + 0];
		}
		catch(System.Exception)
		{
		}
		return Color.clear;
	}
	
	/// <summary>
	/// gets the color tint of a tile
	/// </summary>
	public Color Tint(IVector2 tile)		
	{
		return Tint(tile,Color.clear);
	}

	
	/// <summary>
	/// sets the vertex color of number of tiles
	/// </summary>
	public void VColor(IVector2[] tiles, Color color)		
	{
		try
		{
			Color[] colors = mesh.colors;						
			for (int t=0; t<tiles.Length; t++)
			{
				IVector2 tile = tiles[t];
				int vidx = ((tile.x * tileMatrix.y) + tile.y) * 4;	
				colors[vidx + 0] = color;
				colors[vidx + 1] = color;
				colors[vidx + 2] = color;
				colors[vidx + 3] = color;
				alphas[tile.x][tile.y] = color.a;
			}			
			mesh.colors = colors;						
		}
		catch(System.Exception)
		{
		}
	}
	
	/// <summary>
	/// sets the vertex color of a tile
	/// </summary>
	public Color VColor(IVector2 tile, Color color)		
	{
		try
		{
			Color[] colors = mesh.colors;						
			int vidx = ((tile.x * tileMatrix.y) + tile.y) * 4;
			if (!color.Equals(Color.clear))
			{
				colors[vidx + 0] = color;
				colors[vidx + 1] = color;
				colors[vidx + 2] = color;
				colors[vidx + 3] = color;
				mesh.colors = colors;						
			}			
			return colors[vidx + 0];
		}
		catch(System.Exception)
		{
		}
		return Color.clear;
	}
	
	/// <summary>
	/// gets the vertex color of a tile
	/// </summary>
	public Color VColor(IVector2 tile)		
	{
		return VColor(tile,Color.clear);
	}
	
	
	
	/// <summary>
	/// Repaints a number of tiles, indicated by the IVector2 array;
	/// </summary>
	public void Repaint(IVector2[] tiles)
	{
		if (spriteContainer==null || !spriteContainer.isReady)
			return;
		
		Vector2[] _uv = mesh.uv;
		bool colorsChanged = false;
		Color[] colors = mesh.colors;
		
		for (int t=0; t<tiles.Length; t++)
		{
			IVector2 tile = tiles[t];
			try
			{
				int idx = _tiles[tile.x][tile.y];			
				int vidx = ((tile.x * tileMatrix.y) + tile.y) * 4;
				if (idx >= 0)
				{
					try
					{
						Vector2[] tUV = spriteContainer.GetFrame(idx).uv;					
						_uv[vidx + 0] = tUV[3];
						_uv[vidx + 1] = tUV[0];
						_uv[vidx + 2] = tUV[1];
						_uv[vidx + 3] = tUV[2];															
					}
					catch(System.Exception)
					{
						// we could not find the frame so this tiles becomes transparent
						idx = -1;
						_tiles[tile.x][tile.y] = idx;
					}
				}
							
				if (idx == -1 && alphas[tile.x][tile.y]!=0)				
					alphas[tile.x][tile.y] = 0;
				
				if (colors[vidx + 0].a!=alphas[tile.x][tile.y])
				{
					colors[vidx + 0].a = alphas[tile.x][tile.y];
					colors[vidx + 1].a = alphas[tile.x][tile.y];
					colors[vidx + 2].a = alphas[tile.x][tile.y];
					colors[vidx + 3].a = alphas[tile.x][tile.y];
					colorsChanged = true;
				}
			}
			catch(System.Exception)
			{
			}				
		}
		mesh.uv = _uv;						
		if (colorsChanged)
			mesh.colors = colors;
	}
		
	/// <summary>
	/// Repaints a tile.
	/// </summary>
	public void Repaint(IVector2 tile)
	{
		Repaint(new IVector2[]{ tile });
	}	
	
	/// <summary>
	/// Repaints all tiles
	/// </summary>
	public void Repaint()
	{
		Repaint(allTiles);
	}
	
	
	delegate void TileDelegate(IVector2 tile);	
	void Loop(TileDelegate tileMethod, Rect r)
	{
		for (int x=(int)r.xMin; x<(int)r.xMax; x++)
			for (int y=(int)r.yMin; y<(int)r.yMax; y++)
				tileMethod(new IVector2(x,y));
		
	}

	void Loop(TileDelegate tileMethod)
	{
		Loop(tileMethod, new Rect(0,0,tileMatrix.x, tileMatrix.y));
	}
	
	void LoopY(int y, TileDelegate tileMethod)
	{
		Loop(tileMethod, new Rect(0,y,tileMatrix.x, 1));
	}
	void LoopX(int x, TileDelegate tileMethod)
	{
		Loop(tileMethod, new Rect(x,0,1, tileMatrix.y));
	}
	
	public void FillTiles(Rect fillRect, int[] tiles)
	{
		if (tiles.Length == 0)
			return;
		
		List<IVector2> repaintTiles = new List<IVector2>();
		
		for (int x = (int)fillRect.xMin; x<(int)fillRect.xMax; x++)
			for (int y = (int)fillRect.yMin; y<(int)fillRect.yMax; y++)
				if (x>=0 && x<tileMatrix.x && y>=0 && y<tileMatrix.y)
				{
					this.tiles[x][y] = tiles[((x-(int)fillRect.xMin)*(int)fillRect.height) + (((int)fillRect.height-1)-(y-(int)fillRect.yMin))];
					if (this.tiles[x][y]==-1)
						alphas[x][y] = 0;
					else
						alphas[x][y] = 1;				
					repaintTiles.Add(new IVector2(x,y));
				}
			
		Repaint(repaintTiles.ToArray());				
	}
	
	public void FillFromTileMap(OTTileMap map, IVector2 offset, Rect fillRect, OTTileMapLayer layer, bool checkTileset)
	{				
		FillTiles(fillRect,map.GetTiles(
			new Rect(offset.x, offset.y, fillRect.width, fillRect.height), 
			map.TileSetFromImage(spriteContainer.GetTexture()), 
			layer, checkTileset));
	}
	
	public void FillFromTileMap(OTTileMap map, IVector2 offset, Rect fillRect)
	{				
		FillFromTileMap(map,offset,fillRect,null,true);
	}
	
	/// <summary>
	/// Fills the sprite with random tiles 
	/// </summary>
	public void FillWithRandomTiles(bool useTransparent)
	{
		if (spriteContainer == null || (spriteContainer !=null && !spriteContainer.isReady))
			return;

		int fc = spriteContainer.frameCount;				
		DimTiles();
		Loop( delegate(IVector2 tile)
		{
			alphas[tile.x][tile.y] = 1;
			if (useTransparent)
			{	
				int idx = Mathf.FloorToInt(Random.value * ((float)fc+0.999f));
				if (idx == fc)
				{
					tiles[tile.x][tile.y] = -1;
					alphas[tile.x][tile.y] = 0;
				}
				else
					tiles[tile.x][tile.y] = idx;
			}
			else
				tiles[tile.x][tile.y] = Mathf.FloorToInt(Random.value * ((float)fc-0.001f));
		});		
		Repaint();
	}

	public void FillWithRandomTiles()
	{
		FillWithRandomTiles(true);
	}
	
	
	/// <summary>
	/// Clear a number of tiles
	/// </summary>
	public void Clear(IVector2[] tiles)
	{
		try
		{
			for (int t=0; t<tiles.Length; t++)
			{
				IVector2 tile = tiles[t];
				this.tiles[tile.x][tile.y] = -1;
				this.objects[tile.x][tile.y] = null;
			}
			Repaint(tiles);
		}
		catch(System.Exception)
		{
		}
	}

	/// <summary>
	/// Clear a single tile
	/// </summary>
	public void Clear(IVector2 tile)
	{
		Clear(new IVector2[] { tile });
	}
		
	/// <summary>
	/// Clears all tiles
	/// </summary>
	public void Clear()
	{
		Clear(allTiles);
	}
				
	
	// ------------------------------------------------------------------------------------------------------------------------------	
	// private and protected methods
	// ------------------------------------------------------------------------------------------------------------------------------	
	
	void DimTiles()
	{				
		if (Mathf.Floor(tileMatrix.x)<=0 || Mathf.Floor(tileMatrix.y)<=0)				
			_tileMatrix = new IVector2(
				(tileMatrix.x<=0)?1:tileMatrix.x,
				(tileMatrix.y<=0)?1:tileMatrix.y);
		
		if (_tiles == null) _tiles = new int[][]{ new int[]{} };
		if (_objects == null) _objects = new object[][]{ new object[]{} };
		if (_alphas == null) _alphas = new float[][]{ new float[]{} };
		
		if (_tiles.Length!= tileMatrix.x)
		{
			System.Array.Resize<int[]>(ref _tiles, tileMatrix.x);			
		}
		if (_objects.Length!= tileMatrix.x)
		{
			System.Array.Resize<object[]>(ref _objects, tileMatrix.x);			
		}
		if (_alphas.Length!= tileMatrix.x)
		{
			System.Array.Resize<float[]>(ref _alphas, tileMatrix.x);			
		}
		
		for (int x=0; x< tileMatrix.x; x++)
		{
			if (_tiles[x] == null) _tiles[x] = new int[]{};
			if (_tiles[x].Length!= tileMatrix.y)
			{
				System.Array.Resize<int>(ref _tiles[x], tileMatrix.y);			
			}			
			if (_objects[x] == null) _objects[x] = new object[]{};
			if (_objects[x].Length!= tileMatrix.y)
			{
				System.Array.Resize<object>(ref _objects[x], tileMatrix.y);			
			}			
			if (_alphas[x] == null) _alphas[x] = new float[]{};
			if (_alphas[x].Length!= tileMatrix.y)
			{
				int os = _alphas[x].Length;
				System.Array.Resize<float>(ref _alphas[x], tileMatrix.y);			
				for (int i=os; i<tileMatrix.y; i++)
					alphas[x][i] = 1;
			}			
		}		
	}
	
    protected override void HandleUV()
    {
    }	
			
    protected override Mesh GetMesh()
    {	
		if (spriteContainer!=null && !spriteContainer.isReady)
			return null;
		
		DimTiles();
		Mesh mesh =InitMesh();
				
		Vector3[] verts = new Vector3[4 * tileCount];
		Vector2[] _uv = new Vector2[4 * tileCount];
		Color[] colors = new Color[4 * tileCount];
		int[] tris = new int[ 6 * tileCount ];

		float mf = 0.001f;
		float sf = OT.view.sizeFactor;
		Color c = Color.white;
		
		bool isEven = false;
		if ((Mathf.Round(tileMatrix.x)/2) == Mathf.Floor(Mathf.Round(tileMatrix.x)/2))
			isEven = true;
			
		
		int ix = 0;
		for (int y=0; y<tileMatrix.y; y++)
		{
			if (isEven)
			{
				if (c==Color.white)
					c = Color.black;
				else
					c = Color.white;												
			}
			
			for (int x=0; x<tileMatrix.x; x++)
			{		
				int vidx = ((x * tileMatrix.y) + y) * 4;
				int tridx = ((x * tileMatrix.y) + y) * 6;
								
				verts[vidx + 0] = new Vector3(x * tileSize.x, y * tileSize.y, 0) * sf;
				verts[vidx + 1] = new Vector3(x * tileSize.x, (y * tileSize.y) + tileSize.y - mf, 0) * sf;
				verts[vidx + 2] = new Vector3(x * tileSize.x + tileSize.x - mf, (y * tileSize.y) + tileSize.y - mf, 0) * sf;
				verts[vidx + 3] = new Vector3(x * tileSize.x + tileSize.x - mf, y * tileSize.y, 0) * sf;				
				
				if (spriteContainer==null)
				{				
					colors[vidx + 0] = new Color(c.r,c.g,c.b,0.25f);
					colors[vidx + 1] = new Color(c.r,c.g,c.b,0.25f);
					colors[vidx + 2] = new Color(c.r,c.g,c.b,0.25f);
					colors[vidx + 3] = new Color(c.r,c.g,c.b,0.25f);
										
					_uv[vidx + 0] = Vector2.zero;
					_uv[vidx + 1] = Vector2.zero;
					_uv[vidx + 2] = Vector2.zero;
					_uv[vidx + 3] = Vector2.zero;					
					
				}
				else
				{															
					int idx = -1;
					if (x < _tiles.Length)
					{
						if (y < _tiles[x].Length)
							idx = _tiles[x][y];
					}
										
					c = Color.white;
					if (idx == -1)
					{
						idx = 0;
						c = new Color(1,1,1,0);
					}
					
					// filled
					Vector2[] tUV = spriteContainer.GetFrame(idx).uv;
					
					colors[vidx + 0] = c;
					colors[vidx + 1] = c;
					colors[vidx + 2] = c;
					colors[vidx + 3] = c;
					
					_uv[vidx + 0] = tUV[3];
					_uv[vidx + 1] = tUV[0];
					_uv[vidx + 2] = tUV[1];
					_uv[vidx + 3] = tUV[2];
					
				}
		
				tris[tridx + 0] = ix;
				tris[tridx + 1] = ix+1;
				tris[tridx + 2] = ix+2;
				tris[tridx + 3] = ix;
				tris[tridx + 4] = ix+2;
				tris[tridx + 5] = ix+3;
								
				if (c==Color.white)
					c = Color.black;
				else
					c = Color.white;												
				
				ix +=4;			
				
			}				
		}
				
		mesh.vertices = TranslatePivotVerts(mesh, verts);		
        mesh.uv = _uv;		
        mesh.triangles = tris;
        mesh.colors = colors;
							
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
							
        return mesh;
    }
	
	protected override void AfterMesh()
	{
		base.AfterMesh();
		if (!Application.isPlaying && showRandomTiles)
			FillWithRandomTiles();		
		else
			Clear();
		
		if (registerInput)
			SetCollider();
		
	}
			
    //-----------------------------------------------------------------------------
    // overridden subclass methods
    //-----------------------------------------------------------------------------	
    
    protected override void CheckSettings()
    {
		Vector2 oldSize = size;
				
		if (spriteContainer!=_spriteContainer_)
		{
			meshDirty = true;
			DimTiles();
			Clear();
			if (!Application.isPlaying && showRandomTiles)
				FillWithRandomTiles();
		}
				
		base.CheckSettings();
		
		if (!Application.isPlaying)
			if (!size.Equals(oldSize))
		{
			size = oldSize;
		}		
		return;
    }
	
	protected override void CheckDirty()
	{
		base.CheckDirty();
		
		if (!meshDirty)
		{
			if (!_tileMatrix.Equals(_tileMatrix_))
			{
				meshDirty = true;
				_tileMatrix_ = _tileMatrix.Clone();
			}
			if (!_tileSize.Equals(_tileSize_))
			{
				meshDirty = true;
				_tileSize_ = _tileSize.Clone();
			}
		}
		
	}
    
    protected override string GetTypeName()
    {
        return "TilesSprite";
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
		_tileMatrix_ = _tileMatrix.Clone();
		_tileSize_ = _tileSize.Clone();
		_showRandomTiles = showRandomTiles;
		if (Application.isPlaying)
		{
			DimTiles();
			Clear();		
		}
    }
	
	public void Rebuild()
	{
		meshDirty = true;
	}

    new void Start()
    {
        base.Start();
    }
	
	bool repainted = false;
    // Update is called once per frame
    new void Update()
    {		
		if (!Application.isPlaying)
		{			
			if (Mathf.Floor(tileMatrix.x)<=0 || Mathf.Floor(tileMatrix.y)<=0)				
				tileMatrix = new IVector2(
					(tileMatrix.x<=0)?1:tileMatrix.x,
					(tileMatrix.y<=0)?1:tileMatrix.y);
													
			
			if (showRandomTiles!=_showRandomTiles)
			{
				_showRandomTiles = showRandomTiles;
				if (showRandomTiles)
					FillWithRandomTiles();
				else
					Clear();
			}
		}
		else
		{
			if (!repainted && OT.isValid && spriteContainer.isReady)
			{
				Repaint();
				repainted = true;
			}
		}
        base.Update();
    }
		
	
	void GizmoDrawMatrix()
	{
		Vector3 bl = otRenderer.bounds.center-otRenderer.bounds.extents;
		Vector3 up = new Vector3(0,1,0)* OT.view.sizeFactor * otTransform.localScale.y;
		Vector3 ri = new Vector3(1,0,0)* OT.view.sizeFactor * otTransform.localScale.x;
		
		if (OT.world == OT.World.WorldTopDown2D)
			up = new Vector3(0,0,1);
		
		for (int y=0; y<=Mathf.FloorToInt(tileMatrix.y); y++)
		{
			Vector3 yp = ( y * up * Mathf.FloorToInt(tileSize.y));
			Gizmos.DrawLine(bl + yp, bl + yp + (ri * Mathf.FloorToInt(tileSize.x) * Mathf.FloorToInt(tileMatrix.x)));
			for (int x=0; x<=Mathf.FloorToInt(tileMatrix.x); x++)
			{
				Vector3 rp = ( x * ri * Mathf.FloorToInt(tileSize.x));
				Gizmos.DrawLine(bl + rp, bl + rp + (up * Mathf.FloorToInt(tileSize.y) * Mathf.FloorToInt(tileMatrix.y)));
			}
		}					
	}
	
	void OnDrawGizmos()
	{
		GizmoDrawMatrix();
	}
	
	void OnDrawGizmosSelected()
	{
		GizmoDrawMatrix();
	}
	
			
}
