using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTransformation : MonoBehaviour
{
    public float Speed = 2.0f;
    private bool isRotating = false;

    private Vector2 startMousePosition;

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
            Vector2 currentMousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 mouseMouv = currentMousePos - startMousePosition;

            transform.Rotate(mouseMouv.x, mouseMouv.y, 0);
            startMousePosition = currentMousePos;
        }
    }
}
