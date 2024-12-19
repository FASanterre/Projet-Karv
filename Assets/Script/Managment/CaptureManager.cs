// The captureManager is responsible for receiving inputs from
// the controllers, and to send the data to the deformer

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KARV
{
    public class CaptureManager : MonoBehaviour
    {
        private static CaptureManager _instance;
        public static CaptureManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Create a new GameObject and attach the CaptureManager script
                    GameObject singletonObject = new GameObject("CaptureManager");
                    _instance = singletonObject.AddComponent<CaptureManager>();

                    // Optional: Prevent the GameObject from being destroyed when changing scenes
                    DontDestroyOnLoad(singletonObject);
                }
                return _instance;
            }
        }
        public enum DeformationTypes : int
        {
            none =              0,
            impact =            1,
            impactFPS =         2,
            twistingImpact =    3,
            twisterStarRail =   4,
            rotationPush =      5,
            rotationRoast =     6,
            zoomIn =            7,
            zoomOut =           8,
            zoomInOut =         9
        }

        public float ImpactForce { get; set; } = 10f;
        public float ImpactForceOffset { get; set; } = 0.1f;
        public float ImpactSpringForce { get; set; } = 20f;
        public float ImpactDamping { get; set; } = 5f;
        public float twistForce { get; set; } = 10f;
        public Vector3 twistAxis { get; set; } = Vector3.up;
        public float twistRadius { get; set; } = 0.3f;
        public float twistRatio { get; set; } = 2f;

        public float rotationAngleX { get; set; } = 15.0f;
        public float rotationAngleY { get; set; } = 15.0f;
        public float epsilon { get; set; } = 1;
        public float acceptedDiffX { get; set; } = 0.3f;
        public float acceptedDiffY { get; set; } = 0.3f;

        //List<KinectInterop.JointType> activeJoints;
        public List<JointDeformer> DynamicPositionJoints = new List<JointDeformer>();
        public List<JointDeformer> FPSJoints = new List<JointDeformer>();
        public JointDeformer rightHand { get; set; }
        public JointDeformer leftHand { get; set; }

        //public List<MeshDeformer> deformableObjects = new List<MeshDeformer>();
        MeshDeformer activeObject;

        private DeformationTypes activeDeformation = DeformationTypes.none;

        private void Start()
        {
            //activeObject = FindObjectOfType<MeshDeformer>();
            //if (deformableObjects.Count == 0) return;
            //
            //activeObject = deformableObjects[0];
        }

        public void SetActiveObject(MeshDeformer newActiveObject)
        {
            if (newActiveObject == null)
            {
                Debug.LogWarning("Trying to set a null active object.");
                return;
            }

            // Disable the previous active object's GameObject if it exists
            if (activeObject != null && activeObject.gameObject.activeSelf)
            {
                activeObject.gameObject.SetActive(false);
                Debug.Log($"Disabled previous active object: {activeObject.name}");
            }

            // Assign the new active object
            activeObject = newActiveObject;

            // Enable the new active object's GameObject
            if (!activeObject.gameObject.activeSelf)
            {
                activeObject.gameObject.SetActive(true);
                Debug.Log($"Enabled new active object: {activeObject.name}");
            }
        }

        private void Update()
        {
            // Update is solely used for rotation, return if we're not doing that
            if (activeDeformation != DeformationTypes.rotationPush) return;
            if (activeObject == null) return;
            if (rightHand == null) return;
            if (leftHand == null) return;
            if (!rightHand.gameObject.activeSelf) return;
            if (!leftHand.gameObject.activeSelf) return;
            if (rightHand != null && leftHand != null)
            {
                Vector3 rightHandCurrentPos = rightHand.getPosition();
                Vector3 leftHandCurrentPos = leftHand.getPosition();

                float dist = Vector2.Distance(rightHand.getPosition(), leftHand.getPosition());

                if (dist < epsilon)
                {
                    float distX = Mathf.Abs(rightHandCurrentPos.x - leftHandCurrentPos.x);
                    float distY = Mathf.Abs(rightHandCurrentPos.y - leftHandCurrentPos.y);

                    //si la main gauche est au-dessus et verticale
                    if (leftHandCurrentPos.y > rightHandCurrentPos.y && distX < acceptedDiffX)
                    {
                        //rotation vers la gauche
                        activeObject.rotateVerticalyLeft(rightHandCurrentPos, leftHandCurrentPos);
                    }
                    else if (rightHandCurrentPos.y > leftHandCurrentPos.y && distX < acceptedDiffX)
                    {
                        //rotation vers la droite
                        activeObject.rotateVerticalyRight(rightHandCurrentPos, leftHandCurrentPos);
                    }
                    else if ((leftHandCurrentPos.x > rightHandCurrentPos.x && distY < acceptedDiffY) ||
                      (rightHandCurrentPos.x > leftHandCurrentPos.x && distY < acceptedDiffY))
                    {
                        //rotation vers le haut ou le bas
                        activeObject.rotateHorizontaly(rightHandCurrentPos, leftHandCurrentPos);
                    }
                    else
                    {
                        activeObject.stopRotation(rightHandCurrentPos, leftHandCurrentPos);
                    }
                }
            }
        }

        // Set the active deformation and the joints it uses
        // Since we can customize the joints needed at runtime for fineTuning purposes
        // (and we might want to do it at runtime too)
        // We don't hard-assign the joints associated with each deformation
        public void ChangeActiveDeformation(DeformationTypes deformation, List<KinectInterop.JointType> neededJoints)
        {
            activeDeformation = deformation;
            //activeJoints = neededJoints;

            List<JointDeformer> joints = (deformation == DeformationTypes.impactFPS) ? FPSJoints : DynamicPositionJoints;

            if (deformation == DeformationTypes.impactFPS)
                joints = FPSJoints;
            else
                joints = DynamicPositionJoints;

            if (joints == null)
            {
                Debug.LogWarning("Empty Joints List");
                return;
            }
                foreach (JointDeformer joint in joints)
                {
                    if (neededJoints.Contains(joint.GetJointTpe()))
                        joint.EnableContacts();
                    else
                        joint.DisableContacts();
                }
        }

        public void HandleInput()
        {
            switch (activeDeformation)
            {
                case DeformationTypes.none:
                    activeObject.StopDeforming();
                    break;
                default:
                    break;
            }
        }

        // Call this for input that need collision data
        public void handleInput(Collision collision)
        {
            switch (activeDeformation)
            {
                case DeformationTypes.none:
                    break;
                case DeformationTypes.impact:
                    HandleImpact(collision);
                    break;
                case DeformationTypes.impactFPS:
                    break;
                case DeformationTypes.twistingImpact:
                    HandleTwist(collision);
                    break;
                case DeformationTypes.twisterStarRail: 
                    break;
                default: break;
            }
        }

        private void HandleImpact(Collision collision)
        {
            if (collision.collider.TryGetComponent(out MeshDeformer meshDeformer) && meshDeformer == activeObject)
            {
                Vector3 contactCenter = new Vector3();
                Vector3 contactNormal = new Vector3();
                foreach (ContactPoint contact in collision.contacts)
                {
                    contactCenter += contact.point;
                    contactNormal += contact.normal;
                }
                contactCenter /= collision.contacts.Length;
                contactNormal /= collision.contacts.Length;

                Vector3 point = contactCenter + contactNormal * ImpactForceOffset;
                meshDeformer.AddDeformingForce(point, ImpactForce);
            }
        }

        private void HandleTwist(Collision collision)
        {
            if (collision.collider.TryGetComponent(out MeshDeformer meshDeformer) && meshDeformer == activeObject)
            {
                Vector3 contactCenter = new Vector3();
                foreach (ContactPoint contact in collision.contacts)
                {
                    contactCenter += contact.point;
                }
                contactCenter /= collision.contacts.Length;

                float twistDirection = 0f;
                if (collision.collider.TryGetComponent(out JointDeformer jointDeformer))
                {
                    if (jointDeformer == rightHand)
                    {
                        twistDirection = 1f;
                    }
                    else if (jointDeformer == leftHand)
                    {
                        twistDirection = -1f;
                    }

                    if(twistDirection != 0f)
                    {
                        float directionalTwistForce = twistForce * twistDirection;
                        meshDeformer.AddTwistForce(contactCenter, directionalTwistForce);
                    }
                    else
                    {
                        Debug.Log("Twist ignoré : Aucune main détectée");
                    }
                }
            }
        }
    }
}

