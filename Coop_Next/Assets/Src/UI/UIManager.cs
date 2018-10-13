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

    string prefix = "prefabs/UI/";

    string[] preloadUI =
    {
        "InGameLevelUpPanel"
    };

    Dictionary<string, GameObject> UIPanels = new Dictionary<string, GameObject>();

    public void Initialize()
    {
        UIRoot = GameObject.FindObjectOfType<Canvas>().gameObject;
        if (UIRoot != null)
            inputConfig.menuButton = InputAxisEnum.Menu.Value;

        for (int i = 0; i < preloadUI.Length; i++)
        {
            GameObject panel = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(prefix + preloadUI[i]));
            panel.SetActive(false);
            panel.transform.parent = UIRoot.transform;
            panel.GetComponent<RectTransform>().localPosition = Vector3.zero;
            UIPanels.Add(preloadUI[i], panel);
        }
    }

    public void ActiveUIPanel(string name, bool stopGame)
    {
        if (UIPanels.ContainsKey(name) && UIPanels[name] != null)
        {
            bool isActive = UIPanels[name].activeSelf;
            UIPanels[name].SetActive(!isActive);
        }
        if (stopGame)
        {
            UpdateManager.isActive = false;
            Time.timeScale = 0;
        }

    }

    public void HideUIPanel(string name, bool restoreGame)
    {
        if (UIPanels.ContainsKey(name) && UIPanels[name] != null)
        {
            UIPanels[name].SetActive(false);
        }
        if (restoreGame)
        {
            UpdateManager.isActive = true;
            Time.timeScale = 1;
        }
    }

    public void Destory()
    {

    }
    
    public void Update()
    {
    //    if (Input.GetButtonDown(inputConfig.menuButton) ||
    //Input.GetButtonDown(inputConfig.menuButton))
    //    {
    //        if (!UIPanels["InGameLevelUpPanel"].activeSelf)
    //            ActiveUIPanel("InGameLevelUpPanel", true);
    //        else
    //            HideUIPanel("InGameLevelUpPanel", true);
    //    }
    }
}
