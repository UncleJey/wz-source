using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UISlider : MonoBehaviour 
{

	public int maxValue;
	public bool itsTime;
	public int padding = 4;

	public RectTransform table;
	public RectTransform slider;
	public Text text;

	float _value = 0;
	public float Value
	{
		get
		{
			return _value;
		}
		set
		{
			slider.sizeDelta = new Vector2((table.sizeDelta.x - padding * 2) * value / maxValue, slider.sizeDelta.y);
			text.text = value.ToString();
			_value = value;
		}
	}

}
