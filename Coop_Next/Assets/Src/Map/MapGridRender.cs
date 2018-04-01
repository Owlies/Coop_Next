using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGridRender {

    GameObject planePrefab;
    Mesh planeMesh;
    float size = 0.5f;


    public MapGridRender()
    {
        planeMesh = new Mesh();
        planeMesh.vertices = new Vector3[] {
             new Vector3(-size, 0.01f, -size),
             new Vector3(size, 0.01f, -size),
             new Vector3(size, 0.01f, size),
             new Vector3(-size, 0.01f, size)
         };
        planeMesh.uv = new Vector2[] {
             new Vector2 (0, 0),
             new Vector2 (0, 1),
             new Vector2(1, 1),
             new Vector2 (1, 0)
         };
        planeMesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        planeMesh.RecalculateNormals();
    }

    public void Draw()
    {

    }
}
