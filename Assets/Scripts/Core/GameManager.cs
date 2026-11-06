using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using SuperUnity.Managers;
using SuperUnity.Player;

namespace SuperUnity
{
    public enum GameState
    {
        MainMenu,
        WorldSelect,
        LevelSelect,
        Settings,
        Instructions,
        Playing,
        Paused,
        LevelClear,
        GameOver,
        GameWon
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Level & World Progression (20 Levels Total)")]
        public int currentLevel = 1; // 1 to 20
        public int currentWorld => ((currentLevel - 1) / 5) + 1; // 1 to 4
        public int currentStage => ((currentLevel - 1) % 5) + 1; // 1 to 5
        public string worldName => $"{currentWorld}-{currentStage}";

        public static readonly string[] LevelTitles = new string[]
        {
            "Beginner's Journey",         // Level 1
            "Mushroom Fields",            // Level 2
            "Underground Adventure",      // Level 3
            "Dangerous Bridges",          // Level 4
            "Castle: Bowser Jr",          // Level 5 (Boss 1)
            "Desert Run",                 // Level 6
            "Moving Sands",               // Level 7
            "Pyramid Mystery",            // Level 8
            "Desert Storm",               // Level 9
            "Castle: Pharaoh Sphinx",     // Level 10 (Boss 2)
            "Snowy Mountains",            // Level 11
            "Frozen Lake",                // Level 12
            "Ice Caves",                  // Level 13
            "Blizzard Challenge",         // Level 14
            "Castle: Frost Titan",        // Level 15 (Boss 3)
            "Volcanic Valley",            // Level 16
            "Burning Bridges",            // Level 17
            "Dark Underground",           // Level 18
            "Ultimate Challenge",         // Level 19
            "FINAL BOSS: VOLCANO KING"    // Level 20 (Final Boss)
        };

        public string currentLevelTitle => (currentLevel >= 1 && currentLevel <= 20) ? LevelTitles[currentLevel - 1] : "Adventure";

        [Header("Game Stats")]
        public int score = 0;
        public int coins = 0;
        public int lives = 3;
        public int maxLives = 3;
        public float timeRemaining = 400f;

        [Header("State")]
        public GameState gameState = GameState.MainMenu;
        public bool isGameOver => gameState == GameState.GameOver;
        public bool isLevelClear => gameState == GameState.LevelClear;
        public bool isGameWon => gameState == GameState.GameWon;
        public bool isPaused => gameState == GameState.Paused;
        public bool isTimerRunning = true;

        [Header("Checkpoints")]
        public Vector3 levelStartPosition = new Vector3(2.5f, 2.5f, 0f);
        public bool hasCheckpoint = false;
        public Vector3 checkpointPosition;

        [Header("Boss Fight Tracking")]
        public bool hasActiveBoss = false;
        public string activeBossName = "";
        public int bossCurrentHealth = 0;
        public int bossMaxHealth = 0;

        public event Action OnStatsChanged;
        public event Action OnGameOverEvent;
        public event Action OnLevelClearEvent;
        public event Action OnBossHealthChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            Time.timeScale = 1.0f;
        }

        private void Start()
        {
            // Sync hero choice
            var mario = FindAnyObjectByType<MarioController>();
            if (mario != null)
            {
                mario.activeModel = (CharacterModel)SaveManager.SelectedHero;
                mario.ApplyModelStats();
            }

            var boot = FindAnyObjectByType<SuperUnityBootstrapper>();
            if (boot == null)
            {
                boot = gameObject.AddComponent<SuperUnityBootstrapper>();
            }
            boot.BuildLevel(currentLevel);

            OnStatsChanged?.Invoke();
        }

        private void Update()
        {
            if (gameState == GameState.Playing && isTimerRunning)
            {
                timeRemaining -= Time.deltaTime * 2.5f;
                if (timeRemaining <= 0)
                {
                    timeRemaining = 0;
                    isTimerRunning = false;
                    TimeRanOut();
                }
                OnStatsChanged?.Invoke();
            }

            // Pause toggle with Escape or P
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                if (gameState == GameState.Playing) TogglePause(true);
                else if (gameState == GameState.Paused) TogglePause(false);
            }

