// This script draws a debug line around mesh triangles
// as you move the mouse over them.
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExampleClass : MonoBehaviour
{
    Camera cam;
    List<Mesh> modifiedMeshes = new List<Mesh>();
    //List<Mesh> originalMeshes = new List<Mesh>();

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        RaycastHit hit;
        if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            return;
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;

        Mesh mesh = meshCollider.sharedMesh;
        modifiedMeshes.Add(mesh);
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
        Transform hitTransform = hit.collider.transform;
        p0 = hitTransform.TransformPoint(p0);
        p1 = hitTransform.TransformPoint(p1);
        p2 = hitTransform.TransformPoint(p2);
        Debug.DrawLine(p0, p1);
        Debug.DrawLine(p1, p2);
        Debug.DrawLine(p2, p0);


        Color[] meshColors = mesh.colors;
        if(meshColors.Length == 0)
            meshColors = new Color[vertices.Length];
        meshColors[triangles[hit.triangleIndex * 3 + 0]] = 
            Color.Lerp(Color.red, Color.green, vertices[triangles[hit.triangleIndex * 3 + 0]].y);
        meshColors[triangles[hit.triangleIndex * 3 + 1]] =
            Color.Lerp(Color.red, Color.green, vertices[triangles[hit.triangleIndex * 3 + 1]].y);
        meshColors[triangles[hit.triangleIndex * 3 + 2]] =
            Color.Lerp(Color.red, Color.green, vertices[triangles[hit.triangleIndex * 3 + 2]].y);

        mesh.colors = meshColors;
    }

    private void OnApplicationQuit()
    {
        foreach(Mesh mesh in modifiedMeshes)
        {
            Color[] emptyColorArray = new Color[mesh.colors.Length];
            mesh.colors = emptyColorArray;
        }
    }
}