using KARV;
using UnityEngine;
using System.Collections.Generic;


public class KeyboardInputDetector : MonoBehaviour
{
    CaptureManager captureManagerInstace;

    private void Start()
    {
        captureManagerInstace = CaptureManager.Instance;
    }

    void Update()
    {
        // Loop through numerical keys 0-9
        for (KeyCode key = KeyCode.Alpha0; key <= KeyCode.Alpha9; key++)
        {
            if (Input.GetKeyDown(key))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    // Action when Shift + Numerical key is pressed
                    Debug.Log($"Shift + {key} pressed!");

                }
                else
                {
                    // Action when only the Numerical key is pressed
                    Debug.Log($"{key} pressed!");
                    CaptureManager.DeformationTypes deformation = CaptureManager.DeformationTypes.none;
                    List<KinectInterop.JointType> neededJoints = new List<KinectInterop.JointType>();
                    switch (key)
                    {
                        // Numerical keys
                        case KeyCode.Alpha0:
                            deformation = CaptureManager.DeformationTypes.none;
                            break;
                        case KeyCode.Alpha1:
                            deformation = CaptureManager.DeformationTypes.impact;
                            neededJoints.Add(KinectInterop.JointType.HandLeft);
                            neededJoints.Add(KinectInterop.JointType.HandRight);
                            break;
                        case KeyCode.Alpha2:
                            deformation = CaptureManager.DeformationTypes.impactFPS;
                            neededJoints.Add(KinectInterop.JointType.HandLeft);
                            neededJoints.Add(KinectInterop.JointType.HandRight);
                            break;
                        case KeyCode.Alpha3:
                            deformation = CaptureManager.DeformationTypes.twistingImpact;
                            neededJoints.Add(KinectInterop.JointType.HandLeft);
                            neededJoints.Add(KinectInterop.JointType.HandRight);
                            break;
                        case KeyCode.Alpha4:
                            deformation = CaptureManager.DeformationTypes.twisterStarRail;
                            neededJoints.Add(KinectInterop.JointType.HandLeft);
                            neededJoints.Add(KinectInterop.JointType.HandRight);
                            break;
                        case KeyCode.Alpha5:
                            deformation = CaptureManager.DeformationTypes.rotationPush;
                            neededJoints.Add(KinectInterop.JointType.HandLeft);
                            neededJoints.Add(KinectInterop.JointType.HandRight);
                            break;
                        case KeyCode.Alpha6:
                            deformation = CaptureManager.DeformationTypes.rotationRoast;
                            neededJoints.Add(KinectInterop.JointType.HandLeft);
                            neededJoints.Add(KinectInterop.JointType.HandRight);
                            break;
                        case KeyCode.Alpha7:
                            deformation = CaptureManager.DeformationTypes.zoomIn;
                            neededJoints.Add(KinectInterop.JointType.HandLeft);
                            neededJoints.Add(KinectInterop.JointType.HandRight);
                            break;
                        case KeyCode.Alpha8:
                            deformation = CaptureManager.DeformationTypes.zoomOut;
                            neededJoints.Add(KinectInterop.JointType.HandLeft);
                            neededJoints.Add(KinectInterop.JointType.HandRight);
                            break;
                        case KeyCode.Alpha9:
                            deformation = CaptureManager.DeformationTypes.zoomInOut;
                            neededJoints.Add(KinectInterop.JointType.HandLeft);
                            neededJoints.Add(KinectInterop.JointType.HandRight);
                            break;
                    }

                    captureManagerInstace.ChangeActiveDeformation(deformation, neededJoints);
                }
            }
        }
    }
}
