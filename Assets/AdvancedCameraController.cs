using UnityEngine;

public class AdvancedCameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform targetLookat;
    public Vector3 offsetFromTarget = new Vector3(0, 1.7f, 0);
    public float baseCameraDistance = 10f;

    [Header("Camera Controls")]
    public float mouseSensitivity = 100f;
    public float scrollSensitivity = 2f;
    public float minDistance = 1f;
    public float maxDistance = 20f;
    
    [Header("Movement Limits")]
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 60f;
    
    [Header("Spring System")]
    [Range(0, 10)] public float positionSpringStrength = 1.5f;
    [Range(0, 1)] public float positionDamping = 0.75f;
    [Range(0, 20)] public float rotationSpringStrength = 5f;
    [Range(0, 1)] public float rotationDamping = 0.9f;
    
    // Internal variables
    private float rotationX, rotationY;
    private float currentDistance;
    private Vector3 positionVelocity = Vector3.zero;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Vector3 rotationVelocity = Vector3.zero;
    
    // Cache for performance
    private Transform cachedTransform;
    private Camera mainCamera;
    
    private void Awake()
    {
        cachedTransform = transform;
        mainCamera = GetComponent<Camera>();
        
        if (mainCamera == null)
        {
            Debug.LogError("Camera component missing!");
            enabled = false;
            return;
        }
        
        // Initialize the camera starting position
        if (targetLookat != null)
        {
            // Calculate initial angles based on position
            Vector3 direction = (cachedTransform.position - targetLookat.position).normalized;
            rotationX = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            rotationY = -Mathf.Asin(direction.y) * Mathf.Rad2Deg;
            
            // Set initial distance
            currentDistance = baseCameraDistance;
        }
        else
        {
            Debug.LogError("Target not assigned to camera!");
            enabled = false;
        }
        
        // Hide cursor for game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void Update()
    {
        if (targetLookat == null) return;
        
        // Get input with consistent time scaling
        float timeScale = Time.deltaTime * 60f; // Normalize for 60 FPS
        
        // Mouse look
        rotationX += Input.GetAxis("Mouse X") * mouseSensitivity * 0.01f * timeScale;
        rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivity * 0.01f * timeScale;
        
        // Clamp vertical angle
        rotationY = Mathf.Clamp(rotationY, minVerticalAngle, maxVerticalAngle);
        
        // Handle zoom
        float scrollInput = Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
        currentDistance = Mathf.Clamp(currentDistance - scrollInput * timeScale, minDistance, maxDistance);
        
        // Compute target orientation and position
        UpdateCameraTargets();
    }
    
    private void FixedUpdate()
    {
        if (targetLookat == null) return;
        
        // Apply spring system
        ApplySpringSystem();
    }
    
    private void UpdateCameraTargets()
    {
        // Calculate target rotation
        targetRotation = Quaternion.Euler(rotationY, rotationX, 0);
        
        // Calculate the offset point of the target (usually head height)
        Vector3 targetPoint = targetLookat.position + offsetFromTarget;
        
        // Calculate the camera position based on rotation and distance
        targetPosition = targetPoint - targetRotation * Vector3.forward * currentDistance;
    }
    
    private void ApplySpringSystem()
    {
        float dt = Time.fixedDeltaTime;
        
        // Position spring physics
        Vector3 positionError = targetPosition - cachedTransform.position;
        Vector3 springForce = positionError * positionSpringStrength;
        
        // Apply damping to the velocity
        positionVelocity *= Mathf.Pow(positionDamping, dt * 60f);
        
        // Accelerate by the spring force
        positionVelocity += springForce * dt;
        
        // Apply velocity to position
        cachedTransform.position += positionVelocity * dt;
        
        // Look-at spring (for smooth rotation)
        Vector3 targetPoint = targetLookat.position + offsetFromTarget;
        Vector3 directionToTarget = (targetPoint - cachedTransform.position).normalized;
        Quaternion targetLook = Quaternion.LookRotation(directionToTarget);
        
        // Apply rotation directly to avoid overshooting
        cachedTransform.rotation = Quaternion.Slerp(cachedTransform.rotation, targetLook, rotationSpringStrength * dt);
    }
    
    public void SetNewTarget(Transform target)
    {
        targetLookat = target;
    }
    
    // Optional: Visualize the camera targets in the editor
    private void OnDrawGizmosSelected()
    {
        if (targetLookat != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(targetLookat.position + offsetFromTarget, 0.2f);
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetLookat.position + offsetFromTarget);
        }
    }
}