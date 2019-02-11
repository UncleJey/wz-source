using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Используется совместно с одним из лейаутов. Инстанцирует и создаёт элементы лейаута через пул.
/// </summary>
public class GroupLayoutPool : MonoBehaviour 
{
	[SerializeField]
	private Transform elementPrefab;
	private List<Transform> activeElements = new List<Transform>();
	private Stack<Transform> ElementsStack = new Stack<Transform>(); 

	void Awake()
	{
		if (elementPrefab!= null && elementPrefab.transform.parent == transform)	// Если префаб лежит внутри
			DestroyElement(elementPrefab);
	}

	/// <summary>
	/// Инстанцирует элемент с префаба или достаёт из пула.
	/// </summary>
	/// <returns></returns>
	public Transform InstantiateElement(bool gameObjActive = true) 
	{
        Transform element;
		if (ElementsStack.Count > 0) 
		{
			element = ElementsStack.Pop();
		}
		else 
		{
			element = Instantiate<Transform>(elementPrefab);
			element.SetParent(this.transform);
		}
		element.gameObject.SetActive(gameObjActive);
		activeElements.Add(element);
		element.SetAsLastSibling();
		element.localScale = Vector3.one;
		element.localPosition = elementPrefab.localPosition;

		return element;
	}

	/// <summary>
	/// Деактивирует элемент и помещает его в пул доступных объектов.
	/// </summary>
	public void DestroyElement(Transform element) 
	{
		ElementsStack.Push(element);
		activeElements.Remove(element);
		element.gameObject.SetActive(false);
	}

	/// <summary>
	/// Получить активный элемент с порядковым номером.
	/// </summary>
	public Transform getElement(int pNo, bool recreate=false, bool gameObjActive = true)
	{
		if (activeElements.Count > pNo)
			return activeElements[pNo];
		else if (recreate)
			return InstantiateElement(gameObjActive);
		else 
			return null;
	}

	/// <summary>
	/// Деактивирует все элементы
	/// </summary>
	public void Clear() 
	{
		if (activeElements != null)
		{
			while(activeElements.Count > 0)
				DestroyElement(activeElements[0]);
		}

		for (int i = transform.childCount - 1; i >= 0; i--) 
		{
            Transform e = transform.GetChild (i).GetComponent<Transform> ();
			if (e != null && e.gameObject.activeSelf)
				DestroyElement (e);
		}
	}
}
