using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : Singleton<Util> {
    public GameObject[] GetFirstLayerChildComponents(GameObject parentGameObject) {
        List<GameObject> result = new List<GameObject>();
        foreach (GameObject childComponent in parentGameObject.GetComponentsInChildren<GameObject>()) {
            if (childComponent.transform.parent == parentGameObject) {
                result.Add(childComponent);
            }
        }

        return result.ToArray();
    }

    public Transform[] GetFirstLayerChildComponents(Transform parentGameObject) {
        List<Transform> result = new List<Transform>();
        foreach (Transform childComponent in parentGameObject.GetComponentsInChildren<Transform>()) {
            if (childComponent.transform.parent == parentGameObject) {
                result.Add(childComponent);
            }
        }

        return result.ToArray();
    }
}
