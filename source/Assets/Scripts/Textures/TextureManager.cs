using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureManager : MonoBehaviour 
{
	public GameObject bodyItemPrefab;

	static TextureManager instance;
	static Dictionary<string, Material> materials = new Dictionary<string, Material>();
	public Material defaultMaterial;

	public Sprite[] images;

	void Awake() 
	{
		instance = this;
	}

	/// <summary>
	/// Заготовка элемента объекта
	/// </summary>
	public static GameObject BodyItemPrefab
	{
		get 
		{
			return instance.bodyItemPrefab;
		}
	}


	/// <summary>
	/// Возвращает спрайт по имени
	/// </summary>
	public static Sprite GetSprite(string pName)
	{
		pName = pName.ToLower().Trim();
		for (int i=instance.images.Length-1; i>=0; i--)
			if (instance.images[i].name == pName)
				return instance.images[i];

		return null;
	}

	/// <summary>
	/// Возвращает материал с нужным атласом
	/// </summary>
	public static Material GetMaterial(string pName)
	{
		if (materials.ContainsKey (pName))
			return materials[pName];
		Debug.Log("material: "+pName);
		
		Texture tex = Resources.Load<Texture> ("texpages/" + pName.Replace(".png",""));

		Material m = (Material) Instantiate (instance.defaultMaterial);
		m.mainTexture = tex;

		materials.Add (pName, m);

		return m;
	}
}
