using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Тип Дроида к которому применим корпус
/// </summary>
public enum DroidType : byte
{
	 None				= 0
	,PERSON				= 1
	,CYBORG				= 2
	,CYBORG_SUPER		= 3
	,TRANSPORTER		= 4
	,DROID				= 5
	,CYBORG_CONSTRUCT	= 6
	,CYBORG_REPAIR		= 7
	,SUPERTRANSPORTER	= 8
}

/// <summary>
/// Типы численных переменных
/// </summary>
public enum BodyEnums : byte
{
	 None			= 0
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

	,armourHeat		= 5 //1
	,armourKinetic	= 6 //1
	,powerOutput	= 7 //125
	,weaponSlots	= 8 //1
}

/*
* Данные корпуса
*/
public class BodyClass : BaseDataClass 
{
	public Dictionary<BodyEnums, int> values = new Dictionary<BodyEnums, int>();
	public static Dictionary<BodyEnums, int> maxValues = new Dictionary<BodyEnums, int>();

	public string _class; //"Babas"
	public DroidType droidType; //"PERSON"
	public string size;	//"LIGHT",
	/// <summary>
	/// Модели для разных подвесок
	/// </summary>
	public Dictionary<string, PropModels> propulsionExtraModels = new Dictionary<string, PropModels>();

	public BodyClass()
	{
	}

	public BodyClass(JsonObject pData)
	{
		Init (pData);
	}

	public override void Init(JsonObject pData)
	{
		base.Init (pData);
		type = StatType.Body;
		_class = pData.Get<string> ("class", "");
		size = pData.Get<string> ("size", "");

		values.Clear ();
		foreach (BodyEnums e in System.Enum.GetValues(typeof(BodyEnums)))
		{
			if (pData.ContainsKey (e.ToString ())) 
			{
				int v = pData.Get<int> (e.ToString ());
				values [e] = v;
				if (!maxValues.ContainsKey (e) || maxValues [e] < v)
					maxValues [e] = v;
#if CHECK_VARIABLES
				pData.Remove(e.ToString());
#endif
			}
		}		

		droidType = GetType (pData.Get<string>("droidType","None"));
		propulsionExtraModels.Clear ();
		if (pData.ContainsKey ("propulsionExtraModels")) 
		{
			JsonObject mdls = pData["propulsionExtraModels"] as JsonObject;
			foreach(string k in mdls.Keys)
			{
				//PropType p = PropulsionClass.GetType (k);
				propulsionExtraModels[k] = new PropModels(mdls[k] as JsonObject);
			}
#if CHECK_VARIABLES
			pData.Remove("propulsionExtraModels");
#endif
		}

#if CHECK_VARIABLES
		pData.Remove("id");
		pData.Remove("name");
		pData.Remove("model");
		pData.Remove("droidType");
		pData.Remove("size");
		pData.Remove("class");
		pData.Remove("designable");
		foreach (string s in pData.Keys)
			Debug.LogError("unknown key "+s+" in "+name);
#endif	
	}

	/// <summary>
	/// Набор параметров для сравнения и отображения
	/// </summary>
	public override List<StatClass> stats ()
	{
		List<StatClass> stats = new List<StatClass>();
		foreach (BodyEnums e in System.Enum.GetValues(typeof(BodyEnums)))
		{
			if (values.ContainsKey(e) && values[e] != 0)
				stats.Add(new StatClass (e.ToString (), values [e], maxValues [e]));
		}
		return stats;
	}

	/// <summary>
	/// Модель для конктретного шасси
	/// </summary>
	public PropModels GetExtraModel(string pClassName)
	{
		if (propulsionExtraModels != null && propulsionExtraModels.ContainsKey (pClassName))
			return propulsionExtraModels [pClassName];
		return null;
	}

	public static DroidType GetType(string pName)
	{
		try
		{
			return (DroidType) System.Enum.Parse (typeof(DroidType), pName.Replace("-","_").Replace(" ","_"));
		}
		catch 
		{
			Debug.LogError("unknown type "+ pName);
		}
		return DroidType.None;
	}

	public override string ToString ()
	{
		string v = string.Format("id: {0}, name: {1}, model: {2}, mount: {3}\r\n",id,name,model,mountModel);
		foreach (BodyEnums e in System.Enum.GetValues(typeof(BodyEnums))) 
		{
			if (values.ContainsKey (e))
				v += e.ToString () + ": " + values [e].ToString () + "\r\n";
		}
		return v;
	}
}
