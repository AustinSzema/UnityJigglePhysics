using UnityEngine;

public class SmoothCameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform targetLookat;
    public float distance = 10f;

    [Header("Camera Controls")]
    public float mouseSensitivity = 2f;
    public float smoothTime = 0.1f;
    public float scrollSensitivity = 2f;
    public float minDistance = 1f;
    public float maxDistance = 20f;
    
    [Header("Movement Limits")]
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 60f;

    // Internal variables
    private Vector2 rotation;
    private Vector3 currentVelocity;
    private Vector3 targetPosition;
    private float currentDistance;

    private void Start()
    {
        // Initialize variables
        if (targetLookat == null)
        {
            Debug.LogError("Camera target not assigned!");
            enabled = false;
            return;
        }

        // Set initial rotation based on current camera orientation
        Vector3 direction = (transform.position - targetLookat.position).normalized;
        float yaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float pitch = -Mathf.Asin(direction.y) * Mathf.Rad2Deg;
        rotation = new Vector2(yaw, pitch);
        
        // Set initial distance
        currentDistance = distance;
        
        // Hide cursor for game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (targetLookat == null) return;

        // Get mouse input
        rotation.x += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotation.y -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Clamp vertical rotation
        rotation.y = Mathf.Clamp(rotation.y, minVerticalAngle, maxVerticalAngle);
        
        // Adjust distance with scroll wheel
        float scrollInput = Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
        currentDistance = Mathf.Clamp(currentDistance - scrollInput, minDistance, maxDistance);
        
        // Calculate rotation quaternion
        Quaternion targetRotation = Quaternion.Euler(rotation.y, rotation.x, 0);
        
        // Calculate target position
        targetPosition = targetLookat.position - targetRotation * Vector3.forward * currentDistance;
        
        // Smoothly move camera
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
        
        // Look at target
        transform.LookAt(targetLookat.position);
    }

    public void SetNewTarget(Transform target)
    {
        targetLookat = target;
    }
}