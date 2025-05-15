using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 vel;
    public Transform targetLookat;

    public void SetNewTarget(Transform target)
    {
        targetLookat = target;
    }

    public float distance = 10f;
    private Vector2 offset;

    public void Simulate()
    {
        offset += new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y")) * 2f;

        /*
        if (Input.GetButton("Fire1") || Input.GetButton("Fire2"))
        {
            offset += new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y")) * 2f;
        }
        */
        /*
        else
        {
            //offset += new Vector2(Input.GetAxis("Horizontal"), 0f)*2f;
        }
        */

        transform.rotation = Quaternion.AngleAxis(offset.x, Vector3.up) * Quaternion.AngleAxis(offset.y, Vector3.right);
        transform.position = Vector3.SmoothDamp(transform.position,
            targetLookat.position - transform.forward * distance, ref vel, Time.deltaTime);
        transform.rotation =
            Quaternion.LookRotation((targetLookat.position - transform.position).normalized, Vector3.up);
        distance -= Input.GetAxis("Mouse ScrollWheel");
        distance = Mathf.Max(distance, 1f);
    }

    /*private void Update()
    {
        Simulate();
    }*/
    public void LateUpdate()
    {
        Simulate();
    }
    
    // public void FixedUpdate()
    // {
    //     Simulate();
    // }
}