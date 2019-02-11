using UnityEngine;
using UnityEngine.UI;

public class UIInfoBlock : MonoBehaviour
{
    [SerializeField]
    private GroupLayoutPool btnsPool;

    private iInfoComponent[] infos;

    private TemplateClass myTemplate;

    [SerializeField]
    private Sprite selectedSprite;
    [SerializeField]
    private Sprite normalSprite;

    void Awake()
    {
        infos = GetComponentsInChildren<iInfoComponent>(true);
    }

    private void Start()
    {
        btnsPool.Clear();
        Debug.Log("found panels: " + infos.Length.ToString());
        for (int i=infos.Length-1; i>=0; i--)
        {
            Button b = btnsPool.InstantiateElement(true).GetComponent<Button>();
            b.name = "btn_" + i.ToString();
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(()=> { onSelectBtnClick(b.name); });
        }
        selectCMP(infos[0], "btn_0");
    }

    /// <summary>
    /// По клику на кнопку вкладки
    /// </summary>
    private void onSelectBtnClick(string pName)
    {
        int cNo = 0;
        string[] nm = pName.Split('_');

        if (nm.Length == 2)
        {
            int.TryParse(nm[1], out cNo);

            if (cNo >= 0 && cNo < infos.Length)
            {
                selectCMP(infos[cNo], pName);
            }
        }
    }

    /// <summary>
    /// Активация нужного компанента
    /// </summary>
    void selectCMP(iInfoComponent cmp, string pName)
    {
        // Переключаем вкладки
        for (int i=infos.Length-1; i>=0; i--)
        {
            iInfoComponent c = infos[i];
            if (c.Equals(cmp))
            {
                c.Show(myTemplate);
            }
            else
            {
                c.Hide();
            }
        }
        int e = 0;
        // Переключаем кнопки
        Transform t = btnsPool.getElement(e++);
        while (t != null)
        {
            if (t.name.Equals(pName))
            {
                t.GetComponent<Image>().sprite = selectedSprite;
            }
            else
            {
                t.GetComponent<Image>().sprite = normalSprite;
            }
            t = btnsPool.getElement(e++);
        }
    }

}
