using UnityEngine;
using System.Collections.Generic;

public class LevelSetup : MonoBehaviour
{
    [Header("Level Settings")]
    public int levelWidth = 20;
    public int levelDepth = 20;
    public int levelHeight = 10;
    
    [Header("Platform Settings")]
    public int numberOfPlatforms = 8;
    public Vector2 platformSizeRange = new Vector2(2f, 5f);
    public Vector2 platformHeightRange = new Vector2(1f, 8f);
    
    [Header("Pickups")]
    public int numberOfCoins = 10;
    public int numberOfBombs = 5;
    
    private List<GameObject> coins = new List<GameObject>();
    private List<GameObject> bombs = new List<GameObject>();
    private List<Vector3> coinStartPositions = new List<Vector3>();
    private List<Vector3> bombStartPositions = new List<Vector3>();
    
    void Start()
    {
        CreateLevel();
    }
    
    void CreateLevel()
    {
        CreateGroundPlane();
        CreateBoundaries();
        CreatePlatforms();
        CreatePlayer();
        CreateCoins();
        CreateBombs();
        CreateCamera();
        CreateLighting();
    }
    
    void CreateGroundPlane()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "Ground";
        ground.transform.position = new Vector3(0, -0.5f, 0);
        ground.transform.localScale = new Vector3(levelWidth, 1f, levelDepth);
        
        // Create ground material
        Material groundMaterial = new Material(Shader.Find("Standard"));
        groundMaterial.color = new Color(0.4f, 0.3f, 0.2f); // Brown ground
        ground.GetComponent<MeshRenderer>().material = groundMaterial;
        
