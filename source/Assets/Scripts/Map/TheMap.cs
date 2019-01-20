using UnityEngine;
using System.IO;
using System.Collections.Generic;

/*
 * Формат карты
 * map%20		4 CHAR			идентификатор карты
 * 0A 00 00 00	4 UDWORD		версия
 * 30 00 00 00  4 UDWORD		width
 * 5c 00 00 00  4 UDWORD		height
 * Далее идёт массив из данных номер тайла + высота
 * 40 10		2 UWORD		Старший байт - вращение-отображение. Младший байт номер тайла
 * 80			1 UBYTE		высота
 * 
 * Далее идёт список ворот
 * Сначала заголовок
 * 01 00 00 00	4 UDWORD	версия
 * 1D 00 00 00	4 UDWORD	количество
 * Массив
 * 03			1 UBYTE		X0
 * 0F			1 UBYTE		Y0
 * 05			1 UBYTE		X1
 * 0F			1 UBYTE		Y1
 */ 

/// <summary>
/// Высота. Она же - начало тайла
/// </summary>
public class MapCell
{
	public byte tile;
	public byte rotate;
	public byte height;

	/// <summary>
	/// Кем занята ячейка (ID)
	/// </summary>
	public
	ushort u1 = 0
		  ,u2 = 0
		  ,u3 = 0
		  ,u4 = 0;

	/// <summary>
	/// Ячейка полностью свободна
	/// </summary>
	public bool free
	{
		get
		{
			return (u1 == 0 && u2 == 0 && u3 == 0 && u4 == 0);
		}
	}

	/// <summary>
	/// Занять ячейку
	/// </summary>
	public bool use(int pNo, ushort pID)
	{
		switch (pNo)
		{
			case 1: 
				if (u1 == 0)
					u1 = pID;
				else
					return false;
			break;
			case 2: 
				if (u2 == 0)
					u2 = pID;
				else
					return false;
			break;
			case 3: 
				if (u3 == 0)
					u3 = pID;
				else
					return false;
			break;
			case 4: 
				if (u4 == 0)
					u4 = pID;
				else
					return false;
			break;
			default:
				Debug.LogError ("Wrong place! " + pNo);
				return false;
			break;
		}
		return true;
	}

	/// <summary>
	/// Освободить ячейку
	/// </summary>
	public void unUse(int pNo, ushort pID)
	{
		switch (pNo)
		{
			case 1: 
				if (u1 == pID)
					u1 = 0;
			break;
			case 2: 
				if (u2 == pID)
					u2 = 0;
			break;
			case 3: 
				if (u3 == pID)
					u3 = 0;
			break;
			case 4: 
				if (u4 == pID)
					u4 = 0;
			break;
			default:
				Debug.LogError ("Wrong place! " + pNo);
			break;
		}
	}
}

[RequireComponent (typeof (MeshFilter))]
[RequireComponent (typeof (MeshRenderer))]
[RequireComponent (typeof (MeshCollider))]
public class TheMap : MonoBehaviour 
{
	/// <summary>
	/// Порог высоты доступной для преодоления
	/// </summary>
	static TheMap instance;

	readonly int textureSize = 4096;
    /* The shift on a world coordinate to get the tile coordinate */
    readonly static int TILE_SHIFT = 7;

    /* The number of units accross a tile */
    readonly static int TILE_UNITS = 1 << 7;// TILE_SHIFT;

    public string mapPath;

	MapCell[,] map;
	byte[,] navCells;

	[System.NonSerialized]
	public int width, height;

    [SerializeField]
    LayerMask myLayer;

	int version;
	MeshFilter meshFilter;
	MeshCollider _collider;

	public Material baseMaterial;
	Texture2D texture;
	public Texture2D mainTextureBot;
	public Texture2D mainTextureTop;
	public Texture2D[] tileset;
	public Vector3 scale = new Vector3 (1, 1, 1f/50f) * 200;

	float cellw;
	float cellh;

