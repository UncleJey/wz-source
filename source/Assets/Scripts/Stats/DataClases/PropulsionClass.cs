using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Тип шасси
/// </summary>
public enum PropType : byte
{
	 None			= 0 // Undifined
	,Legged			= 1 // Cyborg
	,Wheeled		= 2 // 
	,Half_Tracked	= 3 //
	,Lift			= 4 // Helicopter, V-Tol
	,Tracked		= 5 //
	,Hover			= 6 //
	,Propellor		= 7 //
}

public enum PropEnums : byte
{
	 None				= 0  //Undefined
	/// <summary>
	/// Time required to build the component
	/// </summary>
	,buildPoints		= 1	 //15
	/// <summary>
	/// Power required to build the component
	/// </summary>
	,buildPower			= 2  //10
	/// <summary>
	/// HP
	/// </summary>
	,hitpoints			= 3  //1
	/// <summary>
	/// Component's weight
	/// </summary>
	,weight				= 4  //10
	,deceleration		= 5	 //450
	,speed				= 6  //200
	,spinAngle			= 7  //45
	,spinSpeed			= 8  //450
	,turnSpeed			= 9  //225
	,skidDeceleration	= 11 //500
	,acceleration		= 12 //200
}

/// <summary>
/// Разные модельки на разные ситуации
/// </summary>
public class PropModels
{
	public string left;
	public string right;
	public string moving;
	public string still;

	public PropModels(JsonObject pData)
	{
		left 	= pData.Get<string> ("left", "");
		right	= pData.Get<string> ("right", "");
		moving	= pData.Get<string> ("moving", "");
		still	= pData.Get<string> ("still", "");
	}

	public PropModels()
	{
	}
}

/// <summary>
///  Структура данных Шасси
/// </summary>
public class PropulsionClass : BaseDataClass
{
	/// <summary>
	/// Тип подвески
	/// </summary>
	public PropType ptype;		//"Legged",

	public Dictionary<PropEnums, int> values = new Dictionary<PropEnums, int>();
	public static Dictionary<PropEnums, int> maxValues = new Dictionary<PropEnums, int>();

	public PropulsionClass()
	{}

	public PropulsionClass(JsonObject pData)
	{
		Init (pData);
	}

	public override void Init(JsonObject pData)
	{
		base.Init (pData);
		type = StatType.Propulsion;
		values.Clear ();
		foreach (PropEnums e in System.Enum.GetValues(typeof(PropEnums)))
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
		ptype = GetType(pData.Get<string> ("type", "None"));

#if CHECK_VARIABLES
		pData.Remove("id");
		pData.Remove("name");
		pData.Remove("model");
		pData.Remove("type");
		pData.Remove("designable");
		foreach (string s in pData.Keys)
			Debug.LogError("unknown key "+s+" in "+name);
#endif		
	}

	/// <summary>
	/// Возвращает тип подвески по имени
	/// </summary>
	public static PropType GetType(string pName)
	{
		try
		{
			return (PropType) System.Enum.Parse (typeof(PropType), pName.Replace("-","_"));
		}
		catch 
		{
			Debug.LogError("unknown type "+ pName);
		}
		return PropType.None;
	}

	/// <summary>
	/// Набор параметров для сравнения и отображения
	/// </summary>
	public override List<StatClass> stats ()
	{
		List<StatClass> stats = new List<StatClass>();
		foreach (PropEnums e in System.Enum.GetValues(typeof(PropEnums)))
		{
			if (values.ContainsKey(e))
				stats.Add(new StatClass (e.ToString (), values [e], maxValues [e]));
		}
		return stats;
	}

	public override string ToString ()
	{
		string v = "specific\n ptype: ["+ptype.ToString()+"]\n";
		foreach (PropEnums e in System.Enum.GetValues(typeof(PropEnums))) 
		{
			if (values.ContainsKey (e))
				v += " "+e.ToString () + ": [" + values [e].ToString () + "]\n";
		}
		return base.ToString() + v;
	}

	/// <summary>
	/// Значение параметра
	/// </summary>
	public int val(PropEnums pType)
	{
		if (values.ContainsKey (pType))
			return values [pType];
		return 0;
	}
}
