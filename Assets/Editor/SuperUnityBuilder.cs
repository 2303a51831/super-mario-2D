#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using SuperUnity.Player;
using SuperUnity.Enemies;
using SuperUnity.Blocks;
using SuperUnity.Items;
using SuperUnity.Core;
using SuperUnity.UI;
using System.Collections.Generic;

namespace SuperUnity.Editor
{
    public static class SuperUnityBuilder
    {
        [MenuItem("Tools/Super Unity/Build All Levels (1-1, 1-2, 1-3)")]
        public static void BuildAllLevels()
        {
            Debug.Log("<color=yellow><b>[Super Unity] Building All Levels...</b></color>");

            // Build Level 1-1 and SuperUnityScene
            BuildLevelScene(1, "Assets/Scenes/1-1.unity");
            BuildLevelScene(1, "Assets/Scenes/SuperUnityScene.unity");

            // Build Level 1-2 (Underground)
            BuildLevelScene(2, "Assets/Scenes/1-2.unity");

            // Build Level 1-3 (Athletic Sky)
            BuildLevelScene(3, "Assets/Scenes/1-3.unity");

            // Register all scenes in Unity Build Settings
            var buildScenes = new List<EditorBuildSettingsScene>
            {
                new EditorBuildSettingsScene("Assets/Scenes/1-1.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/1-2.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/1-3.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/SuperUnityScene.unity", true)
            };
            EditorBuildSettings.scenes = buildScenes.ToArray();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("<color=green><b>[Super Unity] SUCCESS! All 3 Levels built and registered in Build Settings!</b></color>");
        }

        [MenuItem("Tools/Super Unity/Build Level 1-1 (Overworld)")]
        public static void BuildLevel1() => BuildLevelScene(1, "Assets/Scenes/1-1.unity");

        [MenuItem("Tools/Super Unity/Build Level 1-2 (Underground)")]
        public static void BuildLevel2() => BuildLevelScene(2, "Assets/Scenes/1-2.unity");

        [MenuItem("Tools/Super Unity/Build Level 1-3 (Athletic Sky)")]
        public static void BuildLevel3() => BuildLevelScene(3, "Assets/Scenes/1-3.unity");

