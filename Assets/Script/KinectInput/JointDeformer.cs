using KARV;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KARV
{
    public class JointDeformer : MonoBehaviour
    {
        private CaptureManager captureManager;

        private SphereCollider DeformingCollider;
        private MeshRenderer mesh;
        KinectInterop.JointType jointType;

        // Start is called before the first frame update
        void Start()
        {
            DeformingCollider = GetComponent<SphereCollider>();
            if (DeformingCollider == null)
                return; // TODO handle error instead

            mesh = GetComponent<MeshRenderer>();
            if (mesh == null)
                return; // TODO handle error instead

            captureManager = CaptureManager.Instance;
        }
        public KinectInterop.JointType GetJointTpe() { return this.jointType; }
        public void SetJointType(KinectInterop.JointType jointType) { this.jointType = jointType; }
        public void EnableContacts() { DeformingCollider.enabled = true; mesh.enabled = true; }
        public void DisableContacts() { DeformingCollider.enabled = false; mesh.enabled = false; }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.TryGetComponent(out MeshCollider meshCollider) && meshCollider.sharedMesh != null)
            {
                captureManager.handleInput(collision);
            }
        }
        public Vector3 getPosition()
        {
            return this.transform.position;

        }

        public Quaternion getRotation()
        {
            return this.transform.rotation;
        }

    }
}