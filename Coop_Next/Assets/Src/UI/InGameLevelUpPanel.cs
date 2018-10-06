using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameLevelUpPanel : MonoBehaviour {

    public GameObject checkMark1;
    public GameObject checkMark2;

    public Button selectButton;

    public Text techText1;
    public Text techText2;

    private string[] techName = new string[2];
    private int currentIdx = 0;

    public void InitPanel(string tech1, int tech1Level, string tech2, int tech2Level)
    {
        techText1.text = tech1 + "\n LV " + tech1Level;
        techText2.text = tech2 + "\n LV " + tech2Level;
        techName[0] = tech1;
        techName[1] = tech2;
    }

    public void SelectTech(int idx)
    {
        currentIdx = idx;
        if (idx == 0)
        {
            checkMark1.SetActive(true);
            checkMark2.SetActive(false);
        }
        else if (idx == 1)
        {
            checkMark1.SetActive(false);
            checkMark2.SetActive(true);
        }
    }

    public void ConfirmSelection()
    {
        UpdateManager.isActive = !UpdateManager.isActive;
    }
}
