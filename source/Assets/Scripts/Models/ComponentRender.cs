using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentRenderer : MonoBehaviour
{
    /// <summary>
    /// основная моделька 
    /// </summary>
	public BodyRender fireWork;
    /// <summary>
    /// Основа модельки
    /// </summary>
	public BodyRender fireBase;

	/// <summary>
	/// Тип элемента
	/// </summary>
	public StatType type;

	public void Init(BaseDataClass pData, StatType pType)
	{
        type = pType;

        if (pType == StatType.Propulsion)
        {
            PropulsionClass prop = pData as PropulsionClass;
            if ((prop != null) && (prop.ptype == PropType.Tracked || prop.ptype == PropType.Wheeled || prop.ptype == PropType.Half_Tracked))
            {
                pData.mountModel = pData.model;
            }
        }

		if (!string.IsNullOrEmpty (pData.model))
        {
            if (fireWork == null)
            {
                fireWork = AddObject("worker");
            }
			fireWork.DoRender (pData.model, UnitConfig.scale);
        }
		else if (fireWork != null)
        {
			fireWork.Clear (true);
        }

		if (!string.IsNullOrEmpty (pData.mountModel))
        {
            if (fireBase == null)
            {
                fireBase = AddObject("base");
            }
			fireBase.DoRender (pData.mountModel, UnitConfig.scale);
            if (pType == StatType.Propulsion)
            {
                fireBase.gameObject.transform.localRotation = Quaternion.Euler(0,180,0);
            }
        }
		else if (fireBase != null)
        {
			fireBase.Clear (true);
        }
	}

    /// <summary>
    /// Точка крепления
    /// </summary>
    public Vector3 connector
    {
        get
        {
            return transform.localPosition / UnitConfig.scale;
        }
        set
        {
            transform.localPosition = value * UnitConfig.scale;
        }
    }

    /*
     * Добавляет элемент конструкции
     */
    BodyRender AddObject(string pName)
    {
        GameObject obj = new GameObject();

        obj.name = pName;
        obj.transform.SetParent(gameObject.transform);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;

        BodyRender ren = obj.AddComponent<BodyRender>();
        return ren;
    }

	public void Clear()
	{
		fireWork.Clear (true);
		fireBase.Clear (true);
	}

}
