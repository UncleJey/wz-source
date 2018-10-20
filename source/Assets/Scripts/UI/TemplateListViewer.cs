using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateListViewer : MonoBehaviour 
{
	[SerializeField]
	private GroupLayoutPool pool;
	Dictionary<DroidType, List<TemplateClass>> datas = new Dictionary<DroidType, List<TemplateClass>>();

	[SerializeField]
	private GroupLayoutPool btnPool;

	private void Start() 
	{
		pool.Clear();
		List <TemplateClass> templs = Templates.Get();
		foreach (TemplateClass t in templs)
		{
			if (!datas.ContainsKey(t.dtype))
				datas[t.dtype] = new List<TemplateClass>();
			datas[t.dtype].Add (t);
		}

		foreach (DroidType t in datas.Keys)
		{
			IconicButton btn = btnPool.InstantiateElement().GetComponent<IconicButton>();
			btn.text.text = t.ToString()+": "+datas[t].Count.ToString();
			btn.droidType = t;
			btn.button.onClick.RemoveAllListeners();
			btn.button.onClick.AddListener(() =>{OnTypeBtnClick(t);});
			btn.icon.sprite = Icons.Get(t);
		}
	}

	void OnTypeBtnClick(DroidType pType)
	{
		Debug.Log(pType);
	}
}
