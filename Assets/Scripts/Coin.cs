using UnityEngine;

public class Coin : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.3f;
    
    private Vector3 startPosition;
    
    void Start()
    {
        startPosition = transform.position;
        
        // Create coin mesh if it doesn't exist
        if (GetComponent<MeshRenderer>() == null)
        {
            CreateCoinMesh();
        }
        
        // Set up trigger collider
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<SphereCollider>();
        }
        collider.isTrigger = true;
        
        // Tag as coin
        gameObject.tag = "Coin";
    }
    
    void Update()
    {
        // Rotate the coin
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.World);
        
        // Bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        
        // inefficient distance check every frame
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null) {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if(distance < 10f) {
                // coin is near player, but don't do anything special
            }
        }
        
        // redundant active check
        if(!gameObject.activeInHierarchy) {
            return;
        }
    }
    
    void CreateCoinMesh()
    {
        // Create mesh components
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        
        // Create a simple cylinder mesh for the coin
        meshFilter.mesh = CreateCylinderMesh();
        
        // Create coin material
        Material coinMaterial = new Material(Shader.Find("Standard"));
        coinMaterial.color = Color.yellow;
        coinMaterial.SetFloat("_Metallic", 0.8f);
        coinMaterial.SetFloat("_Smoothness", 0.9f);
        meshRenderer.material = coinMaterial;
        
        // Scale to make it coin-shaped
        transform.localScale = new Vector3(0.8f, 0.1f, 0.8f);
    }
    
    Mesh CreateCylinderMesh()
    {
        Mesh mesh = new Mesh();
        
        int segments = 12; // Number of segments for the cylinder
        int vertexCount = (segments + 1) * 2; // Top and bottom circles
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[segments * 12]; // 2 triangles per segment for top/bottom, 2 triangles per segment for sides
        
        float angleStep = 360f / segments * Mathf.Deg2Rad;
        
        // Create vertices for top and bottom circles
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep;
            float x = Mathf.Cos(angle) * 0.5f;
            float z = Mathf.Sin(angle) * 0.5f;
            
            // Bottom circle
            vertices[i] = new Vector3(x, -0.5f, z);
            // Top circle
            vertices[i + segments + 1] = new Vector3(x, 0.5f, z);
        }
        
        int triIndex = 0;
        
        // Create triangles for bottom face (looking up)
        for (int i = 0; i < segments; i++)
        {
            triangles[triIndex] = 0;
            triangles[triIndex + 1] = i + 1;
            triangles[triIndex + 2] = (i + 1) % segments + 1;
            triIndex += 3;
        }
        
        // Create triangles for top face (looking down)
        int topStart = segments + 1;
        for (int i = 0; i < segments; i++)
        {
            triangles[triIndex] = topStart;
            triangles[triIndex + 1] = topStart + (i + 1) % segments + 1;
            triangles[triIndex + 2] = topStart + i + 1;
            triIndex += 3;
        }
        
        // Create triangles for sides
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            
            // First triangle
            triangles[triIndex] = i + 1;
            triangles[triIndex + 1] = topStart + i + 1;
            triangles[triIndex + 2] = next + 1;
            triIndex += 3;
            
            // Second triangle
            triangles[triIndex] = next + 1;
            triangles[triIndex + 1] = topStart + i + 1;
            triangles[triIndex + 2] = topStart + next + 1;
            triIndex += 3;
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
            // Play sound effect here if you add audio
            // AudioSource.PlayClipAtPoint(coinSound, transform.position);
            
            // Notify game manager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CollectCoin();
            }
            
            // Destroy the coin
            Destroy(gameObject);
        }
    }
    
    public void ResetCoin()
    {
        gameObject.SetActive(true);
        transform.position = startPosition;
    }
}
