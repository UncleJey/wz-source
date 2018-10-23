using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TemplateStatViewer : MonoBehaviour
{
	[SerializeField]
	private Text value;
	[SerializeField]
	private Image image;
	[SerializeField]
	private Button button;

	TemplateClass template;

	/// <summary>
	/// Глобальное событие - выбран шаблон
	/// </summary>
	public static System.Action<TemplateClass> templateChoosen;

	private void Awake() {
		button.onClick.AddListener(btnClick);
	}

	public void Init(TemplateClass pTemplate)
	{
		template = pTemplate;
		image.sprite = null;
		value.text = string.Format("{0} [{1}]", pTemplate.name, pTemplate.id);
	}

	void btnClick()
	{
		if (templateChoosen != null)
			templateChoosen(template);
	}
}
