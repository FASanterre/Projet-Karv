using UnityEngine;
using System.Collections.Generic;

public class RaycastInput : MonoBehaviour
{
    private MeshColorer mesh;

    private bool isRaising = false;
    private bool isLowering = false;

    private void Start()
    {
        mesh = FindObjectOfType<MeshColorer>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                MeshColorer colorer = hit.collider.GetComponent<MeshColorer>();
                if (colorer != null)
                {
                    MeshCollider meshCollider = hit.collider as MeshCollider;
                    if (meshCollider != null && meshCollider.sharedMesh != null)
                    {
                        List<int> hitTriangles = new List<int> { hit.triangleIndex };
                        colorer.ColorMesh(hitTriangles);
                    }
                }
            }
        }

        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                MeshColorer colorer = hit.collider.GetComponent<MeshColorer>();
                if (colorer != null)
                {
                    MeshCollider meshCollider = hit.collider as MeshCollider;
                    if (meshCollider != null && meshCollider.sharedMesh != null)
                    {
                        List<int> hitTriangles = new List<int> { hit.triangleIndex };
                        colorer.EraseMesh(hitTriangles);
                    }
                }
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            isLowering = true;
        }
        else
        {
            isLowering = false;
        }

        if (Input.GetKey(KeyCode.T))
        {
            isRaising = true;
        }
        else
        {
            isRaising = false;
        }

        if (isLowering)
        {
            mesh?.ApplyDeformation(-0.005f);
        }
        else if (isRaising)
        {
            mesh?.ApplyDeformation(0.005f);
        }
    }
}