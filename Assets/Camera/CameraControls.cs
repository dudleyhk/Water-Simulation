/*
 
 
 
 */
using System;
using UnityEngine;




public class CameraControls : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float normalMoveSpeed = 500;
    public float climbSpeed = 500;
    public float smoothMotion = 10f;
    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3;


    private float xRot = 0f;
    private float yRot = 0f;


    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Cursor.visible = false;
            CameraMove();
            CameraLook();
        }
        else
        {
            Cursor.visible = true;
        }
    }



    private void CameraMove()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        else
        {
            transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.position += transform.up * climbSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            transform.position -= transform.up * climbSpeed * Time.deltaTime;
        }
    }



    private void CameraLook()
    {
        xRot += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        yRot -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        var fromRotation = transform.rotation;
        var toRotation = Quaternion.Euler(yRot, xRot, 0);

        transform.rotation = Quaternion.Lerp(fromRotation, toRotation, Time.deltaTime * smoothMotion);

    }
}