using UnityEngine;
using System.Collections.Generic;

public class ColliderOutput : MonoBehaviour
{
    private MeshColorer meshColorer;
    private Camera mainCamera;
    //private Vector3[] normals;
    private void Start()
    {
        //Debug.Log("A New Brush Is Ready To Touch The Beacon");
        meshColorer = FindObjectOfType<MeshColorer>();
        //Debug.Log("The Brush Has Found The Beacon");
        mainCamera = Camera.main;
        //Debug.Log("The Brush Knows Of The Light");
    }
    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("A New Hand Touches The Beacon");
        if (collision.collider.TryGetComponent(out MeshCollider meshCollider) && meshCollider.sharedMesh != null)
        {
            Debug.Log("A New Hand Touches The Brick");
            Mesh mesh = meshCollider.sharedMesh;
            List<int> triIndices = new List<int>();
            //Vector3[] normals = meshColorer.GetTriangleNormals(); // Assuming MeshColorer has this method
            foreach (ContactPoint contact in collision.contacts)
            {
                int triangleIndex = GetClosestTriangleIndex(contact.point, mesh, meshCollider);
                if(!triIndices.Contains(triangleIndex)) triIndices.Add(triangleIndex);
                //if (IsTriangleVisible(triangleIndex, mesh, meshCollider, normals))
                //{
                //    meshColorer?.ColorMesh(new List<int> { triangleIndex });
                //}
            }
            meshColorer?.EraseMesh(triIndices);
        }
    }
    
    //private void OnTriggerStay(Collider other)
    //{
    //    Debug.Log("A new hand touches the beacon");
    //    // TODO - Check if it's a meshColorer instead!
    //    if (other.TryGetComponent(out MeshCollider meshCollider) && meshCollider.sharedMesh != null)
    //    {
    //        Mesh mesh = meshCollider.sharedMesh;
    //        //normals = meshColorer.GetTriangleNormals();
    //        List<int> visibleTriangles = GetVisibleTriangles(mesh, meshCollider);
    //
    //        foreach (int triangleIndex in visibleTriangles)
    //        {
    //            meshColorer?.ColorMesh(new List<int> { triangleIndex });
    //        }
    //    }
    //}

    //private List<int> GetVisibleTriangles(Mesh mesh, MeshCollider meshCollider)
    //{
    //    List<int> visibleTriangles = new List<int>();
    //    int[] triangles = mesh.triangles;
    //    Vector3[] vertices = mesh.vertices;
    //    //normals = meshColorer.GetTriangleNormals();
    //
    //    for (int i = 0; i < triangles.Length; i += 3)
    //    {
    //        int v0 = triangles[i + 0];
    //        int v1 = triangles[i + 1];
    //        int v2 = triangles[i + 2];
    //
    //        Vector3 worldV0 = meshCollider.transform.TransformPoint(vertices[v0]);
    //        Vector3 worldV1 = meshCollider.transform.TransformPoint(vertices[v1]);
    //        Vector3 worldV2 = meshCollider.transform.TransformPoint(vertices[v2]);
    //
    //        if (IsTriangleInColliderBounds(worldV0, worldV1, worldV2) &&
    //            IsTriangleVisible(i / 3, normals, meshCollider))
    //        {
    //            visibleTriangles.Add(i / 3);
    //        }
    //    }
    //
    //    return visibleTriangles;
    //}

    private bool IsTriangleInColliderBounds(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        Collider triggerCollider = GetComponent<Collider>();
        return triggerCollider.bounds.Contains(v0) ||
               triggerCollider.bounds.Contains(v1) ||
               triggerCollider.bounds.Contains(v2);
    }

    private bool IsTriangleVisible(int triangleIndex, Vector3[] normals, MeshCollider meshCollider)
    {
        Vector3 triangleNormal = meshCollider.transform.TransformDirection(normals[triangleIndex]);

        if (Vector3.Dot(triangleNormal, mainCamera.transform.forward) > 0)
        {
            return false; // Backface culling
        }

        return true;
    }

    private int GetClosestTriangleIndex(Vector3 contactPoint, Mesh mesh, MeshCollider meshCollider)
    {
        Vector3 localPoint = meshCollider.transform.InverseTransformPoint(contactPoint);
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        float minDistance = float.MaxValue;
        int closestTriangleIndex = 0;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];
            Vector3 centroid = (v0 + v1 + v2) / 3f;

            float distance = Vector3.Distance(localPoint, centroid);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTriangleIndex = i / 3;
            }
        }

        return closestTriangleIndex;
    }

    //private bool IsTriangleVisible(int triangleIndex, Mesh mesh, MeshCollider meshCollider)
    //{
    //    Vector3[] vertices = mesh.vertices;
    //    int[] triangles = mesh.triangles;
    //
    //    int v0 = triangles[triangleIndex * 3 + 0];
    //    int v1 = triangles[triangleIndex * 3 + 1];
    //    int v2 = triangles[triangleIndex * 3 + 2];
    //
    //    Vector3 worldV0 = meshCollider.transform.TransformPoint(vertices[v0]);
    //    Vector3 worldV1 = meshCollider.transform.TransformPoint(vertices[v1]);
    //    Vector3 worldV2 = meshCollider.transform.TransformPoint(vertices[v2]);
    //
    //    Vector3 triangleNormal = meshCollider.transform.TransformDirection(normals[triangleIndex]);
    //
    //    // Check if the triangle normal is facing the camera
    //    if (Vector3.Dot(triangleNormal, mainCamera.transform.forward) > 0)
    //    {
    //        return false; // Backface culling
    //    }
    //
    //    // Check if the triangle is in the camera's view
    //    if (IsInView(worldV0) || IsInView(worldV1) || IsInView(worldV2))
    //    {
    //        return true;
    //    }
    //
    //    return false;
    //}
    private bool IsInView(Vector3 worldPoint)
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(worldPoint);
        return viewportPoint.z > 0 && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1;
    }
}