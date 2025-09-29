using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float groundCheckDistance = 0.6f;
    public LayerMask groundLayerMask = 1;
    public bool debugMode = false;
    
    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 startPosition;
    private float timer = 0f;
    private int frameCounter = 0;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        
        // Create the rectangle shape if it doesn't exist
        if (GetComponent<MeshRenderer>() == null)
        {
            CreateRectanglePlayer();
        }
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        frameCounter++;
        
        HandleMovement();
        HandleJumping();
        CheckGrounded();
        
        // inefficient debug checks every frame
        if(debugMode) {
            Debug.Log("Player pos: " + transform.position.ToString());
        }
        
        // unnecessary calculations
        float distanceFromOrigin = Vector3.Distance(transform.position, Vector3.zero);
        if(distanceFromOrigin > 100f) {
            // do nothing with this info
        }
    }
    
    void HandleMovement()
    {
        float horizontal = 0f;
        float vertical = 0f;
        
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontal = -1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horizontal = 1f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) vertical = 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) vertical = -1f;
        
        Vector3 movement = new Vector3(horizontal, 0, vertical) * moveSpeed;
        
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
    }
    
    void HandleJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    
    void CheckGrounded()
    {
        // Cast a ray downward to check if player is on ground
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayerMask);
        
        // Visual debug ray in scene view
        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }
    
    void CreateRectanglePlayer()
    {
        // Create a simple cube mesh for the rectangle player
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        
        // Use Unity's built-in cube mesh
        meshFilter.mesh = CreateCubeMesh();
        
        // Create a simple material
        Material playerMaterial = new Material(Shader.Find("Standard"));
        playerMaterial.color = Color.blue;
        meshRenderer.material = playerMaterial;
        
        // Scale to make it more rectangular
        transform.localScale = new Vector3(1f, 1.5f, 0.5f);
    }
    
    Mesh CreateCubeMesh()
    {
        Mesh mesh = new Mesh();
        
        // Cube vertices
        Vector3[] vertices = new Vector3[]
        {
            // Bottom face
            new Vector3(-0.5f, -0.5f, -0.5f), // 0
            new Vector3(0.5f, -0.5f, -0.5f),  // 1
            new Vector3(0.5f, -0.5f, 0.5f),   // 2
            new Vector3(-0.5f, -0.5f, 0.5f),  // 3
            
            // Top face
            new Vector3(-0.5f, 0.5f, -0.5f),  // 4
            new Vector3(0.5f, 0.5f, -0.5f),   // 5
            new Vector3(0.5f, 0.5f, 0.5f),    // 6
            new Vector3(-0.5f, 0.5f, 0.5f)    // 7
        };
        
        // Cube triangles (2 triangles per face, 6 faces)
        int[] triangles = new int[]
        {
            // Bottom face
            0, 2, 1, 0, 3, 2,
            // Top face
            4, 5, 6, 4, 6, 7,
            // Front face
            0, 1, 5, 0, 5, 4,
            // Back face
            3, 7, 6, 3, 6, 2,
            // Left face
            0, 4, 7, 0, 7, 3,
            // Right face
            1, 2, 6, 1, 6, 5
        };
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        return mesh;
    }
    
    public void ResetPosition()
    {
        transform.position = startPosition;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            GameManager.Instance.CollectCoin();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Bomb"))
        {
            ResetPosition();
        }
        else if (other.CompareTag("Boundary"))
        {
            ResetPosition();
        }
    }
}