        public static void BuildLevelScene(int stage, string scenePath)
        {
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Frictionless physics material
            var noFriction = new PhysicsMaterial2D("NoFriction") { friction = 0f, bounciness = 0f };

            // 1. Camera & Audio Listener
            GameObject camObj = new GameObject("Main Camera");
            Camera cam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
            cam.orthographic = true;
            cam.orthographicSize = 7.5f;

            if (stage == 2)
            {
                cam.backgroundColor = new Color(0.04f, 0.05f, 0.1f); // Dark Underground
            }
            else if (stage == 3)
            {
                cam.backgroundColor = new Color(0.42f, 0.6f, 0.95f); // Sky Cyan
            }
            else
            {
                cam.backgroundColor = new Color(0.36f, 0.58f, 0.99f); // Classic Overworld Sky Blue
            }

            cam.clearFlags = CameraClearFlags.SolidColor;
            camObj.AddComponent<AudioListener>();
            camObj.transform.position = new Vector3(3f, 6.0f, -10f);

            var camFollow = camObj.AddComponent<CameraFollow>();
            camFollow.fixedY = 6.0f;
            camFollow.minX = 0f;
            camFollow.maxX = 210f;
            camFollow.smoothSpeed = 10f;

            // 2. Managers
            GameObject managersObj = new GameObject("--- GAME MANAGERS ---");
            var gm = managersObj.AddComponent<GameManager>();
            gm.currentLevel = stage;
            gm.timeRemaining = 400f;

            managersObj.AddComponent<SoundManager>();
            managersObj.AddComponent<UIManager>();

            // Sprite Loader Helper
            Sprite LoadSprite(string name) => AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/{name}.png");

            Sprite sMarioIdle = LoadSprite("mario_small_idle");
            Sprite sMarioRun1 = LoadSprite("mario_small_run1");
            Sprite sMarioRun2 = LoadSprite("mario_small_run2");
            Sprite sMarioRun3 = LoadSprite("mario_small_run3");
            Sprite sMarioJump = LoadSprite("mario_small_jump");
            Sprite sMarioSkid = LoadSprite("mario_small_skid");
            Sprite sMarioDie = LoadSprite("mario_small_die");

            Sprite sSuperIdle = LoadSprite("mario_super_idle");
            Sprite sSuperRun1 = LoadSprite("mario_super_run1");
            Sprite sSuperRun2 = LoadSprite("mario_super_run2");
            Sprite sSuperJump = LoadSprite("mario_super_jump");
            Sprite sSuperSkid = LoadSprite("mario_super_skid");
            Sprite sSuperCrouch = LoadSprite("mario_super_crouch");

            Sprite sFireIdle = LoadSprite("mario_fire_idle");
            Sprite sFireRun1 = LoadSprite("mario_fire_run1");

            Sprite sGoombaWalk1 = LoadSprite("goomba_walk1");
            Sprite sGoombaWalk2 = LoadSprite("goomba_walk2");
            Sprite sGoombaSquish = LoadSprite("goomba_squished");

            Sprite sKoopaWalk1 = LoadSprite("koopa_walk1");
            Sprite sKoopaWalk2 = LoadSprite("koopa_walk2");
            Sprite sKoopaShell = LoadSprite("koopa_shell");

            Sprite sPiranha = LoadSprite("piranha_plant");

            Sprite sQ1 = LoadSprite("block_question_1");
            Sprite sQ2 = LoadSprite("block_question_2");
            Sprite sQ3 = LoadSprite("block_question_3");
            Sprite sEmpty = LoadSprite("block_empty");
            Sprite sBrick = LoadSprite("block_brick");
            Sprite sDebris = LoadSprite("brick_debris");

            Sprite sMushroom = LoadSprite("item_mushroom");
            Sprite sFlower = LoadSprite("item_flower");
            Sprite sFireball = LoadSprite("item_fireball");
            Sprite sCoin1 = LoadSprite("item_coin_1");
            Sprite sCoin2 = LoadSprite("item_coin_2");

            Sprite sGround = LoadSprite("tile_ground");
            Sprite sDirt = LoadSprite("tile_dirt");
            Sprite sPipeTop = LoadSprite("pipe_top");
            Sprite sPipeBody = LoadSprite("pipe_body");
            Sprite sFlagpole = LoadSprite("flagpole");
            Sprite sFlag = LoadSprite("flag");
            Sprite sCastle = LoadSprite("castle");

            // 3. Templates / Prefabs
            GameObject fireballTemplate = new GameObject("FireballPrefab");
            var fbSr = fireballTemplate.AddComponent<SpriteRenderer>();
            fbSr.sprite = sFireball;
            fbSr.sortingOrder = 6;
            fireballTemplate.AddComponent<CircleCollider2D>().radius = 0.25f;
            fireballTemplate.AddComponent<Fireball>();
            fireballTemplate.SetActive(false);

            GameObject coinEffectTemplate = new GameObject("CoinEffectPrefab");
            var ceSr = coinEffectTemplate.AddComponent<SpriteRenderer>();
            ceSr.sprite = sCoin1;
            ceSr.sortingOrder = 10;
            coinEffectTemplate.AddComponent<CoinEffect>();
            coinEffectTemplate.SetActive(false);

            GameObject mushroomTemplate = new GameObject("MushroomPrefab");
            var mSr = mushroomTemplate.AddComponent<SpriteRenderer>();
            mSr.sprite = sMushroom;
            mSr.sortingOrder = 4;
            var mCol = mushroomTemplate.AddComponent<BoxCollider2D>();
            mCol.size = new Vector2(0.9f, 0.9f);
            mCol.offset = new Vector2(0f, 0.45f);
            mushroomTemplate.AddComponent<Mushroom>();
            mushroomTemplate.SetActive(false);

            GameObject flowerTemplate = new GameObject("FlowerPrefab");
            var flSr = flowerTemplate.AddComponent<SpriteRenderer>();
            flSr.sprite = sFlower;
            flSr.sortingOrder = 4;
            var flCol = flowerTemplate.AddComponent<BoxCollider2D>();
            flCol.isTrigger = true;
            flCol.size = new Vector2(0.9f, 0.9f);
            flCol.offset = new Vector2(0f, 0.45f);
            flowerTemplate.AddComponent<FireFlower>();
            flowerTemplate.SetActive(false);

            // 4. Mario Player
            GameObject marioObj = new GameObject("Mario");
            marioObj.transform.position = new Vector3(2.5f, 2.5f, 0f);
            var marioSr = marioObj.AddComponent<SpriteRenderer>();
            marioSr.sprite = sMarioIdle;
            marioSr.sortingOrder = 5;

            var marioCtrl = marioObj.AddComponent<MarioController>();
            marioCtrl.spriteRenderer = marioSr;
            marioCtrl.fireballPrefab = fireballTemplate;
            marioCtrl.jumpForce = 16.5f;

            var marioCol = marioObj.GetComponent<BoxCollider2D>();
            if (marioCol != null) marioCol.sharedMaterial = noFriction;

            var marioAnim = marioObj.AddComponent<MarioAnimation>();
            marioAnim.smallIdle = sMarioIdle;
            marioAnim.smallRun = new Sprite[] { sMarioRun1, sMarioRun2, sMarioRun3 };
            marioAnim.smallJump = sMarioJump;
            marioAnim.smallSkid = sMarioSkid;
            marioAnim.smallDie = sMarioDie;

            marioAnim.superIdle = sSuperIdle;
            marioAnim.superRun = new Sprite[] { sSuperRun1, sSuperRun2 };
            marioAnim.superJump = sSuperJump;
            marioAnim.superSkid = sSuperSkid;
            marioAnim.superCrouch = sSuperCrouch;

            marioAnim.fireIdle = sFireIdle;
            marioAnim.fireRun = new Sprite[] { sFireRun1 };

            camFollow.target = marioObj.transform;

            // 5. Level Environment Root
            GameObject envRoot = new GameObject("--- LEVEL ENVIRONMENT ---");

            // Helpers
            void CreateGroundSection(float startX, float length, float groundTopY = 1.5f)
            {
                GameObject section = new GameObject($"Ground_{startX}_{startX + length}");
                section.transform.parent = envRoot.transform;

                for (int x = 0; x < length; x++)
                {
                    GameObject grass = new GameObject($"G_{x}");
                    grass.transform.parent = section.transform;
                    grass.transform.position = new Vector3(startX + x + 0.5f, groundTopY, 0f);
                    var sr = grass.AddComponent<SpriteRenderer>();
                    sr.sprite = sGround;
                    if (stage == 2) sr.color = new Color(0.2f, 0.5f, 0.9f); // Blue underground ground
                    sr.sortingOrder = 2;

                    GameObject dirt = new GameObject($"D_{x}");
                    dirt.transform.parent = section.transform;
                    dirt.transform.position = new Vector3(startX + x + 0.5f, groundTopY - 1f, 0f);
                    var srD = dirt.AddComponent<SpriteRenderer>();
                    srD.sprite = sDirt;
                    if (stage == 2) srD.color = new Color(0.1f, 0.3f, 0.6f);
                    srD.sortingOrder = 2;
                }

                var col = section.AddComponent<BoxCollider2D>();
                col.size = new Vector2(length, 2f);
                col.offset = new Vector2(startX + length * 0.5f, groundTopY - 0.5f);
                col.sharedMaterial = noFriction;
            }

            // Death Pit
            GameObject deathPit = new GameObject("DeathPit");
            deathPit.transform.parent = envRoot.transform;
            deathPit.transform.position = new Vector3(100f, -3f, 0f);
            var pitCol = deathPit.AddComponent<BoxCollider2D>();
            pitCol.isTrigger = true;
            pitCol.size = new Vector2(250f, 4f);
            deathPit.AddComponent<DeathPit>();

            // Blocks Root
            GameObject blocksRoot = new GameObject("--- BLOCKS ---");
            blocksRoot.transform.parent = envRoot.transform;

            void CreateQuestionBlock(float x, float y, BlockContent type)
            {
                GameObject qb = new GameObject($"QBlock_{x}_{y}");
                qb.transform.parent = blocksRoot.transform;
                qb.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0f);

                var sr = qb.AddComponent<SpriteRenderer>();
                sr.sprite = sQ1;
                sr.sortingOrder = 3;

                var comp = qb.AddComponent<QuestionBlock>();
                comp.content = type;
                comp.activeFrames = new Sprite[] { sQ1, sQ2, sQ3 };
                comp.emptySprite = sEmpty;
                comp.coinEffectPrefab = coinEffectTemplate;
                comp.mushroomPrefab = mushroomTemplate;
                comp.fireFlowerPrefab = flowerTemplate;

                var col = qb.AddComponent<BoxCollider2D>();
                col.size = Vector2.one;
                col.sharedMaterial = noFriction;
            }

