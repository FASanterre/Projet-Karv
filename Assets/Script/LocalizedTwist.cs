using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizedTwist : MonoBehaviour
{
    public float twistAmount = 5f; // Amount of twist
    public float influenceRadius = 1.0f; // How far the twist effect reaches from the mouse point

    private MeshFilter meshFilter;
    private Vector3[] originalVertices;

    private Camera mainCamera;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            originalVertices = meshFilter.mesh.vertices.Clone() as Vector3[];
        }

        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) // Check if the left mouse button is pressed
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                ApplyLocalizedTwist(hit.point);
            }
        }
        else
        {
            //ResetTwist();
        }
    }

    // Apply localized twist deformation
    void ApplyLocalizedTwist(Vector3 hitPoint)
    {
        Vector3[] vertices = meshFilter.mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = transform.TransformPoint(originalVertices[i]); // Get world space position of the vertex
            float distanceToHit = Vector3.Distance(vertex, hitPoint);

            if (distanceToHit < influenceRadius) // Only affect vertices within the influence radius
            {
                // Twist factor based on proximity to the hit point
                float twistFactor = (1 - (distanceToHit / influenceRadius)) * twistAmount;
                float angle = twistFactor * Mathf.Deg2Rad;

                // Localize twist to the area near the hit point
                Vector3 localVertex = originalVertices[i];
                float newX = localVertex.x * Mathf.Cos(angle) - localVertex.z * Mathf.Sin(angle);
                float newZ = localVertex.x * Mathf.Sin(angle) + localVertex.z * Mathf.Cos(angle);

                vertices[i] = new Vector3(newX, localVertex.y, newZ);
            }
        }

        // Apply new vertices to the mesh
        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateNormals();
    }

    // Reset the mesh to its original state
    void ResetTwist()
    {
        meshFilter.mesh.vertices = originalVertices;
        meshFilter.mesh.RecalculateNormals();
    }
}
