using UnityEngine;

[DefaultExecutionOrder(100)]
public class GameHUD : MonoBehaviour
{
    private GUIStyle headerStyle;
    private GUIStyle valueStyle;
    private GUIStyle messageStyle;
    private GUIStyle subMessageStyle;
    private Texture2D coinIconTexture;

    private void Awake()
    {
        CreateCoinTexture();
    }

    private void CreateCoinTexture()
    {
        // 12x16 retro coin texture in procedural NES gold palette
        coinIconTexture = new Texture2D(12, 16);
        Color gold = new Color(0.98f, 0.78f, 0.16f);
        Color darkGold = new Color(0.70f, 0.45f, 0.05f);
        Color highlight = new Color(1f, 0.95f, 0.6f);
        Color clear = Color.clear;

        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 12; x++)
            {
                if (x == 0 || x == 11 || y == 0 || y == 15) {
                    coinIconTexture.SetPixel(x, y, clear);
                } else if (x == 1 || x == 10 || y == 1 || y == 14) {
                    coinIconTexture.SetPixel(x, y, darkGold);
                } else if (x >= 3 && x <= 4 && y >= 4 && y <= 11) {
                    coinIconTexture.SetPixel(x, y, highlight);
                } else {
                    coinIconTexture.SetPixel(x, y, gold);
                }
            }
        }
        coinIconTexture.filterMode = FilterMode.Point;
        coinIconTexture.Apply();
    }

    private void InitStyles()
    {
        if (headerStyle != null) return;

        headerStyle = new GUIStyle();
        headerStyle.fontSize = 20;
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.normal.textColor = Color.white;
        headerStyle.alignment = TextAnchor.UpperLeft;

        valueStyle = new GUIStyle(headerStyle);
        valueStyle.normal.textColor = Color.white;

        messageStyle = new GUIStyle();
        messageStyle.fontSize = 36;
        messageStyle.fontStyle = FontStyle.Bold;
        messageStyle.normal.textColor = Color.white;
        messageStyle.alignment = TextAnchor.MiddleCenter;

        subMessageStyle = new GUIStyle(messageStyle);
        subMessageStyle.fontSize = 22;
        subMessageStyle.normal.textColor = new Color(0.98f, 0.85f, 0.25f);
    }

    private void OnGUI()
    {
        InitStyles();

        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        // Virtual 800x600 resolution scaling
        float targetWidth = 800f;
        float targetHeight = 600f;
        float scaleX = Screen.width / targetWidth;
        float scaleY = Screen.height / targetHeight;
        Matrix4x4 origMatrix = GUI.matrix;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scaleX, scaleY, 1f));

        // Column 1: MARIO & SCORE
        GUI.Label(new Rect(40, 16, 160, 24), "MARIO", headerStyle);
        GUI.Label(new Rect(40, 40, 160, 24), gm.score.ToString("D6"), valueStyle);

        // Column 2: COINS
        if (coinIconTexture != null) {
            GUI.DrawTexture(new Rect(240, 42, 12, 16), coinIconTexture);
        }
        GUI.Label(new Rect(258, 40, 100, 24), "x" + gm.coins.ToString("D2"), valueStyle);

        // Column 3: WORLD
        GUI.Label(new Rect(420, 16, 120, 24), "WORLD", headerStyle);
        GUI.Label(new Rect(435, 40, 120, 24), $"{gm.world}-{gm.stage}", valueStyle);

        // Column 4: TIME
        GUI.Label(new Rect(580, 16, 100, 24), "TIME", headerStyle);
        int timeInt = Mathf.CeilToInt(gm.timeRemaining);
        GUI.Label(new Rect(590, 40, 100, 24), timeInt.ToString("D3"), valueStyle);

        // Column 5: LIVES
        GUI.Label(new Rect(700, 16, 100, 24), "LIVES", headerStyle);
        GUI.Label(new Rect(715, 40, 100, 24), "x " + gm.lives, valueStyle);

        // Game Over Banner
        if (gm.isGameOver)
        {
            GUI.Box(new Rect(200, 220, 400, 120), GUIContent.none);
            GUI.Label(new Rect(200, 240, 400, 40), "GAME OVER", messageStyle);
            GUI.Label(new Rect(200, 285, 400, 30), "RESTARTING...", subMessageStyle);
        }

        // Victory Banner
        if (gm.isVictory)
        {
            GUI.Box(new Rect(150, 190, 500, 180), GUIContent.none);
            GUI.Label(new Rect(150, 210, 500, 40), "STAGE CLEAR!", messageStyle);
            GUI.Label(new Rect(150, 260, 500, 30), "ALL LEVELS COMPLETED!", subMessageStyle);
            GUI.Label(new Rect(150, 305, 500, 30), $"FINAL SCORE: {gm.score}", valueStyle);
        }

        GUI.matrix = origMatrix;
    }
}
