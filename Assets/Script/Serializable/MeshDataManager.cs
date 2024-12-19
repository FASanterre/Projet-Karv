using UnityEngine;
using System.Collections.Generic;

public class MeshDataManager : MonoBehaviour
{
    public MeshData meshData;

    public void PrecomputeMeshData(Mesh mesh)
    {
        // Precompute triangle normals and edge relationships
        int triangleCount = mesh.triangles.Length / 3;
        meshData.triangleNormals = new Vector3[triangleCount];
        meshData.edgeTrianglePairs = new List<EdgeTrianglePair>();
        meshData.triangleMaxDepths = new float[triangleCount];

        Vector3[] vertices = mesh.vertices;
        Vector3 meshCenter = Vector3.zero;

        foreach (Vector3 vertex in vertices)
        {
            meshCenter += vertex;
        }
        meshCenter /= vertices.Length;

        // Precompute normals and edge-triangle pairs
        for (int i = 0; i < triangleCount; i++)
        {
            int vertexIndex0 = mesh.triangles[i * 3 + 0];
            int vertexIndex1 = mesh.triangles[i * 3 + 1];
            int vertexIndex2 = mesh.triangles[i * 3 + 2];

            Vector3 vertex0 = vertices[vertexIndex0];
            Vector3 vertex1 = vertices[vertexIndex1];
            Vector3 vertex2 = vertices[vertexIndex2];

            // Calculate normal for the triangle
            meshData.triangleNormals[i] = Vector3.Cross(vertex1 - vertex0, vertex2 - vertex0).normalized;

            Vector3 centroid = (vertex0 + vertex1 + vertex2) / 3f;

            float initialDistance = Vector3.Dot(centroid - meshCenter, meshData.triangleNormals[i]);
            meshData.triangleMaxDepths[i] = Mathf.Abs(initialDistance);
            // Add edges to the edge-triangle dictionary
            // TODO make this thing work!
            //AddEdge(vertexIndex0, vertexIndex1, i);
            //AddEdge(vertexIndex1, vertexIndex2, i);
            //AddEdge(vertexIndex2, vertexIndex0, i);
        }
    }

    private void AddEdge(int vertexA, int vertexB, int triangleIndex)
    {
        int edgeStart = Mathf.Min(vertexA, vertexB);
        int edgeEnd = Mathf.Max(vertexA, vertexB);
        int edgeKey = edgeStart * 100000 + edgeEnd;

        EdgeTrianglePair existingEdge = meshData.edgeTrianglePairs.Find(pair => pair.edgeKey == edgeKey);
        if (existingEdge == null)
        {
            existingEdge = new EdgeTrianglePair { edgeKey = edgeKey, triangleIndices = new List<int>() };
            meshData.edgeTrianglePairs.Add(existingEdge);
        }
        existingEdge.triangleIndices.Add(triangleIndex);
    }

    //private void OnDrawGizmos()
    //{
    //    // Check if meshData is initialized
    //    if (meshData == null || meshData.triangleNormals == null) return;
    //
    //    Mesh mesh = GetComponent<MeshCollider>().sharedMesh;
    //    if (mesh == null) return;
    //
    //    Vector3[] vertices = mesh.vertices;
    //
    //    //float drawPercentage = 0.1f;
    //    int testIndex = 5000;
    //    // Visualize triangle normals
    //    for (int i = testIndex; i < (testIndex + 10); i++)
    //    {
    //        //if (Random.value < drawPercentage) continue;
    //    
    //        int vertexIndex0 = mesh.triangles[i * 3 + 0];
    //        int vertexIndex1 = mesh.triangles[i * 3 + 1];
    //        int vertexIndex2 = mesh.triangles[i * 3 + 2];
    //    
    //        // Find the centroid of the triangle
    //        Vector3 centroid = (vertices[vertexIndex0] + vertices[vertexIndex1] + vertices[vertexIndex2]) / 3f;
    //    
    //        centroid = transform.TransformPoint(centroid);
    //
    //        Vector3 triangleNormal = meshData.triangleNormals[i];
    //        triangleNormal = transform.TransformDirection(triangleNormal);
    //
    //        // Draw an arrow along the normal
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawLine(centroid, centroid + triangleNormal * 0.05f);  // Adjust length here
    //        Gizmos.DrawWireSphere(centroid + triangleNormal * 0.05f, 0.01f);  // End of the arrow
    //    }
    //}
}
