using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TemplateStatViewer : StatViewerBase
{
	[SerializeField]
	private Text caption;
	[SerializeField]
	private Text value;
	[SerializeField]
	private Image image;

	public void Init(TemplateClass pTemplate)
	{
		caption.text = pTemplate.dtype.ToString() + " : ";
		image.sprite = null;
		value.text = pTemplate.name;
	}

}
