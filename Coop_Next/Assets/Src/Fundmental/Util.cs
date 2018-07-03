using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Constants
{
    public const double EPS = 10e-6;

}

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

    public GameObject GetChildComponentWithName(GameObject parentGameObject, string name) {
        foreach (GameObject childComponent in parentGameObject.GetComponentsInChildren<GameObject>()) {
            if (childComponent.name.Equals(name)) {
                return childComponent;
            }
        }

        return null;
    }

    static public float Get2DDistanceSquared(GameObject a, GameObject b)
    {
        Vector3 posA = a.transform.position;
        Vector3 posB = b.transform.position;
        Vector3 result = posB - posA;
        result.y = 0;

        return result.sqrMagnitude;
    }
}