            void CreateBrickBlock(float x, float y, bool hasCoin = false)
            {
                GameObject bb = new GameObject($"Brick_{x}_{y}");
                bb.transform.parent = blocksRoot.transform;
                bb.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0f);

                var sr = bb.AddComponent<SpriteRenderer>();
                sr.sprite = sBrick;
                if (stage == 2) sr.color = new Color(0.2f, 0.5f, 0.9f); // Blue underground bricks
                sr.sortingOrder = 3;

                var comp = bb.AddComponent<BrickBlock>();
                comp.containsCoin = hasCoin;
                comp.coinCount = hasCoin ? 5 : 0;
                comp.brickDebrisPrefab = null;
                comp.coinEffectPrefab = coinEffectTemplate;

                var col = bb.AddComponent<BoxCollider2D>();
                col.size = Vector2.one;
                col.sharedMaterial = noFriction;
            }

            // Pipes Root
            GameObject pipesRoot = new GameObject("--- PIPES ---");
            pipesRoot.transform.parent = envRoot.transform;

            void CreatePipe(float x, int height, bool hasPiranha = true)
            {
                GameObject pipe = new GameObject($"Pipe_{x}");
                pipe.transform.parent = pipesRoot.transform;
                pipe.transform.position = new Vector3(x, 2.0f, 0f);

                for (int h = 0; h < height - 1; h++)
                {
                    GameObject body = new GameObject($"Body_{h}");
                    body.transform.parent = pipe.transform;
                    body.transform.position = new Vector3(x + 1.0f, 2.0f + h + 0.5f, 0f);
                    body.transform.localScale = new Vector3(2f, 1f, 1f);
                    var sr = body.AddComponent<SpriteRenderer>();
                    sr.sprite = sPipeBody;
                    sr.drawMode = SpriteDrawMode.Sliced;
                    sr.size = new Vector2(2f, 1f);
                    if (stage == 2) sr.color = new Color(0.15f, 0.45f, 0.8f);
                    sr.sortingOrder = 3;
                }

                GameObject top = new GameObject("Top");
                top.transform.parent = pipe.transform;
                top.transform.position = new Vector3(x + 1.0f, 2.0f + height - 0.5f, 0f);
                top.transform.localScale = new Vector3(2f, 1f, 1f);
                var srTop = top.AddComponent<SpriteRenderer>();
                srTop.sprite = sPipeTop;
                srTop.drawMode = SpriteDrawMode.Sliced;
                srTop.size = new Vector2(2f, 1f);
                if (stage == 2) srTop.color = new Color(0.15f, 0.45f, 0.8f);
                srTop.sortingOrder = 3;

                var col = pipe.AddComponent<BoxCollider2D>();
                col.size = new Vector2(2.0f, height);
                col.offset = new Vector2(1.0f, height * 0.5f);
                col.sharedMaterial = noFriction;

                if (hasPiranha)
                {
                    GameObject piranha = new GameObject("PiranhaPlant");
                    piranha.transform.parent = pipe.transform;
                    piranha.transform.position = new Vector3(x + 1.0f, 2.0f + height - 0.5f, 0f);

                    var pSr = piranha.AddComponent<SpriteRenderer>();
                    pSr.sprite = sPiranha;
                    pSr.sortingOrder = 2;

                    var pCol = piranha.AddComponent<BoxCollider2D>();
                    pCol.isTrigger = true;
                    pCol.size = new Vector2(0.9f, 1.4f);

                    var pp = piranha.AddComponent<PiranhaPlant>();
                    pp.spriteRenderer = pSr;
                    pp.moveDistance = 1.6f;
                }
            }

