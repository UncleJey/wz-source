using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Тип шасси
/// </summary>
public enum SensorType : byte
{
	 None			= 0 // Undifined
	,STANDARD		= 1
	,RADAR_DETECTOR = 2
	,SUPER			= 3
	,INDIRECT_CB	= 4
	,VTOL_CB		= 5
	,VTOL_INTERCEPT = 6
}

public enum SensorEnums : byte
{
	 None			= 0  // Undefined
	,buildPoints	= 1	 // 15
	,hitpoints		= 2  // 1
	,buildPower		= 3  // 1
	,power			= 4  // 1000
	,range			= 5  // 3072
	,weight			= 6  // 1
}

/// <summary>
///  Структура данных Шасси
/// </summary>
public class SensorClass : BaseDataClass
{
	/// <summary>
	/// Тип
	/// </summary>
	public SensorType stype;
	/// <summary>
	/// specifies whether the Sensor is default or for the Turret
	/// </summary>
	public string location;		//TURRET

	public Dictionary<SensorEnums, int> values = new Dictionary<SensorEnums, int>();
	public static Dictionary<SensorEnums, int> maxValues = new Dictionary<SensorEnums, int>();

	public SensorClass()
	{}

	public SensorClass(JsonObject pData)
	{
		Init (pData);
	}

	public override void Init(JsonObject pData)
	{
		base.Init (pData);
		type = StatType.Sys;
		values.Clear ();
		foreach (SensorEnums e in System.Enum.GetValues(typeof(SensorEnums)))
		{
			int v = pData.Get<int> (e.ToString ());
			if (pData.ContainsKey (e.ToString ())) 
			{
				values [e] = v;
				if (!maxValues.ContainsKey (e) || maxValues [e] < v)
					maxValues [e] = v;
#if CHECK_VARIABLES
				pData.Remove(e.ToString());
#endif
			}
		}		
		stype = GetType(pData.Get<string> ("type", "None"));
		location = pData.Get<string> ("location", "");

#if CHECK_VARIABLES
		pData.Remove("id");
		pData.Remove("name");
		pData.Remove("model");
		pData.Remove("type");
		pData.Remove("location");
		pData.Remove("mountModel");
		pData.Remove("sensorModel");
		pData.Remove("designable");
		foreach (string s in pData.Keys)
			Debug.LogError("unknown key "+s+" in "+name);
#endif		
	}

	/// <summary>
	/// Возвращает тип Сенсора по имени
	/// </summary>
	public static SensorType GetType(string pName)
	{
		try
		{
			return (SensorType) System.Enum.Parse (typeof(SensorType), pName.Replace("-","_").Replace(" ","_"));
		}
		catch 
		{
			Debug.LogError("unknown type "+ pName);
		}
		return SensorType.None;
	}

	/// <summary>
	/// Набор параметров для сравнения и отображения
	/// </summary>
	public override List<StatClass> stats ()
	{
		List<StatClass> stats = new List<StatClass>();
		foreach (SensorEnums e in System.Enum.GetValues(typeof(SensorEnums)))
		{
			if (values.ContainsKey(e))
				stats.Add(new StatClass (e.ToString (), values [e], maxValues [e]));
		}
		return stats;
	}
}
