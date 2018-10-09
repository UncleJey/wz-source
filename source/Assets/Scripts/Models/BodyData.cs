using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Polygon
{
	public int[] pixels;
	public Vector3[] points;

	string dumpData(string[] pData)
	{
		string res = " ";
		for (int i = 0; i < pData.Length; i++)
			res += pData[i] + " ";
		return res;
	}

	public Polygon (string[] pData)
	{
		/*
		a00 = 200 (textured) + 800 (no filter)
		4200 = 200 (textured) + 4000 (team-colors)
		4a00 = 200 (textured) + 800 (no filter) + 4000 (team-colors)
		6a00 = 200 (textured) + 800 (no filter) + 2000 (two-sided) + 4000 (team-colors)


		type  points         color          uvs coordinates

		200   3 59 58 63                    234 63  218 71  234 87
		4200  3 75 76 78     3 100 17 18    131 238 131 256 148 256
		a00   3 22 19 20                    106 222 111 216 104 216
	    6a00  3 26 25 24     8 1   11 10    110 224 115 234 104 234
		4a00  3 10 8 6       8 1   8  16    170 210 168 216 175 216
     		*/

		if (pData.Length > 1) 
		{
			int pointOffset = pData.Length - 11;
			int cnt = 0;
			if (int.TryParse (pData [1], out cnt)) 
			{
				pixels = new int[cnt];
				points = new Vector3[cnt];
				for (int nr = 0; nr < cnt; nr++) 
				{
					pixels [nr] = int.Parse (pData [nr + 2]);
					points [nr] = BodyData.FromStr(pData [pointOffset + nr * 2 + 2 + cnt], pData [pointOffset + nr * 2 + 2 + cnt + 1], "0");
				}
			}
			else 
				Debug.LogError ("error in polygon " + dumpData(pData));
		}
	}

	public Polygon()
	{
	}
}

public class BodyLevel
{
	public Vector3[] points;
	public Polygon[] polygons;
}

public class UVPoint
{
	public Vector3 point;
	public Vector3 uv;

	public UVPoint (Vector3 pPoint, Vector3 pUV)
	{
		point = pPoint;
		uv = pUV;
	}

	public UVPoint ()
	{
	}
}

public class WTexture
{
	public string name;
	public int width;
	public int height;
	public int num;

	public WTexture(string[] itms)
	{
		if (itms.Length > 4) 
		{
			int.TryParse (itms [1], out num);
			name = itms [2];
			int.TryParse (itms [3], out width);
			int.TryParse (itms [4], out height);
		}
	}

	public WTexture()
	{
	}
}

public class BodyData
{
	BodyLevel[] levels;
	public WTexture myTexture;
	public Vector3[] connector;

	public BodyData(string pData)
	{
		parce (pData);
	}

	public int lCount
	{
		get 
		{
			if (levels == null)
				return 0;
			return levels.Length;
		}
	}

#region retults
	List<UVPoint> _verts = new List<UVPoint> ();
	List<int> _trs = new List<int> ();
	float divider = 256;

	/// <summary>
	/// Возвращает готовый набор данных для построения меша
	/// </summary>
	/// <param name="pLevel">Уровень (номер подмеша)</param>
	/// <param name="verts">Набор вершин</param>
	/// <param name="tri">Набор треугольников</param>
	/// <param name="uvs">Данные для развёртки</param>
	public void GetArrays(int pLevel, out Vector3[] verts, out int[] tri, out Vector2[] uvs)
	{
		_verts.Clear ();
		_trs.Clear ();

		BodyLevel level = levels [pLevel];

		foreach (Polygon plg in level.polygons) 
		{
			for (int i = 0; i < 3; i++)
				_trs.Add (GetPoint (level.points[plg.pixels [i]], plg.points [i]));
		}

		tri = _trs.ToArray ();
		verts = new Vector3[_verts.Count];
		uvs = new Vector2[_verts.Count];

		for (int i = _verts.Count - 1; i >= 0; i--) 
		{
			verts [i] = _verts [i].point;
			uvs [i] = new Vector2 (_verts [i].uv.x, divider - _verts [i].uv.y) / divider;
		}
	}

	/// <summary>
	/// Возвращает вершину с привязкой к координатам развёртки
	/// </summary>
	/// <returns>The point.</returns>
	/// <param name="p">Координаты вершины</param>
	/// <param name="v">Координаты на развёртке</param>
	int GetPoint(Vector3 p, Vector3 v)
	{
		for (int i = _verts.Count - 1; i >= 0; i--) 
		{
			UVPoint u = _verts [i];
			if (p.Equals (u.point) && v.Equals (u.uv)) 
			{
				//Debug.Log(string.Format("{0}:{1}:{2}",i,p,v));
				return i;
			}
		}

		_verts.Add(new UVPoint (p, v));
		//Debug.Log(string.Format("{0}:{1}:{2}",_verts.Count - 1,p,v));

		return _verts.Count - 1;
	}

