using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HMUIManager : MonoBehaviour 
{
	static HMUIManager instance;

	void Awake () 
	{
		instance = this;
		WindowBase.InitElements(this);
	}

	void OnDestroy()
	{
	}

#region windows
	/// <summary>
	/// Открыть окно
	/// </summary>
	public static T Open<T>() where T : WindowBase
	{
		T wnd = GetWindow<T>();
		if (!wnd.isActiveAndEnabled)
			wnd.Open();
		return wnd;
	}

	/// <summary>
	/// Возвращает окно
	/// </summary>
	public static TWindowBase GetWindow<TWindowBase>() where TWindowBase: WindowBase
	{
		return GUIElement<WindowBase>.GetElement<TWindowBase>();
	}
#endregion windows

}