            // Coins Root
            GameObject coinsRoot = new GameObject("--- COINS ---");
            coinsRoot.transform.parent = envRoot.transform;

            void CreateCoin(float x, float y)
            {
                GameObject coin = new GameObject($"Coin_{x}_{y}");
                coin.transform.parent = coinsRoot.transform;
                coin.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0f);

                var sr = coin.AddComponent<SpriteRenderer>();
                sr.sprite = sCoin1;
                sr.sortingOrder = 5;

                var col = coin.AddComponent<CircleCollider2D>();
                col.isTrigger = true;
                col.radius = 0.4f;

                var fc = coin.AddComponent<FloatingCoin>();
                fc.frames = new Sprite[] { sCoin1, sCoin2 };
            }

            // Enemies Root
            GameObject enemiesRoot = new GameObject("--- ENEMIES ---");
            enemiesRoot.transform.parent = envRoot.transform;

            void CreateGoomba(float x, float y = 2.0f)
            {
                GameObject g = new GameObject($"Goomba_{x}");
                g.transform.parent = enemiesRoot.transform;
                g.transform.position = new Vector3(x + 0.5f, y + 0.45f, 0f);

                var sr = g.AddComponent<SpriteRenderer>();
                sr.sprite = sGoombaWalk1;
                if (stage == 2) sr.color = new Color(0.2f, 0.6f, 0.95f); // Blue underground Goomba
                sr.sortingOrder = 4;

                var col = g.AddComponent<BoxCollider2D>();
                col.size = new Vector2(0.85f, 0.85f);
                col.sharedMaterial = noFriction;

                var goomba = g.AddComponent<Goomba>();
                goomba.walkSprite1 = sGoombaWalk1;
                goomba.walkSprite2 = sGoombaWalk2;
                goomba.squishedSprite = sGoombaSquish;
            }

            void CreateKoopa(float x, float y = 2.0f)
            {
                GameObject k = new GameObject($"Koopa_{x}");
                k.transform.parent = enemiesRoot.transform;
                k.transform.position = new Vector3(x + 0.5f, y + 0.9f, 0f);

                var sr = k.AddComponent<SpriteRenderer>();
                sr.sprite = sKoopaWalk1;
                if (stage == 2) sr.color = new Color(0.2f, 0.7f, 1f);
                sr.sortingOrder = 4;

                var col = k.AddComponent<BoxCollider2D>();
                col.size = new Vector2(0.85f, 1.4f);
                col.sharedMaterial = noFriction;

                var koopa = k.AddComponent<Koopa>();
                koopa.walkSprite1 = sKoopaWalk1;
                koopa.walkSprite2 = sKoopaWalk2;
                koopa.shellSprite = sKoopaShell;
            }

            void CreateStaircase(float startX, int steps, bool ascending)
            {
                for (int s = 0; s < steps; s++)
                {
                    int height = ascending ? (s + 1) : (steps - s);
                    float x = startX + s;
                    for (int h = 0; h < height; h++)
                    {
                        CreateBrickBlock(x, 2 + h);
                    }
                }
            }

            // === STARTING COINS (Common to all stages at spawn x = 4 to 10) ===
            for (int cx = 4; cx <= 10; cx++)
            {
                CreateCoin(cx, 2.5f);
            }

            // === STAGE SPECIFIC LAYOUT ===
            if (stage == 1)
            {
                // WORLD 1-1 OVERWORLD
                CreateGroundSection(0, 69);
                CreateGroundSection(71, 15);
                CreateGroundSection(89, 64);
                CreateGroundSection(155, 60);

                CreateQuestionBlock(16, 5, BlockContent.Coin);
                CreateBrickBlock(20, 5);
                CreateQuestionBlock(21, 5, BlockContent.Mushroom);
                CreateBrickBlock(22, 5);
                CreateQuestionBlock(23, 5, BlockContent.Coin);
                CreateBrickBlock(24, 5);
                CreateQuestionBlock(22, 9, BlockContent.Coin);

                CreatePipe(28, 2, true);
                CreatePipe(38, 3, true);
                CreatePipe(46, 4, true);
                CreatePipe(57, 4, false);

                // Pipe coins
                CreateCoin(28.5f, 4.5f);
                CreateCoin(29.5f, 4.5f);
                CreateCoin(38.5f, 5.5f);
                CreateCoin(39.5f, 5.5f);
                CreateCoin(46.5f, 6.5f);
                CreateCoin(47.5f, 6.5f);

                CreateGoomba(22);
                CreateGoomba(41);
                CreateGoomba(51);
                CreateGoomba(53);
                CreateKoopa(106);
                CreateGoomba(124);
                CreateGoomba(126);

                CreateBrickBlock(77, 5);
                CreateQuestionBlock(78, 5, BlockContent.Mushroom);
                CreateBrickBlock(79, 5);

                for (int bx = 80; bx <= 87; bx++)
                {
                    CreateBrickBlock(bx, 9);
                    CreateCoin(bx, 11);
                }

                CreateBrickBlock(91, 9);
                CreateBrickBlock(92, 9);
                CreateBrickBlock(93, 9);
                CreateQuestionBlock(94, 9, BlockContent.Coin);
                CreateBrickBlock(100, 5, true);

                CreateStaircase(134, 4, true);
                CreateStaircase(140, 4, false);
                CreateStaircase(148, 4, true);
                CreateStaircase(155, 4, false);
                CreateStaircase(181, 8, true);
            }
            else if (stage == 2)
            {
                // WORLD 1-2 UNDERGROUND BLUE CAVE
                CreateGroundSection(0, 45);
                CreateGroundSection(49, 35);
                CreateGroundSection(88, 30);
                CreateGroundSection(122, 25);
                CreateGroundSection(151, 60);

                // Blue ceiling
                for (int cx = 0; cx < 200; cx += 2)
                {
                    CreateBrickBlock(cx, 11);
                }

                // Underground coin gallery
                for (int cx = 15; cx <= 28; cx++)
                {
                    CreateCoin(cx, 4);
                    if (cx % 3 == 0) CreateBrickBlock(cx, 5);
                }

                CreatePipe(32, 3, true);
                CreatePipe(40, 4, true);
                CreatePipe(70, 3, true);
                CreatePipe(105, 4, true);
                CreatePipe(140, 2, false);

                // Floating stepping platforms over pits
                for (int px = 46; px <= 48; px++) CreateBrickBlock(px, 4);
                for (int px = 85; px <= 87; px++) CreateBrickBlock(px, 5);
                for (int px = 119; px <= 121; px++) CreateBrickBlock(px, 5);

                CreateGoomba(18);
                CreateGoomba(24);
                CreateKoopa(55);
                CreateGoomba(62);
                CreateGoomba(95);
                CreateGoomba(98);
                CreateKoopa(130);

                CreateQuestionBlock(12, 5, BlockContent.Mushroom);
                CreateQuestionBlock(50, 6, BlockContent.Coin);
                CreateQuestionBlock(52, 6, BlockContent.Coin);
                CreateQuestionBlock(54, 6, BlockContent.Coin);

                CreateStaircase(175, 7, true);
            }
            else if (stage == 3)
            {
                // WORLD 1-3 ATHLETIC SKY
                CreateGroundSection(0, 22, 1.5f);
                CreateGroundSection(26, 18, 3.5f);
                CreateGroundSection(48, 24, 1.5f);
                CreateGroundSection(76, 20, 4.5f);
                CreateGroundSection(100, 22, 2.5f);
                CreateGroundSection(126, 26, 3.5f);
                CreateGroundSection(156, 56, 1.5f);

                // High leap coin arcs
                for (int cx = 22; cx <= 25; cx++) CreateCoin(cx, 5);
                for (int cx = 44; cx <= 47; cx++) CreateCoin(cx, 6);
                for (int cx = 72; cx <= 75; cx++) CreateCoin(cx, 7);
                for (int cx = 96; cx <= 99; cx++) CreateCoin(cx, 6);

                CreateQuestionBlock(30, 7, BlockContent.Mushroom);
                CreateBrickBlock(31, 7);
                CreateQuestionBlock(32, 7, BlockContent.Coin);

                CreateBrickBlock(56, 6);
                CreateQuestionBlock(57, 6, BlockContent.Coin);
                CreateBrickBlock(58, 6);

                CreateKoopa(33, 4.0f);
                CreateKoopa(55, 2.0f);
                CreateKoopa(80, 5.0f);
                CreateKoopa(110, 3.0f);
                CreateKoopa(135, 4.0f);

                CreateStaircase(172, 9, true);
            }

            // Flagpole and Castle
            GameObject goalObj = new GameObject("--- GOAL ---");
            goalObj.transform.parent = envRoot.transform;

            GameObject flagpole = new GameObject("Flagpole");
            flagpole.transform.parent = goalObj.transform;
            flagpole.transform.position = new Vector3(198.5f, 2.0f, 0f);

            for (int py = 0; py < 10; py++)
            {
                GameObject poleSeg = new GameObject($"Pole_{py}");
                poleSeg.transform.parent = flagpole.transform;
                poleSeg.transform.position = new Vector3(198.5f, 2.5f + py, 0f);
                var pSr = poleSeg.AddComponent<SpriteRenderer>();
                pSr.sprite = sFlagpole;
                pSr.sortingOrder = 3;
            }

            GameObject flag = new GameObject("Flag");
            flag.transform.parent = flagpole.transform;
            flag.transform.position = new Vector3(198.0f, 11.5f, 0f);
            var flgSr = flag.AddComponent<SpriteRenderer>();
            flgSr.sprite = sFlag;
            flgSr.sortingOrder = 4;

            GameObject castle = new GameObject("Castle");
            castle.transform.parent = goalObj.transform;
            castle.transform.position = new Vector3(205.0f, 4.5f, 0f);
            var cSr = castle.AddComponent<SpriteRenderer>();
            cSr.sprite = sCastle;
            cSr.sortingOrder = 2;

            GameObject castleDoor = new GameObject("CastleDoor");
            castleDoor.transform.parent = castle.transform;
            castleDoor.transform.position = new Vector3(205.0f, 2.5f, 0f);

            var poleCol = flagpole.AddComponent<BoxCollider2D>();
            poleCol.isTrigger = true;
            poleCol.size = new Vector2(0.5f, 10f);
            poleCol.offset = new Vector2(0f, 5.0f);

            var fpComp = flagpole.AddComponent<Flagpole>();
            fpComp.flag = flag.transform;
            fpComp.poleBottom = flagpole.transform;
            fpComp.castleDoor = castleDoor.transform;

            // Save Scene
            EditorSceneManager.SaveScene(newScene, scenePath);
            Debug.Log($"<color=cyan>[Super Unity] Saved Scene: {scenePath}</color>");
        }
    }
}
#endif
