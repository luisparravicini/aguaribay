using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float rotationSpeed;
    public float walkSpeed;
    const float minAngle = 0.5f;
    const float minDistance = 0.02f;
    bool rotating;
    bool moving;
    Vector3 dstPos;
    Quaternion dstRotation;

    void Update()
    {
        UpdateMovement();
        ProcessInput();
    }

    void ProcessInput()
    {
        if (rotating || moving)
            return;

        if (Input.GetKeyDown(KeyCode.W))
            Move(1);
        else
        if (Input.GetKeyDown(KeyCode.S))
            Move(-1);
        else
        if (Input.GetKeyDown(KeyCode.A))
            Rotate(-90);
        else
        if (Input.GetKeyDown(KeyCode.D))
            Rotate(90);
    }

    private void UpdateMovement()
    {
        if (rotating)
        {
            var rot = Quaternion.Slerp(transform.rotation, dstRotation, rotationSpeed * Time.deltaTime);
            Debug.Log(dstRotation.eulerAngles.y + "/" + rot.eulerAngles.y + "  " + Mathf.Abs(dstRotation.eulerAngles.y - rot.eulerAngles.y) + " " + minAngle);

            if (Mathf.Abs(dstRotation.eulerAngles.y - rot.eulerAngles.y) <= minAngle)
            {
                rot = dstRotation;
                rotating = false;
            }
            transform.rotation = rot;
        }

        if (moving)
        {
            var pos = Vector3.Lerp(transform.position, dstPos, walkSpeed * Time.deltaTime);
            if (Vector3.Distance(dstPos, pos) <= minDistance)
            {
                pos = dstPos;
                moving = false;
            }
            transform.position = pos;
        }
    }

    private void Move(int delta)
    {
        moving = true;
        dstPos = transform.position + transform.forward * delta;
    }

    private void Rotate(int delta)
    {
        rotating = true;
        dstRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + delta, 0);
    }

}
