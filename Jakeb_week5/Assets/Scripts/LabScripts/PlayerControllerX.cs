using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    Rigidbody rb;
    public float movespeed = 5f;
    [SerializeField] GameObject focalPoint;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        HandleMovement();
        CheckRespawn();
    }

    void HandleMovement() {
        float forwardInput = Input.GetAxis("Vertical");
        rb.AddForce(focalPoint.transform.forward * movespeed *  forwardInput);
    }

    void CheckRespawn() {
        if (transform.position.y < -6) {
            RespawnAtFocalPoint();
        }
    }

    void RespawnAtFocalPoint() {
        transform.position = focalPoint.transform.position;
        rb.velocity = Vector3.zero;  //reset velocity
    }
}