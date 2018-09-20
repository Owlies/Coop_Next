using UnityEngine;

/// <summary>
/// Made by Feiko Joosten
/// 
/// I have based this code on this blogpost. Decided to give it more functionality. http://blogs.unity3d.com/2015/12/23/1k-update-calls/
/// Use this to speed up your performance when you have a lot of update, fixed update and or late update calls in your scene
/// Let the object you want to give increased performance inherit from OverridableMonoBehaviour
/// Replace your void Update() for public override void UpdateMe()
/// Or replace your void FixedUpdate() for public override void FixedUpdateMe()
/// Or replace your void LateUpdate() for public override void LateUpdateMe()
/// OverridableMonoBehaviour will add the object to the update manager
/// UpdateManager will handle all of the update calls
/// </summary>

public class UpdateManager : MonoBehaviour
{
	private static UpdateManager instance;

	private int regularUpdateArrayCount = 0;
	private int fixedUpdateArrayCount = 0;
	private int lateUpdateArrayCount = 0;
	private OverridableMonoBehaviour[] regularArray = new OverridableMonoBehaviour[1024];
	private OverridableMonoBehaviour[] fixedArray = new OverridableMonoBehaviour[1024];
	private OverridableMonoBehaviour[] lateArray = new OverridableMonoBehaviour[1024];

	public UpdateManager()
	{
		instance = this;
	}

	public static void AddItem(OverridableMonoBehaviour behaviour)
	{
		instance.AddItemToArray(behaviour);
	}

	public static void RemoveSpecificItem(OverridableMonoBehaviour behaviour)
	{
		instance.RemoveSpecificItemFromArray(behaviour);
	}

	public static void RemoveSpecificItemAndDestroyIt(OverridableMonoBehaviour behaviour)
	{
		instance.RemoveSpecificItemFromArray(behaviour);

		Destroy(behaviour.gameObject);
	}

	private void AddItemToArray(OverridableMonoBehaviour behaviour)
	{
		if (behaviour.GetType().GetMethod("UpdateMe").DeclaringType != typeof(OverridableMonoBehaviour))
		{
			regularArray = ExtendAndAddItemToArray(regularArray, behaviour, regularUpdateArrayCount);
			regularUpdateArrayCount++;
		}

		if (behaviour.GetType().GetMethod("FixedUpdateMe").DeclaringType != typeof(OverridableMonoBehaviour))
		{
			fixedArray = ExtendAndAddItemToArray(fixedArray, behaviour, fixedUpdateArrayCount);
			fixedUpdateArrayCount++;
		}

		if (behaviour.GetType().GetMethod("LateUpdateMe").DeclaringType == typeof(OverridableMonoBehaviour))
			return;

		lateArray = ExtendAndAddItemToArray(lateArray, behaviour, lateUpdateArrayCount);
		lateUpdateArrayCount++;
	}

	public OverridableMonoBehaviour[] ExtendAndAddItemToArray(OverridableMonoBehaviour[] original, OverridableMonoBehaviour itemToAdd, int originalCount)
	{
		int size = original.Length;
        if (size == originalCount)
        {
            OverridableMonoBehaviour[] finalArray = new OverridableMonoBehaviour[size * 2];
            for (int i = 0; i < size; i++)
                finalArray[i] = original[i];
            original = finalArray;
        }
		original[originalCount] = itemToAdd;
		return original;
	}

	private void RemoveSpecificItemFromArray(OverridableMonoBehaviour behaviour)
	{
		if (CheckIfArrayContainsItem(regularArray, behaviour))
		{
			regularArray = ShrinkAndRemoveItemToArray(regularArray, regularUpdateArrayCount, behaviour);
			regularUpdateArrayCount--;
		}

		if (CheckIfArrayContainsItem(fixedArray, behaviour))
		{
			fixedArray = ShrinkAndRemoveItemToArray(fixedArray, fixedUpdateArrayCount, behaviour);
			fixedUpdateArrayCount--;
		}

		if (!CheckIfArrayContainsItem(lateArray, behaviour)) return;

		lateArray = ShrinkAndRemoveItemToArray(lateArray, lateUpdateArrayCount, behaviour);
		lateUpdateArrayCount--;
	}

	public bool CheckIfArrayContainsItem(OverridableMonoBehaviour[] arrayToCheck, OverridableMonoBehaviour objectToCheckFor)
	{
		int size = arrayToCheck.Length;

		for (int i = 0; i < size; i++)
		{
			if (objectToCheckFor == arrayToCheck[i]) return true;
		}

		return false;
	}

	public OverridableMonoBehaviour[] ShrinkAndRemoveItemToArray(OverridableMonoBehaviour[] original, int originalCount, OverridableMonoBehaviour itemToRemove)
	{
		int size = original.Length;
		for (int i = 0; i < size; i++)
		{
            if (original[i] == itemToRemove)
                original[i] = original[originalCount - 1];
        }
		return original;
	}

	private void Update()
	{
		if (regularUpdateArrayCount == 0) return;

		for (int i = 0; i < regularUpdateArrayCount; i++)
		{
			if (regularArray[i] == null) continue;

            if (regularArray[i].isActiveAndEnabled && regularArray[i].readyToUpdate)
			    regularArray[i].UpdateMe();
		}
	}

	private void FixedUpdate()
	{
		if (fixedUpdateArrayCount == 0) return;

		for (int i = 0; i < fixedUpdateArrayCount; i++)
		{
			if (fixedArray[i] == null) continue;

            if (fixedArray[i].isActiveAndEnabled && fixedArray[i].readyToUpdate)
                fixedArray[i].FixedUpdateMe();
		}
	}

	private void LateUpdate()
	{
		if (lateUpdateArrayCount == 0) return;

		for (int i = 0; i < lateUpdateArrayCount; i++)
		{
			if (lateArray[i] == null) continue;

            if (lateArray[i].isActiveAndEnabled && lateArray[i].readyToUpdate)
                lateArray[i].LateUpdateMe();
		}
	}
}











