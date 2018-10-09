using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class TextureHelper : MonoBehaviour 
{
	public static void  SaveTextureToFile(Texture2D texture, string fileName)
	{
		var bytes=texture.EncodeToPNG();
		var file = File.Open(Application.dataPath + "/Resources/"+fileName, FileMode.Create);
		var binary= new BinaryWriter(file);
		binary.Write(bytes);
		file.Close();
	}

}
