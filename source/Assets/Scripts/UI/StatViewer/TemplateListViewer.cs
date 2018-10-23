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
		List <TemplateClass> templs = Templates.Get();
		foreach (TemplateClass t in templs)
		{
			if (!datas.ContainsKey(t.dtype))
				datas[t.dtype] = new List<TemplateClass>();
			datas[t.dtype].Add (t);
		}

		btnPool.Clear();
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

	private void OnEnable() 
	{
		TemplateStatViewer.templateChoosen += onTemplateChoosen;
	}

	private void OnDisable() 
	{
		TemplateStatViewer.templateChoosen -= onTemplateChoosen;
	}

	void onTemplateChoosen(TemplateClass pTemplate)
	{
		Debug.Log(pTemplate.ToString());
	}

	void OnTypeBtnClick(DroidType pType)
	{
		fillTemplates(pType);
	}

	void fillTemplates(DroidType pType) 
	{
		pool.Clear();
		List <TemplateClass> templs = Templates.Get(pType);
		foreach (TemplateClass t in templs)
		{
			TemplateStatViewer v = pool.InstantiateElement().GetComponent<TemplateStatViewer>();
			v.Init(t);
		}
	}
}