	/// <summary>
	/// Габориты объекта
	/// </summary>
	void Gaborites(out Vector3 min, out Vector3 max)
	{
		min = Vector3.zero;
		max = Vector3.zero;
		foreach (BodyLevel level in levels)
		{
			foreach (Vector3 p in level.points)
			{
				if (p.x < min.x)
					min.x = p.x;
				else if (p.x > max.x)
					max.x = p.x;
				if (p.y < min.y)
					min.y = p.y;
				else if (p.y > max.y)
					max.y = p.y;
				if (p.z < min.z)
					min.z = p.z;
				else if (p.z > max.z)
					max.z = p.z;
			}
		}
	}

	/// <summary>
	/// Центр объекта
	/// </summary>
	public void GetGaborites(out Vector3 vCenter, out Vector3 vGaborites)
	{
		Vector3 min,max = Vector3.zero;
		Gaborites (out min, out max);
		vCenter = (min + max) / 2f;
		vGaborites = max - min;
	}
#endregion retults

#region parcer
	/// <summary>
	/// Парсинг данных
	/// </summary>
	public void parce(string pData)
	{
		pData = pData.Replace('\r',' ').Replace('\t',' ');
		string[] lines = pData.Split ('\n');
		int cnt = lines.Length;
		int num = 0; int pointno = 0;
		string mode = "";
		BodyLevel level = null;

		for (int i = 0; i < cnt; i++) 
		{
			string line = lines [i].Trim ();
			while (line.Contains ("  "))
				line = line.Replace ("  ", " ");

			string[] el = line.Split(' ');

			if (el.Length > 0) 
			{
				string elTag = el [0];
				switch (elTag) 
				{
					case "PIE":
						if (el.Length < 2)
						{
							Debug.LogError ("Unsupported " + line);
							return;
						}
						else if (el [1] == "2") //PIE2 - все значения целые 0..256 и нужно преобразовывать в вещественное
							divider = 256;
						else if (el [1] == "3") //PIE3 - все значения вещественные 
							divider = 1;
						else
						{
							Debug.LogError ("Unsupported " + line);
							return;
						}
					break;
					case "TYPE":
						if (el.Length < 2) 
						{
							Debug.LogError ("Unsupported " + line);
							return;
						}
						else if (el [1] == "200" || el [1] == "10200") 
						{
							//ok
						}
						else 
						{
							Debug.LogError ("Unsupported " + line);
							return;
						}
					break;
					case "TEXTURE":
						myTexture = new WTexture (el);
						mode = "";
					break;
					case "LEVELS":
						int.TryParse (el [1], out num);
						levels = new BodyLevel[num];
						mode = "";
					break;
					case "LEVEL":
						int.TryParse (el [1], out num);
						level = new BodyLevel ();
						levels [num - 1] = level;
						mode = "";
					break;
					case "POINTS":
						int.TryParse (el [1], out num);
						level.points = new Vector3[num];
						pointno = 0;
						mode = "POINTS";
					break;
					case "POLYGONS":
						int.TryParse (el [1], out num);
						pointno = 0;
						level.polygons = new Polygon[num];
						mode = "POLYGONS";
					break;
					case "CONNECTORS":
						mode = "CONNECTORS";
						int.TryParse (el [1], out num);
						connector = new Vector3[num];
						pointno = 0;
					break;
					default:
						if (!string.IsNullOrEmpty(line))
						{
							if (mode.Equals ("POINTS")) 
							{
								if (pointno < level.points.Length)
									level.points [pointno++] = FromStr(line);
								else
									Debug.LogError ("Points error");
							}
							else if (mode.Equals ("POLYGONS")) 
							{
								if (pointno < level.polygons.Length)
									level.polygons [pointno++] = new Polygon (el);
								else
									Debug.LogError ("Polygons error line:["+line+"] \r\n Data:"+pData);
							}
							else if (mode.Equals ("CONNECTORS")) 
							{
								if (pointno < connector.Length && el.Length == 3)
									connector[pointno++] = FromStr(el[0], el[2], el[1]);
							else if (!string.IsNullOrEmpty(line))
									Debug.LogError ("Connector error "+line);
							}
							else
								Debug.Log("Skip line "+line);
						}						
					break;
				}
			}
		}
	}

	/// <summary>
	/// Вектор из строки
	/// </summary>
	public static Vector3 FromStr(string pVec)
	{
		if (string.IsNullOrEmpty (pVec))
			return Vector3.zero;
		
		string[] dt = pVec.Replace(',',' ').Trim ().Split (' ');
		if (dt.Length == 3)
			return FromStr (dt [0], dt [1], dt [2]);
		return Vector3.zero;
	}

	/// <summary>
	/// Вектор из строк
	/// </summary>
	public static Vector3 FromStr(string pX, string pY, string pZ)
	{
		Vector3 res = Vector3.zero;
		float.TryParse (pX, out res.x);
		float.TryParse (pY, out res.y);
		float.TryParse (pZ, out res.z);
		return res;
	}
	#endregion parcer
}
