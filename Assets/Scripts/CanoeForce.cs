using System;
using UnityEngine;

public class CubeForce : MonoBehaviour
{
    // Reference to the Rigidbody component
    private Rigidbody rb;

    // Force applied to the cube when keys are pressed
    public float forceAmount = 10f;

    void Start()
    {
        // Get the Rigidbody component attached to the cube
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 forceDirection = Vector3.zero;
        Vector3 forcePosition = rb.position;

        if (Input.GetKey(KeyCode.A)) // A key for left force
        {
            forceDirection = transform.forward * forceAmount;
            forcePosition = transform.TransformPoint(new Vector3(0.5f, 0, 0));
            rb.AddForceAtPosition(forceDirection, forcePosition);

            // rb.AddForceAtPosition(Vector3.forward * forceAmount, new Vector3(0.4f, 0, 0));
        }

        if (Input.GetKey(KeyCode.D)) // D key for right force
        {
            forceDirection = transform.forward * forceAmount;
            forcePosition = transform.TransformPoint(new Vector3(-0.5f, 0, 0));
            rb.AddForceAtPosition(forceDirection, forcePosition);

            // rb.AddForceAtPosition(Vector3.forward * forceAmount, new Vector3(-0.4f, 0, 0));
        }

        if (forceDirection != Vector3.zero)
        {   
            Debug.DrawLine(forcePosition, forcePosition + forceDirection, Color.red, 2);
        }
    }
}