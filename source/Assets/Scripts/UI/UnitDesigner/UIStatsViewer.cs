using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Отображение статов элемента
/// </summary>
public class UIStatsViewer : MonoBehaviour 
{
	public GroupLayoutPool pool;
	StatsBase stats = null;
	public StatType type;
	public Text caption;

	public void Init(StatType pType, string pID)
	{
		StopAllCoroutines ();
		type = pType;
		stats = StatsBase.Get(pType);
		BaseDataClass data = stats.GetData (pID);
		if (data == null)
			Debug.LogError ("model "+pType.ToString()+": " + pID + " not found");

		Debug.Log (data.ToString ());

		pool.Clear ();
		List<StatClass> datas = data.stats ();

		caption.text = type.ToString()+": "+data.name;

		foreach (StatClass s in datas) 
		{
			Sprite spr = stats.icon (s.name);
			if (spr != null)
			{
				UIValueSlider sld = pool.InstantiateElement (true).GetComponent<UIValueSlider> ();
				s.icon = spr;
				sld.Init (s);
			}
		}
		StartCoroutine (resize ());
	}

	IEnumerator resize()
	{
		yield return null;//new WaitForSeconds(0.5f);
		RectTransform rt = GetComponent<RectTransform> ();
		float sz = pool.GetComponent<RectTransform> ().rect.height + 25;
		if (sz < 120)
			sz = 120;
		rt.sizeDelta = new Vector2 (rt.sizeDelta.x, sz);
	}
}
