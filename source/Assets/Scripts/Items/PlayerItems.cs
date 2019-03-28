using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Имена итемов
public enum ItemName : int 
{
	NONE=0,			// Не задано
	COIN=1,			// Монеты
	STONE=2,		// Камень
	FRUIT=3,		// Фрукт
	CHECKPOINT=4	// Контрольная точка
}

//Защита против взлома памяти
[System.Serializable]
public class counter {
	private int cnt1;
#if UNITY_EDITOR
	[SerializeField]
	public string
		myName;
#endif
	[SerializeField]
	private int
		Value;	//Используется только в инспекторе для настройки начального значения

	public counter() {
		cnt1 = 0;
		Value = 0;
	}

	public counter(int cnt) {
		count = cnt;
	}

	public int count {
		set {
			cnt1 = Random.Range(10, 100);
			Value = value - cnt1;
		}
		get {
			return Value + cnt1;
		}
	}
	
	public void Add(int cnt) {
		Value += cnt;
	}
	
	public void Dec(int cnt) {
		Value -= cnt;
	}

	public static int BoostValue(int value, float boost) {
		if(boost > 0f) {
			return value + (int)(value * boost);
		}
		return value;
	}

	public static counter[] BoostItems(counter[] items, float boost, bool cloneItems=true) {
		if(items == null || items.Length == 0 || boost <= 0) {
			return items;
		}
		counter[] boostedItems;
		if(cloneItems) {
			boostedItems = (counter[])items.Clone();
		}
		else {
			boostedItems = items;
		}
		
		for(int i=0; i<boostedItems.Length; i++) {
			boostedItems [i].count = boostedItems [i].count + (int)(boostedItems [i].count * boost);
		}

		return boostedItems;
	}
#if UNITY_EDITOR
	public void SetValue(int pVal)
	{
		cnt1 = 0;
		Value = pVal;
	}
#endif
}

//Базовый класс для итема
[System.Serializable]
public class ItemType : counter 
{
	[SerializeField]
	public ItemName	name;			// Ресурс

	public override bool Equals(object obj) {
		return this.name == (obj as ItemType).name;
	}

	public ItemType Clone() {
		return new ItemType(name, count);
	}

	public string SaveName {
		get {
			return ((int)name).ToString();
		}
	}

	public ItemType(ItemName pName, int pCount) {
		name = pName;
		count = pCount;
	}

	public static bool IsJsonDataKeyValid(string data) {
		return data == "c";
	}

	public Sprite sprite;
	public ItemType() {}
}

public class PlayerItems : MonoBehaviour 
{
#if UNITY_EDITOR
	[ContextMenuItem("Export Names", "ExportNames")]
#endif
	[SerializeField]
	ItemType[] items;		// Итемы
	static PlayerItems instance;

	void Awake()
	{
		instance = instance ?? this;
	}

	public static ItemType getItem(ItemName pName)
	{
		foreach (ItemType t in instance.items)
			if (t.name == pName)
				return t;

		return null;
	}

	public static int AddItem(ItemName pName, int pCount = 1)
	{
//		Debug.Log ("add " + pName.ToString () + " " + pCount.ToString ());
		ItemType itm = getItem (pName);
		itm.count += pCount;

		Game.updateCounts (itm.name, itm.count);
		return itm.count;
	}

	public static int itemCount(ItemName pName)
	{
		foreach (ItemType t in instance.items)
			if (t.name == pName)
				return t.count;
		return 0;
	}
}