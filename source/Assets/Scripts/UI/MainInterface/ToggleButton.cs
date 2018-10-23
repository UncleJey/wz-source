using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour 
{
	public Sprite ActiveSprite;
	public Sprite PassiveSprite;
	public Sprite EmptySprite;

	Image _image;
	public Image image
	{
		get 
		{
			if (_image == null)
				_image = GetComponent<Image> ();
			return _image;
		}
	}

	Button _button;
	public Button button
	{
		get
		{
			if (_button == null)
				_button = GetComponent<Button> ();
			return _button;
		}
	}

	/// <summary>
	/// Инициализация
	/// </summary>
	public void Init(bool isActive)
	{
		if (isActive)
			image.sprite = ActiveSprite;
		else
			image.sprite = EmptySprite;

		button.interactable = isActive;
	}

	public void Highlight(bool isActive)
	{
		if (isActive)
			image.sprite = ActiveSprite;
		else
			image.sprite = PassiveSprite;

		button.interactable = true;
	}
}
