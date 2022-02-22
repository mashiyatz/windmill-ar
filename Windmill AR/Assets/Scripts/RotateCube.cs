using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCube : MonoBehaviour
{
    public float rotationSpeed = 10.0f;

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, 2 * rotationSpeed * Time.deltaTime, 3 * rotationSpeed * Time.deltaTime);        
    }
}