	public static readonly byte DisabledCell = 0;
	public static readonly byte EnabledCell = 1;

#region Startup
	void Awake()
	{
		instance = this;
		Generate();
	//	AssetDataBase.CreateAsset(meshFilter.sharedMesh, mapPath+"mesh.asset");
//	    AssetDatabase.CreateAsset(meshFilter.sharedMesh, "Assets/Resources/"+mapPath+"mesh.asset");

	}
#endregion Startup

#region Generator
	bool Rebuild(string pMapPath)
	{
		TextAsset asset = Resources.Load<TextAsset>(pMapPath+"game");

		Stream s = new MemoryStream(asset.bytes);
		BinaryReader br = new BinaryReader(s);

		char[] identifier = {'m','a','p',' '};
		for (int i=0; i<4; i++)
		{
			if (identifier[i] != br.ReadChar()) // map%20
			{
				Debug.LogError("Unknown map header format");
				br.Close();
				return false;
			}
		}

		version = (int)br.ReadUInt32();
		width   = (int)br.ReadUInt32();
		height  = (int)br.ReadUInt32();

		cellw = textureSize / width;
		cellh = textureSize / height;

		Debug.Log("Map v.["+version.ToString()+"] "+ width.ToString() + " x " + height.ToString());

		texture = Resources.Load<Texture2D>(pMapPath+"maptex");
		bool loaded = false;
		if (texture)
		{
			baseMaterial.mainTexture = texture;
			loaded = true;
		}
		else
		{
			texture = new Texture2D (textureSize, textureSize, TextureFormat.RGB24, false);
			texture.anisoLevel = 3;
			texture.filterMode = FilterMode.Bilinear;
		}

		int iCellW = (int)cellw + 1;
		int iCellH = (int)cellh + 1;

		int xx, yy, ww, hh;

		map = new MapCell[width,height];
		navCells = new byte[width,height];

		for (int i=0; i<height; i++)
		{
			for (int j=0; j<width; j++)
			{
				MapCell c = new MapCell();
				c.tile = br.ReadByte();
				c.rotate = br.ReadByte();
				c.height = br.ReadByte();
				map[j,i] = c;

				xx = j*iCellW;
				yy = i*iCellH;
				if (xx+iCellW > textureSize)
					ww = textureSize - xx;
				else
					ww = iCellW;

				if (yy+iCellH > textureSize)
				    hh = textureSize - yy;
				else
				    hh = iCellW;
				if (!loaded)
				{
					if (c.height < 128)
						texture.SetPixels (xx, yy, ww, hh, mainTextureBot.GetPixels (0, 0, ww, hh));
					else
						texture.SetPixels (xx, yy, ww, hh, mainTextureTop.GetPixels (0, 0, ww, hh));
				}
			}
		}

		br.Close();

		//int a = 0; 
	//	int b = 0;

		if (!loaded)
		{
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					MapCell cell = map [i, j];
					Texture2D tt = tileset [cell.tile];

					bool rotateX = (cell.rotate & 16) > 0;
					bool rotateY = (cell.rotate & 32) > 0;
					bool flipY = (cell.rotate & 64) > 0;
					bool flipX = (cell.rotate & 128) > 0;

					float tw = cellw / tt.width;
					float th = cellh / tt.height;

					Color cc;

					for (int aa = 0; aa < tt.width; aa++)
					{
						for (int bb = 0; bb < tt.height; bb++)
						{
							if (rotateY)
							{
								if (rotateX)
									cc = tt.GetPixel (flipX ? bb : tt.height - bb, flipY ? aa : tt.width - aa);
								else
									cc = tt.GetPixel (flipX ? aa : tt.width - aa, flipY ? tt.height - bb : bb);
							}
							else
							{
								if (rotateX)
									cc = tt.GetPixel (flipX ? tt.height - bb : bb, flipY ? tt.width - aa : aa);
								else
									cc = tt.GetPixel (flipX ? tt.width - aa : aa, flipY ? bb : tt.height - bb);
							}
							int worldX = (int)(cellw * i + tw * aa);
							int worldY = (int)(cellh * j + th * bb);

							Color ca = texture.GetPixel (worldX, worldY);
							texture.SetPixel (worldX, worldY, Color.Lerp (ca, cc, cc.a / 1.2f));
						}
					}
				}
			}

