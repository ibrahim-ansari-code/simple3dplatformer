using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Animation Settings")]
    public float pulseSpeed = 3f;
    public float pulseAmount = 0.2f;
    
    private Vector3 originalScale;
    private MeshRenderer meshRenderer;
    
    void Start()
    {
        originalScale = transform.localScale;
        
        // Create bomb mesh if it doesn't exist
        if (GetComponent<MeshRenderer>() == null)
        {
            CreateBombMesh();
        }
        
        // Set up trigger collider
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<SphereCollider>();
        }
        collider.isTrigger = true;
        
        // Tag as bomb
        gameObject.tag = "Bomb";
    }
    
    void Update()
    {
        // Pulsing animation
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = originalScale * pulse;
    }
    
    void CreateBombMesh()
    {
        // Create mesh components
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        
        // Create a sphere mesh for the bomb
        meshFilter.mesh = CreateSphereMesh();
        
        // Create bomb material
        Material bombMaterial = new Material(Shader.Find("Standard"));
        bombMaterial.color = Color.red;
        bombMaterial.SetFloat("_Metallic", 0.3f);
        bombMaterial.SetFloat("_Smoothness", 0.7f);
        
        // Add emission for glowing effect
        bombMaterial.EnableKeyword("_EMISSION");
        bombMaterial.SetColor("_EmissionColor", Color.red * 0.5f);
        
        meshRenderer.material = bombMaterial;
        
        // Set appropriate scale
        transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        originalScale = transform.localScale;
    }
    
    Mesh CreateSphereMesh()
    {
        Mesh mesh = new Mesh();
        
        int segments = 16;
        int rings = 12;
        
        int vertexCount = (rings + 1) * (segments + 1);
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[rings * segments * 6];
        
        int vertexIndex = 0;
        
        // Generate vertices
        for (int ring = 0; ring <= rings; ring++)
        {
            float ringAngle = Mathf.PI * ring / rings;
            float y = Mathf.Cos(ringAngle) * 0.5f;
            float ringRadius = Mathf.Sin(ringAngle) * 0.5f;
            
            for (int segment = 0; segment <= segments; segment++)
            {
                float segmentAngle = 2 * Mathf.PI * segment / segments;
                float x = Mathf.Cos(segmentAngle) * ringRadius;
                float z = Mathf.Sin(segmentAngle) * ringRadius;
                
                vertices[vertexIndex] = new Vector3(x, y, z);
                vertexIndex++;
            }
        }
        
        // Generate triangles
        int triangleIndex = 0;
        for (int ring = 0; ring < rings; ring++)
        {
            for (int segment = 0; segment < segments; segment++)
            {
                int current = ring * (segments + 1) + segment;
                int next = current + 1;
                int below = (ring + 1) * (segments + 1) + segment;
                int belowNext = below + 1;
                
                // First triangle
                triangles[triangleIndex] = current;
                triangles[triangleIndex + 1] = below;
                triangles[triangleIndex + 2] = next;
                triangleIndex += 3;
                
                // Second triangle
                triangles[triangleIndex] = next;
                triangles[triangleIndex + 1] = below;
                triangles[triangleIndex + 2] = belowNext;
                triangleIndex += 3;
            }
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        return mesh;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Get player controller and reset position
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ResetPosition();
            }
            
            // Optional: Add explosion effect here
            CreateExplosionEffect();
            
            // Play sound effect here if you add audio
            // AudioSource.PlayClipAtPoint(explosionSound, transform.position);
            
            Debug.Log("Player hit bomb! Resetting position.");
        }
    }
    
    void CreateExplosionEffect()
    {
        // Simple particle-like effect using small spheres
        for (int i = 0; i < 8; i++)
        {
            GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            particle.transform.position = transform.position;
            particle.transform.localScale = Vector3.one * 0.1f;
            
            // Remove collider from particle
            Destroy(particle.GetComponent<Collider>());
            
            // Add random velocity
            Rigidbody particleRb = particle.AddComponent<Rigidbody>();
            Vector3 randomDirection = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0f, 1f),
                Random.Range(-1f, 1f)
            ).normalized;
            particleRb.AddForce(randomDirection * Random.Range(3f, 8f), ForceMode.Impulse);
            
            // Make particle red
            particle.GetComponent<MeshRenderer>().material.color = Color.red;
            
            // Destroy particle after 2 seconds
            Destroy(particle, 2f);
        }
    }
}
