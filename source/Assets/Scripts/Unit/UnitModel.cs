using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
* Моделька
*/
public class UnitModel 
{
   	UnitStats Stats;
    public List<ComponentRenderer> renders = new List<ComponentRenderer> ();
    GameObject gameObject;
    public Vector3[] connectors;
    ComponentRenderer body;

    /*
     * Инициализация
     */
    public void Init(GameObject pObject, UnitStats pStats)
    {
        gameObject = pObject;
        Stats = pStats;
        foreach(KeyValuePair<StatType, BaseDataClass> el in Stats.stats)
		{
			AddObject(el.Key, el.Value);
		}
        OnAfterCreate();
    }

    /*
     * Добавляет элемент конструкции
     */
    public void AddObject(StatType pType, BaseDataClass pData)
    {
#if DEBUG_UNIT
		Debug.Log("add render "+pType.ToString());
#endif
        /*
                if (pType == StatType.Propulsion && body != null)
                {
                    Debug.Log("hav body");
                    pData.mountModel = body.fireWork.data.GetExtraModel(pData.propulsion);
                }
         */
        GameObject obj = new GameObject
        {
            name = pType.ToString()
        };
        obj.transform.SetParent(gameObject.transform);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;

        ComponentRenderer ren = obj.AddComponent<ComponentRenderer>();

        ren.Init(pData, pType);
        renders.Add(ren);

        if (pType == StatType.Body)
        {
            body = ren;
            connectors = body.fireWork.data.connector;
        }
    }

    /*
     * Модификации сразу по создании
     */
    void OnAfterCreate() 
    {
        foreach (ComponentRenderer br in renders)
        {
            if (br.type == StatType.Wpn1 || br.type == StatType.Repair || br.type == StatType.Construction)
            {
                br.connector = Slots.ChooseConnector(connectors, br.type);
            }
        }
    }
}