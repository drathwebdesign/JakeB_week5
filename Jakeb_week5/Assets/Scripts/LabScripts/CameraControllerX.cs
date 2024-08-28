using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerX : MonoBehaviour
{
    float rotationSpeed = 50f;


    void Start()
    {
        
    }

    void Update()
    {
        CameraRotation();
    }

    void CameraRotation() {
        //check for horizontal input
        float horizontalInput = Input.GetAxis("Horizontal");
        //rotate camera
        transform.Rotate(Vector3.up * rotationSpeed * horizontalInput * Time.deltaTime);
    }
}
