using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class DTypeIcon
{
	public string caption;
	public DroidType dType;
	public Sprite icon;
}

public class Icons : MonoBehaviour 
{
	private static Icons instance;
	[SerializeField]
	private DTypeIcon[] dTypes;

	private void Awake() {
		instance = this;
	}

	/// <summary>
	/// Иконка для типа дроида
	/// </summary>
	public static Sprite Get(DroidType pType) {
		DTypeIcon ic = System.Array.Find(instance.dTypes, t => t.dType.Equals(pType));
		if (ic != null)
			return ic.icon;
		else
			return null;
	}

}
