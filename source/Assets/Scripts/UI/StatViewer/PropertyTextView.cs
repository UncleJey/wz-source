using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertyTextView : MonoBehaviour 
{

	[SerializeField]
	private Text caption;
	[SerializeField]
	private Text value;
	[SerializeField]
	private Image image;

	public void Init(Sprite pIcon, string pCaption, string pValue)
	{
		caption.text = pCaption;
		image.sprite = pIcon;
		value.text = pValue;
	}

}
