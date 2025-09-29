using UnityEngine;

public class SimpleSetup : MonoBehaviour
{
    void Start()
    {
        makeStuff();
    }
    
    void makeStuff()
    {
        // ground
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.transform.position = new Vector3(0, -0.5f, 0);
        ground.transform.localScale = new Vector3(20, 1, 20);
        ground.GetComponent<Renderer>().material.color = new Color(0.4f, 0.3f, 0.2f);
        
        // player
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Cube);
        player.name = "Player";
        player.tag = "Player";
        player.transform.position = new Vector3(0, 2, 0);
        player.transform.localScale = new Vector3(1, 1.5f, 0.5f);
        player.GetComponent<Renderer>().material.color = Color.blue;
        player.AddComponent<Rigidbody>();
        player.AddComponent<PlayerController>();
        
        // some platforms
        for(int i = 0; i < 5; i++) {
            GameObject plat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plat.transform.position = new Vector3(Random.Range(-8f, 8f), Random.Range(1f, 6f), Random.Range(-8f, 8f));
            plat.transform.localScale = new Vector3(Random.Range(2f, 4f), 0.5f, Random.Range(2f, 4f));
            plat.GetComponent<Renderer>().material.color = Color.gray;
        }
        
        // coins
        for(int i = 0; i < 8; i++) {
            GameObject coin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            coin.tag = "Coin";
            coin.transform.position = new Vector3(Random.Range(-8f, 8f), Random.Range(2f, 7f), Random.Range(-8f, 8f));
            coin.transform.localScale = new Vector3(0.8f, 0.1f, 0.8f);
            coin.GetComponent<Renderer>().material.color = Color.yellow;
            coin.GetComponent<Collider>().isTrigger = true;
            coin.AddComponent<Coin>();
        }
        
        // bombs  
        for(int i = 0; i < 3; i++) {
            GameObject bomb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bomb.tag = "Bomb";
            bomb.transform.position = new Vector3(Random.Range(-6f, 6f), Random.Range(1.5f, 5f), Random.Range(-6f, 6f));
            bomb.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            bomb.GetComponent<Renderer>().material.color = Color.red;
            bomb.GetComponent<Collider>().isTrigger = true;
            bomb.AddComponent<Bomb>();
        }
        
        // camera setup
        Camera cam = Camera.main;
        if(cam == null) {
            GameObject camObj = new GameObject("Main Camera");
            cam = camObj.AddComponent<Camera>();
            camObj.AddComponent<AudioListener>();
            camObj.tag = "MainCamera";
        }
        cam.transform.position = new Vector3(0, 10, -8);
        cam.transform.rotation = Quaternion.Euler(30, 0, 0);
        cam.AddComponent<CameraFollow>();
    }
}
