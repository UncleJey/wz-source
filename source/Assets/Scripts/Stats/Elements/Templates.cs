using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TemplateIcons
{
#if TemplateY_EDITOR
	public string name;
#endif
	public TemplateEnums type;
	public Sprite sprite;
}

/*
* Список шаблонов
*/
public class Templates : StatsBase 
{
	private static Dictionary<string, TemplateClass> props = new Dictionary<string, TemplateClass> ();
	private static Templates instance;
	[SerializeField]
	private TemplateIcons[] icons;

	public override void Awake()
	{
		instance = this;
		props.Clear ();
		type = StatType.Templates;
		bases [type] = this;

		stats = Stats.GetStat (type);	
		foreach (string s in stats.Keys) 
		{
			JsonObject stat = stats[s] as JsonObject;
			TemplateClass prp = new TemplateClass (stat);
			props[s] = prp;
		}
		base.Awake();
	}

	public override Sprite icon (string pName)
	{
		foreach (TemplateIcons i in icons)
			if (i.type.ToString ().Equals (pName))
				return i.sprite;
		return null;
	}

	/// <summary>
	/// Найти экземпляр данных
	/// </summary>
	/// <param name="pClassName">P class name.</param>
	public static TemplateClass Get(string pClassName)
	{
		if (props.ContainsKey (pClassName))
			return props [pClassName];
		return null;
	}

	/// <summary>
	/// Найти экземпляр данных для отображения
	/// </summary>
	/// <returns>The data.</returns>
	public override BaseDataClass GetData (string pClassName)
	{
		return Get (pClassName) as BaseDataClass;
	}

	/// <summary>
	/// Список доступных классов для использования
	/// </summary>
	public override List<BaseDataClass> GetList ()
	{
		List<BaseDataClass> list = new List<BaseDataClass> ();
		foreach (TemplateClass u in props.Values)
			//TODO: ДОбавить условия
			list.Add (u);

		return list;
	}
}
