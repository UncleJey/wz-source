using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RepairEnums : byte
{
	 None				= 0	//Undefined
	,repairPoints		= 2 //15
    ,time				= 3	//7,
    ,weight				= 4	//800
	,buildPoints		= 5
	,buildPower			= 6
	,range				= 7
	,power				= 8
	,hitpoints			= 9
}

/// <summary>
///  Структура данных Шасси
/// </summary>
public class RepairClass : BaseDataClass
{
	public Dictionary<RepairEnums, int> values = new Dictionary<RepairEnums, int>();
	public static Dictionary<RepairEnums, int> maxValues = new Dictionary<RepairEnums, int>();
	public string location;
	public string sensorModel;

	public RepairClass()
	{}

	public RepairClass(JsonObject pData)
	{
		Init (pData);
	}

	public override void Init(JsonObject pData)
	{
		base.Init (pData);
		type = StatType.Repair;
		location = pData.Get<string> ("location", "");
		sensorModel = pData.Get<string> ("sensorModel", "");

		foreach (RepairEnums e in System.Enum.GetValues(typeof(RepairEnums)))
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
		foreach (RepairEnums e in System.Enum.GetValues(typeof(RepairEnums)))
		{
			if (values.ContainsKey(e))
				stats.Add(new StatClass (e.ToString (), values [e], maxValues [e]));
		}
		return stats;
	}

	public override string ToString ()
	{
		string v = "specific\n";
		foreach (RepairEnums e in System.Enum.GetValues(typeof(RepairEnums))) 
		{
			if (values.ContainsKey (e))
				v += " "+e.ToString () + ": [" + values [e].ToString () + "]\n";
		}
		return base.ToString() + v;
	}

	/// <summary>
	/// Значение параметра
	/// </summary>
	public int val(RepairEnums pType)
	{
		if (values.ContainsKey (pType))
			return values [pType];
		return 0;
	}

}
