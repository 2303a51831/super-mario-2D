using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelBuilder
{
    private const string PREFAB_PATH = "Assets/Prefabs/";

    [MenuItem("Mario/Build All Levels (1-2 and 1-3)")]
    public static void BuildAllLevels()
    {
        BuildLevel12();
        BuildLevel13();
        UpdateBuildSettings();
        Debug.Log("Successfully built Level 1-2 and Level 1-3, and updated Build Settings!");
    }

    public static void BuildLevel12()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Load Prefabs
        GameObject ugBlockPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "UndergroundBlock.prefab");
        GameObject ugBrickPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "UndergroundBrick.prefab");
        GameObject pipePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Pipe.prefab");
        GameObject coinPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Coin.prefab");
        GameObject goombaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Goomba.prefab");
        GameObject koopaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Koopa.prefab");
        GameObject mysteryPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "MysteryBlock.prefab");
        GameObject marioPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Mario.prefab");
        GameObject flagPolePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Flag Pole.prefab");
        GameObject castlePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Castle.prefab");
        GameObject hardBlockPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "HardBlock.prefab");
        GameObject gameManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "GameManager.prefab");

        // Root Stage Container
        GameObject stage = new GameObject("Stage");

        // 1. Mario
        GameObject mario = (GameObject)PrefabUtility.InstantiatePrefab(marioPrefab, stage.transform);
        mario.transform.position = new Vector3(2f, 2f, 0f);

        // 2. Main Camera
        GameObject cameraObj = new GameObject("Main Camera");
        cameraObj.tag = "MainCamera";
        cameraObj.transform.position = new Vector3(8f, 6.5f, -10f);
        Camera cam = cameraObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black; // Underground pitch black theme
        cam.orthographic = true;
        cam.orthographicSize = 7f;
        cameraObj.AddComponent<AudioListener>();

        SideScrollingCamera ssc = cameraObj.AddComponent<SideScrollingCamera>();
        ssc.trackedObject = mario.transform;
        ssc.height = 6.5f;
        ssc.undergroundHeight = -9.5f;

        // 3. Game Manager
        if (gameManagerPrefab != null) {
            PrefabUtility.InstantiatePrefab(gameManagerPrefab);
        } else {
            GameObject gm = new GameObject("Game Manager");
            gm.AddComponent<GameManager>();
        }

        // 4. Death Barrier
        GameObject deathBarrier = new GameObject("Death Barrier");
        deathBarrier.transform.position = new Vector3(80f, -4f, 0f);
        BoxCollider2D bc = deathBarrier.AddComponent<BoxCollider2D>();
        bc.size = new Vector2(300f, 2f);
        bc.isTrigger = true;
        deathBarrier.AddComponent<DeathBarrier>();

        // 5. Grid
        GameObject grid = new GameObject("Grid");
        grid.transform.parent = stage.transform;
        grid.AddComponent<Grid>();

        // 6. Underground Ceiling (Solid roof at y = 13)
        GameObject ceiling = (GameObject)PrefabUtility.InstantiatePrefab(ugBlockPrefab, stage.transform);
        ceiling.name = "Ceiling";
        ceiling.transform.position = new Vector3(80f, 13f, 0f);
        SpriteRenderer ceilingSr = ceiling.GetComponent<SpriteRenderer>();
        if (ceilingSr != null) {
            ceilingSr.size = new Vector2(175f, 2f);
        }
        BoxCollider2D ceilingCol = ceiling.GetComponent<BoxCollider2D>();
        if (ceilingCol != null) {
            ceilingCol.size = new Vector2(175f, 2f);
        }

        // 7. Ground Segments (Floor at y = 0, height = 2, so surface is y = 1)
        CreateUndergroundGround(stage.transform, ugBlockPrefab, "Ground_1", 12.5f, 0f, 35f); // x: -5 to 30
        CreateUndergroundGround(stage.transform, ugBlockPrefab, "Ground_2", 48f, 0f, 28f);   // x: 34 to 62 (gap 30-34)
        CreateUndergroundGround(stage.transform, ugBlockPrefab, "Ground_3", 86.5f, 0f, 37f); // x: 68 to 105 (gap 62-68)
        CreateUndergroundGround(stage.transform, ugBlockPrefab, "Ground_4", 137.5f, 0f, 55f);// x: 110 to 165 (gap 105-110)

        // 8. Pipes
        CreatePipe(stage.transform, pipePrefab, 16f, 2f);
        CreatePipe(stage.transform, pipePrefab, 26f, 3f);
        CreatePipe(stage.transform, pipePrefab, 44f, 2f);
        CreatePipe(stage.transform, pipePrefab, 75f, 3f);
        CreatePipe(stage.transform, pipePrefab, 92f, 4f);

        // 9. Floating Platforms & Bricks
        // Over Gap 1 (x: 30 to 34): 3 bricks at y = 4 with 3 coins hovering
        for (int i = 0; i < 3; i++) {
            CreatePrefab(ugBrickPrefab, stage.transform, 31f + i, 4f);
            CreateCoin(stage.transform, coinPrefab, 31f + i, 5.5f);
        }

        // Mid-platform 1: Bricks & Mystery Blocks at y = 4 (x: 48 to 56)
        CreatePrefab(ugBrickPrefab, stage.transform, 48f, 4f);
        CreatePrefab(mysteryPrefab, stage.transform, 49f, 4f);
        CreatePrefab(ugBrickPrefab, stage.transform, 50f, 4f);
        CreatePrefab(mysteryPrefab, stage.transform, 51f, 4f);
        CreatePrefab(ugBrickPrefab, stage.transform, 52f, 4f);
        CreatePrefab(mysteryPrefab, stage.transform, 53f, 4f);
        CreatePrefab(ugBrickPrefab, stage.transform, 54f, 4f);

        // Over Gap 2 (x: 62 to 68): Stepping stone islands
        CreateUndergroundGround(stage.transform, ugBlockPrefab, "Step_1", 64f, 2f, 2f);
        CreateUndergroundGround(stage.transform, ugBlockPrefab, "Step_2", 66.5f, 4f, 2f);

        // Coin Arch over Gap 2
        CreateCoin(stage.transform, coinPrefab, 63f, 3.5f);
        CreateCoin(stage.transform, coinPrefab, 64f, 5f);
        CreateCoin(stage.transform, coinPrefab, 65f, 6.5f);
        CreateCoin(stage.transform, coinPrefab, 66f, 6.5f);
        CreateCoin(stage.transform, coinPrefab, 67f, 5f);
        CreateCoin(stage.transform, coinPrefab, 68f, 3.5f);

        // Upper Brick Platform at y = 5 (x: 80 to 88) with coins on top
        for (int i = 0; i < 8; i++) {
            CreatePrefab(ugBrickPrefab, stage.transform, 80f + i, 5f);
            CreateCoin(stage.transform, coinPrefab, 80f + i, 6.5f);
        }

        // Secret Coin Vault at y = 8 (x: 98 to 104)
        for (int i = 0; i < 6; i++) {
            CreateCoin(stage.transform, coinPrefab, 99f + i, 8f);
        }

        // 10. Enemies
        float groundY = 1.5f;
        CreateEnemy(goombaPrefab, stage.transform, 12f, groundY);
        CreateEnemy(goombaPrefab, stage.transform, 22f, groundY);
        CreateEnemy(goombaPrefab, stage.transform, 40f, groundY);
        CreateEnemy(koopaPrefab, stage.transform, 50f, 5.5f); // on brick platform
        CreateEnemy(goombaPrefab, stage.transform, 56f, groundY);
        CreateEnemy(goombaPrefab, stage.transform, 72f, groundY);
        CreateEnemy(koopaPrefab, stage.transform, 84f, 6.5f); // on upper platform
        CreateEnemy(goombaPrefab, stage.transform, 88f, groundY);
        CreateEnemy(goombaPrefab, stage.transform, 118f, groundY);
        CreateEnemy(goombaPrefab, stage.transform, 124f, groundY);
        CreateEnemy(koopaPrefab, stage.transform, 128f, groundY);

        // 11. Final Staircase & Exit (Stairs from x = 135 to 142)
        for (int step = 1; step <= 7; step++) {
            for (int h = 1; h <= step; h++) {
                CreatePrefab(hardBlockPrefab, stage.transform, 135f + step, h);
            }
        }

        // Castle & Flag Pole at end of 1-2
        GameObject castle = (GameObject)PrefabUtility.InstantiatePrefab(castlePrefab, stage.transform);
        castle.transform.position = new Vector3(158f, 1f, 0f);

        GameObject flagPole = (GameObject)PrefabUtility.InstantiatePrefab(flagPolePrefab, stage.transform);
        flagPole.transform.position = new Vector3(149f, 1f, 0f);
        FlagPole fp = flagPole.GetComponent<FlagPole>();
        if (fp != null) {
            fp.nextWorld = 1;
            fp.nextStage = 3; // Advance to World 1-3!
            fp.castle = castle.transform;
        }

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/1-2.unity");
    }

    public static void BuildLevel13()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Load Prefabs
        GameObject groundPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Ground.prefab");
        GameObject brickPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Brick.prefab");
        GameObject hardBlockPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "HardBlock.prefab");
        GameObject pipePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Pipe.prefab");
        GameObject coinPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Coin.prefab");
        GameObject goombaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Goomba.prefab");
        GameObject koopaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Koopa.prefab");
        GameObject mysteryPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "MysteryBlock.prefab");
        GameObject marioPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Mario.prefab");
        GameObject flagPolePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Flag Pole.prefab");
        GameObject castlePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Castle.prefab");
        GameObject cloud1 = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Cloud1.prefab");
        GameObject cloud2 = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Cloud2.prefab");
        GameObject cloud3 = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Cloud3.prefab");
        GameObject hill1 = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Hill1.prefab");
        GameObject hill2 = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Hill2.prefab");
        GameObject bush1 = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Bush1.prefab");
        GameObject bush2 = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "Bush2.prefab");
        GameObject gameManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH + "GameManager.prefab");

        GameObject stage = new GameObject("Stage");

        // 1. Mario
        GameObject mario = (GameObject)PrefabUtility.InstantiatePrefab(marioPrefab, stage.transform);
        mario.transform.position = new Vector3(2f, 2f, 0f);

        // 2. Camera (Classic Sky Blue)
        GameObject cameraObj = new GameObject("Main Camera");
        cameraObj.tag = "MainCamera";
        cameraObj.transform.position = new Vector3(8f, 6.5f, -10f);
        Camera cam = cameraObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.36f, 0.58f, 0.99f);
        cam.orthographic = true;
        cam.orthographicSize = 7f;
        cameraObj.AddComponent<AudioListener>();

        SideScrollingCamera ssc = cameraObj.AddComponent<SideScrollingCamera>();
        ssc.trackedObject = mario.transform;
        ssc.height = 6.5f;

        // 3. Game Manager
        if (gameManagerPrefab != null) {
            PrefabUtility.InstantiatePrefab(gameManagerPrefab);
        } else {
            GameObject gm = new GameObject("Game Manager");
            gm.AddComponent<GameManager>();
        }

        // 4. Death Barrier
        GameObject deathBarrier = new GameObject("Death Barrier");
        deathBarrier.transform.position = new Vector3(90f, -4f, 0f);
        BoxCollider2D bc = deathBarrier.AddComponent<BoxCollider2D>();
        bc.size = new Vector2(350f, 2f);
        bc.isTrigger = true;
        deathBarrier.AddComponent<DeathBarrier>();

        // 5. Scenery (Clouds & Hills for sky athletic feel)
        for (int x = 10; x <= 160; x += 25) {
            CreatePrefab(cloud1, stage.transform, x, 9.5f);
            CreatePrefab(cloud2, stage.transform, x + 8f, 11f);
            CreatePrefab(cloud3, stage.transform, x + 16f, 8.5f);
        }
        CreatePrefab(hill1, stage.transform, 5f, 1f);
        CreatePrefab(hill2, stage.transform, 45f, 1f);
        CreatePrefab(hill1, stage.transform, 115f, 1f);
        CreatePrefab(bush1, stage.transform, 10f, 1f);
        CreatePrefab(bush2, stage.transform, 120f, 1f);

        // 6. Ground Segments (Athletic bridges with gaps)
        CreateGround(stage.transform, groundPrefab, "Ground_Start", 7f, 0f, 24f); // x: -5 to 19
        CreateGround(stage.transform, groundPrefab, "Ground_Mid", 102f, 0f, 18f);  // x: 93 to 111
        CreateGround(stage.transform, groundPrefab, "Ground_End", 145f, 0f, 60f);  // x: 115 to 175

        // 7. High Athletic Floating Platforms
        // Bridge 1: x = 23 to 33, y = 3
        for (int i = 0; i < 10; i++) {
            CreatePrefab(brickPrefab, stage.transform, 23f + i, 3f);
        }
        CreatePrefab(mysteryPrefab, stage.transform, 27f, 6f);
        CreateCoin(stage.transform, coinPrefab, 26f, 6f);
        CreateCoin(stage.transform, coinPrefab, 28f, 6f);

        // Stepping stone at x = 36, y = 5
        CreatePrefab(hardBlockPrefab, stage.transform, 36f, 5f);
        CreatePrefab(hardBlockPrefab, stage.transform, 37f, 5f);
        CreateCoin(stage.transform, coinPrefab, 36.5f, 6.5f);

        // Bridge 2: x = 40 to 52, y = 6 (High bridge)
        for (int i = 0; i < 12; i++) {
            CreatePrefab(brickPrefab, stage.transform, 40f + i, 6f);
            if (i >= 3 && i <= 8) {
                CreateCoin(stage.transform, coinPrefab, 40f + i, 7.5f);
            }
        }
        CreateEnemy(koopaPrefab, stage.transform, 46f, 7.5f);

        // Stepping stone at x = 55, y = 4
        CreatePrefab(hardBlockPrefab, stage.transform, 55f, 4f);
        CreatePrefab(hardBlockPrefab, stage.transform, 56f, 4f);

        // Bridge 3: x = 60 to 72, y = 3
        for (int i = 0; i < 12; i++) {
            CreatePrefab(brickPrefab, stage.transform, 60f + i, 3f);
        }
        CreateEnemy(goombaPrefab, stage.transform, 64f, 4.5f);
        CreateEnemy(goombaPrefab, stage.transform, 68f, 4.5f);

        // Giant Coin Leap Arc between x = 73 and 82
        CreateCoin(stage.transform, coinPrefab, 73f, 4.5f);
        CreateCoin(stage.transform, coinPrefab, 75f, 6f);
        CreateCoin(stage.transform, coinPrefab, 77f, 7.5f);
        CreateCoin(stage.transform, coinPrefab, 79f, 7.5f);
        CreateCoin(stage.transform, coinPrefab, 81f, 6f);
        CreateCoin(stage.transform, coinPrefab, 83f, 4.5f);

        // Sky Island: x = 84 to 92, y = 6
        for (int i = 0; i < 8; i++) {
            CreatePrefab(hardBlockPrefab, stage.transform, 84f + i, 6f);
        }
        CreatePrefab(mysteryPrefab, stage.transform, 87f, 9f);
        CreatePrefab(mysteryPrefab, stage.transform, 89f, 9f);

        // Mid ground section (x = 93 to 111): Pipes and Enemies
        CreatePipe(stage.transform, pipePrefab, 98f, 2f);
        CreatePipe(stage.transform, pipePrefab, 106f, 3f);
        CreateEnemy(goombaPrefab, stage.transform, 95f, 1.5f);
        CreateEnemy(goombaPrefab, stage.transform, 102f, 1.5f);

        // 8. Monumental Staircase to Flagpole
        for (int step = 1; step <= 8; step++) {
            for (int h = 1; h <= step; h++) {
                CreatePrefab(hardBlockPrefab, stage.transform, 136f + step, h);
            }
        }

        // Castle & Flag Pole
        GameObject castle = (GameObject)PrefabUtility.InstantiatePrefab(castlePrefab, stage.transform);
        castle.transform.position = new Vector3(165f, 1f, 0f);

        GameObject flagPole = (GameObject)PrefabUtility.InstantiatePrefab(flagPolePrefab, stage.transform);
        flagPole.transform.position = new Vector3(154f, 1f, 0f);
        FlagPole fp = flagPole.GetComponent<FlagPole>();
        if (fp != null) {
            fp.nextWorld = 1;
            fp.nextStage = 1; // Completed all levels -> loops or triggers victory!
            fp.castle = castle.transform;
        }

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/1-3.unity");
    }

    public static void UpdateBuildSettings()
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>
        {
            new EditorBuildSettingsScene("Assets/Scenes/1-1.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/1-2.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/1-3.unity", true)
        };

        EditorBuildSettings.scenes = scenes.ToArray();
    }

    private static GameObject CreatePrefab(GameObject prefab, Transform parent, float x, float y)
    {
        if (prefab == null) return null;
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
        obj.transform.position = new Vector3(x, y, 0f);
        return obj;
    }

    private static void CreateCoin(Transform parent, GameObject coinPrefab, float x, float y)
    {
        CreatePrefab(coinPrefab, parent, x, y);
    }

    private static void CreateEnemy(GameObject enemyPrefab, Transform parent, float x, float y)
    {
        CreatePrefab(enemyPrefab, parent, x, y);
    }

    private static void CreatePipe(Transform parent, GameObject pipePrefab, float x, float height)
    {
        GameObject pipe = (GameObject)PrefabUtility.InstantiatePrefab(pipePrefab, parent);
        pipe.transform.position = new Vector3(x, height, 0f);
    }

    private static void CreateUndergroundGround(Transform parent, GameObject ugBlockPrefab, string name, float centerX, float y, float width)
    {
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(ugBlockPrefab, parent);
        obj.name = name;
        obj.transform.position = new Vector3(centerX, y, 0f);
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null) {
            sr.size = new Vector2(width, 2f);
        }
        BoxCollider2D col = obj.GetComponent<BoxCollider2D>();
        if (col != null) {
            col.size = new Vector2(width, 2f);
        }
    }

    private static void CreateGround(Transform parent, GameObject groundPrefab, string name, float centerX, float y, float width)
    {
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(groundPrefab, parent);
        obj.name = name;
        obj.transform.position = new Vector3(centerX, y, 0f);
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null) {
            sr.size = new Vector2(width, 2f);
        }
        BoxCollider2D col = obj.GetComponent<BoxCollider2D>();
        if (col != null) {
            col.size = new Vector2(width, 2f);
        }
    }
}
