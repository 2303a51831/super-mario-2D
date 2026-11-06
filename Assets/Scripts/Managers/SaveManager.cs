using UnityEngine;

namespace SuperUnity.Managers
{
    public static class SaveManager
    {
        private const string KEY_UNLOCKED_LEVEL = "SuperUnity_UnlockedLevel";
        private const string KEY_TOTAL_COINS = "SuperUnity_TotalCoins";
        private const string KEY_MUSIC_VOLUME = "SuperUnity_MusicVolume";
        private const string KEY_SFX_VOLUME = "SuperUnity_SFXVolume";
        private const string KEY_SELECTED_HERO = "SuperUnity_SelectedHero";

        public static int HighestUnlockedLevel
        {
            get => PlayerPrefs.GetInt(KEY_UNLOCKED_LEVEL, 1);
            set
            {
                int current = HighestUnlockedLevel;
                if (value > current)
                {
                    PlayerPrefs.SetInt(KEY_UNLOCKED_LEVEL, Mathf.Clamp(value, 1, 20));
                    PlayerPrefs.Save();
                }
            }
        }

        public static int TotalCoins
        {
            get => PlayerPrefs.GetInt(KEY_TOTAL_COINS, 0);
            set
            {
                PlayerPrefs.SetInt(KEY_TOTAL_COINS, value);
                PlayerPrefs.Save();
            }
        }

        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat(KEY_MUSIC_VOLUME, 0.8f);
            set
            {
                PlayerPrefs.SetFloat(KEY_MUSIC_VOLUME, Mathf.Clamp01(value));
                PlayerPrefs.Save();
            }
        }

        public static float SFXVolume
        {
            get => PlayerPrefs.GetFloat(KEY_SFX_VOLUME, 1.0f);
            set
            {
                PlayerPrefs.SetFloat(KEY_SFX_VOLUME, Mathf.Clamp01(value));
                PlayerPrefs.Save();
            }
        }

        public static int SelectedHero
        {
            get => PlayerPrefs.GetInt(KEY_SELECTED_HERO, 0);
            set
            {
                PlayerPrefs.SetInt(KEY_SELECTED_HERO, Mathf.Clamp(value, 0, 3));
                PlayerPrefs.Save();
            }
        }

        public static bool IsLevelUnlocked(int levelNumber)
        {
            if (levelNumber <= 1) return true;
            return levelNumber <= HighestUnlockedLevel;
        }

        public static bool IsLevelCompleted(int levelNumber)
        {
            return PlayerPrefs.GetInt($"SuperUnity_LevelCompleted_{levelNumber}", 0) == 1;
        }

        public static void SetLevelCompleted(int levelNumber)
        {
            PlayerPrefs.SetInt($"SuperUnity_LevelCompleted_{levelNumber}", 1);
            if (levelNumber < 20 && levelNumber >= HighestUnlockedLevel)
            {
                HighestUnlockedLevel = levelNumber + 1;
            }
            PlayerPrefs.Save();
        }

        public static int GetLevelHighScore(int levelNumber)
        {
            return PlayerPrefs.GetInt($"SuperUnity_HighScore_{levelNumber}", 0);
        }

        public static void SetLevelHighScore(int levelNumber, int score)
        {
            int current = GetLevelHighScore(levelNumber);
            if (score > current)
            {
                PlayerPrefs.SetInt($"SuperUnity_HighScore_{levelNumber}", score);
                PlayerPrefs.Save();
            }
        }

        public static int GetLevelStars(int levelNumber)
        {
            return PlayerPrefs.GetInt($"SuperUnity_Stars_{levelNumber}", 0);
        }

        public static void SetLevelStars(int levelNumber, int stars)
        {
            int current = GetLevelStars(levelNumber);
            if (stars > current)
            {
                PlayerPrefs.SetInt($"SuperUnity_Stars_{levelNumber}", Mathf.Clamp(stars, 1, 3));
                PlayerPrefs.Save();
            }
        }

        public static void ResetAllProgress()
        {
            for (int i = 1; i <= 20; i++)
            {
                PlayerPrefs.DeleteKey($"SuperUnity_LevelCompleted_{i}");
                PlayerPrefs.DeleteKey($"SuperUnity_HighScore_{i}");
                PlayerPrefs.DeleteKey($"SuperUnity_Stars_{i}");
            }
            PlayerPrefs.SetInt(KEY_UNLOCKED_LEVEL, 1);
            PlayerPrefs.SetInt(KEY_TOTAL_COINS, 0);
            PlayerPrefs.Save();
        }
    }
}
