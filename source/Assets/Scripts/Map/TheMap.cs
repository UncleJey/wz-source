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
            texture = new Texture2D(textureSize, textureSize, TextureFormat.RGB24, false)
            {
                anisoLevel = 3,
                filterMode = FilterMode.Bilinear
            };
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
                MapCell c = new MapCell
                {
                    tile = br.ReadByte(),
                    rotate = br.ReadByte(),
                    height = br.ReadByte()
                };
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
    /// Высота в точке краты
    /// </summary>
    public static float Height(Vector3 p)
    {
        return Height(p.x, p.y, p.z);
    }

    /// <summary>
    /// Высота в точке карты
    /// </summary>
    public static float Height(float pX, float pY, float pZ)
    {
        RaycastHit hit;
        const int d = 20;
        Ray r = new Ray(new Vector3(pX, -1, pZ), Vector3.up);

        if (Physics.Raycast(r, out hit, d, instance.myLayer))
        {
            return hit.distance - 1;
        }
        return 0;
    }
#endregion Navigation
}
