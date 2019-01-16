using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConstructionEnums : byte
{
	 None				= 0	//Undefined
    ,constructPoints    = 1 //8
    ,weight				= 4	//800
   	,buildPoints		= 5
	,buildPower			= 6
	,hitpoints			= 9

}

/// <summary>
///  Структура данных конструкторов
/// </summary>
public class ConstructionClass : BaseDataClass
{
	public Dictionary<ConstructionEnums, int> values = new Dictionary<ConstructionEnums, int>();
	public static Dictionary<ConstructionEnums, int> maxValues = new Dictionary<ConstructionEnums, int>();
	public string location;
	public string sensorModel;

	public ConstructionClass()
	{}

	public ConstructionClass(JsonObject pData)
	{
		Init (pData);
	}

	public override void Init(JsonObject pData)
	{
		base.Init (pData);
		type = StatType.Construction;
		location = pData.Get<string> ("location", "");
		sensorModel = pData.Get<string> ("sensorModel", "");

		foreach (ConstructionEnums e in System.Enum.GetValues(typeof(ConstructionEnums)))
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

#if CHECK_VARIABLES
		pData.Remove("location");
		pData.Remove("sensorModel");
		
		pData.Remove("type");
		foreach (string s in pData.Keys)
			Debug.LogError("unknown key "+s+" = "+pData.Get<string>(s)+" in "+name);
#endif		
	}

	/// <summary>
	/// Набор параметров для сравнения и отображения
	/// </summary>
	public override List<StatClass> stats ()
	{
		List<StatClass> stats = new List<StatClass>();
		foreach (ConstructionEnums e in System.Enum.GetValues(typeof(ConstructionEnums)))
		{
			if (values.ContainsKey(e))
				stats.Add(new StatClass (e.ToString (), values [e], maxValues [e]));
		}
		return stats;
	}

	public override string ToString ()
	{
		string v = "specific\n";
		foreach (ConstructionEnums e in System.Enum.GetValues(typeof(ConstructionEnums))) 
		{
			if (values.ContainsKey (e))
				v += " "+e.ToString () + ": [" + values [e].ToString () + "]\n";
		}
		return base.ToString() + v;
	}

	/// <summary>
	/// Значение параметра
	/// </summary>
	public int val(ConstructionEnums pType)
	{
		if (values.ContainsKey (pType))
			return values [pType];
		return 0;
	}

}
