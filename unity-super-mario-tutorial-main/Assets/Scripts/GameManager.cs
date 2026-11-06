using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int world { get; private set; } = 1;
    public int stage { get; private set; } = 1;
    public int lives { get; private set; } = 3;
    public int coins { get; private set; } = 0;
    public int score { get; private set; } = 0;
    public float timeRemaining { get; private set; } = 400f;
    public bool isGameOver { get; private set; } = false;
    public bool isVictory { get; private set; } = false;

    private const int TOTAL_STAGES_PER_WORLD = 3;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (GetComponent<GameHUD>() == null) {
                gameObject.AddComponent<GameHUD>();
            }
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        NewGame();
    }

    private void Update()
    {
        if (!isGameOver && !isVictory && timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                // Time up! Mario dies
                Player player = FindObjectOfType<Player>();
                if (player != null && !player.dead) {
                    player.Death();
                } else {
                    ResetLevel(0f);
                }
            }
        }
    }

    public void NewGame()
    {
        lives = 3;
        coins = 0;
        score = 0;
        isGameOver = false;
        isVictory = false;

        LoadLevel(1, 1);
    }

    public void GameOver()
    {
        isGameOver = true;
        CancelInvoke(nameof(RestartAfterGameOver));
        Invoke(nameof(RestartAfterGameOver), 3f);
    }

    private void RestartAfterGameOver()
    {
        NewGame();
    }

    public void Victory()
    {
        isVictory = true;
        CancelInvoke(nameof(RestartAfterVictory));
        Invoke(nameof(RestartAfterVictory), 6f);
    }

    private void RestartAfterVictory()
    {
        NewGame();
    }

    public void LoadLevel(int world, int stage)
    {
        this.world = world;
        this.stage = stage;
        this.timeRemaining = 400f;
        this.isGameOver = false;
        this.isVictory = false;

        string sceneName = $"{world}-{stage}";
        if (Application.CanStreamedLevelBeLoaded(sceneName)) {
            SceneManager.LoadScene(sceneName);
        } else {
            // If scene doesn't exist, player finished all available levels!
            Victory();
        }
    }

    public void NextLevel()
    {
        int nextStage = stage + 1;
        int nextWorld = world;
        if (nextStage > TOTAL_STAGES_PER_WORLD) {
            nextStage = 1;
            nextWorld = world + 1;
        }

        string nextSceneName = $"{nextWorld}-{nextStage}";
        if (Application.CanStreamedLevelBeLoaded(nextSceneName)) {
            LoadLevel(nextWorld, nextStage);
        } else {
            Victory();
        }
    }

    public void ResetLevel(float delay)
    {
        CancelInvoke(nameof(ResetLevel));
        Invoke(nameof(ResetLevel), delay);
    }

    public void ResetLevel()
    {
        lives--;

        if (lives > 0) {
            LoadLevel(world, stage);
        } else {
            GameOver();
        }
    }

    public void AddCoin()
    {
        coins++;
        AddScore(200);

        if (coins >= 100)
        {
            coins = 0;
            AddLife();
        }
    }

    public void AddLife()
    {
        lives++;
        AddScore(1000);
    }

    public void AddScore(int points)
    {
        score += points;
    }

}
