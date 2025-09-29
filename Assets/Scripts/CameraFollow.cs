using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 12f, -8f);
    public float smoothSpeed = 2f;
    public bool lookAtPlayer = true;
    
    [Header("Rotation Settings")]
    public float rotationSpeed = 2f;
    public Vector2 rotationRange = new Vector2(-30f, 80f);
    
    private Vector3 velocity;
    private float currentRotationX;
    
    void Start()
    {
        // Find player if not assigned
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
        
        // Set initial rotation
        currentRotationX = transform.eulerAngles.x;
        if (currentRotationX > 180f) currentRotationX -= 360f;
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        HandleCameraMovement();
        HandleCameraRotation();
    }
    
    void HandleCameraMovement()
    {
        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;
        
        // Smoothly move to desired position
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
    
    void HandleCameraRotation()
    {
        if (lookAtPlayer)
        {
            // Look at the player with smooth rotation
            Vector3 direction = target.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            // Handle manual camera rotation with mouse (optional)
            if (Input.GetMouseButton(1)) // Right mouse button
            {
                float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
                float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
                
                // Rotate around Y axis (horizontal)
                transform.RotateAround(target.position, Vector3.up, mouseX);
                
                // Rotate around X axis (vertical) with constraints
                currentRotationX -= mouseY;
                currentRotationX = Mathf.Clamp(currentRotationX, rotationRange.x, rotationRange.y);
                
                Vector3 eulerAngles = transform.eulerAngles;
                eulerAngles.x = currentRotationX;
                transform.eulerAngles = eulerAngles;
                
                // Update offset based on new position
                offset = transform.position - target.position;
            }
        }
    }
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
}
