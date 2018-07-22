using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootObject : InteractiveObject
{
    private ObjectMetadata lootItemData;

    //[HideInInspector]
    //public ObjectMetadata lootItemData
    //{
    //    get { return m_lootItemData; }
    //    set
    //    {
    //        m_lootItemData = value;
    //        descBillboard.Initialize(m_lootItemData.objectName, m_lootItemData.description);
    //    }
    //}
    public string lootBehaviour;
    public OnMapUIBillboard descBillboard;

    public void Init(string behaviour, ObjectMetadata data)
    {
        lootBehaviour = behaviour;
        lootItemData = data;
        if (LootEnum.DROP_BEHAVIOUR.Equals(lootBehaviour))
        {
            descBillboard.Initialize(lootItemData.objectName, lootItemData.description);
        }
        else if (LootEnum.UNLOCK_BEHAVIOUR.Equals(lootBehaviour))
        {
            descBillboard.Initialize(lootItemData.objectName, "Drop " + lootItemData.objectName);
        }

    }

    private new void Awake()
    {
        base.Awake();
        descBillboard = GetComponentInChildren<OnMapUIBillboard>();
        descBillboard.gameObject.SetActive(false);
        callbacks.OnBeingNearestToPlayer = OnBeingNearestToPlayer;
        callbacks.OnNotBeingNearestToPlayer = OnNotBeingNearestToPlayer;
    }

    public override bool ShortPressAction(Player actor)
    {
        if (actor.GetCarryingItem() != null)
        {
            return base.ShortPressAction(actor);
        }

        if (LootEnum.DROP_BEHAVIOUR.Equals(lootBehaviour)) {
            return HandleDropBehaviour(actor);
        }

        if (LootEnum.UNLOCK_BEHAVIOUR.Equals(lootBehaviour)) {
            return HandleUnlockBehaviour();
        }

        return false;
    }

    private bool HandleDropBehaviour(Player actor) {
        GameObject obj = lootItemData.GetGameObjectFromPool();

        if (obj != null) {
            actor.SetCarryingItem(obj.GetComponent<InteractiveObject>());
        }

        GameObject.Destroy(gameObject);

        return true;
    }

    private bool HandleUnlockBehaviour() {
        bool success = TechTreeManager.Instance.UnlockItem(lootItemData.GetPrefab().GetComponent<InteractiveObject>());
        GameObject.Destroy(gameObject);

        return success;
    }

    private void OnBeingNearestToPlayer()
    {
        descBillboard.gameObject.SetActive(true);
    }

    private void OnNotBeingNearestToPlayer()
    {
        descBillboard.gameObject.SetActive(false);
    }
}
