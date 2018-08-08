using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "CoopNext/Level Config", order = 1)]
public class LevelConfig : ScriptableObject {
    public Vector2Int mapSize = new Vector2Int(0,0);

    [SerializeField]
    public ObjectInstance[] objectInstances;

    [SerializeField]
    public GameObject[] enemyPrefabs;

    [SerializeField]
    public int[] initialUnlockedBuildingIds;

    [HideInInspector]
    private List<ObjectMetadata> m_initialUnlockedBuildings = null;
    public List<ObjectMetadata> initialUnlockedBuildings
    {
        get
        {
            if (m_initialUnlockedBuildings == null)
            {
                m_initialUnlockedBuildings = new List<ObjectMetadata>();
                for (int i = 0; i < initialUnlockedBuildingIds.Length; ++i)
                {
                    m_initialUnlockedBuildings.Add(MetadataManager.Instance.GetObjectMetadataWithObjectId(initialUnlockedBuildingIds[i]));
                }
            }

            return m_initialUnlockedBuildings;
        }
    }

    private void OnEnable()
    {
        m_initialUnlockedBuildings = null;
    }
}

public enum ObjectDir
{
    Horizontal = 0,
    Vertical,
}

[System.Serializable]
public struct ObjectInstance
{
    public int objectId;
    public ObjectDir dir;
    public Vector2Int position;
}