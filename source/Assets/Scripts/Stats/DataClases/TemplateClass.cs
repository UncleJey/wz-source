using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TemplateEnums : byte
{
	None				= 0  //Undefined
}

/// <summary>
///  Структура данных Юнита
/// </summary>
public class TemplateClass : BaseDataClass
{
	public string body; 		// Body5REC
	public string propulsion;	// tracked01
	public string repair;		// LightRepair1
	public string construct;	// LightRepair1
	public string brain;		// CommandBrain01
	public bool available;		// true
	public string sensor;
	public string[] weapons;

	public DroidType dtype;		// DROID

	public TemplateClass()
	{}

	public TemplateClass(JsonObject pData)
	{
		Init (pData);
	}

	public override void Init(JsonObject pData)
	{
		base.Init (pData);
		type = StatType.Templates;
		dtype = BodyClass.GetType(pData.Get<string> ("type","None"));
		body = pData.Get<string> ("body", "");
		propulsion = pData.Get<string> ("propulsion", "");
		repair = pData.Get<string> ("repair", "");
		construct = pData.Get<string> ("construct", "");
		brain = pData.Get<string> ("brain", "");
		available = pData.Get<bool> ("available", false);
		sensor = pData.Get<string> ("sensor", "");

		if (pData.ContainsKey ("weapons")) 
		{
			JsonArray _weapons = (JsonArray) pData ["weapons"];
			if (_weapons != null && _weapons.Count > 0) 
			{
				weapons = new string[_weapons.Count];
				for (int i = _weapons.Count - 1; i >= 0; i--)
					weapons [i] = _weapons [i].ToString ();
			}
		}

#if CHECK_VARIABLES
		pData.Remove("id");
		pData.Remove("name");
		pData.Remove("model");
		pData.Remove("type");
		pData.Remove("body");
		pData.Remove("propulsion");
		pData.Remove("repair");
		pData.Remove("weapons");
		pData.Remove("construct");
		pData.Remove("brain");
		pData.Remove("available");
		pData.Remove("sensor");
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
		//TODO: Сделать сбор параметров
		return stats;
	}

	/// <summary>
	/// Получить модель оружия по номеру
	/// </summary>
	public string GetWeapon(int pNum)
	{
		if (weapons == null || weapons.Length <= pNum)
			return string.Empty;
		return weapons [pNum];
	}
}
