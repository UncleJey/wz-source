using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour, iSelector
{
    #region iSelector
    /// <summary>
    /// Выбрать
    /// </summary>
    public void Select()
    {
        HMUICastle.Show(this);
    }

    /// <summary>
    /// Отменить выбор
    /// </summary>
    public void DeSelect()
    {
        HMUIManager.GetWindow<HMUICastle>().Close();
    }
    #endregion iSelector
}
