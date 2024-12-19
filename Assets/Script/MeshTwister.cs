using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTwister : MonoBehaviour
{
    private Mesh deformingMesh;
    MeshCollider deformingTrigger;
    private Vector3[] originalVertices, displacedVertices;
    
    // L'intensité de la torsion
    public float twistForce = 10f; 

    // L'axis selon lequel on veut appliquer la torsion
    public Vector3 twistAxis = Vector3.up;

    // Le rayon de la torsion
    public float twistRadius = 0.3f; 

    // Le ratio de torsion par rapport à la distance
    public float twistRatio = 2f;

    private Vector3 meshCenter;

    void Start()
    {
        deformingMesh = GetComponent<MeshFilter>().mesh;
        deformingTrigger = GetComponent<MeshCollider>();
        originalVertices = deformingMesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
        System.Array.Copy(originalVertices, displacedVertices, originalVertices.Length);

        meshCenter = Vector3.zero;
        foreach (Vector3 vertex in originalVertices)
        {
            meshCenter += vertex;
        }
        meshCenter /= originalVertices.Length;
    }

    public void AddTwistForce(Vector3 point, float force)
    {
        point = transform.InverseTransformPoint(point);
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            ApplyTwistToVertex(i, point, force);
        }
        RecenterMesh();

        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();
        deformingMesh.RecalculateBounds(); // Ensure updated bounds for rendering

        if (deformingTrigger)
        {
            deformingTrigger.sharedMesh = deformingMesh;
        }
    }

    private void RecenterMesh()
    {
        // Calculate the new center of the mesh
        Vector3 newCenter = Vector3.zero;
        foreach (Vector3 vertex in displacedVertices)
        {
            newCenter += vertex;
        }
        newCenter /= displacedVertices.Length;

        // Calculate the offset to recenter the mesh
        Vector3 offset = meshCenter - newCenter;

        // Apply the offset to all vertices
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            displacedVertices[i] += offset;
        }
    }

    private void ApplyTwistToVertex(int i, Vector3 point, float force)
    {
        Vector3 vertex = displacedVertices[i];
        Vector3 direction = vertex - point;

        // Passer par dessus les vertices en dehors du rayon de torsion
        if (direction.magnitude > twistRadius)
            return;

        // Calculer la force de la torsion en fonction de la distance
        float distanceFactor = Mathf.Max(0, 1 - (direction.magnitude / twistRadius));
        float twistStrength = force * distanceFactor;

        // Déterminer l'angle de la torsion en fonction de la force
        float angle = twistStrength / twistRatio;

        // Créer une rotation autour de l'axe
        Quaternion twistRotation = Quaternion.AngleAxis(angle, twistAxis);

        // Appliquer la torsion au vertice
        displacedVertices[i] = point + twistRotation * direction;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Number of contact points: " +  collision.contacts.Length);
        Debug.Log("Number of vertex in the mesh: " + originalVertices.Length);
        Debug.Log("Total number of vertex whose normals we recalculate: " + (collision.contacts.Length * originalVertices.Length));
        // Conseil Alexis:
        // En accrochant involontairement le mesh je crée une collision en 6 points
        // Cela donne 162390 normales à recalculer (puisque l'on applique TwistForce sur chaque point de contact, 
        // et que AddTwistForce fait un calcul sur chaque vertex du mesh)
        // Je te conseille plutôt de calculer le point de contact moyen de la collision, et de n'utiliser 
        // AddTwistForce qu'avec ce point. 
        //
        // Tu peux consulter le script MeshDeformerSkeletonInput où je fais un tel calcul avant d'appeler MeshDeformer
        Vector3 averageContactPoint = Vector3.zero;
        foreach (ContactPoint contact in collision.contacts)
        {
            averageContactPoint += contact.point;
        }
        averageContactPoint /= collision.contacts.Length;

        float relativeSpeed = collision.relativeVelocity.magnitude;
        float scaledTwistForce = twistForce * relativeSpeed;

        AddTwistForce(averageContactPoint, scaledTwistForce);
    }

    // Utilise le code ci-dessous pour tester de chez toi. 
    // Il simule une collision avec un clic de souris. 
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            Debug.Log("Casting Ray");
            MeshTwister twister = hit.collider.GetComponent<MeshTwister>();
            if (twister)
            {
                Debug.Log("Hit Twister");
                Vector3 point = hit.point;
                //point += hit.normal * forceOffset;
                AddTwistForce(point, twistForce);
            }
            else
            {
                Debug.Log("Did not hit Twister");
            }
        }
    }
}
