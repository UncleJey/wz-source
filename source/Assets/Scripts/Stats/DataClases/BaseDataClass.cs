using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Базовый клас статов для численных величин
/// </summary>
public class StatClass
{
	/// <summary>
	/// Название
	/// </summary>
	public string name;
	/// <summary>
	/// Значение
	/// </summary>
	public int value;
	/// <summary>
	/// Максимально возможное значение
	/// </summary>
	public int MaxValue;
	/// <summary>
	/// Иконка
	/// </summary>
	public Sprite icon;

	/// <summary>
	/// Конструктор
	/// </summary>
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
	/// <summary>
	/// Порядковый номер в общем массиве для последовательной выборки
	/// </summary>
	public int index;
	
	public abstract List<StatClass> stats();

	public virtual void Init(JsonObject pData)
	{
		name = pData.Get<string> ("name", "");
		id = pData.Get<string> ("id", "");
		model = pData.Get<string> ("model", pData.Get<string> ("sensorModel", ""));
		mountModel = pData.Get<string> ("mountModel", "");
		designable = pData.Get<int> ("designable", 0) == 1;

#if CHECK_VARIABLES
		pData.Remove("id");
		pData.Remove("name");
		pData.Remove("model");
		pData.Remove("mountModel");
		pData.Remove("designable");
#endif
	}

	/*
	* Инициализированные данные
	*/
	public override string ToString ()
	{
		return string.Format("\nbase\n name: [{0}]\n id: [{1}]\n model: [{2}]\n mountModel: [{3}]\n designable: [{4}]\n type: [{5}]", name, id, model, mountModel, designable, type.ToString());
	}

}
