using UnityEngine;

public class MeshDeformerCameraInput : MonoBehaviour
{
    public float force = 10f;
    public float forceOffset = 0.1f;

    private void Update()
    {
        if(Input.GetMouseButton(0))
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
            MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();
            if(deformer)
            {
                Debug.Log("Is a deformer");
                Vector3 point = hit.point;
                point += hit.normal * forceOffset;
                deformer.AddDeformingForce(point, force);
            }
            else
            {
                Debug.Log("Not a deformer");
            }
        }
    }
}
