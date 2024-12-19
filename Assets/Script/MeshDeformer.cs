using KARV;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour
{
    CaptureManager captureManageInstance;
    Mesh deformingMesh;
    MeshCollider deformingTrigger;
    Vector3[] originalVertices, displacedVertices, vertexVelocities, activeVertices;
    //Vector3[] originalTriggerVertices, displacedTriggerVertices;
    float uniformScale = 1f;

    // Impact
    private float springForce = 0f;
    private float damping = 0f;
    // Twist
    public float twistForce;
    public Vector3 twistAxis;
    public float twistRadius;
    public float twistRatio;
    // Rotation
    //public float rotationSpeed = 2.0f;
    public float rotationAngleX = 15.0f;
    public float rotationAngleY = 15.0f;

    private Vector3 meshCenter;
    private Vector3 startRightPosition;
    private Vector3 startLeftPosition;

    bool isDeforming = false;

    private void Start()
    {
        captureManageInstance = CaptureManager.Instance;
        deformingMesh = GetComponent<MeshFilter>().mesh;
        deformingTrigger = GetComponent<MeshCollider>();
        if (!deformingMesh) Debug.Log("Not a deforming mesh!");
        // Assume the mesh DeformingCollider is initially set to the mesh
        // TODO - Check and enforce ?
        if (!deformingTrigger) Debug.Log("No MeshCollider!");
        originalVertices = deformingMesh.vertices;
        activeVertices = deformingMesh.vertices;
        vertexVelocities = new Vector3[originalVertices.Length];
        displacedVertices = new Vector3[originalVertices.Length];
        for(int i = 0; i < originalVertices.Length; i++)
        {
            displacedVertices[i] = originalVertices[i];
        }

        meshCenter = Vector3.zero;
        foreach (Vector3 v in originalVertices)
        {
            meshCenter += v;
        }
        meshCenter /= originalVertices.Length;
    }

    private void Update()
    {
        uniformScale = transform.localScale.x;
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            UpdateVertex(i);
        }
        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();
        deformingTrigger.sharedMesh = deformingMesh;
    }


    public void StopDeforming()
    {
        for (int i = 0; i < originalVertices.Length; i++)
        {
            originalVertices[i] = displacedVertices[i];
        }
        isDeforming = false;
    }

    /*
     * Impact
     * 
     */ 

    void UpdateVertex (int i)
    {
        Vector3 velocity = vertexVelocities[i];
        Vector3 displacement = displacedVertices[i] - originalVertices[i];
        displacement *= uniformScale;
        velocity -= displacement * springForce * Time.deltaTime;
        velocity *= 1f - damping * Time.deltaTime;
        vertexVelocities[i] = velocity;
        displacedVertices[i] += velocity * (Time.deltaTime / uniformScale);
    }

    public void AddDeformingForce(Vector3 point, float force)
    {
        springForce = captureManageInstance.ImpactSpringForce;
        damping = captureManageInstance.ImpactDamping;
        if (!isDeforming) 
        {
            isDeforming = true;
        }
        //Debug.DrawLine(Camera.main.transform.position, point);
        point = transform.InverseTransformPoint(point);
        for(int i = 0; i < displacedVertices.Length; i++)
        {
            AddForceToVertex(i, point, force);
        }
    }

    void AddForceToVertex (int i, Vector3 point, float force)
    {
        Vector3 pointToVertex = displacedVertices[i] - point;
        pointToVertex *= uniformScale;
        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
        float velocity = attenuatedForce * Time.deltaTime;
        vertexVelocities[i] += pointToVertex.normalized * velocity;
    }

    /*
     * Twist
     * 
     */
    public void AddTwistForce(Vector3 point, float force)
    {
        twistAxis = captureManageInstance.twistAxis;
        twistRadius = captureManageInstance.twistRadius;
        twistRatio = captureManageInstance.twistRatio;

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

    /*
     * Rotation
     * 
     */
    public void rotateVerticalyLeft(Vector3 rightPos, Vector3 leftPos)
    {
        rotationAngleY = captureManageInstance.rotationAngleY;
        if (startRightPosition == null && startLeftPosition == null)
        {
            startRightPosition = rightPos;
            startLeftPosition = leftPos;
        }
        else
        {
            float distX = Mathf.Abs(rightPos.x - startRightPosition.x);

            Debug.Log(distX.ToString());
            if (distX > 0.01f)
            {
                if (leftPos.x < startLeftPosition.x)
                {
                    transform.Rotate(0.0f, -rotationAngleY, 0.0f, Space.Self);
                }
                startRightPosition = rightPos;
                startLeftPosition = leftPos;
            }
        }
    }
    public void rotateVerticalyRight(Vector3 rightPos, Vector3 leftPos)
    {
        rotationAngleY = captureManageInstance.rotationAngleY;
        if (startRightPosition == null && startLeftPosition == null)
        {
            startRightPosition = rightPos;
            startLeftPosition = leftPos;
        }
        else
        {
            float distX = Mathf.Abs(rightPos.x - startRightPosition.x);

            Debug.Log(distX.ToString());
            if (distX > 0.01f)
            {
                if (rightPos.x > startRightPosition.x)
                {
                    transform.Rotate(0.0f, rotationAngleY, 0.0f, Space.Self);
                }
                startRightPosition = rightPos;
                startLeftPosition = leftPos;
            }
        }
    }

    public void rotateHorizontaly(Vector3 rightPos, Vector3 leftPos)
    {
        rotationAngleX = captureManageInstance.rotationAngleX;
        if (startRightPosition == null && startLeftPosition == null)
        {
            startRightPosition = rightPos;
            startLeftPosition = leftPos;
        }
        else
        {
            float distY = Mathf.Abs(rightPos.y - startRightPosition.y);

            Debug.Log(distY.ToString());
            if (distY > 0.01f)
            {
                if (leftPos.y > startLeftPosition.y)
                {
                    transform.Rotate(rotationAngleX, 0.0f, 0.0f, Space.World);
                }
                else
                {
                    transform.Rotate(-rotationAngleX, 0.0f, 0.0f, Space.World);
                }
                startRightPosition = rightPos;
                startLeftPosition = leftPos;
            }
        }
    }

    public void stopRotation(Vector3 rightPos, Vector3 leftPos)
    {
        startRightPosition = rightPos;
        startLeftPosition = leftPos;
    }
}