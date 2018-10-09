using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PieReader : MonoBehaviour 
{
	static Dictionary<string, BodyData> datas = new Dictionary<string, BodyData>();
	public string[] pieFolders;

	static PieReader instance;
	void Awake()
	{
		instance = this;
	}

	public static BodyData GetData(string pName)
	{
		pName = pName.ToLower().Replace (".txt", "").Replace (".pie", "").Trim();

		if (datas.ContainsKey (pName))
			return datas [pName];

		TextAsset pData = null;

		foreach (string folder in instance.pieFolders) 
		{
			pData = Resources.Load <TextAsset> (folder + pName);
			if (pData != null && !string.IsNullOrEmpty (pData.text))
				break;
		}

		if (pData == null || string.IsNullOrEmpty (pData.text)) 
		{
			Debug.LogError ("["+ pName + "] not found!");
			return null;
		}
		
		BodyData data = new BodyData (pData.text);

		datas.Add (pName, data);

		return data;
	}

}
