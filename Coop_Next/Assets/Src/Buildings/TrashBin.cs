using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBin : BuildingBase {
	public override bool ShortPressAction(Player actor) {
		if (TryDestroyPlayerCarryingItem(actor)) {
			return true;
		}

		return false;
	}

	private bool TryDestroyPlayerCarryingItem(Player actor) {
		InteractiveObject carryingItem = actor.GetCarryingItem();

		if (carryingItem == null) {
			return false;
		}

		actor.UnsetCarryingItem();
		MapManager.Instance.OnItemDestroyed(carryingItem.gameObject);
		GameObject.DestroyObject(carryingItem.gameObject);

		return true;
	}
}
