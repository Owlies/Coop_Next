using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : Singleton<LootManager>
{
    public GameObject lootPrefab = null;
    private GameObject lootRoot = null;

    public void DropLoot(int lootId, Vector3 pos)
    {
        LootMetadata lootMetadata = MetadataLoader.Instance.GetLootMetadataById(lootId);
        int itemId = MetadataManager.Instance.GetRandomLoot(lootMetadata);

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
                if (lootObject == null) {
                    Debug.LogError("No LootObject on lootObject");
                    return;
                } else {
                    ObjectMetadata item = MetadataManager.Instance.GetObjectMetadataWithObjectId(itemId);
                    if (item == null) {
                        Debug.LogError("loot item " + itemId + " is not in db!");
                    } else {
                        lootObject.lootItemData = item;
                        lootObject.lootBehaviour = lootMetadata.behaviour;
                    }
                }
            }
        }
    }
}
