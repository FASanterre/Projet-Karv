using UnityEngine;
using System.Collections.Generic;

public class MeshColorer : MonoBehaviour
{
    private Mesh mesh;
    private MeshDataManager meshDataManager;
    private Vector3[] originalVertices;
    private Vector3[] currentVertices;
    private Vector3[] triangleNormals;

    //private HashSet<int> maxDepthTriangles = new HashSet<int>();
    private List<int> coloredTriangles = new List<int>();

    // TODO - bake this dictionary inside the mesh
    private List<EdgeTrianglePair> edgeTrianglePairs;

    public Vector3[] GetTriangleNormals()
    {
        return meshDataManager.meshData.triangleNormals;
    }
    void Start()
    {
        mesh = GetComponent<MeshCollider>().sharedMesh;
        if (mesh == null) return;

        originalVertices = mesh.vertices;
        currentVertices = (Vector3[])originalVertices.Clone();
        // originalNormals = mesh.normals;
        mesh.colors = new Color[mesh.vertexCount];

        meshDataManager = GetComponent<MeshDataManager>();
        if (meshDataManager != null)
        {
            triangleNormals = meshDataManager.meshData.triangleNormals;
            edgeTrianglePairs = meshDataManager.meshData.edgeTrianglePairs;
        }

        mesh.MarkDynamic();
    }

    public void ColorMesh(List<int> triangleIndices)
    {
        if (mesh == null) return;

        Color[] colors = mesh.colors;

        foreach (int triangleIndex in triangleIndices)
        {
            int vertexIndex0 = mesh.triangles[triangleIndex* 3 + 0];
            int vertexIndex1 = mesh.triangles[triangleIndex* 3 + 1];
            int vertexIndex2 = mesh.triangles[triangleIndex* 3 + 2];
            colors[vertexIndex0] = Color.Lerp(Color.red, Color.green, currentVertices[vertexIndex0].y);
            colors[vertexIndex1] = Color.Lerp(Color.red, Color.green, currentVertices[vertexIndex1].y);
            colors[vertexIndex2] = Color.Lerp(Color.red, Color.green, currentVertices[vertexIndex2].y);

            if (!coloredTriangles.Contains(triangleIndex)) coloredTriangles.Add(triangleIndex);
        }

        mesh.colors = colors;
    }

    public void EraseMesh(List<int> triangleIndices)
    {
        if (mesh == null) return;

        Color[] colors = mesh.colors;

        foreach (int triangleIndex in triangleIndices)
        {
            if (!coloredTriangles.Contains(triangleIndex)) continue;

            int vertexIndex0 = mesh.triangles[triangleIndex * 3 + 0];
            int vertexIndex1 = mesh.triangles[triangleIndex * 3 + 1];
            int vertexIndex2 = mesh.triangles[triangleIndex * 3 + 2];
            colors[vertexIndex0] = Color.black;
            colors[vertexIndex0].a = 0;
            colors[vertexIndex1] = Color.black;
            colors[vertexIndex1].a = 0;
            colors[vertexIndex2] = Color.black;
            colors[vertexIndex2].a = 0;

            coloredTriangles.Remove(triangleIndex);
        }

        mesh.colors = colors;
    }

    public void ApplyRaisinEffect(float intensity = 0.1f)
    {
        ModifyTriangles(-intensity);
    }

    public void RemoveRaisinEffect(float intensity = 0.1f)
    {
        ModifyTriangles(intensity);
    }

    private void ModifyTriangles(float intensity)
    {
        if (mesh == null) return;

        Vector3[] updatedVertices = currentVertices;

        // This was only relevant when calculating edgePairs...
        //HashSet<int> movedTriangles = new HashSet<int>();

        foreach (int triangleIndex in coloredTriangles) 
        {
            //if (movedTriangles.Contains(triangleIndex)) continue;

            int vertexIndex0 = mesh.triangles[triangleIndex * 3 + 0];
            int vertexIndex1 = mesh.triangles[triangleIndex * 3 + 1];
            int vertexIndex2 = mesh.triangles[triangleIndex * 3 + 2];

            Vector3 triangleNormal = triangleNormals[triangleIndex];
            float maxDepth = meshDataManager.meshData.triangleMaxDepths[triangleIndex];

            Vector3 currentCentroid = (currentVertices[vertexIndex0] + currentVertices[vertexIndex1] + currentVertices[vertexIndex2]) / 3f;
            Vector3 originalCentroid = (originalVertices[vertexIndex0] + originalVertices[vertexIndex1] + originalVertices[vertexIndex2]) / 3f;
            float currentDepth = Vector3.Dot(currentCentroid - originalCentroid, triangleNormal);

            float allowedMovement = Mathf.Clamp(intensity, -maxDepth - currentDepth, maxDepth - currentDepth);
            //float moveAmount = intensity; // TODO - use Time.deltaTime?
            updatedVertices[vertexIndex0] += triangleNormal * allowedMovement;
            updatedVertices[vertexIndex1] += triangleNormal * allowedMovement;
            updatedVertices[vertexIndex2] += triangleNormal * allowedMovement;

            //movedTriangles.Add(triangleIndex);

            //RecalculateTriangleNormal(vertexIndex0, vertexIndex1, vertexIndex2);

            // Surrounding triangles are stationary and counted as 'moved'
            // This does not work and slows the computation to a crawl
            //foreach (var edgeTrianglePair in edgeTrianglePairs)
            //{
            //    if (!edgeTrianglePair.triangleIndices.Contains(triangleIndex)) continue;
            //
            //    foreach(int oppositeTriangle in edgeTrianglePair.triangleIndices)
            //    {
            //        if ((oppositeTriangle != triangleIndex) && (!movedTriangles.Contains(oppositeTriangle)))
            //        {
            //            movedTriangles.Add((oppositeTriangle));
            //            RecalculateTriangleNormal(
            //                mesh.triangles[oppositeTriangle * 3 + 0],
            //                mesh.triangles[oppositeTriangle * 3 + 1],
            //                mesh.triangles[oppositeTriangle * 3 + 2]);
            //        }
            //    }
            //}
        }

        currentVertices = updatedVertices;
        mesh.vertices = currentVertices;
    }

    private void RecalculateTriangleNormal(int vertexIndex0, int vertexIndex1, int vertexIndex2)
    {
        Vector3 v0 = currentVertices[vertexIndex0];
        Vector3 v1 = currentVertices[vertexIndex1];
        Vector3 v2 = currentVertices[vertexIndex2];

        Vector3 edge1 = v1 - v0;
        Vector3 edge2 = v2 - v0;

        Vector3 normal = Vector3.Cross(edge1, edge2).normalized;

        mesh.normals[vertexIndex0] = normal;
        mesh.normals[vertexIndex1] = normal;
        mesh.normals[vertexIndex2] = normal;
    }

    public void ApplyDeformation(float intensity)
    {
        ModifyTriangles(intensity);
        if(Time.frameCount % 5 == 0)
        {
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            GetComponent<MeshCollider>().sharedMesh = mesh;
        }
    }

    private void OnApplicationQuit()
    {
        if (mesh == null) return;

        // Reset vertex positions and colors on quit
        mesh.vertices = originalVertices;
        mesh.colors = new Color[mesh.vertexCount];
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
