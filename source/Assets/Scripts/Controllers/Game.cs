using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private float quality = 0.75f;
    public Camera camera;
    public float Quality
    {
        get { return quality; }
        set
        {
            quality = value;
            if (camera.targetTexture != null)
            {
                camera.targetTexture.width = Mathf.CeilToInt(Screen.width * quality);
                camera.targetTexture.height = Mathf.CeilToInt(Screen.height * quality);
            }
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = 30;
        QualitySettings.maxQueuedFrames = 3;
        Quality = quality;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