			texture.Apply ();
			baseMaterial.mainTexture = texture;
			TextureHelper.SaveTextureToFile (texture, pMapPath + "maptex.png");
		}
		return true;
	}

	public void Generate()
	{
		meshFilter = GetComponent<MeshFilter>();
		_collider = GetComponent<MeshCollider> ();
		if (meshFilter==null)
		{
			Debug.LogError("MeshFilter not found!");
			return;
		}
		
		Mesh mesh = meshFilter.sharedMesh;
		if (mesh == null)
		{
			meshFilter.mesh = new Mesh();
			mesh = meshFilter.sharedMesh;
		}
		mesh.Clear();

		if (Rebuild(mapPath))
		{
			Vector3[] points = new Vector3[width * height];
			List<int> triangles = new List<int>();
			Vector2[] uvs = new Vector2[width * height];
			for (int i=0; i<width; i++)
			{
				for (int j=0; j<height; j++)
				{
					points[i*height+j] = new Vector3(i * scale.x, j * scale.y, scale.z * map[i,j].height) ;
					uvs[i*height+j] = new Vector2(((float) i) / width, ((float)j) / height);
				}
			}
			mesh.vertices = points;
			mesh.uv = uvs;
			for (int i=0; i<width-1; i++)
			{
				for(int j=0; j<height-1; j++)
				{
					triangles.Add((int)((i+1)*height+j));
					triangles.Add((int)(i*height+j+1));
					triangles.Add((int)(i*height+j));

					triangles.Add((int)((i+1)*height+j));
					triangles.Add((int)((i+1)*height+j+1));
					triangles.Add((int)(i*height+j+1));

				}
			}
			mesh.triangles = triangles.ToArray();
		}

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		_collider.sharedMesh = mesh;
		
	}
#endregion Generator

#region Internal
	/// <summary>
	/// Высота по координатам с защитой
	/// </summary>
	byte CellHeight(int pX, int pY)
	{
		if (pX < 0 || pX >= width)
			return 0;

		if (pY < 0 || pY >= height)
			return 0;

		return map [pX, pY].height;
	}

	/// <summary>
	/// Разница высот
	/// </summary>
	byte DeltaHeight(byte pH1, byte pH2)
	{
		if (pH1 < pH2)
			return (byte)(pH2 - pH1);
		return (byte)(pH1 - pH2);
	}

#endregion Internal

#region Navigation
    /// <summary>
    /// Координаты карты в координаты мира
    /// </summary>
    public static Vector3 MapToWorld(Vector2Int pPoint)
    {
        return MapToWorld(pPoint.x, pPoint.y);
    }


    /// <summary>
    /// Координаты карты в координаты мира
    /// </summary>
    public static Vector3 MapToWorld(int pX, int pY)
	{
		if (pX < 3)
			pX = 3;
		else if (pX > instance.width - 2)
			pX = instance.width - 2;

		if (pY < 3)
			pY = 3;
		else if (pY > instance.height - 2)
			pY = instance.height - 2;

		return new Vector3 (instance.scale.x * pX, instance.scale.z * instance.map [pX, pY].height, -instance.scale.y * pY);//+ instance.transform.position;
	}

	/// <summary>
	/// Координаты мира в координаты карта
	/// </summary>
	public static Vector2Int WorldToMap(Vector3 pPoint)
	{
        return new Vector2Int (Mathf.RoundToInt(pPoint.x / instance.scale.x), Mathf.RoundToInt(-pPoint.z / instance.scale.y));
	}

    public static float Height(Vector3 p)
    {
        return Height(p.x, p.y, p.z);
    }

    /// The max height of the terrain and water at the specified world coordinates
    public static float Height(float pX, float pY, float pZ)
    {
        RaycastHit hit;
        const int d = 20;
        Ray r = new Ray(new Vector3(pX, -1, pZ), Vector3.up);
        Debug.DrawLine(r.GetPoint(0), r.GetPoint(d), Color.red);

        if (Physics.Raycast(r, out hit, d, instance.myLayer))
        {
            return hit.distance - 1;
        }
        return 0;
    }

#endregion Navigation
}
