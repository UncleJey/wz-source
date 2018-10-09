using UnityEngine;
using System.Collections;

[System.Serializable]
public class SomeStats
{
	public string name;
	public StatType type;
	public string path;
	public JsonObject data;
}

public class Stats : MonoBehaviour 
{
	public SomeStats[] stats;
	public string[] startTech;

	static Stats instance;

	//TODO: Сделать объединение статов

	void Awake()
	{
		instance = this;

		foreach (SomeStats stat in stats)
		{
			TextAsset tdata = Resources.Load<TextAsset> (stat.path);
			stat.data = (JsonObject) MiniJson.Deserialize (tdata.text);
		}
	}

	/// <summary>
	/// Возвращает стату
	/// </summary>
	public static JsonObject GetObject(string pName)
	{
		for (int i=instance.stats.Length - 1; i>=0; i--)
		{
			SomeStats stat = instance.stats[i];
			if (stat.data.ContainsKey(pName))
				return (JsonObject) stat.data[pName];
		}
		Debug.LogError ("Have no " + pName);
		return null;
	}

	public static JsonObject GetObject(string pName, StatType pType)
	{
		for (int i=instance.stats.Length - 1; i>=0; i--) // Может быть подключено несколько файлов с одним статом
		{
			SomeStats stat = instance.stats[i];
			if (stat.type == pType)
			{
				JsonObject data = stat.data;
				if (data != null && data.ContainsKey(pName))
					return (JsonObject) data[pName];
			}
		}

		Debug.LogError ("Have no " + pName);
		return null;
	}

	/// <summary>
	/// Возвращает раздел настроек. Нужно быть аккуратным. Может быть несколько подключенных файлов со статами
	/// </summary>
	public static JsonObject GetStat(StatType pType)
	{
		foreach (SomeStats stat in instance.stats)
			if (stat.type == pType)
				return stat.data;

		Debug.LogError("Have no "+pType.ToString());
		return null;
	}

}
