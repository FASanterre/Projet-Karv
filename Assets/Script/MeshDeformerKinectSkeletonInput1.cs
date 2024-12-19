using System.Collections.Generic;
using UnityEngine;

public class MeshDeformerKinectSkeletonInput : MonoBehaviour
{
    public float force = 10f;
    public float forceOffset = 0.1f;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("A New Hand Touches The Beacon");
        if (collision.collider.TryGetComponent(out MeshCollider meshCollider) && meshCollider.sharedMesh != null)
        {
            Debug.Log("A New Hand Touches The Brick");
            Mesh mesh = meshCollider.sharedMesh;
            List<int> triIndices = new List<int>();
            Vector3 contactCenter = new Vector3();
            Vector3 contactNormal = new Vector3();
            //Vector3[] normals = meshColorer.GetTriangleNormals(); // Assuming MeshColorer has this method
            foreach (ContactPoint contact in collision.contacts)
            {
                contactCenter += contact.point;
                contactNormal += contact.normal;
            }
            contactCenter /= collision.contacts.Length;
            contactNormal /= collision.contacts.Length;

            MeshDeformer deformer = collision.collider.GetComponent<MeshDeformer>();
            if (deformer)
            {
                Debug.Log("Is a deformer");
                Vector3 point = contactCenter + contactNormal * forceOffset;
                //deformer.AddDeformingForce(point, force);
            }
            else
            {
                Debug.Log("Not a deformer");
            }
        }
    }
}
