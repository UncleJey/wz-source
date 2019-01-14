using UnityEngine;
using System;
using System.Collections;

public class BodyRender : MonoBehaviour 
{
	public Vector3 center, gab;

	GameObject[] childs;
	public float scale = 1;
	public BodyData data;
	/// <summary>
	/// Тип элемента
	/// </summary>
	public StatType type;

	public void DoRender(string pName)
	{
		if (string.IsNullOrEmpty (pName))
		{
			Clear ();
			return;
		}
		data = PieReader.GetData (pName);

		if (data != null)
		{
			DoRender (data);
			/*
			if (data.connector != null)
				Debug.Log (pName+" connectors: " + data.connector.Length);
			else
				Debug.Log (pName+" no connectors");
				*/
		}
		else
		{
			Debug.LogError ("model not found " + pName);
			Clear ();
		}
	}

	public void Clear(bool pGoOffline = false)
	{
		for (int i=transform.childCount-1; i>=0; i--)
			Destroy(transform.GetChild(i).gameObject);
		if (!gameObject.activeSelf) // При повторной инициализации уничтожаем то что было раньше
			gameObject.SetActive(!pGoOffline);
		if (pGoOffline)
			gameObject.SetActive(false);

	}

	public void DoRender(BodyData pData)
	{
		Vector3[] vrts = null;
		int[] trs = null;
		Vector2[] uvs = null;

		Clear ();
		if (pData == null)
			return;

		childs = new GameObject[pData.lCount];
		pData.GetGaborites (out center, out gab);

		if (scale > 1.1f)
			transform.localScale = Vector3.one * scale / Mathf.Max (gab.x, gab.y, gab.z);

		for (int i = childs.Length-1; i >=0 ; i--) 
		{
			GameObject go = (GameObject) Instantiate(TextureManager.BodyItemPrefab);
			go.transform.SetParent (transform);
			go.transform.localScale = Vector3.one;
			go.transform.localRotation = Quaternion.Euler (Vector3.zero);
			go.transform.localPosition = Vector3.zero;// -center;
			go.name = "level_" + i.ToString ();

			go.layer = this.gameObject.layer;

			childs [i] = go;

			Mesh msh = new Mesh ();

			MeshRenderer mr = go.GetComponent<MeshRenderer> ();
			mr.material = TextureManager.GetMaterial (pData.myTexture.name);

			MeshFilter mf = go.GetComponent<MeshFilter> ();
			mf.mesh = msh;

			pData.GetArrays (i, out vrts, out trs, out uvs);
			msh.vertices = vrts;
			msh.triangles = trs;
			msh.uv = uvs;

		}
	}

}
