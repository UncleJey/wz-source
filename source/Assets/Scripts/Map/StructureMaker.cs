using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StrucPoint
{
	public Vector3 point;
	public int U = -1;
	public int V = -1;
	public int number = -1;
	public int baseNumber = -1;
}

[RequireComponent (typeof (MeshFilter))]
[RequireComponent (typeof (MeshRenderer))]
public class StructureMaker : MonoBehaviour 
{
	static readonly string pointsTag = "POINTS";
	static readonly string trianglesTag = "POLYGONS";

	public TextAsset ass;

	public float divider = 256f;

	List<StrucPoint> points = new List<StrucPoint>();

	public void Rebuild () 
	{
		MeshFilter meshFilter = GetComponent<MeshFilter>();
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

		points.Clear ();

		string[] lines = ass.text.Split('\n');
		int cnt = -1;
		int nr=0;
		foreach (string line in lines)
		{
			if (line.Contains(pointsTag))
			{
				string cnts = line.Substring(pointsTag.Length, line.Length - pointsTag.Length).Trim();
				int.TryParse(cnts, out cnt);
			}
			else if (cnt-- > 0)
			{
                StrucPoint p = new StrucPoint
                {
                    number = nr++
                };
                p.baseNumber = p.number;
				p.point = StringToVector3(line);
				if (p.point.sqrMagnitude > 0)
					points.Add(p);
				if (cnt<=0)
					break;
			}
		}

		cnt = -1;
		List<int> trs = new List<int> ();
		foreach (string line in lines)
		{
			if (line.Contains(trianglesTag))
			{
				string cnts = line.Substring(trianglesTag.Length, line.Length - trianglesTag.Length).Trim();
				int.TryParse(cnts, out cnt);
			}
			else if (cnt-- > 0)
			{
				int[] plg = StringToArray(line);
				if (plg.Length > 4)
				{
					trs.Add(GetPointNo(plg[2], plg[5], plg[ 6]));
					trs.Add(GetPointNo(plg[3], plg[7], plg[ 8]));
					trs.Add(GetPointNo(plg[4], plg[9], plg[10]));
				}
				if (cnt<=0)
					break;
			}
		}

		Vector3[] pnts = new Vector3[points.Count];
		Vector2[] uvs  = new Vector2[points.Count];
		for (int i=points.Count-1; i>=0; i--)
		{
			pnts [i] = points [i].point;
			uvs [i] = new Vector2(points[i].U, 256-points[i].V) / divider;
		}

		mesh.vertices = pnts;
		mesh.triangles = trs.ToArray();

		mesh.uv = uvs;

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		;
	}

	int GetPointNo(int pBaseNo, int pU, int pV)
	{
		StrucPoint basePoint = null;
		foreach (StrucPoint p in points)
		{
			if (p.baseNumber == pBaseNo)
			{
				basePoint = p;
				if (p.U < 0)
				{
					p.U = pU;
					p.V = pV;
					return p.number;
				}
				else if (p.U == pU && p.V == pV)
					return p.number;
			}
		}

        StrucPoint newPoint = new StrucPoint
        {
            point = basePoint.point,
            baseNumber = pBaseNo,
            U = pU,
            V = pV,
            number = points.Count
        };
        points.Add (newPoint);
		return newPoint.number;
	}

	Vector3 StringToVector3(string pString)
	{
		string[] lines = pString.Trim().Split(' ');
		Vector3 res = Vector3.zero;
		if (lines.Length == 3)
		{
			float.TryParse(lines[0], out res.x);
			float.TryParse(lines[1], out res.y);
			float.TryParse(lines[2], out res.z);
		}
		return res;
	}

	int[] StringToArray(string pString)
	{
		string[] lines = pString.Trim().Split(' ');
		int[] res = new int[lines.Length];
		for (int i=lines.Length-1; i>=0; i--)
			int.TryParse(lines[i], out res[i]);
		return res;
	}

	void Start()
	{
		Rebuild();
	}
}
