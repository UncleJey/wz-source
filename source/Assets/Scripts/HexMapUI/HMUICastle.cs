using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HMUICastle : WindowBase
{
    [SerializeField]
    private Button closeButton;
    static Castle myCastle;

    public static void Show(Castle pCastle)
    {
        myCastle = pCastle;
        HMUIManager.GetWindow<HMUICastle>().Open();
    }

    protected override void Awake()
    {
        base.Awake();
        closeButton.onClick.AddListener(()=> { base.Close(); });
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
    }

}
