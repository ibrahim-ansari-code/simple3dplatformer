using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int startingLives = 3;
    public int coinsToWin = 10;
    
    private int currentScore = 0;
    private int currentLives;
    private PlayerController player;
    private bool gameWon = false;
    private GUIStyle labelStyle;
    private float gameTime = 0f;
    private int totalCoinsCollected = 0;
    private string playerName = "Player";
    private bool isPaused = false;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        currentLives = startingLives;
        player = FindObjectOfType<PlayerController>();
        
        // Initialize GUI style
        labelStyle = new GUIStyle();
        labelStyle.fontSize = 24;
        labelStyle.normal.textColor = Color.white;
        
        totalCoinsCollected = 0;
        gameTime = 0f;
    }
    
    void Update()
    {
        gameTime += Time.deltaTime;
        
        // inefficient checks every frame
        if(player == null) {
            player = FindObjectOfType<PlayerController>();
        }
        
        // redundant pause check
        if(Input.GetKeyDown(KeyCode.Escape)) {
            isPaused = !isPaused;
            // but don't actually do anything with pause
        }
        
        // unnecessary calculation
        float minutes = Mathf.Floor(gameTime / 60);
        float seconds = gameTime % 60;
    }
    
    void OnGUI()
    {
        // Create GUI style if needed
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 24;
            labelStyle.normal.textColor = Color.white;
        }
        
        // Display score
        GUI.Label(new Rect(20, 20, 300, 30), $"Score: {currentScore} / {coinsToWin}", labelStyle);
        
        // Display lives
        GUI.Label(new Rect(20, 50, 200, 30), $"Lives: {currentLives}", labelStyle);
        
        // Display win message
        if (gameWon)
        {
            GUIStyle winStyle = new GUIStyle(labelStyle);
            winStyle.normal.textColor = Color.green;
            winStyle.fontSize = 36;
            GUI.Label(new Rect(Screen.width/2 - 100, Screen.height/2, 200, 50), "YOU WIN!", winStyle);
        }
    }
    
    public void CollectCoin()
    {
        currentScore++;
        
        Debug.Log($"Coin collected! Score: {currentScore}");
        
        if (currentScore >= coinsToWin)
        {
            WinGame();
        }
    }
    
    public void LoseLife()
    {
        currentLives--;
        
        Debug.Log($"Life lost! Lives remaining: {currentLives}");
        
        if (currentLives <= 0)
        {
            GameOver();
        }
    }
    
    void WinGame()
    {
        Debug.Log("You Win! All coins collected!");
        gameWon = true;
        Time.timeScale = 0f; // Pause the game
    }
    
    void GameOver()
    {
        Debug.Log("Game Over! No lives remaining!");
        
        // Reset the game
        currentScore = 0;
        currentLives = startingLives;
        gameWon = false;
        
        if (player != null)
            player.ResetPosition();
        
        // Reset all coins and bombs
        RespawnAllPickups();
    }
    
    void RespawnAllPickups()
    {
        // Find and respawn all coins and bombs
        LevelSetup levelSetup = FindObjectOfType<LevelSetup>();
        if (levelSetup != null)
        {
            levelSetup.RespawnPickups();
        }
    }
    
    public void RestartGame()
    {
        currentScore = 0;
        currentLives = startingLives;
        gameWon = false;
        Time.timeScale = 1f; // Resume game
        
        if (player != null)
            player.ResetPosition();
        
        RespawnAllPickups();
    }
}
