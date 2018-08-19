using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneRenderer {

    static GameObject m_planePrefab = null;
    static public GameObject GetPlaneGameObject()
    {
        if (m_planePrefab == null)
        {
            m_planePrefab = new GameObject("plane");
            var meshFilter = m_planePrefab.AddComponent<MeshFilter>();
            meshFilter.mesh = planeMesh;
            var meshRenderer = m_planePrefab.AddComponent<MeshRenderer>();
            m_planePrefab.SetActive(false);
        }
        var instant = GameObject.Instantiate(m_planePrefab);
        instant.SetActive(true);
        return instant;
    }

    static Mesh m_planeMesh = null;
    static public Mesh planeMesh
    {
        get
        {
            if (m_planeMesh == null)
            {
                m_planeMesh = new Mesh();
                m_planeMesh.vertices = new Vector3[] {
                     new Vector3(-0.5f, 0.01f, -0.5f),
                     new Vector3(-0.5f, 0.01f, 0.5f),
                     new Vector3(0.5f, 0.01f, 0.5f),
                     new Vector3(0.5f, 0.01f, -0.5f)
                 };
                m_planeMesh.uv = new Vector2[] {
                     new Vector2 (0, 0),
                     new Vector2 (0, 1),
                     new Vector2(1, 1),
                     new Vector2 (1, 0)
                 };
                m_planeMesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
                m_planeMesh.RecalculateNormals();
            }

            return m_planeMesh;
        }
    }

    static public void Draw(Vector3 pos, float scale, Color color, Material material)
    {
        if (material == null)
            return;
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        Matrix4x4 matrix = new Matrix4x4();
        matrix.SetTRS(pos, Quaternion.identity, new Vector3(0.9f, 0.9f, 0.9f) * scale);
        block.SetColor("_Color", color);
        Graphics.DrawMesh(planeMesh, matrix, material, 0, null, 0, block, false, true);
    }
}
