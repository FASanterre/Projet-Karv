using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Twist : MonoBehaviour
{
    public float twistForce = 5f;
    private MeshFilter meshFilter;
    private Vector3[] originalVertices;

    private bool isTwisting = false;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        if(meshFilter != null)
        {
            originalVertices = meshFilter.mesh.vertices.Clone() as Vector3[];
        }
    }

    void Update()
    {
        if(isTwisting && meshFilter != null)
        {
            ApplyTwist();
        }
        /*else if(!isTwisting && meshFilter != null)
        {
            ResetTwist();
        }*/
    }

    private void OnMouseOver()
    {
        isTwisting=true;
    }

    private void OnMouseExit()
    {
        isTwisting=false;
    }

    void ApplyTwist()
    {
        Vector3[] vertices = meshFilter.mesh.vertices;

        for(int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = originalVertices[i];

            float twistFactor = vertex.y * twistForce;

            float angle = twistFactor * Mathf.Deg2Rad;
            float newX = vertex.x * Mathf.Cos(angle) - vertex.z * Mathf.Sin(angle);
            float newZ = vertex.x * Mathf.Sin(angle) + vertex.z * Mathf.Cos(angle);

            vertices[i] = new Vector3(newX, vertex.y, newZ);
        }

        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateNormals();
    }

    private void ResetTwist()
    {
        meshFilter.mesh.vertices = originalVertices;
        meshFilter.mesh.RecalculateNormals();
    }
}
