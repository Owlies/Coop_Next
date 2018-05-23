using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGridRender {

    GameObject planePrefab;
    Mesh planeMesh;
    Material material;

    public MapGridRender(float unitSize)
    {
        planeMesh = new Mesh();
        planeMesh.vertices = new Vector3[] {
             new Vector3(0, 0.01f, 0),
             new Vector3(0, 0.01f, unitSize),
             new Vector3(unitSize, 0.01f, unitSize),
             new Vector3(unitSize, 0.01f, 0)
         };
        planeMesh.uv = new Vector2[] {
             new Vector2 (0, 0),
             new Vector2 (0, 1),
             new Vector2(1, 1),
             new Vector2 (1, 0)
         };
        planeMesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        planeMesh.RecalculateNormals();

        material = new Material(Resources.Load("Materials/GridRenderer") as Material);
    }

    public void Draw(Vector3 pos, Color color)
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        Matrix4x4 matrix = new Matrix4x4();
        matrix.SetTRS(pos, Quaternion.identity, new Vector3(0.9f, 0.9f, 0.9f));
        block.SetColor("_Color", color);
        Graphics.DrawMesh(planeMesh, matrix, material, 0, null, 0, block, false, true);
    }
}