            // Restart shortcut when in game over or level clear
            if ((isGameOver || isLevelClear || isGameWon) && Input.GetKeyDown(KeyCode.R))
            {
                if (isGameWon)
                {
                    LoadLevel(1);
                }
                else
                {
                    RestartCurrentLevel();
                }
            }
        }

        public void TogglePause(bool pause)
        {
            if (pause)
            {
                gameState = GameState.Paused;
                Time.timeScale = 0f;
            }
            else
            {
                gameState = GameState.Playing;
                Time.timeScale = 1.0f;
            }
            OnStatsChanged?.Invoke();
        }

        public void AddScore(int amount)
        {
            score += amount;
            OnStatsChanged?.Invoke();
        }

        public void AddCoin(int amount = 1)
        {
            coins += amount;
            score += amount * 200;
            SaveManager.TotalCoins += amount;

            if (coins >= 100)
            {
                coins -= 100;
                lives++;
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayOneUp();
                }
            }
            OnStatsChanged?.Invoke();
        }

        public void SetCheckpoint(Vector3 pos)
        {
            hasCheckpoint = true;
            checkpointPosition = pos;
        }

        public void ClearCheckpoint()
        {
            hasCheckpoint = false;
            checkpointPosition = Vector3.zero;
        }

        public void UpdateBossHealth(string bName, int current, int max)
        {
            hasActiveBoss = current > 0;
            activeBossName = bName;
            bossCurrentHealth = current;
            bossMaxHealth = max;
            OnBossHealthChanged?.Invoke();
        }

        public void OnMarioDied()
        {
            lives--;
            isTimerRunning = false;
            OnStatsChanged?.Invoke();

            if (lives <= 0)
            {
                lives = 0;
                gameState = GameState.GameOver;
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayGameOver();
                }
                OnGameOverEvent?.Invoke();
            }
            else
            {
                RespawnAfterDeath();
            }
        }

        public void RespawnAfterDeath()
        {
            timeRemaining = 400f;
            isTimerRunning = true;
            gameState = GameState.Playing;

            var mario = FindAnyObjectByType<MarioController>();
            if (mario != null)
            {
                Vector3 spawnPos = hasCheckpoint ? checkpointPosition : levelStartPosition;
                mario.RespawnAt(spawnPos);
            }
            OnStatsChanged?.Invoke();
        }

        private void TimeRanOut()
        {
            var mario = FindAnyObjectByType<MarioController>();
            if (mario != null && !mario.IsDead)
            {
                mario.Die();
            }
        }

        public void ReachGoal(int heightBonus)
        {
            gameState = GameState.LevelClear;
            isTimerRunning = false;
            AddScore(heightBonus);

            int timeBonus = Mathf.FloorToInt(timeRemaining) * 50;
            AddScore(timeBonus);

            // Calculate stars (1 star for completion, 2 stars for >= 20 coins, 3 stars for >= 25 coins & > 150s remaining)
            int stars = 1;
            if (coins >= 15 || timeRemaining >= 150f) stars = 2;
            if (coins >= 25 && timeRemaining >= 200f) stars = 3;

            // Save Progress
            SaveManager.SetLevelCompleted(currentLevel);
            SaveManager.SetLevelHighScore(currentLevel, score);
            SaveManager.SetLevelStars(currentLevel, stars);

            OnLevelClearEvent?.Invoke();
        }

        public void NextLevel()
        {
            if (currentLevel < 20)
            {
                LoadLevel(currentLevel + 1);
            }
            else
            {
                TriggerGameWon();
            }
        }

        public void LoadLevel(int levelNumber)
        {
            Time.timeScale = 1.0f;
            currentLevel = Mathf.Clamp(levelNumber, 1, 20);
            timeRemaining = 400f;
            lives = maxLives;
            gameState = GameState.Playing;
            isTimerRunning = true;
            hasActiveBoss = false;
            ClearCheckpoint();

            if (UI.UIManager.Instance != null)
            {
                UI.UIManager.Instance.HideLevelClear();
            }

            var boot = FindAnyObjectByType<SuperUnityBootstrapper>();
            if (boot == null)
            {
                boot = gameObject.AddComponent<SuperUnityBootstrapper>();
            }
            boot.BuildLevel(currentLevel);

            OnStatsChanged?.Invoke();
        }

        public void RestartCurrentLevel()
        {
            Time.timeScale = 1.0f;
            timeRemaining = 400f;
            lives = maxLives;
            gameState = GameState.Playing;
            isTimerRunning = true;
            hasActiveBoss = false;
            ClearCheckpoint();

            if (UI.UIManager.Instance != null)
            {
                UI.UIManager.Instance.HideLevelClear();
            }

            var boot = FindAnyObjectByType<SuperUnityBootstrapper>();
            if (boot != null)
            {
                boot.BuildLevel(currentLevel);
            }
            OnStatsChanged?.Invoke();
        }

        public void TriggerGameWon()
        {
            gameState = GameState.GameWon;
            isTimerRunning = false;
            Time.timeScale = 1.0f;

            // Huge completion bonus!
            AddScore(50000);

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayStageClear();
            }

            OnStatsChanged?.Invoke();
        }
    }
}
