using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUnitDesigner : WindowBase 
{
	[SerializeField]
	private Button closeButton;

	/// <summary>
	/// Заголовок
	/// </summary>
	[SerializeField]
	private Text Caption;

    private TemplateClass myTemplate;

    private void OnEnable() 
	{
		closeButton.onClick.AddListener( ()=> { base.Close(); });
	}

	protected override void  OnDisable()
	{
		closeButton.onClick.RemoveAllListeners();

		base.OnDisable();
	}

	public void Init (TemplateClass pTemplate)
	{
        myTemplate = pTemplate;
        Caption.text = string.Format("{0} [{1}]", pTemplate.name, pTemplate.id);
	}

}
