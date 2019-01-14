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
	public bool available;		// true

	/// <summary>
	/// Элементы объекта
	/// </summary>
	public Dictionary<StatType, string> elements;

	public DroidType dtype;		// DROID

	public TemplateClass()
	{}

	public TemplateClass(JsonObject pData)
	{
		Init (pData);
	}

	void AddElement(StatType pType, string pValue)
	{
		if (!string.IsNullOrEmpty(pValue))
		{
			elements[pType] = pValue;
		}
	}

	/// <summary>
	/// Получить элемент по типу
	/// </summary>
	public string GetElement(StatType pType)
	{
		if (elements.ContainsKey(pType))
		{
			return elements[pType];
		}

		return null;
	}

	public override void Init(JsonObject pData)
	{
		base.Init (pData);
		type = StatType.Templates;
		dtype = BodyClass.GetType(pData.Get<string> ("type","None"));
		available = pData.Get<bool> ("available", false);

		elements = new Dictionary<StatType, string>();
		AddElement(StatType.Body, pData.Get<string> ("body", ""));
		AddElement(StatType.Propulsion, pData.Get<string> ("propulsion", ""));
		AddElement(StatType.Repair, pData.Get<string> ("repair", ""));
		AddElement(StatType.Construction, pData.Get<string> ("construct", ""));
		AddElement(StatType.Comp, pData.Get<string> ("brain", "")); //FIXME: наверное то поле
		AddElement(StatType.Sys, pData.Get<string> ("sensor", ""));

		if (pData.ContainsKey ("weapons")) 
		{
			JsonArray _weapons = (JsonArray) pData ["weapons"];
			if (_weapons != null && _weapons.Count > 0) 
			{
				for (int i = _weapons.Count - 1; i >= 0; i--)
				{
					switch (i)
					{
						case 0: AddElement(StatType.Wpn1, _weapons [i].ToString ()); break;
						case 1: AddElement(StatType.Wpn2, _weapons [i].ToString ()); break;
						case 2: AddElement(StatType.Wpn3, _weapons [i].ToString ()); break;
						case 3: AddElement(StatType.Wpn4, _weapons [i].ToString ()); break;
						default: Debug.LogError("too much weapons");break;
					}
				}
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

	/*
	* Инициализированные данные
	*/
	public override string ToString ()
	{
		string str = "\nspecific:\n";
		foreach(KeyValuePair<StatType, string> el in elements)
		{
			str += string.Format(" {0}: [{1}]\n",el.Key.ToString(), el.Value);
		}

		str += string.Format(" dtype:[{0}]\n", dtype.ToString());

		return base.ToString() + str;
	}
}
