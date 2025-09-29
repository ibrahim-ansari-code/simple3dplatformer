using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    
    private Rigidbody rb;
    private bool isGrounded = true;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
    }
    
    void Update()
    {
        // Movement without Input class - use transform directly
        float h = 0f;
        float v = 0f;
        
        // Check if specific keys are held using the new system
        try {
            h = UnityEngine.InputSystem.Keyboard.current?.aKey.isPressed == true ? -1f : 0f;
            h += UnityEngine.InputSystem.Keyboard.current?.dKey.isPressed == true ? 1f : 0f;
            v = UnityEngine.InputSystem.Keyboard.current?.wKey.isPressed == true ? 1f : 0f;
            v += UnityEngine.InputSystem.Keyboard.current?.sKey.isPressed == true ? -1f : 0f;
            
            if (UnityEngine.InputSystem.Keyboard.current?.spaceKey.wasPressedThisFrame == true && isGrounded) {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
            }
        } catch {
            // Fallback - just move automatically for testing
            h = Mathf.Sin(Time.time * 0.5f);
            v = Mathf.Cos(Time.time * 0.3f);
        }
        
        Vector3 movement = new Vector3(h, 0, v) * moveSpeed;
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
        
        // Simple ground check
        if (transform.position.y < 0.6f) {
            isGrounded = true;
        }
    }
}
