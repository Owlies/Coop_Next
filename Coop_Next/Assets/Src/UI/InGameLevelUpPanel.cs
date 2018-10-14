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
    private int[] techLevel = new int[2];
    private int currentIdx = -1;

    //public void InitPanel(string tech1, int tech1Level, string tech2, int tech2Level)
    //{
    //    techText1.text = tech1 + "\n LV " + tech1Level;
    //    techText2.text = tech2 + "\n LV " + tech2Level;
    //    techName[0] = tech1;
    //    techName[1] = tech2;
    //}

    public void OnEnable()
    {
        var techs = TechTreeManager.Instance.GetSubTypeLevelMap();
        int id1 = Random.Range(0, techs.Count);
        int id2 = Random.Range(0, techs.Count);
        while (techs.Count > 1 && id2 == id1)
            id2 = Random.Range(0, techs.Count);

        var iter = techs.GetEnumerator();
        int idx = 0;
        while(iter.MoveNext())
        {
            if (idx == id1)
            {
                techName[0] = iter.Current.Key;
                techLevel[0] = iter.Current.Value + 1;
                techText1.text = techName[0] + "\n LV " + techLevel[0];
            }
            if (idx == id2)
            {
                techName[1] = iter.Current.Key;
                techLevel[1] = iter.Current.Value + 1;
                techText2.text = techName[1] + "\n LV " + techLevel[1];
            }
            idx++;
        }

        currentIdx = -1;
        selectButton.enabled = false;
        checkMark1.SetActive(false);
        checkMark2.SetActive(false);
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

        selectButton.enabled = true;
    }

    public void ConfirmSelection()
    {
        UIManager.Instance.HideUIPanel("InGameLevelUpPanel", true);

        var techs = TechTreeManager.Instance.GetTechTreeLevelMap();
        techs[techName[currentIdx]]++;
        MapManager.Instance.UpgradeBuildings(techName[currentIdx], techLevel[currentIdx]);
    }
}
