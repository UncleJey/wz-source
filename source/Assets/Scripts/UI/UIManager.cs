using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour 
{
	public static UIManager instance;

	public ToggleButton briefengButton;
	public ToggleButton designButton;
	public ToggleButton buildButton;
	public ToggleButton researchdButton;
	public ToggleButton produceButton;
	public ToggleButton commandButton;
	public ToggleButton mapButton;

	void Awake () 
	{
		instance = this;

	}

	void OnDestroy()
	{
	}

}
