using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : Singleton<LootManager>
{
    public GameObject lootPrefab = null;
    public GameObject lootRoot = null;

    public void DropLoot(int lootId, Vector3 pos)
    {
        int itemId = MetadataManager.Instance.GetRandomLoot(lootId);

        if (itemId != 0)
        {
            if (lootPrefab == null)
                Debug.LogError("no loot prefab");
            else
            {
                if (lootRoot == null)
                {
                    lootRoot = new GameObject("[LootRoot]");
                    lootRoot.transform.position = Vector3.zero;
                    lootRoot.transform.rotation = Quaternion.identity;
                    lootRoot.transform.localScale = Vector3.one;
                }
                GameObject loot = GameObject.Instantiate(lootPrefab,lootRoot.transform) as GameObject;

                Vector3 position = pos;
                position.y = 0;
                loot.transform.position = position;

                var lootObject = loot.GetComponent<LootObject>();
                if (lootObject == null)
                    Debug.LogError("No LootObject on lootObject");
                else
                {
                    ItemMetadata item = MetadataLoader.Instance.GetItemMetadataById(itemId);
                    if (item == null)
                        Debug.LogError("loot item " + itemId + " is not in db!");
                    else
                        lootObject.lootItemData = item;
                }
            }
        }
    }
}
