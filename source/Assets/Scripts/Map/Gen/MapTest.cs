using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapTest : MonoBehaviour
{
    public Button test;
    public Text text;
    public int w=16,h=16,p = 50;
    /*
    private void Awake()
    {
        test.onClick.AddListener(()=> { Start(); } );
    }
    */
    void Start()
    {
        MapHandler mh = new MapHandler(w,h,p);
        text.text = mh.MapToString();
    }

}
