using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRotation : MonoBehaviour
{
    public float rotationSpeed = 2.0f;
    private bool isRotating = false;
    public float rotationAngleX = 15.0f;
    public float rotationAngleY = 15.0f;

    private Vector3 startRightPosition;
    private Vector3 startLeftPosition;

    private Vector3 startMousePosition;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isRotating = true;
            startMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isRotating = false;
        }


        if (isRotating)
        {
            Vector3 currentMousePos = Input.mousePosition;

            float direction = Vector3.Angle(startMousePosition, currentMousePos);

            Vector3 vector = currentMousePos - startMousePosition;
            Vector3 axis = Vector3.one;

            float component = Vector3.Dot(vector, axis.normalized);

            Vector3 proj = axis.normalized * component;

            transform.Rotate(proj, direction);

            startMousePosition = currentMousePos;
        }
    }

    public void rotateVerticalyLeft(Vector3 rightPos, Vector3 leftPos)
    {
        if (startRightPosition == null && startLeftPosition == null)
        {
            startRightPosition = rightPos;
            startLeftPosition = leftPos;
        }
        else
        {
            float distX = Mathf.Abs(rightPos.x - startRightPosition.x);

            Debug.Log(distX.ToString()) ;
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
                    transform.Rotate(rotationAngleX,0.0f , 0.0f, Space.World);
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
