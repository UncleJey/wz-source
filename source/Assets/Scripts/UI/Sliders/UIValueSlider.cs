using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIValueSlider : MonoBehaviour 
{
	public Image sprite;
	public Text val;
	public Slider slider;

	StatClass myStat;

	public void Init(StatClass pStat)
	{
		myStat = pStat;
		slider.minValue = 0;
		slider.maxValue = pStat.MaxValue;
		slider.value = pStat.value;

		val.text = pStat.value.ToString ();
		sprite.sprite = pStat.icon;
		sprite.SetNativeSize ();
	}

}
