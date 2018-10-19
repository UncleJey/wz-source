using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateListViewer : MonoBehaviour 
{
	[SerializeField]
	private GroupLayoutPool pool;

	private void Start() {
		pool.Clear();
		for (int i=0; i<30; i++)
		{
			TemplateClass t = Templates.Get(i);
			if (t != null)
			{
				TemplateStatViewer sv = pool.InstantiateElement().GetComponent<TemplateStatViewer>();
				sv.Init(t);
			} else 
			Debug.Log(i+" null ");
		}
	}

}
