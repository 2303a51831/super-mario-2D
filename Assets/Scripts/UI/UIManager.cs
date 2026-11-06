using UnityEngine;
using SuperUnity.Core;
using SuperUnity.Player;
using SuperUnity.Managers;

namespace SuperUnity.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Display Settings")]
        public bool useModernInterface = true;
        public Font customFont;

        // Cached GUI textures
        private static Texture2D texCardBg;
        private static Texture2D texCardHover;
        private static Texture2D texModalBg;
        private static Texture2D texDarkOverlay;
        private static Texture2D texSpeedFill;
        private static Texture2D texCoinFill;
        private static Texture2D texBossHealthBg;
        private static Texture2D texBossHealthFill;
        private static Texture2D texProgressTrack;
        private static Texture2D texProgressFill;

        // Styles
        private GUIStyle titleStyle;
        private GUIStyle subTitleStyle;
        private GUIStyle menuBtnStyle;
        private GUIStyle heroStyle;
        private GUIStyle heroSubStyle;
        private GUIStyle cardValueStyle;
        private GUIStyle cardLabelStyle;
        private GUIStyle bannerStyle;
        private GUIStyle modalStyle;
        private GUIStyle controlsStyle;
        private GUIStyle miniProgressStyle;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            InitTextures();
        }

        public void HideLevelClear()
        {
            // Handled dynamically via gameState
        }

        private static Texture2D MakeSolidTex(int w, int h, Color col)
        {
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            Color[] pixels = new Color[w * h];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = col;
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        private static Texture2D MakeCardTex(int w, int h, Color bg, Color border, int bWidth = 2)
        {
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    bool isBorder = (x < bWidth || x >= w - bWidth || y < bWidth || y >= h - bWidth);
                    tex.SetPixel(x, y, isBorder ? border : bg);
                }
            }
            tex.Apply();
            return tex;
        }

        private void InitTextures()
        {
            if (texCardBg == null) texCardBg = MakeCardTex(32, 32, new Color(0.06f, 0.08f, 0.16f, 0.90f), new Color(0.25f, 0.45f, 0.85f, 0.75f), 2);
            if (texCardHover == null) texCardHover = MakeCardTex(32, 32, new Color(0.12f, 0.18f, 0.32f, 0.95f), new Color(0.4f, 0.7f, 1f, 0.95f), 2);
            if (texModalBg == null) texModalBg = MakeCardTex(32, 32, new Color(0.04f, 0.06f, 0.12f, 0.96f), new Color(1f, 0.8f, 0.2f, 0.9f), 3);
            if (texDarkOverlay == null) texDarkOverlay = MakeSolidTex(4, 4, new Color(0f, 0f, 0f, 0.75f));
            if (texSpeedFill == null) texSpeedFill = MakeSolidTex(4, 4, new Color(0f, 0.95f, 1f, 0.95f));
            if (texCoinFill == null) texCoinFill = MakeSolidTex(4, 4, new Color(1f, 0.82f, 0.1f, 0.95f));
            if (texBossHealthBg == null) texBossHealthBg = MakeSolidTex(4, 4, new Color(0.2f, 0.05f, 0.05f, 0.9f));
            if (texBossHealthFill == null) texBossHealthFill = MakeSolidTex(4, 4, new Color(1f, 0.15f, 0.15f, 0.95f));
            if (texProgressTrack == null) texProgressTrack = MakeSolidTex(4, 4, new Color(0.12f, 0.16f, 0.26f, 0.85f));
            if (texProgressFill == null) texProgressFill = MakeSolidTex(4, 4, new Color(0.3f, 0.75f, 1f, 0.95f));
        }

        private void SetupStyles()
        {
            if (titleStyle != null) return;
            InitTextures();

            titleStyle = new GUIStyle();
            titleStyle.font = customFont;
            titleStyle.fontSize = Mathf.Clamp(Screen.height / 16, 28, 54);
            titleStyle.normal.textColor = new Color(1f, 0.85f, 0.15f);
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.alignment = TextAnchor.MiddleCenter;

            subTitleStyle = new GUIStyle(titleStyle);
            subTitleStyle.fontSize = Mathf.Clamp(Screen.height / 38, 14, 22);
            subTitleStyle.normal.textColor = new Color(0.7f, 0.85f, 1f);

            menuBtnStyle = new GUIStyle();
            menuBtnStyle.font = customFont;
            menuBtnStyle.fontSize = Mathf.Clamp(Screen.height / 34, 16, 24);
            menuBtnStyle.normal.textColor = Color.white;
            menuBtnStyle.hover.textColor = new Color(1f, 0.9f, 0.2f);
            menuBtnStyle.fontStyle = FontStyle.Bold;
            menuBtnStyle.alignment = TextAnchor.MiddleCenter;
            menuBtnStyle.normal.background = texCardBg;
            menuBtnStyle.hover.background = texCardHover;

            heroStyle = new GUIStyle();
            heroStyle.font = customFont;
            heroStyle.fontSize = Mathf.Clamp(Screen.height / 42, 13, 20);
            heroStyle.normal.textColor = Color.white;
            heroStyle.fontStyle = FontStyle.Bold;
            heroStyle.alignment = TextAnchor.MiddleLeft;

            heroSubStyle = new GUIStyle(heroStyle);
            heroSubStyle.fontSize = Mathf.Clamp(Screen.height / 56, 10, 14);
            heroSubStyle.normal.textColor = new Color(0.3f, 0.85f, 1f);

            cardValueStyle = new GUIStyle(heroStyle);
            cardValueStyle.alignment = TextAnchor.MiddleCenter;
            cardValueStyle.fontSize = Mathf.Clamp(Screen.height / 40, 14, 22);

            cardLabelStyle = new GUIStyle(cardValueStyle);
            cardLabelStyle.fontSize = Mathf.Clamp(Screen.height / 58, 10, 13);
            cardLabelStyle.normal.textColor = new Color(0.7f, 0.8f, 0.95f);

            controlsStyle = new GUIStyle();
            controlsStyle.font = customFont;
            controlsStyle.fontSize = Mathf.Clamp(Screen.height / 54, 11, 15);
            controlsStyle.normal.textColor = new Color(0.9f, 0.95f, 1f, 0.95f);
            controlsStyle.alignment = TextAnchor.MiddleLeft;

            bannerStyle = new GUIStyle(titleStyle);
            bannerStyle.fontSize = Mathf.Clamp(Screen.height / 20, 24, 46);

            modalStyle = new GUIStyle(bannerStyle);
            modalStyle.fontSize = Mathf.Clamp(Screen.height / 36, 15, 24);
            modalStyle.normal.textColor = Color.white;

            miniProgressStyle = new GUIStyle(heroStyle);
            miniProgressStyle.fontSize = Mathf.Clamp(Screen.height / 50, 11, 15);
            miniProgressStyle.alignment = TextAnchor.MiddleCenter;
        }

        private void OnGUI()
        {
            if (!useModernInterface || GameManager.Instance == null) return;
            SetupStyles();

            float screenW = Screen.width;
            float screenH = Screen.height;
            GameState state = GameManager.Instance.gameState;

            // Route to appropriate view
            switch (state)
            {
                case GameState.MainMenu:
                    DrawMainMenu(screenW, screenH);
                    break;
                case GameState.WorldSelect:
                    DrawWorldSelect(screenW, screenH);
                    break;
                case GameState.LevelSelect:
                    DrawLevelSelect(screenW, screenH);
                    break;
                case GameState.Settings:
                    DrawSettings(screenW, screenH);
                    break;
                case GameState.Instructions:
                    DrawInstructions(screenW, screenH);
                    break;
                case GameState.Paused:
                    DrawInGameHUD(screenW, screenH);
                    DrawPauseMenu(screenW, screenH);
                    break;
                case GameState.Playing:
                    DrawInGameHUD(screenW, screenH);
                    break;
                case GameState.LevelClear:
                    DrawInGameHUD(screenW, screenH);
                    DrawLevelClearModal(screenW, screenH);
                    break;
                case GameState.GameOver:
                    DrawInGameHUD(screenW, screenH);
                    DrawGameOverModal(screenW, screenH);
                    break;
                case GameState.GameWon:
                    DrawGameWonModal(screenW, screenH);
                    break;
            }
        }

        // ==========================================
        // 1. IN-GAME HUD & DASHBOARD
        // ==========================================
        private void DrawInGameHUD(float screenW, float screenH)
        {
            var mario = FindAnyObjectByType<MarioController>();
            float topY = 10f;
            float cardH = 54f;
            float gap = 8f;
            float sideMargin = 14f;
            float availW = screenW - (sideMargin * 2f);

            // Card 1: HERO MODEL (Left)
            float heroCardW = Mathf.Clamp(availW * 0.22f, 170f, 250f);
            Rect heroRect = new Rect(sideMargin, topY, heroCardW, cardH);
            GUI.DrawTexture(heroRect, texCardBg);

            string modelIcon = "🍄";
            string modelName = "MARIO";
            Color modelCol = Color.white;
            if (mario != null)
            {
                modelCol = mario.ModelColor;
                switch (mario.activeModel)
                {
                    case CharacterModel.Luigi: modelIcon = "🍀"; modelName = "LUIGI"; break;
                    case CharacterModel.FireKnight: modelIcon = "🔥"; modelName = "FIRE KNIGHT"; break;
                    case CharacterModel.CyberNeon: modelIcon = "⚡"; modelName = "CYBER NEON"; break;
                    default: modelIcon = "🍄"; modelName = "MARIO"; break;
                }
            }

            GUI.color = modelCol;
            GUI.Label(new Rect(heroRect.x + 10f, heroRect.y + 6f, heroRect.width - 20f, 22f), $"{modelIcon} {modelName}", heroStyle);
            GUI.color = Color.white;
            GUI.Label(new Rect(heroRect.x + 10f, heroRect.y + 28f, heroRect.width - 20f, 18f), "Tap / [C] Switch Hero", heroSubStyle);

            if (Event.current.type == EventType.MouseDown && heroRect.Contains(Event.current.mousePosition))
            {
                if (mario != null) mario.SwitchCharacterModel();
                Event.current.Use();
            }

            // Cards 2-5: COINS, SCORE, WORLD, TIME & LIVES
            float remainingW = availW - heroCardW - (gap * 4f);
            float subCardW = remainingW / 4f;

            // Coins
            Rect coinRect = new Rect(heroRect.xMax + gap, topY, subCardW, cardH);
            GUI.DrawTexture(coinRect, texCardBg);
            GUI.Label(new Rect(coinRect.x, coinRect.y + 5f, coinRect.width, 16f), "COINS", cardLabelStyle);
            GUI.Label(new Rect(coinRect.x, coinRect.y + 22f, coinRect.width, 24f), $"🪙 x{GameManager.Instance.coins:D2}", cardValueStyle);

            // Score
            Rect scoreRect = new Rect(coinRect.xMax + gap, topY, subCardW, cardH);
            GUI.DrawTexture(scoreRect, texCardBg);
            GUI.Label(new Rect(scoreRect.x, scoreRect.y + 5f, scoreRect.width, 16f), "SCORE", cardLabelStyle);
            GUI.Label(new Rect(scoreRect.x, scoreRect.y + 22f, scoreRect.width, 24f), $"⭐ {GameManager.Instance.score:D6}", cardValueStyle);

            // World & Level Title
            Rect worldRect = new Rect(scoreRect.xMax + gap, topY, subCardW, cardH);
            GUI.DrawTexture(worldRect, texCardBg);
            GUI.Label(new Rect(worldRect.x, worldRect.y + 5f, worldRect.width, 16f), $"WORLD {GameManager.Instance.worldName}", cardLabelStyle);
            GUI.Label(new Rect(worldRect.x, worldRect.y + 22f, worldRect.width, 24f), $"🌍 {GameManager.Instance.currentLevelTitle}", cardValueStyle);

            // Time & Lives
            Rect timeRect = new Rect(worldRect.xMax + gap, topY, subCardW, cardH);
            GUI.DrawTexture(timeRect, texCardBg);
            bool lowTime = GameManager.Instance.timeRemaining < 60f;
            if (lowTime && Mathf.PingPong(Time.time * 4f, 1f) > 0.5f) GUI.color = new Color(1f, 0.3f, 0.3f);
            GUI.Label(new Rect(timeRect.x, timeRect.y + 5f, timeRect.width, 16f), $"TIME: {Mathf.CeilToInt(GameManager.Instance.timeRemaining):D3}s", cardLabelStyle);
            GUI.color = Color.white;

            string livesHearts = "";
            for (int l = 0; l < Mathf.Min(GameManager.Instance.lives, 5); l++) livesHearts += "❤️";
            GUI.Label(new Rect(timeRect.x, timeRect.y + 22f, timeRect.width, 24f), $"{livesHearts} x{GameManager.Instance.lives}", cardValueStyle);

            // Speedometer gauge
            float speedY = topY + cardH + 5f;
            float speedW = heroCardW;
            float speedH = 12f;
            Rect speedTrackRect = new Rect(sideMargin, speedY, speedW, speedH);
            GUI.DrawTexture(speedTrackRect, texProgressTrack);

            float currentSpeed = mario != null ? Mathf.Abs(mario.Rb.linearVelocity.x) : 0f;
            float maxSpeed = mario != null ? mario.runSpeed : 14f;
            float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeed);

            if (speedRatio > 0.05f)
            {
                Color speedCol = (speedRatio > 0.8f) ? new Color(0f, 1f, 0.85f) : new Color(0.3f, 0.7f, 1f);
                GUI.color = speedCol;
                GUI.DrawTexture(new Rect(speedTrackRect.x + 2f, speedTrackRect.y + 2f, (speedW - 4f) * speedRatio, speedH - 4f), texSpeedFill);
                GUI.color = Color.white;
            }

            // Boss Health Bar (if active boss present in Level 5, 10, 15, 20!)
            if (GameManager.Instance.hasActiveBoss)
            {
                float bossBarW = Mathf.Clamp(screenW * 0.45f, 300f, 550f);
                float bossBarH = 28f;
                float bossBarX = (screenW - bossBarW) * 0.5f;
                float bossBarY = topY + cardH + 6f;

                Rect bRect = new Rect(bossBarX, bossBarY, bossBarW, bossBarH);
                GUI.DrawTexture(bRect, texBossHealthBg);

                float hpRatio = (float)GameManager.Instance.bossCurrentHealth / Mathf.Max(GameManager.Instance.bossMaxHealth, 1);
                GUI.DrawTexture(new Rect(bRect.x + 3f, bRect.y + 3f, (bossBarW - 6f) * hpRatio, bossBarH - 6f), texBossHealthFill);

                GUI.Label(new Rect(bossBarX, bossBarY + 3f, bossBarW, bossBarH), $"⚔️ {GameManager.Instance.activeBossName} [{GameManager.Instance.bossCurrentHealth}/{GameManager.Instance.bossMaxHealth}]", cardValueStyle);
            }

            // Stage Progress Mini-Map
            float progBarW = Mathf.Clamp(screenW * 0.42f, 300f, 540f);
            float progBarH = 22f;
            float progBarX = (screenW - progBarW) * 0.5f;
            float progBarY = screenH - 36f;

            Rect trackRect = new Rect(progBarX, progBarY, progBarW, progBarH);
            GUI.DrawTexture(trackRect, texCardBg);

            float marioX = mario != null ? mario.transform.position.x : 0f;
            float progressRatio = Mathf.Clamp01((marioX - 2.5f) / 196.0f);

            GUI.DrawTexture(new Rect(trackRect.x + 3f, trackRect.y + 3f, (progBarW - 6f) * progressRatio, progBarH - 6f), texProgressFill);
            GUI.Label(new Rect(trackRect.x + 8f, trackRect.y + 2f, 70f, progBarH), "🚩 START", cardLabelStyle);
            GUI.Label(new Rect(trackRect.xMax - 80f, trackRect.y + 2f, 70f, progBarH), "CASTLE 🏰", cardLabelStyle);

            float markerX = trackRect.x + 3f + (progBarW - 22f) * progressRatio;
            GUI.Label(new Rect(markerX, trackRect.y + 1f, 22f, progBarH), modelIcon, miniProgressStyle);

            // Controls Hint Dock
            float dockW = 380f;
            float dockH = 26f;
            Rect dockRect = new Rect(sideMargin, screenH - 38f, dockW, dockH);
            GUI.DrawTexture(dockRect, texCardBg);
            GUI.Label(new Rect(dockRect.x + 8f, dockRect.y + 3f, dockW - 16f, 20f), "🎮 [A][D] Move | [SPACE] Jump/Air-Dash | [SHIFT] Sprint | [C] Hero | [ESC] Pause", controlsStyle);

            // Pause button on top right
            Rect pauseBtnRect = new Rect(screenW - 55f, speedY, 40f, 24f);
            if (GUI.Button(pauseBtnRect, "⏸️", menuBtnStyle))
            {
                GameManager.Instance.TogglePause(true);
            }
        }

        // ==========================================
        // 2. MAIN MENU SCREEN
        // ==========================================
        private void DrawMainMenu(float screenW, float screenH)
        {
            GUI.DrawTexture(new Rect(0, 0, screenW, screenH), texDarkOverlay);

            float centerY = screenH * 0.16f;
            GUI.Label(new Rect(0, centerY, screenW, 60f), "SUPER MARIO 2D ADVENTURE", titleStyle);
            GUI.Label(new Rect(0, centerY + 58f, screenW, 30f), "★ 20 FULLY PLAYABLE WORLDS & BOSS BATTLES ★", subTitleStyle);

            float btnW = Mathf.Clamp(screenW * 0.35f, 260f, 380f);
            float btnH = 50f;
            float startY = centerY + 110f;
            float btnGap = 14f;
            float btnX = (screenW - btnW) * 0.5f;

            if (GUI.Button(new Rect(btnX, startY, btnW, btnH), "▶ PLAY GAME", menuBtnStyle))
            {
                GameManager.Instance.LoadLevel(SaveManager.HighestUnlockedLevel);
            }

            if (GUI.Button(new Rect(btnX, startY + (btnH + btnGap) * 1, btnW, btnH), "🗺️ LEVEL SELECT (1 - 20)", menuBtnStyle))
            {
                GameManager.Instance.gameState = GameState.LevelSelect;
            }

            if (GUI.Button(new Rect(btnX, startY + (btnH + btnGap) * 2, btnW, btnH), "🌍 WORLD SELECT (WORLDS 1 - 4)", menuBtnStyle))
            {
                GameManager.Instance.gameState = GameState.WorldSelect;
            }

            if (GUI.Button(new Rect(btnX, startY + (btnH + btnGap) * 3, btnW, btnH), "⚙️ SETTINGS", menuBtnStyle))
            {
                GameManager.Instance.gameState = GameState.Settings;
            }

            if (GUI.Button(new Rect(btnX, startY + (btnH + btnGap) * 4, btnW, btnH), "📖 HOW TO PLAY", menuBtnStyle))
            {
                GameManager.Instance.gameState = GameState.Instructions;
            }

            if (GUI.Button(new Rect(btnX, startY + (btnH + btnGap) * 5, btnW, btnH), "❌ QUIT GAME", menuBtnStyle))
            {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
            }
        }

        // ==========================================
        // 3. WORLD SELECTION (4 WORLDS)
        // ==========================================
        private void DrawWorldSelect(float screenW, float screenH)
        {
            GUI.DrawTexture(new Rect(0, 0, screenW, screenH), texDarkOverlay);

            GUI.Label(new Rect(0, 30f, screenW, 50f), "SELECT WORLD", titleStyle);

            float cardW = Mathf.Clamp(screenW * 0.42f, 280f, 480f);
            float cardH = 110f;
            float startX = (screenW - (cardW * 2f + 20f)) * 0.5f;
            float startY = screenH * 0.22f;

            string[] worldNames = new string[] {
                "WORLD 1: GREEN GRASSLAND\n(Levels 1 - 5 | Boss: Bowser Jr)",
                "WORLD 2: DESERT ADVENTURE\n(Levels 6 - 10 | Boss: Pharaoh Sphinx)",
                "WORLD 3: FROZEN KINGDOM\n(Levels 11 - 15 | Boss: Frost Titan)",
                "WORLD 4: DARK VOLCANO\n(Levels 16 - 20 | Boss: Volcano King)"
            };

            for (int w = 0; w < 4; w++)
            {
                int firstLevel = w * 5 + 1;
                bool unlocked = SaveManager.IsLevelUnlocked(firstLevel);
                int row = w / 2;
                int col = w % 2;
                Rect wRect = new Rect(startX + col * (cardW + 20f), startY + row * (cardH + 20f), cardW, cardH);

                string label = unlocked ? worldNames[w] : $"{worldNames[w]}\n🔒 LOCKED";
                GUI.color = unlocked ? Color.white : new Color(0.6f, 0.6f, 0.6f);

                if (GUI.Button(wRect, label, menuBtnStyle) && unlocked)
                {
                    GameManager.Instance.LoadLevel(firstLevel);
                }
            }
            GUI.color = Color.white;

            if (GUI.Button(new Rect((screenW - 220f) * 0.5f, screenH - 70f, 220f, 45f), "◄ BACK TO MENU", menuBtnStyle))
            {
                GameManager.Instance.gameState = GameState.MainMenu;
            }
        }

        // ==========================================
        // 4. LEVEL SELECTION (20 LEVELS GRID)
        // ==========================================
        private void DrawLevelSelect(float screenW, float screenH)
        {
            GUI.DrawTexture(new Rect(0, 0, screenW, screenH), texDarkOverlay);

            GUI.Label(new Rect(0, 20f, screenW, 45f), "LEVEL SELECT (20 PLAYABLE LEVELS)", titleStyle);

            float gridMarginX = Mathf.Clamp(screenW * 0.08f, 20f, 80f);
            float availGridW = screenW - (gridMarginX * 2f);
            float btnW = (availGridW - (4f * 12f)) / 5f;
            float btnH = 64f;
            float gridStartY = screenH * 0.16f;

            for (int lvl = 1; lvl <= 20; lvl++)
            {
                int row = (lvl - 1) / 5;
                int col = (lvl - 1) % 5;
                Rect lRect = new Rect(gridMarginX + col * (btnW + 12f), gridStartY + row * (btnH + 14f), btnW, btnH);

                bool isUnlocked = SaveManager.IsLevelUnlocked(lvl);
                int stars = SaveManager.GetLevelStars(lvl);
                string starText = "";
                for (int s = 0; s < stars; s++) starText += "★";

                string btnText = isUnlocked ? $"W{GameManager.Instance.currentWorld}-L{((lvl - 1) % 5) + 1}\n{GameManager.LevelTitles[lvl - 1]}\n{starText}" : $"LVL {lvl}\n🔒 LOCKED";
                GUI.color = isUnlocked ? Color.white : new Color(0.55f, 0.55f, 0.55f);

                if (GUI.Button(lRect, btnText, menuBtnStyle) && isUnlocked)
                {
                    GameManager.Instance.LoadLevel(lvl);
                }
            }
            GUI.color = Color.white;

            if (GUI.Button(new Rect((screenW - 220f) * 0.5f, screenH - 65f, 220f, 45f), "◄ BACK TO MENU", menuBtnStyle))
            {
                GameManager.Instance.gameState = GameState.MainMenu;
            }
        }

        // ==========================================
        // 5. SETTINGS SCREEN
        // ==========================================
        private void DrawSettings(float screenW, float screenH)
        {
            GUI.DrawTexture(new Rect(0, 0, screenW, screenH), texDarkOverlay);

            float cardW = Mathf.Clamp(screenW * 0.55f, 360f, 600f);
            float cardH = 380f;
            float cardX = (screenW - cardW) * 0.5f;
            float cardY = (screenH - cardH) * 0.45f;
            Rect sRect = new Rect(cardX, cardY, cardW, cardH);

            GUI.DrawTexture(sRect, texModalBg);
            GUI.Label(new Rect(cardX, cardY + 20f, cardW, 45f), "⚙️ GAME SETTINGS", bannerStyle);

            // Music Volume
            GUI.Label(new Rect(cardX + 40f, cardY + 80f, cardW - 80f, 25f), $"MUSIC VOLUME: {Mathf.RoundToInt(SaveManager.MusicVolume * 100)}%", heroStyle);
            float newMusic = GUI.HorizontalSlider(new Rect(cardX + 40f, cardY + 110f, cardW - 80f, 20f), SaveManager.MusicVolume, 0f, 1f);
            if (newMusic != SaveManager.MusicVolume && SoundManager.Instance != null)
            {
                SoundManager.Instance.SetMusicVolume(newMusic);
            }

            // SFX Volume
            GUI.Label(new Rect(cardX + 40f, cardY + 145f, cardW - 80f, 25f), $"SFX VOLUME: {Mathf.RoundToInt(SaveManager.SFXVolume * 100)}%", heroStyle);
            float newSfx = GUI.HorizontalSlider(new Rect(cardX + 40f, cardY + 175f, cardW - 80f, 20f), SaveManager.SFXVolume, 0f, 1f);
            if (newSfx != SaveManager.SFXVolume && SoundManager.Instance != null)
            {
                SoundManager.Instance.SetSFXVolume(newSfx);
            }

            // Reset progress
            if (GUI.Button(new Rect(cardX + 40f, cardY + 230f, cardW - 80f, 40f), "⚠️ RESET ALL SAVED PROGRESS", menuBtnStyle))
            {
                SaveManager.ResetAllProgress();
            }

            // Back button
            if (GUI.Button(new Rect(cardX + 40f, cardY + 295f, cardW - 80f, 48f), "◄ SAVE & RETURN TO MENU", menuBtnStyle))
            {
                GameManager.Instance.gameState = GameState.MainMenu;
            }
        }

        // ==========================================
        // 6. INSTRUCTIONS SCREEN
        // ==========================================
        private void DrawInstructions(float screenW, float screenH)
        {
            GUI.DrawTexture(new Rect(0, 0, screenW, screenH), texDarkOverlay);

            float cardW = Mathf.Clamp(screenW * 0.65f, 420f, 750f);
            float cardH = 440f;
            float cardX = (screenW - cardW) * 0.5f;
            float cardY = (screenH - cardH) * 0.45f;
            Rect iRect = new Rect(cardX, cardY, cardW, cardH);

            GUI.DrawTexture(iRect, texModalBg);
            GUI.Label(new Rect(cardX, cardY + 18f, cardW, 40f), "📖 HOW TO PLAY", bannerStyle);

            string guide =
                "• [A] / [D] or ARROW KEYS : Move Left / Right\n" +
                "• [SPACE] or [W] : Jump (Tap for low jump, hold for high jump)\n" +
                "• [SPACE] in Mid-Air : Double Jump / Acrobatic Air-Dash!\n" +
                "• [LEFT SHIFT] : Hold to Turbo Sprint across platforms!\n" +
                "• [C] : Switch Character Model (Mario, Luigi, Fire Knight, Cyber Neon)\n" +
                "• [F] or [CTRL] : Shoot Fireballs\n" +
                "• [ESC] or [P] : Pause Game\n\n" +
                "★ COLLECTIBLES & POWER-UPS:\n" +
                "  🪙 COINS: Collect 100 for 1-UP! | 🍄 MUSHROOM: Super Mario size & HP\n" +
                "  🔥 FIRE FLOWER: Fireball power | ⭐ STAR: 12s Invincibility!";

            GUI.Label(new Rect(cardX + 35f, cardY + 70f, cardW - 70f, 290f), guide, controlsStyle);

            if (GUI.Button(new Rect(cardX + 40f, cardY + cardH - 60f, cardW - 80f, 45f), "◄ RETURN TO MENU", menuBtnStyle))
            {
                GameManager.Instance.gameState = GameState.MainMenu;
            }
        }

        // ==========================================
        // 7. PAUSE MENU
        // ==========================================
        private void DrawPauseMenu(float screenW, float screenH)
        {
            GUI.DrawTexture(new Rect(0, 0, screenW, screenH), texDarkOverlay);

            float cardW = 340f;
            float cardH = 340f;
            float cardX = (screenW - cardW) * 0.5f;
            float cardY = (screenH - cardH) * 0.5f;
            Rect pRect = new Rect(cardX, cardY, cardW, cardH);

            GUI.DrawTexture(pRect, texModalBg);
            GUI.Label(new Rect(cardX, cardY + 20f, cardW, 40f), "⏸️ GAME PAUSED", bannerStyle);

            float bW = cardW - 60f;
            float bH = 46f;
            float bX = cardX + 30f;
            float bY = cardY + 75f;

            if (GUI.Button(new Rect(bX, bY, bW, bH), "▶ RESUME", menuBtnStyle))
            {
                GameManager.Instance.TogglePause(false);
            }
            if (GUI.Button(new Rect(bX, bY + 56f, bW, bH), "🔄 RESTART LEVEL", menuBtnStyle))
            {
                GameManager.Instance.RestartCurrentLevel();
            }
            if (GUI.Button(new Rect(bX, bY + 112f, bW, bH), "🗺️ LEVEL SELECT", menuBtnStyle))
            {
                GameManager.Instance.gameState = GameState.LevelSelect;
            }
            if (GUI.Button(new Rect(bX, bY + 168f, bW, bH), "🏠 MAIN MENU", menuBtnStyle))
            {
                GameManager.Instance.gameState = GameState.MainMenu;
            }
        }

        // ==========================================
        // 8. LEVEL CLEAR VICTORY MODAL
        // ==========================================
        private void DrawLevelClearModal(float screenW, float screenH)
        {
            float modalW = Mathf.Clamp(screenW * 0.55f, 420f, 620f);
            float modalH = 320f;
            float modalX = (screenW - modalW) * 0.5f;
            float modalY = (screenH - modalH) * 0.45f;
            Rect mRect = new Rect(modalX, modalY, modalW, modalH);

            GUI.DrawTexture(mRect, texModalBg);

            int stars = SaveManager.GetLevelStars(GameManager.Instance.currentLevel);
            string starsText = "";
            for (int s = 0; s < stars; s++) starsText += "⭐ ";

            GUI.Label(new Rect(modalX, modalY + 20f, modalW, 45f), $"{starsText}STAGE CLEAR!{starsText}", bannerStyle);
            GUI.Label(new Rect(modalX, modalY + 65f, modalW, 30f), $"WORLD {GameManager.Instance.worldName} : {GameManager.Instance.currentLevelTitle}", modalStyle);
            GUI.Label(new Rect(modalX, modalY + 105f, modalW, 30f), $"TOTAL SCORE: {GameManager.Instance.score:D6}   COINS: {GameManager.Instance.coins:D2}", modalStyle);
            GUI.Label(new Rect(modalX, modalY + 140f, modalW, 30f), $"TIME REMAINING: {Mathf.CeilToInt(GameManager.Instance.timeRemaining)}s", cardLabelStyle);

            float bW = (modalW - 50f) * 0.5f;
            float bY = modalY + 230f;

            string nextBtnText = GameManager.Instance.currentLevel < 20 ? "▶ NEXT LEVEL" : "🏆 GRAND VICTORY";
            if (GUI.Button(new Rect(modalX + 20f, bY, bW, 55f), nextBtnText, menuBtnStyle))
            {
                GameManager.Instance.NextLevel();
            }

            if (GUI.Button(new Rect(modalX + 30f + bW, bY, bW, 55f), "🗺️ LEVEL SELECT", menuBtnStyle))
            {
                GameManager.Instance.gameState = GameState.LevelSelect;
            }
        }

        // ==========================================
        // 9. GAME OVER MODAL
        // ==========================================
        private void DrawGameOverModal(float screenW, float screenH)
        {
            GUI.DrawTexture(new Rect(0, 0, screenW, screenH), texDarkOverlay);

            float modalW = Mathf.Clamp(screenW * 0.48f, 380f, 540f);
            float modalH = 280f;
            float modalX = (screenW - modalW) * 0.5f;
            float modalY = (screenH - modalH) * 0.45f;
            Rect gRect = new Rect(modalX, modalY, modalW, modalH);

            GUI.DrawTexture(gRect, texModalBg);
            GUI.color = new Color(1f, 0.35f, 0.35f);
            GUI.Label(new Rect(modalX, modalY + 25f, modalW, 45f), "💀 GAME OVER 💀", bannerStyle);
            GUI.color = Color.white;
            GUI.Label(new Rect(modalX, modalY + 75f, modalW, 30f), $"WORLD {GameManager.Instance.worldName} : {GameManager.Instance.currentLevelTitle}", modalStyle);
            GUI.Label(new Rect(modalX, modalY + 115f, modalW, 30f), $"FINAL SCORE: {GameManager.Instance.score:D6}   COINS: {GameManager.Instance.coins:D2}", cardLabelStyle);

            float bW = (modalW - 40f) * 0.5f;
            float bY = modalY + 185f;

            if (GUI.Button(new Rect(modalX + 15f, bY, bW, 52f), "🔄 RETRY LEVEL", menuBtnStyle))
            {
                GameManager.Instance.RestartCurrentLevel();
            }

            if (GUI.Button(new Rect(modalX + 25f + bW, bY, bW, 52f), "🏠 MAIN MENU", menuBtnStyle))
            {
                GameManager.Instance.gameState = GameState.MainMenu;
            }
        }

        // ==========================================
        // 10. GRAND VICTORY CAMPAIGN COMPLETE MODAL
        // ==========================================
        private void DrawGameWonModal(float screenW, float screenH)
        {
            GUI.DrawTexture(new Rect(0, 0, screenW, screenH), texDarkOverlay);

            float modalW = Mathf.Clamp(screenW * 0.68f, 480f, 800f);
            float modalH = 420f;
            float modalX = (screenW - modalW) * 0.5f;
            float modalY = (screenH - modalH) * 0.45f;
            Rect wRect = new Rect(modalX, modalY, modalW, modalH);

            GUI.DrawTexture(wRect, texModalBg);

            GUI.Label(new Rect(modalX, modalY + 25f, modalW, 50f), "👑 ALL 20 WORLDS CLEARED! 👑", bannerStyle);
            GUI.Label(new Rect(modalX, modalY + 80f, modalW, 35f), "★ YOU DEFEATED THE VOLCANO KING! ★", subTitleStyle);

            string winText =
                "CONGRATULATIONS! THE ENTIRE MUSHROOM KINGDOM IS RESTORED!\n\n" +
                $"• CAMPAIGN TOTAL SCORE: {GameManager.Instance.score:D6}\n" +
                $"• TOTAL COINS HOARDED: {SaveManager.TotalCoins}\n" +
                $"• ALL 4 REALMS SAVED: GRASSLAND, DESERT, FROZEN, VOLCANO\n\n" +
                "THANK YOU FOR PLAYING SUPER MARIO 2D ADVENTURE!";

            GUI.Label(new Rect(modalX + 30f, modalY + 125f, modalW - 60f, 180f), winText, controlsStyle);

            float bW = 280f;
            if (GUI.Button(new Rect(modalX + (modalW - bW) * 0.5f, modalY + modalH - 70f, bW, 50f), "🏆 PLAY AGAIN FROM LEVEL 1", menuBtnStyle))
            {
                GameManager.Instance.LoadLevel(1);
            }
        }
    }
}
