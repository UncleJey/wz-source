using UnityEngine;

public class Angler : MonoBehaviour
{
    void Update()
    {
        Vector3 ang = transform.localEulerAngles;
        ang.y = 0;
        transform.localEulerAngles = ang;
    }
}