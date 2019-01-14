using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
* Моделька
*/

public class UnitModel {
   	UnitStats Stats;
    public List<BodyRender> renders = new List<BodyRender> ();
    GameObject gameObject;

    public void Init(GameObject pObject, UnitStats pStats)
    {
        gameObject = pObject;
        Stats = pStats;
        foreach(KeyValuePair<StatType, BaseDataClass> el in Stats.stats)
		{
			AddObject(el.Key, el.Value);
		}

    }

    public void AddObject(StatType pType, BaseDataClass pData)
    {
#if DEBUG_UNIT
		Debug.Log("add render "+pType.ToString());
#endif
        GameObject obj = new GameObject();
        BodyRender ren = obj.AddComponent<BodyRender>();
        ren.DoRender(pData.model);

        obj.name = pType.ToString();
        obj.transform.SetParent(gameObject.transform);
        obj.transform.localScale = Vector3.one;
    }
}