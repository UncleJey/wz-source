using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatClass
{
	public string name;
	public int value;
	public int MaxValue;
	public Sprite icon;

	public StatClass(string pName, int pVal, int pMax)
	{
		name = pName;
		value = pVal;
		MaxValue = pMax;
	}
}

/*
* Базовый класс для данных
*/
public abstract class BaseDataClass
{
	/// <summary>
	/// Full / real name of the item
	/// </summary>
	public string name;			//"BaBaLegs",
	/// <summary>
	/// Text id (i.e. short language-independant name) 
	/// </summary>
	public string id;			//"BaBaLegs",
	/// <summary>
	/// PIE model file name
	/// </summary>
	public string model;		//"PRLRHTR1.PIE"
	/// <summary>
	/// PIE base model file name
	/// </summary>
	public string mountModel;
	/// <summary>
	/// flag to indicate whether this component can be used in the design screen
	/// </summary>
	public bool designable = false;
	/// <summary>
	/// Тип данных
	/// </summary>
	public StatType type;

	public abstract List<StatClass> stats();

	public virtual void Init(JsonObject pData)
	{
		name = pData.Get<string> ("name", "");
		id = pData.Get<string> ("id", "");
		model = pData.Get<string> ("model", pData.Get<string> ("sensorModel", ""));
		mountModel = pData.Get<string> ("mountModel", "");
		designable = pData.Get<int> ("designable", 0) == 1;
	}

}
