using KARV;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationManager : MonoBehaviour
{
    CaptureManager captureManager;

    private HandRotation rightHand;
    private HandRotation leftHand;

    private MeshRotation meshRotation;

    //threshold
    float epsilon = 1;
    float acceptedDiffX = 0.3f;
    float acceptedDiffY = 0.3f;


    // singleton instance of the class
    private static RotationManager _instance;
    public static RotationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Create a new GameObject and attach the CaptureManager script
                GameObject singletonObject = new GameObject("CaptureManager");
                _instance = singletonObject.AddComponent<RotationManager>();

                // Optional: Prevent the GameObject from being destroyed when changing scenes
                DontDestroyOnLoad(singletonObject);
            }
            return _instance;
        }
    }

    private void Start()
    {
        captureManager = CaptureManager.Instance;
    }

    public void setRightHand(HandRotation rotationController)
    {
        rightHand = rotationController;
    }

    public void setLeftHand(HandRotation rotationController)
    {
        leftHand = rotationController;
    }

    private void Update()
    {
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
                    meshRotation.rotateVerticalyLeft(rightHandCurrentPos, leftHandCurrentPos);
                } else if (rightHandCurrentPos.y > leftHandCurrentPos.y && distX < acceptedDiffX)
                {
                    //rotation vers la droite
                    meshRotation.rotateVerticalyRight(rightHandCurrentPos, leftHandCurrentPos);
                }
                else if ((leftHandCurrentPos.x > rightHandCurrentPos.x && distY < acceptedDiffY) ||
                  (rightHandCurrentPos.x > leftHandCurrentPos.x && distY < acceptedDiffY))
                {
                    //rotation vers le haut ou le bas
                    meshRotation.rotateHorizontaly(rightHandCurrentPos, leftHandCurrentPos);
                }
                else
                {
                    meshRotation.stopRotation(rightHandCurrentPos, leftHandCurrentPos);
                }
            }
        }

    }
}
