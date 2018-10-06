using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public struct UIInputConfig
    {
        public string menuButton;
    };

    GameObject UIRoot = null;
    UIInputConfig inputConfig;

    string[] preloadUI =
    {
        "prefabs/UI/InGameLevelUpPanel"
    };

    Dictionary<string, GameObject> UIPanels = new Dictionary<string, GameObject>();

    public void Initialize()
    {
        UIRoot = GameObject.FindObjectOfType<Canvas>().gameObject;
        if (UIRoot != null)
            inputConfig.menuButton = InputAxisEnum.Menu.Value;

        for (int i = 0; i < preloadUI.Length; i++)
        {
            GameObject panel = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(preloadUI[i]));
            panel.SetActive(false);
            panel.transform.parent = UIRoot.transform;
            panel.GetComponent<RectTransform>().localPosition = Vector3.zero;
            UIPanels.Add(preloadUI[i], panel);
        }
    }

    public void ActiveUIPanel(string name)
    {
        if (UIPanels.ContainsKey(name) && UIPanels[name] != null)
        {
            bool isActive = UIPanels[name].activeSelf;
            UIPanels[name].SetActive(!isActive);
        }
    }

    public void HideUIPanel(string name)
    {
        if (UIPanels.ContainsKey(name) && UIPanels[name] != null)
        {
            UIPanels[name].SetActive(false);
        }
    }

    public void Destory()
    {

    }

    public void Update()
    {
        if (Input.GetButtonDown(inputConfig.menuButton) ||
    Input.GetButtonDown(inputConfig.menuButton))
        {
            UpdateManager.isActive = !UpdateManager.isActive;
            ActiveUIPanel("prefabs/UI/InGameLevelUpPanel");
        }
    }
}
