using UnityEngine;

public class TestScript : MonoBehaviour
{
    void Start()
    {
        Debug.Log("TestScript Start() called!");
        
        // Create a simple red cube to verify the script is running
        GameObject testCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        testCube.name = "TestCube";
        testCube.transform.position = new Vector3(0, 1, 0);
        
        // Make it red
        Material redMaterial = new Material(Shader.Find("Standard"));
        redMaterial.color = Color.red;
        testCube.GetComponent<MeshRenderer>().material = redMaterial;
        
        Debug.Log("Red cube created at (0, 1, 0)");
    }
    
    void Update()
    {
        // Log every 60 frames to verify Update is running
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"TestScript Update running - Frame: {Time.frameCount}");
        }
    }
}
