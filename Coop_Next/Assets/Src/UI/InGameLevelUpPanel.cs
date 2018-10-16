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

    private ObjectSubType[] techSubType = new ObjectSubType[2];
    private int[] techLevel = new int[2];
    private int currentIdx = -1;

    Dictionary<int, int> techs = new Dictionary<int, int>();
    public void OnEnable()
    {
        var subtypes = TechTreeManager.Instance.GetSubTypeLevelMap();
        techs.Clear();
        foreach (var id in subtypes)
            if (id.Value < 3)
                techs.Add(id.Key,id.Value);
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
                techSubType[0] = (ObjectSubType)iter.Current.Key;
                techLevel[0] = iter.Current.Value + 1;
                techText1.text = techSubType[0].ToString() + "\n LV " + techLevel[0];
            }
            if (idx == id2)
            {
                techSubType[1] = (ObjectSubType)iter.Current.Key;
                techLevel[1] = iter.Current.Value + 1;
                techText2.text = techSubType[1] + "\n LV " + techLevel[1];
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

        TechTreeManager.Instance.UpgradeBuildingWithSubType(techSubType[currentIdx]);
        MapManager.Instance.UpgradeBuildings(techSubType[currentIdx], techLevel[currentIdx]);
    }
}
