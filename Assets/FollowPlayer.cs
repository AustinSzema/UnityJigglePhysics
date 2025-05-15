using System;
using UnityEngine;
using UnityEngine.Serialization;

public class FollowPlayer : MonoBehaviour
{
    [FormerlySerializedAs("targetLookat")] public Transform target;


    private Vector3 offset;
    
    private void Start()
    {
        offset = transform.position - target.position;
    }
    

    public float rotationSpeed = 5f;    // Speed of the mouse-based rotation.
    public float minYAngle = -35f;      // Min vertical angle.
    public float maxYAngle = 60f;       // Max vertical angle.
    public float followSmoothness = 10f;

    private float currentYaw = 0f;
    private float currentPitch = 20f;

    void LateUpdate()
    {
        if (!target) return;

        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = -Input.GetAxis("Mouse Y") * rotationSpeed;

        currentYaw += mouseX;
        currentPitch = Mathf.Clamp(currentPitch + mouseY, minYAngle, maxYAngle);

        // Create rotation
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);

        // Desired position
        Vector3 desiredPosition = target.position + rotation * offset;

        // Smoothly move camera to desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSmoothness * Time.deltaTime);

        // Always look at the player
        transform.LookAt(target.position + Vector3.up * 1.5f); // Offset the look point slightly above the target's origin
    }
}