using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBasic : MonoBehaviour
{
    [SerializeField]
    float movementSpeed = 40f;

    [SerializeField]
    float rotationSpeed = 230f;


    float forwardVel = 0f;

    float rotationVel = 0f;

    void Update()
    {
        forwardVel = Input.GetAxis("Vertical") * movementSpeed;
        rotationVel= Input.GetAxis("Horizontal") * rotationSpeed;
    }

    private void FixedUpdate()
    {
        transform.Translate(transform.forward * forwardVel * Time.fixedDeltaTime, Space.World);
        transform.Rotate(Vector3.up * rotationVel * Time.fixedDeltaTime, Space.World);
    }
}
