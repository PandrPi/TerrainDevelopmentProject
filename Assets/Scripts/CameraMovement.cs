using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float sensitivity = 10f;
    [SerializeField] private float maxYAngle = 80f;

    private Transform myTransform;
    private Vector2 currentRotation;
    void Start()
    {
        myTransform = transform;
    }


    void Update()
    {
        float deltaTime = Time.deltaTime;
        float speedMultiplier = Input.GetKey(KeyCode.LeftShift) ? 2 : 1;
        float x = Input.GetAxis("Horizontal")   * movementSpeed * speedMultiplier * deltaTime;
        float z = Input.GetAxis("Depthical")    * movementSpeed * speedMultiplier * deltaTime;
        float y = Input.GetAxis("Vertical")     * movementSpeed * speedMultiplier * deltaTime;

        myTransform.position += myTransform.forward * z + myTransform.right * x + transform.up * y;

        if (Input.GetMouseButton(0))
        {
            currentRotation.x += Input.GetAxis("Mouse X") * sensitivity;
            currentRotation.y -= Input.GetAxis("Mouse Y") * sensitivity;
            currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
            currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);
        }
        myTransform.rotation = Quaternion.Lerp(myTransform.rotation,
                Quaternion.Euler(currentRotation.y, currentRotation.x, 0), deltaTime * 5);
    }
}
