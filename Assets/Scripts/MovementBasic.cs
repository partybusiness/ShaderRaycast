using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBasic : MonoBehaviour
{
    [SerializeField]
    float movementSpeed = 40f;

    [SerializeField]
    float rotationSpeed = 230f;


    [SerializeField]
    Texture2D map;

    float forwardVel = 0f;

    float rotationVel = 0f;

    int storedX = 0;
    int storedY = 0;

    void Update()
    {
        forwardVel = Input.GetAxis("Vertical") * movementSpeed;
        rotationVel= Input.GetAxis("Horizontal") * rotationSpeed;
    }

    private void FixedUpdate()
    {
        var forwardDirection = transform.forward * forwardVel * Time.fixedDeltaTime;
        var goalX = Mathf.FloorToInt(transform.position.x + forwardDirection.x) + 1;
        var goalY = Mathf.FloorToInt(transform.position.z + forwardDirection.z) + 1;

        if (goalX!=storedX || goalY !=storedY)
        {
            var targetSample = map.GetPixel(goalX, goalY);
            if (targetSample.r >0.1)
            {
                forwardDirection.x = 0;
                forwardDirection.z = 0; //could make this fancier so you can slide along the wall?
            } else
            {
                storedX = goalX;
                storedY = goalY;
            }            
        }
        transform.Translate(forwardDirection, Space.World);

        //Debug.Log("Sampling "+targetSample.ToString("F2") + ", " + Mathf.FloorToInt(forwardDirection.x) + ", " + Mathf.FloorToInt(forwardDirection.z));
        
        transform.Rotate(Vector3.up * rotationVel * Time.fixedDeltaTime, Space.World);
    }
}
