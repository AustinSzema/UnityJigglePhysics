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

    private float initialDistance = 10f;

    private void Start()
    {
        initialDistance = distance;
    }

    public void Simulate()
    {


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
        
    }

    private void Update()
    {
        offset += new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y")) * 2f;
        distance -= Input.GetAxis("Mouse ScrollWheel") * 5f;
        distance = Mathf.Clamp(distance, 2f, initialDistance);
        distance = Mathf.Max(distance, 1f);
    }
    public void LateUpdate()
    {
        Simulate();
    }
    
    // public void FixedUpdate()
    // {
    //     Simulate();
    // }
}