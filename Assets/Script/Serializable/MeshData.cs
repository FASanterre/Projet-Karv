using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

[System.Serializable]
public class EdgeTrianglePair
{
    public int edgeKey;
    public List<int> triangleIndices;
}

[System.Serializable]
public class MeshData
{
    public Vector3[] triangleNormals;
    public List<EdgeTrianglePair> edgeTrianglePairs;
    public float[] triangleMaxDepths;
}