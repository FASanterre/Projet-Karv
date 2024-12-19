using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

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