        // Add to ground layer
        ground.layer = 0; // Default layer for ground detection
    }
    
    void CreateBoundaries()
    {
        // Create invisible boundaries around the level
        float boundaryHeight = levelHeight;
        float boundaryThickness = 1f;
        
        // North boundary
        CreateBoundary(new Vector3(0, boundaryHeight/2, levelDepth/2 + boundaryThickness/2), 
                      new Vector3(levelWidth + boundaryThickness*2, boundaryHeight, boundaryThickness));
        
        // South boundary
        CreateBoundary(new Vector3(0, boundaryHeight/2, -levelDepth/2 - boundaryThickness/2), 
                      new Vector3(levelWidth + boundaryThickness*2, boundaryHeight, boundaryThickness));
        
        // East boundary
        CreateBoundary(new Vector3(levelWidth/2 + boundaryThickness/2, boundaryHeight/2, 0), 
                      new Vector3(boundaryThickness, boundaryHeight, levelDepth));
        
        // West boundary
        CreateBoundary(new Vector3(-levelWidth/2 - boundaryThickness/2, boundaryHeight/2, 0), 
                      new Vector3(boundaryThickness, boundaryHeight, levelDepth));
        
        // Bottom boundary (kill plane)
        CreateBoundary(new Vector3(0, -5f, 0), 
                      new Vector3(levelWidth + 20, 1f, levelDepth + 20));
    }
    
    void CreateBoundary(Vector3 position, Vector3 scale)
    {
        GameObject boundary = GameObject.CreatePrimitive(PrimitiveType.Cube);
        boundary.name = "Boundary";
        boundary.transform.position = position;
        boundary.transform.localScale = scale;
        boundary.tag = "Boundary";
        
        // Make boundaries invisible but keep colliders
        MeshRenderer renderer = boundary.GetComponent<MeshRenderer>();
        renderer.enabled = false;
        
        // Make collider a trigger for bottom boundary, solid for walls
        Collider collider = boundary.GetComponent<Collider>();
        if (position.y < 0) // Bottom boundary
        {
            collider.isTrigger = true;
        }
    }
    
    void CreatePlatforms()
    {
        for (int i = 0; i < numberOfPlatforms; i++)
        {
            CreatePlatform();
        }
    }
    
    void CreatePlatform()
    {
        GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platform.name = "Platform";
        
        // Random size
        float width = Random.Range(platformSizeRange.x, platformSizeRange.y);
        float height = 0.5f;
        float depth = Random.Range(platformSizeRange.x, platformSizeRange.y);
        platform.transform.localScale = new Vector3(width, height, depth);
        
        // Random position
        float x = Random.Range(-levelWidth/2 + width/2, levelWidth/2 - width/2);
        float y = Random.Range(platformHeightRange.x, platformHeightRange.y);
        float z = Random.Range(-levelDepth/2 + depth/2, levelDepth/2 - depth/2);
        platform.transform.position = new Vector3(x, y, z);
        
        // Platform material
        Material platformMaterial = new Material(Shader.Find("Standard"));
        platformMaterial.color = new Color(0.6f, 0.6f, 0.6f); // Gray platform
        platform.GetComponent<MeshRenderer>().material = platformMaterial;
    }
    
    void CreatePlayer()
    {
        GameObject player = new GameObject("Player");
        player.transform.position = new Vector3(0, 2f, 0);
        player.tag = "Player";
        
        // Add components
        player.AddComponent<Rigidbody>();
        BoxCollider playerCollider = player.AddComponent<BoxCollider>();
        playerCollider.size = new Vector3(1f, 1.5f, 0.5f); // Match the rectangle scale
        
        // Add player controller script
        player.AddComponent<PlayerController>();
    }
    
    void CreateCoins()
    {
        for (int i = 0; i < numberOfCoins; i++)
        {
            Vector3 position = GetRandomSafePosition(1f);
            GameObject coin = new GameObject("Coin");
            coin.transform.position = position;
            coin.AddComponent<Coin>();
            
            coins.Add(coin);
            coinStartPositions.Add(position);
        }
    }
    
    void CreateBombs()
    {
        for (int i = 0; i < numberOfBombs; i++)
        {
            Vector3 position = GetRandomSafePosition(1f);
            GameObject bomb = new GameObject("Bomb");
            bomb.transform.position = position;
            bomb.AddComponent<Bomb>();
            
            bombs.Add(bomb);
            bombStartPositions.Add(position);
        }
    }
    
    Vector3 GetRandomSafePosition(float heightOffset)
    {
        // Try to place objects on platforms or slightly above ground
        for (int attempts = 0; attempts < 20; attempts++)
        {
            float x = Random.Range(-levelWidth/2 + 2f, levelWidth/2 - 2f);
            float z = Random.Range(-levelDepth/2 + 2f, levelDepth/2 - 2f);
            float y = 1f + heightOffset; // Start above ground
            
            // Raycast down to find the highest platform at this position
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(x, levelHeight, z), Vector3.down, out hit, levelHeight * 2))
            {
                y = hit.point.y + heightOffset;
            }
            
            Vector3 position = new Vector3(x, y, z);
            
            // Check if position is too close to other objects
            bool validPosition = true;
            foreach (GameObject coin in coins)
            {
                if (coin != null && Vector3.Distance(position, coin.transform.position) < 2f)
                {
                    validPosition = false;
                    break;
                }
            }
            
            foreach (GameObject bomb in bombs)
            {
                if (bomb != null && Vector3.Distance(position, bomb.transform.position) < 3f)
                {
                    validPosition = false;
                    break;
                }
            }
            
            // Don't place too close to player spawn
            if (Vector3.Distance(position, new Vector3(0, 2f, 0)) < 3f)
            {
                validPosition = false;
            }
            
            if (validPosition)
            {
                return position;
            }
        }
        
        // Fallback position
        return new Vector3(Random.Range(-5f, 5f), 2f, Random.Range(-5f, 5f));
    }
    
    void CreateCamera()
    {
        GameObject cameraObj = new GameObject("Main Camera");
        cameraObj.tag = "MainCamera";
        
        Camera camera = cameraObj.AddComponent<Camera>();
        cameraObj.AddComponent<AudioListener>();
        
        // Position camera for good view of the level
        cameraObj.transform.position = new Vector3(0, 12f, -8f);
        cameraObj.transform.rotation = Quaternion.Euler(45f, 0, 0);
        
        // Add camera follow script
        cameraObj.AddComponent<CameraFollow>();
    }
    
    void CreateLighting()
    {
        // Create directional light (sun)
        GameObject lightObj = new GameObject("Directional Light");
        Light directionalLight = lightObj.AddComponent<Light>();
        directionalLight.type = LightType.Directional;
        directionalLight.color = Color.white;
        directionalLight.intensity = 1f;
        lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0);
        
        // Set ambient lighting
        RenderSettings.ambientLight = new Color(0.3f, 0.3f, 0.4f);
    }
    
    public void RespawnPickups()
    {
        // Respawn coins
        for (int i = 0; i < coinStartPositions.Count; i++)
        {
            if (i < coins.Count && coins[i] == null)
            {
                GameObject coin = new GameObject("Coin");
                coin.transform.position = coinStartPositions[i];
                coin.AddComponent<Coin>();
                coins[i] = coin;
            }
        }
        
        // Respawn bombs (they don't get destroyed, just reset if needed)
        for (int i = 0; i < bombStartPositions.Count; i++)
        {
            if (i < bombs.Count && bombs[i] != null)
            {
                bombs[i].transform.position = bombStartPositions[i];
            }
        }
    }
}
