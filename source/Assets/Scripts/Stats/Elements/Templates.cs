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
	private static List<TemplateClass> props = new List<TemplateClass> ();
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
			props.Add(prp);
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
		return props.Find(p=> p.id.Equals(pClassName));
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public static List<TemplateClass> Get(DroidType pType = DroidType.None)
	{
		if (pType == DroidType.None)
			return props;
		else
			return props.FindAll(p => p.dtype.Equals(pType));
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
		List<BaseDataClass> res = new List<BaseDataClass>();
		foreach (TemplateClass t in props)
			res.Add(t as BaseDataClass);
		return res;
	}
}
