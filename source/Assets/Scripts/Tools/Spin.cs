using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Spin : MonoBehaviour
{
	public float speed = 1;

	void Update ()
	{
		transform.Rotate (0, speed * Time.deltaTime, 0, Space.Self);
	}
}
