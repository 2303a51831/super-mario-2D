using UnityEngine;
using SuperUnity.Player;
using SuperUnity.Enemies;
using SuperUnity.Blocks;
using SuperUnity.Items;
using SuperUnity.Core;
using SuperUnity.Levels;
using SuperUnity.Bosses;
using SuperUnity.Managers;

namespace SuperUnity
{
    public class SuperUnityBootstrapper : MonoBehaviour
    {
        private static bool spritesCached = false;
        private static Sprite sGround, sDirt, sBrick, sQ1, sQ2, sQ3, sEmpty;
        private static Sprite sPipeTop, sPipeBody, sPiranha;
        private static Sprite sGoombaWalk1, sGoombaWalk2, sGoombaSquish;
        private static Sprite sKoopaWalk1, sKoopaWalk2, sKoopaShell;
        private static Sprite sFlagpole, sFlag, sCastle;
        private static Sprite sMushroom, sFlower, sFireball, sCoin;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnRuntimeInit()
        {
            if (FindAnyObjectByType<SuperUnityBootstrapper>() == null)
            {
                GameObject boot = new GameObject("--- SUPER UNITY BOOTSTRAPPER ---");
                boot.AddComponent<SuperUnityBootstrapper>();
            }
        }

        private void Awake()
        {
            CacheSceneSprites();
        }

        private void Start()
        {
            CacheSceneSprites();
        }

        public static void CacheSceneSprites()
        {
            if (spritesCached) return;

            var renderers = FindObjectsByType<SpriteRenderer>();
            foreach (var sr in renderers)
            {
                if (sr == null || sr.sprite == null) continue;
                string sName = sr.sprite.name;
                if (sName == "tile_ground" && sGround == null) sGround = sr.sprite;
                else if (sName == "tile_dirt" && sDirt == null) sDirt = sr.sprite;
                else if (sName == "block_brick" && sBrick == null) sBrick = sr.sprite;
                else if (sName == "block_question_1" && sQ1 == null) sQ1 = sr.sprite;
                else if (sName == "block_question_2" && sQ2 == null) sQ2 = sr.sprite;
                else if (sName == "block_question_3" && sQ3 == null) sQ3 = sr.sprite;
                else if (sName == "block_empty" && sEmpty == null) sEmpty = sr.sprite;
                else if (sName == "pipe_top" && sPipeTop == null) sPipeTop = sr.sprite;
                else if (sName == "pipe_body" && sPipeBody == null) sPipeBody = sr.sprite;
                else if (sName == "piranha_plant" && sPiranha == null) sPiranha = sr.sprite;
                else if (sName == "goomba_walk1" && sGoombaWalk1 == null) sGoombaWalk1 = sr.sprite;
                else if (sName == "goomba_walk2" && sGoombaWalk2 == null) sGoombaWalk2 = sr.sprite;
                else if (sName == "goomba_squished" && sGoombaSquish == null) sGoombaSquish = sr.sprite;
                else if (sName == "koopa_walk1" && sKoopaWalk1 == null) sKoopaWalk1 = sr.sprite;
                else if (sName == "koopa_walk2" && sKoopaWalk2 == null) sKoopaWalk2 = sr.sprite;
                else if (sName == "koopa_shell" && sKoopaShell == null) sKoopaShell = sr.sprite;
                else if (sName == "flagpole" && sFlagpole == null) sFlagpole = sr.sprite;
                else if (sName == "flag" && sFlag == null) sFlag = sr.sprite;
                else if (sName == "castle" && sCastle == null) sCastle = sr.sprite;
                else if (sName == "item_mushroom" && sMushroom == null) sMushroom = sr.sprite;
                else if (sName == "item_flower" && sFlower == null) sFlower = sr.sprite;
                else if (sName == "item_fireball" && sFireball == null) sFireball = sr.sprite;
                else if (sName.StartsWith("item_coin") && sCoin == null) sCoin = sr.sprite;
            }

            // Fallback sprites
            if (sGround == null) sGround = CreateColorSprite(new Color(0f, 0.7f, 0.1f));
            if (sDirt == null) sDirt = CreateColorSprite(new Color(0.6f, 0.35f, 0.1f));
            if (sBrick == null) sBrick = CreateColorSprite(new Color(0.72f, 0.3f, 0.05f));
            if (sQ1 == null) sQ1 = CreateColorSprite(new Color(1f, 0.75f, 0.1f));
            if (sEmpty == null) sEmpty = CreateColorSprite(new Color(0.5f, 0.35f, 0.2f));
            if (sPipeTop == null) sPipeTop = CreateColorSprite(new Color(0f, 0.7f, 0.15f));
            if (sPipeBody == null) sPipeBody = CreateColorSprite(new Color(0f, 0.65f, 0.1f));
            if (sPiranha == null) sPiranha = CreateColorSprite(new Color(0.9f, 0.15f, 0.15f));
            if (sGoombaWalk1 == null) sGoombaWalk1 = CreateColorSprite(new Color(0.65f, 0.3f, 0.05f));
            if (sKoopaWalk1 == null) sKoopaWalk1 = CreateColorSprite(new Color(0f, 0.75f, 0.2f));
            if (sFlagpole == null) sFlagpole = CreateColorSprite(Color.white);
            if (sFlag == null) sFlag = CreateColorSprite(new Color(0f, 0.75f, 0.1f));
            if (sCastle == null) sCastle = CreateColorSprite(new Color(0.72f, 0.3f, 0.05f));

            spritesCached = true;
        }

        private static Sprite CreateColorSprite(Color col)
        {
            Texture2D tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            Color[] pixels = new Color[16 * 16];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = col;
            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16f);
        }

        public void BuildLevel(int levelNumber)
        {
            CacheSceneSprites();

            // 1. Destroy previous level environment & enemies
            string[] oldNames = new string[] {
                "--- LEVEL ENVIRONMENT ---",
                "Environment",
                "--- STARTING COINS ---",
                "--- BLOCKS ---",
                "--- PIPES ---",
                "--- ENEMIES ---",
                "--- GOAL ---",
                "--- CHECKPOINTS ---",
                "--- PLATFORMS ---",
                "--- HAZARDS ---",
                "--- BOSS ---"
            };
            foreach (var n in oldNames)
            {
                var obj = GameObject.Find(n);
                if (obj != null) Destroy(obj);
            }

            var oldEnemies = FindObjectsByType<EnemyBase>();
            foreach (var e in oldEnemies) Destroy(e.gameObject);

            var oldBosses = FindObjectsByType<BossBase>();
            foreach (var b in oldBosses) Destroy(b.gameObject);

            // Determine World (1 to 4) and Stage (1 to 5)
            int world = ((levelNumber - 1) / 5) + 1;
            int stage = ((levelNumber - 1) % 5) + 1;

            // 2. Camera Setup according to World
            var cam = Camera.main;
            if (cam != null)
            {
                Color skyColor = world switch
                {
                    1 => (stage == 3) ? new Color(0.04f, 0.05f, 0.1f) : ((stage == 5) ? new Color(0.12f, 0.12f, 0.18f) : new Color(0.36f, 0.58f, 0.99f)),
                    2 => (stage == 3 || stage == 5) ? new Color(0.12f, 0.08f, 0.04f) : new Color(0.92f, 0.68f, 0.32f),
                    3 => (stage == 3 || stage == 5) ? new Color(0.04f, 0.10f, 0.20f) : new Color(0.55f, 0.78f, 0.98f),
                    4 => (stage == 3 || stage == 5) ? new Color(0.08f, 0.02f, 0.02f) : new Color(0.18f, 0.05f, 0.05f),
                    _ => new Color(0.36f, 0.58f, 0.99f)
                };

                cam.backgroundColor = skyColor;
                cam.transform.position = new Vector3(3f, 6.0f, -10f);

                var camFollow = cam.GetComponent<CameraFollow>();
                if (camFollow != null)
                {
                    camFollow.minX = 0f;
                    camFollow.maxX = 225f;
                    camFollow.fixedY = 6.0f;
                    camFollow.smoothSpeed = 18.0f;
                }
            }

            // 3. Reset Mario
            var mario = FindAnyObjectByType<MarioController>();
            if (mario != null)
            {
                mario.ResetForNextLevel(new Vector3(2.5f, 2.5f, 0f));
            }

            // 4. Prefabs Cache
            GameObject fbTpl = GameObject.Find("FireballPrefab");
            GameObject ceTpl = GameObject.Find("CoinEffectPrefab");
            GameObject mushTpl = GameObject.Find("MushroomPrefab");
            GameObject flwTpl = GameObject.Find("FlowerPrefab");

            // 5. Environment Root
            GameObject envRoot = new GameObject("--- LEVEL ENVIRONMENT ---");

            // Physics Material per world
            float frictionVal = (world == 3) ? 0.005f : 0f; // Slippery ice in World 3!
            var worldPhysicsMat = new PhysicsMaterial2D("WorldPhys") { friction = frictionVal, bounciness = 0f };

            // Sub-roots
            GameObject blocksRoot = new GameObject("--- BLOCKS ---"); blocksRoot.transform.parent = envRoot.transform;
            GameObject pipesRoot = new GameObject("--- PIPES ---"); pipesRoot.transform.parent = envRoot.transform;
            GameObject enemiesRoot = new GameObject("--- ENEMIES ---"); enemiesRoot.transform.parent = envRoot.transform;
            GameObject coinsRoot = new GameObject("--- COINS ---"); coinsRoot.transform.parent = envRoot.transform;
            GameObject goalRoot = new GameObject("--- GOAL ---"); goalRoot.transform.parent = envRoot.transform;

            // Palette tints per world
            Color groundTopCol = world switch
            {
                1 => (stage == 3) ? new Color(0.2f, 0.5f, 0.95f) : ((stage == 5) ? new Color(0.5f, 0.5f, 0.55f) : Color.white),
                2 => new Color(0.95f, 0.82f, 0.35f), // Golden sand
                3 => new Color(0.9f, 0.98f, 1f),     // Snow white
                4 => new Color(0.2f, 0.2f, 0.2f),     // Basalt black
                _ => Color.white
            };

            Color dirtCol = world switch
            {
                1 => (stage == 3) ? new Color(0.1f, 0.28f, 0.65f) : ((stage == 5) ? new Color(0.3f, 0.3f, 0.35f) : Color.white),
                2 => new Color(0.75f, 0.55f, 0.22f), // Sandstone dirt
                3 => new Color(0.45f, 0.65f, 0.85f), // Icy dirt
                4 => new Color(0.35f, 0.12f, 0.08f), // Magma rock
                _ => Color.white
            };

            Color brickCol = world switch
            {
                1 => (stage == 3) ? new Color(0.2f, 0.5f, 0.95f) : ((stage == 5) ? new Color(0.6f, 0.6f, 0.65f) : Color.white),
                2 => new Color(0.85f, 0.65f, 0.25f), // Sandstone brick
                3 => new Color(0.4f, 0.8f, 1f),      // Ice crystal brick
                4 => new Color(0.5f, 0.15f, 0.1f),    // Obsidian fire brick
                _ => Color.white
            };

            Color pipeCol = world switch
            {
                1 => (stage == 3) ? new Color(0.15f, 0.5f, 0.9f) : Color.white,
                2 => new Color(0.85f, 0.55f, 0.2f),
                3 => new Color(0.2f, 0.65f, 0.85f),
                4 => new Color(0.4f, 0.15f, 0.1f),
                _ => Color.white
            };

            // Helpers
            void CreateGround(float startX, float length, float groundTopY = 1.5f)
            {
                GameObject section = new GameObject($"Ground_{startX}");
                section.transform.parent = envRoot.transform;

                for (int x = 0; x < length; x++)
                {
                    GameObject grass = new GameObject($"G_{x}");
                    grass.transform.parent = section.transform;
                    grass.transform.position = new Vector3(startX + x + 0.5f, groundTopY, 0f);
                    var sr = grass.AddComponent<SpriteRenderer>();
                    sr.sprite = sGround;
                    sr.color = groundTopCol;
                    sr.sortingOrder = 2;

                    GameObject dirt = new GameObject($"D_{x}");
                    dirt.transform.parent = section.transform;
                    dirt.transform.position = new Vector3(startX + x + 0.5f, groundTopY - 1f, 0f);
                    var srD = dirt.AddComponent<SpriteRenderer>();
                    srD.sprite = sDirt;
                    srD.color = dirtCol;
                    srD.sortingOrder = 2;
                }

                var col = section.AddComponent<BoxCollider2D>();
                col.size = new Vector2(length, 2f);
                col.offset = new Vector2(startX + length * 0.5f, groundTopY - 0.5f);
                col.sharedMaterial = worldPhysicsMat;
            }

            void CreatePit(float width = 250f, bool isLava = false)
            {
                GameObject pit = new GameObject(isLava ? "LavaPit" : "DeathPit");
                pit.transform.parent = envRoot.transform;
                pit.transform.position = new Vector3(100f, -3f, 0f);
                var pitCol = pit.AddComponent<BoxCollider2D>();
                pitCol.isTrigger = true;
                pitCol.size = new Vector2(width, 4f);
                pit.AddComponent<DeathPit>();

                if (isLava)
                {
                    GameObject lavaVis = new GameObject("LavaVisual");
                    lavaVis.transform.parent = pit.transform;
                    lavaVis.transform.position = new Vector3(100f, -1.8f, 0f);
                    lavaVis.transform.localScale = new Vector3(width, 2.5f, 1f);
                    var lSr = lavaVis.AddComponent<SpriteRenderer>();
                    lSr.sprite = sDirt;
                    lSr.color = new Color(1f, 0.25f, 0.05f, 0.9f);
                    lSr.sortingOrder = 3;
                }
            }

            void CreateBrick(float x, float y, bool coin = false)
            {
                GameObject bb = new GameObject($"Brick_{x}_{y}");
                bb.transform.parent = blocksRoot.transform;
                bb.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0f);
                var sr = bb.AddComponent<SpriteRenderer>();
                sr.sprite = sBrick;
                sr.color = brickCol;
                sr.sortingOrder = 3;

                var comp = bb.AddComponent<BrickBlock>();
                comp.containsCoin = coin;
                comp.coinCount = coin ? 5 : 0;
                comp.coinEffectPrefab = ceTpl;

                var col = bb.AddComponent<BoxCollider2D>();
                col.size = Vector2.one;
                col.sharedMaterial = worldPhysicsMat;
            }

            void CreateQuestion(float x, float y, BlockContent type)
            {
                GameObject qb = new GameObject($"Q_{x}_{y}");
                qb.transform.parent = blocksRoot.transform;
                qb.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0f);
                var sr = qb.AddComponent<SpriteRenderer>();
                sr.sprite = sQ1;
                sr.sortingOrder = 3;

                var comp = qb.AddComponent<QuestionBlock>();
                comp.content = type;
                comp.activeFrames = new Sprite[] { sQ1, sQ2 != null ? sQ2 : sQ1, sQ3 != null ? sQ3 : sQ1 };
                comp.emptySprite = sEmpty;
                comp.coinEffectPrefab = ceTpl;
                comp.mushroomPrefab = mushTpl;
                comp.fireFlowerPrefab = flwTpl;

                var col = qb.AddComponent<BoxCollider2D>();
                col.size = Vector2.one;
                col.sharedMaterial = worldPhysicsMat;
            }

            void CreatePipe(float x, int h, bool piranha = true)
            {
                GameObject pipe = new GameObject($"Pipe_{x}");
                pipe.transform.parent = pipesRoot.transform;
                pipe.transform.position = new Vector3(x, 2.0f, 0f);

                for (int i = 0; i < h - 1; i++)
                {
                    GameObject body = new GameObject($"Body_{i}");
                    body.transform.parent = pipe.transform;
                    body.transform.position = new Vector3(x + 1.0f, 2.0f + i + 0.5f, 0f);
                    body.transform.localScale = new Vector3(2f, 1f, 1f);
                    var sr = body.AddComponent<SpriteRenderer>();
                    sr.sprite = sPipeBody;
                    sr.drawMode = SpriteDrawMode.Sliced;
                    sr.size = new Vector2(2f, 1f);
                    sr.color = pipeCol;
                    sr.sortingOrder = 3;
                }

                GameObject top = new GameObject("Top");
                top.transform.parent = pipe.transform;
                top.transform.position = new Vector3(x + 1.0f, 2.0f + h - 0.5f, 0f);
                top.transform.localScale = new Vector3(2f, 1f, 1f);
                var srTop = top.AddComponent<SpriteRenderer>();
                srTop.sprite = sPipeTop;
                srTop.drawMode = SpriteDrawMode.Sliced;
                srTop.size = new Vector2(2f, 1f);
                srTop.color = pipeCol;
                srTop.sortingOrder = 3;

                var col = pipe.AddComponent<BoxCollider2D>();
                col.size = new Vector2(2.0f, h);
                col.offset = new Vector2(1.0f, h * 0.5f);
                col.sharedMaterial = new PhysicsMaterial2D("PipeFrictionless") { friction = 0f, bounciness = 0f };

                if (piranha)
                {
                    GameObject p = new GameObject("Piranha");
                    p.transform.parent = pipe.transform;
                    p.transform.position = new Vector3(x + 1.0f, 2.0f + h - 0.5f, 0f);
                    var pSr = p.AddComponent<SpriteRenderer>();
                    pSr.sprite = sPiranha;
                    pSr.sortingOrder = 2;

                    var pCol = p.AddComponent<BoxCollider2D>();
                    pCol.isTrigger = true;
                    pCol.size = new Vector2(0.9f, 1.4f);

                    var pp = p.AddComponent<PiranhaPlant>();
                    pp.spriteRenderer = pSr;
                    pp.moveDistance = 1.6f;
                }
            }

            void CreateCoin(float x, float y)
            {
                GameObject coin = new GameObject($"Coin_{x}_{y}");
                coin.transform.parent = coinsRoot.transform;
                coin.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0f);
                var sr = coin.AddComponent<SpriteRenderer>();
                sr.color = Color.white;
                sr.sortingOrder = 5;
                var col = coin.AddComponent<CircleCollider2D>();
                col.isTrigger = true;
                col.radius = 0.4f;
                coin.AddComponent<FloatingCoin>();
            }

            void CreateGoomba(float x, float y = 2.0f)
            {
                GameObject g = new GameObject($"Goomba_{x}");
                g.transform.parent = enemiesRoot.transform;
                g.transform.position = new Vector3(x + 0.5f, y + 0.45f, 0f);
                var sr = g.AddComponent<SpriteRenderer>();
                sr.sprite = sGoombaWalk1;
                sr.color = (world == 3) ? new Color(0.4f, 0.7f, 1f) : ((world == 2) ? new Color(0.85f, 0.6f, 0.3f) : Color.white);
                sr.sortingOrder = 4;
                var col = g.AddComponent<BoxCollider2D>();
                col.size = new Vector2(0.85f, 0.85f);
                col.sharedMaterial = worldPhysicsMat;
                var goomba = g.AddComponent<Goomba>();
                goomba.walkSprite1 = sGoombaWalk1;
                goomba.walkSprite2 = sGoombaWalk2 != null ? sGoombaWalk2 : sGoombaWalk1;
                goomba.squishedSprite = sGoombaSquish;
            }

            void CreateKoopa(float x, float y = 2.0f)
            {
                GameObject k = new GameObject($"Koopa_{x}");
                k.transform.parent = enemiesRoot.transform;
                k.transform.position = new Vector3(x + 0.5f, y + 0.9f, 0f);
                var sr = k.AddComponent<SpriteRenderer>();
                sr.sprite = sKoopaWalk1;
                sr.sortingOrder = 4;
                var col = k.AddComponent<BoxCollider2D>();
                col.size = new Vector2(0.85f, 1.4f);
                col.sharedMaterial = worldPhysicsMat;
                var koopa = k.AddComponent<Koopa>();
                koopa.walkSprite1 = sKoopaWalk1;
                koopa.walkSprite2 = sKoopaWalk2 != null ? sKoopaWalk2 : sKoopaWalk1;
                koopa.shellSprite = sKoopaShell;
            }

            void CreateParatroopa(float x, float y = 5.0f)
            {
                GameObject p = new GameObject($"Paratroopa_{x}");
                p.transform.parent = enemiesRoot.transform;
                p.transform.position = new Vector3(x, y, 0f);
                var sr = p.AddComponent<SpriteRenderer>();
                sr.sprite = sKoopaWalk1;
                sr.color = new Color(1f, 0.3f, 0.3f);
                sr.sortingOrder = 4;
                var col = p.AddComponent<BoxCollider2D>();
                col.size = new Vector2(0.85f, 1.4f);
                var pt = p.AddComponent<FlyingParatroopa>();
                pt.flySpeed = 3.0f;
            }

            void CreateMovingPlat(float x, float y, PlatformType type, float dist = 5f)
            {
                GameObject plat = new GameObject($"MovingPlat_{x}");
                plat.transform.parent = envRoot.transform;
                plat.transform.position = new Vector3(x, y, 0f);

                for (int i = 0; i < 3; i++)
                {
                    GameObject block = new GameObject($"B_{i}");
                    block.transform.parent = plat.transform;
                    block.transform.position = new Vector3(x + i + 0.5f, y + 0.5f, 0f);
                    var sr = block.AddComponent<SpriteRenderer>();
                    sr.sprite = sBrick;
                    sr.color = brickCol;
                    sr.sortingOrder = 3;
                }

                var col = plat.AddComponent<BoxCollider2D>();
                col.size = new Vector2(3f, 1f);
                col.offset = new Vector2(1.5f, 0.5f);
                col.sharedMaterial = worldPhysicsMat;

                var mp = plat.AddComponent<MovingPlatform>();
                mp.platformType = type;
                mp.moveDistance = dist;
            }

            void CreateCheckpoint(float x, float y = 2.0f)
            {
                GameObject cp = new GameObject($"Checkpoint_{x}");
                cp.transform.parent = envRoot.transform;
                cp.transform.position = new Vector3(x, y, 0f);

                GameObject pole = new GameObject("Pole");
                pole.transform.parent = cp.transform;
                pole.transform.position = new Vector3(x, y + 2.5f, 0f);
                pole.transform.localScale = new Vector3(0.2f, 5f, 1f);
                var pSr = pole.AddComponent<SpriteRenderer>();
                pSr.sprite = sFlagpole;
                pSr.sortingOrder = 3;

                GameObject flag = new GameObject("Flag");
                flag.transform.parent = cp.transform;
                flag.transform.position = new Vector3(x + 0.6f, y + 4.0f, 0f);
                var fSr = flag.AddComponent<SpriteRenderer>();
                fSr.sprite = sFlag;
                fSr.sortingOrder = 4;

                var col = cp.AddComponent<BoxCollider2D>();
                col.isTrigger = true;
                col.size = new Vector2(1.5f, 5f);
                col.offset = new Vector2(0f, 2.5f);

                var comp = cp.AddComponent<Checkpoint>();
                comp.flagRenderer = fSr;
            }

            void CreateStairs(float startX, int steps, bool asc)
            {
                for (int s = 0; s < steps; s++)
                {
                    int h = asc ? (s + 1) : (steps - s);
                    for (int y = 0; y < h; y++)
                    {
                        CreateBrick(startX + s, 2 + y);
                    }
                }
            }

            void CreateGoal(float x = 198.5f)
            {
                GameObject flagpole = new GameObject("Flagpole");
                flagpole.transform.parent = goalRoot.transform;
                flagpole.transform.position = new Vector3(x, 2.0f, 0f);

                for (int py = 0; py < 10; py++)
                {
                    GameObject poleSeg = new GameObject($"Pole_{py}");
                    poleSeg.transform.parent = flagpole.transform;
                    poleSeg.transform.position = new Vector3(x, 2.5f + py, 0f);
                    var pSr = poleSeg.AddComponent<SpriteRenderer>();
                    pSr.sprite = sFlagpole;
                    pSr.sortingOrder = 3;
                }

                GameObject flag = new GameObject("Flag");
                flag.transform.parent = flagpole.transform;
                flag.transform.position = new Vector3(x - 0.5f, 11.5f, 0f);
                var flgSr = flag.AddComponent<SpriteRenderer>();
                flgSr.sprite = sFlag;
                flgSr.sortingOrder = 4;

                GameObject castle = new GameObject("Castle");
                castle.transform.parent = goalRoot.transform;
                castle.transform.position = new Vector3(x + 6.5f, 4.5f, 0f);
                var cSr = castle.AddComponent<SpriteRenderer>();
                cSr.sprite = sCastle;
                cSr.color = (world == 4) ? new Color(0.4f, 0.1f, 0.1f) : ((world == 3) ? new Color(0.6f, 0.8f, 1f) : Color.white);
                cSr.sortingOrder = 2;

                GameObject castleDoor = new GameObject("CastleDoor");
                castleDoor.transform.parent = castle.transform;
                castleDoor.transform.position = new Vector3(x + 6.5f, 2.5f, 0f);

                var poleCol = flagpole.AddComponent<BoxCollider2D>();
                poleCol.isTrigger = true;
                poleCol.size = new Vector2(0.5f, 10f);
                poleCol.offset = new Vector2(0f, 5.0f);

                var fpComp = flagpole.AddComponent<Flagpole>();
                fpComp.flag = flag.transform;
                fpComp.poleBottom = flagpole.transform;
                fpComp.castleDoor = castleDoor.transform;
            }

            // Common Starting Coins at spawn x = 4 to 10
            for (int cx = 4; cx <= 10; cx++)
            {
                CreateCoin(cx, 2.5f);
            }

            // =========================================================================
            // 20 UNIQUE LEVEL LAYOUTS
            // =========================================================================
            CreatePit(260f, world == 4);

            switch (levelNumber)
            {
                // ==================== WORLD 1: GREEN GRASSLAND ====================
                case 1: // Beginner's Journey
                    CreateGround(0, 69);
                    CreateGround(71, 15);
                    CreateGround(89, 64);
                    CreateGround(155, 60);

                    CreateQuestion(16, 5, BlockContent.Coin);
                    CreateBrick(20, 5);
                    CreateQuestion(21, 5, BlockContent.Mushroom);
                    CreateBrick(22, 5);
                    CreateQuestion(23, 5, BlockContent.Coin);
                    CreateBrick(24, 5);
                    CreateQuestion(22, 9, BlockContent.Coin);

                    CreatePipe(28, 2, true);
                    CreatePipe(38, 3, true);
                    CreatePipe(46, 4, true);
                    CreatePipe(57, 4, false);

                    CreateGoomba(22);
                    CreateGoomba(41);
                    CreateGoomba(51);
                    CreateKoopa(106);
                    CreateGoomba(124);

                    CreateStairs(134, 4, true);
                    CreateStairs(140, 4, false);
                    CreateStairs(181, 8, true);
                    CreateGoal(198.5f);
                    break;

                case 2: // Mushroom Fields
                    CreateGround(0, 35);
                    CreateGround(40, 40);
                    CreateGround(85, 45);
                    CreateGround(135, 75);

                    // High bouncy mushroom bridges
                    for (int bx = 22; bx <= 28; bx++) CreateBrick(bx, 5);
                    for (int bx = 48; bx <= 56; bx++) CreateBrick(bx, 7);
                    for (int bx = 95; bx <= 105; bx++) CreateBrick(bx, 6);

                    CreateQuestion(25, 8, BlockContent.Mushroom);
                    CreateQuestion(52, 10, BlockContent.Coin);
                    CreateQuestion(100, 9, BlockContent.FireFlower);

                    CreatePipe(30, 3, true);
                    CreatePipe(60, 4, true);
                    CreatePipe(115, 3, false);

                    CreateKoopa(26, 6f);
                    CreateKoopa(50, 8f);
                    CreateGoomba(44);
                    CreateGoomba(90);

                    CreateStairs(175, 7, true);
                    CreateGoal(198.5f);
                    break;

                case 3: // Underground Adventure
                    CreateGround(0, 42);
                    CreateGround(48, 32);
                    CreateGround(86, 34);
                    CreateGround(126, 36);
                    CreateGround(166, 50);

                    // Cavern ceiling
                    for (int cx = 0; cx < 205; cx += 2) CreateBrick(cx, 11);

                    // Underground coin gallery
                    for (int cx = 15; cx <= 28; cx++)
                    {
                        CreateCoin(cx, 4.5f);
                        if (cx % 3 == 0) CreateBrick(cx, 5);
                    }

                    CreateMovingPlat(43, 3f, PlatformType.Horizontal, 4f);
                    CreateMovingPlat(81, 4f, PlatformType.Horizontal, 4f);

                    CreateQuestion(12, 5, BlockContent.Mushroom);
                    CreateQuestion(52, 6, BlockContent.FireFlower);
                    CreateQuestion(95, 6, BlockContent.Coin);

                    CreatePipe(32, 3, true);
                    CreatePipe(65, 4, true);
                    CreatePipe(105, 3, true);

                    CreateGoomba(18);
                    CreateGoomba(25);
                    CreateKoopa(55);
                    CreateGoomba(92);

                    CreateStairs(175, 8, true);
                    CreateGoal(198.5f);
                    break;

                case 4: // Dangerous Bridges
                    CreateGround(0, 25);
                    CreateGround(32, 20);
                    CreateGround(60, 18);
                    CreateGround(88, 22);
                    CreateGround(118, 25);
                    CreateGround(150, 65);

                    CreateMovingPlat(26, 3.5f, PlatformType.Horizontal, 5f);
                    CreateMovingPlat(53, 4.5f, PlatformType.Vertical, 3f);
                    CreateMovingPlat(111, 4.0f, PlatformType.Horizontal, 6f);

                    CreateCheckpoint(100, 2.5f);
                    CreateParatroopa(40, 6f);
                    CreateParatroopa(75, 6f);
                    CreateParatroopa(135, 7f);

                    CreateQuestion(36, 6, BlockContent.Mushroom);
                    CreateQuestion(66, 7, BlockContent.FireFlower);

                    CreateStairs(172, 8, true);
                    CreateGoal(198.5f);
                    break;

                case 5: // Castle Boss: Bowser Jr
                    CreateGround(0, 145);
                    // Castle ceiling
                    for (int cx = 0; cx < 145; cx += 2) CreateBrick(cx, 10);

                    CreateCheckpoint(80, 2.5f);
                    CreatePipe(30, 3, true);
                    CreatePipe(55, 4, true);

                    CreateGoomba(25);
                    CreateGoomba(45);
                    CreateKoopa(65);

                    // Boss Arena at x = 150 to 195
                    CreateGround(150, 45);
                    for (int cx = 150; cx < 195; cx += 2) CreateBrick(cx, 11);
                    // Arena walls
                    for (int y = 2; y <= 10; y++)
                    {
                        CreateBrick(149, y);
                        CreateBrick(195, y);
                    }

                    // Spawn Boss
                    GameObject bObj = new GameObject("BowserJrBoss");
                    bObj.transform.position = new Vector3(180f, 3.5f, 0f);
                    var bSr = bObj.AddComponent<SpriteRenderer>();
                    bSr.sprite = sKoopaWalk1;
                    bSr.color = new Color(1f, 0.4f, 0.1f);
                    bSr.sortingOrder = 4;
                    var bCol = bObj.AddComponent<BoxCollider2D>();
                    bCol.size = new Vector2(1.5f, 1.8f);
                    var bRb = bObj.AddComponent<Rigidbody2D>();
                    bRb.freezeRotation = true;
                    bRb.gravityScale = 3f;
                    var bComp = bObj.AddComponent<World1Boss>();
                    bComp.ActivateBoss();
                    break;

                // ==================== WORLD 2: DESERT ADVENTURE ====================
                case 6: // Desert Run
                    CreateGround(0, 55);
                    CreateGround(62, 35);
                    CreateGround(104, 45);
                    CreateGround(155, 60);

                    for (int bx = 20; bx <= 26; bx++) CreateBrick(bx, 5);
                    for (int bx = 70; bx <= 76; bx++) CreateBrick(bx, 6);

                    CreatePipe(35, 3, true);
                    CreatePipe(80, 4, true);
                    CreatePipe(120, 3, false);

                    CreateGoomba(22);
                    CreateKoopa(45);
                    CreateGoomba(72);
                    CreateKoopa(112);

                    CreateStairs(175, 7, true);
                    CreateGoal(198.5f);
                    break;

                case 7: // Moving Sands
                    CreateGround(0, 30);
                    CreateGround(40, 25);
                    CreateGround(75, 25);
                    CreateGround(110, 30);
                    CreateGround(150, 65);

                    CreateMovingPlat(31, 3.5f, PlatformType.Falling);
                    CreateMovingPlat(66, 4.0f, PlatformType.Horizontal, 8f);
                    CreateMovingPlat(101, 3.5f, PlatformType.Falling);

                    CreateCheckpoint(95, 2.5f);
                    CreateQuestion(48, 6, BlockContent.Mushroom);
                    CreateQuestion(85, 7, BlockContent.FireFlower);

                    CreateGoomba(45);
                    CreateKoopa(80);
                    CreateParatroopa(125, 6f);

                    CreateStairs(172, 8, true);
                    CreateGoal(198.5f);
                    break;

                case 8: // Pyramid Mystery
                    CreateGround(0, 40);
                    CreateGround(46, 35);
                    CreateGround(88, 32);
                    CreateGround(128, 85);

                    // Stone pyramid ceiling
                    for (int cx = 0; cx < 205; cx += 2) CreateBrick(cx, 10);

                    // Secret treasure alcove
                    for (int cx = 60; cx <= 70; cx++)
                    {
                        CreateCoin(cx, 6.5f);
                        CreateBrick(cx, 5);
                    }

                    CreateCheckpoint(88, 2.5f);
                    CreateQuestion(20, 5, BlockContent.Mushroom);
                    CreateQuestion(100, 6, BlockContent.Coin);

                    CreateGoomba(22);
                    CreateKoopa(52);
                    CreateGoomba(95);

                    CreateStairs(175, 8, true);
                    CreateGoal(198.5f);
                    break;

                case 9: // Desert Storm
                    CreateGround(0, 24);
                    CreateGround(30, 20);
                    CreateGround(58, 22);
                    CreateGround(88, 20);
                    CreateGround(116, 25);
                    CreateGround(150, 65);

                    CreateMovingPlat(25, 3f, PlatformType.Vertical, 4f);
                    CreateMovingPlat(81, 4f, PlatformType.Vertical, 5f);
                    CreateCheckpoint(100, 2.5f);

                    CreateParatroopa(45, 7f);
                    CreateParatroopa(72, 7f);
                    CreateParatroopa(130, 7f);

                    CreateStairs(175, 8, true);
                    CreateGoal(198.5f);
                    break;

                case 10: // Desert Castle Boss: Pharaoh Sphinx
                    CreateGround(0, 140);
                    for (int cx = 0; cx < 140; cx += 2) CreateBrick(cx, 10);

                    CreateCheckpoint(75, 2.5f);
                    CreateGoomba(30);
                    CreateKoopa(60);

                    // Boss Arena
                    CreateGround(145, 50);
                    for (int cx = 145; cx < 195; cx += 2) CreateBrick(cx, 11);
                    for (int y = 2; y <= 10; y++) { CreateBrick(144, y); CreateBrick(195, y); }

                    GameObject pObj = new GameObject("DesertPharaohBoss");
                    pObj.transform.position = new Vector3(175f, 3.5f, 0f);
                    var pSr = pObj.AddComponent<SpriteRenderer>();
                    pSr.sprite = sKoopaWalk1;
                    pSr.color = new Color(0.9f, 0.7f, 0.2f);
                    pSr.sortingOrder = 4;
                    var pCol = pObj.AddComponent<BoxCollider2D>();
                    pCol.size = new Vector2(1.6f, 2f);
                    var pRb = pObj.AddComponent<Rigidbody2D>();
                    pRb.freezeRotation = true;
                    pRb.gravityScale = 3f;
                    var pComp = pObj.AddComponent<World2Boss>();
                    pComp.ActivateBoss();
                    break;

                // ==================== WORLD 3: FROZEN KINGDOM ====================
                case 11: // Snowy Mountains
                    CreateGround(0, 50);
                    CreateGround(58, 38);
                    CreateGround(102, 42);
                    CreateGround(150, 65);

                    for (int bx = 25; bx <= 32; bx++) CreateBrick(bx, 5);
                    for (int bx = 70; bx <= 78; bx++) CreateBrick(bx, 6);

                    CreatePipe(40, 3, true);
                    CreatePipe(85, 4, true);

                    CreateGoomba(24);
                    CreateKoopa(65);
                    CreateGoomba(110);

                    CreateStairs(175, 7, true);
                    CreateGoal(198.5f);
                    break;

                case 12: // Frozen Lake
                    CreateGround(0, 30);
                    CreateGround(38, 25);
                    CreateGround(72, 28);
                    CreateGround(108, 25);
                    CreateGround(142, 70);

                    CreateMovingPlat(31, 3.5f, PlatformType.Falling);
                    CreateMovingPlat(64, 4f, PlatformType.Horizontal, 6f);
                    CreateMovingPlat(101, 3.5f, PlatformType.Falling);

                    CreateCheckpoint(90, 2.5f);
                    CreateQuestion(48, 6, BlockContent.Mushroom);
                    CreateQuestion(80, 7, BlockContent.FireFlower);

                    CreateKoopa(42);
                    CreateKoopa(78);
                    CreateParatroopa(120, 6f);

                    CreateStairs(172, 8, true);
                    CreateGoal(198.5f);
                    break;

                case 13: // Ice Caves
                    CreateGround(0, 45);
                    CreateGround(52, 35);
                    CreateGround(94, 34);
                    CreateGround(135, 75);

                    // Ice cave ceiling
                    for (int cx = 0; cx < 205; cx += 2) CreateBrick(cx, 11);

                    // Falling icicles
                    for (int ix = 25; ix <= 120; ix += 20)
                    {
                        GameObject icicle = new GameObject($"Icicle_{ix}");
                        icicle.transform.position = new Vector3(ix, 10f, 0f);
                        var iSr = icicle.AddComponent<SpriteRenderer>();
                        iSr.sprite = sBrick;
                        iSr.color = new Color(0.7f, 0.9f, 1f);
                        var iCol = icicle.AddComponent<BoxCollider2D>();
                        iCol.isTrigger = true;
                        iCol.size = new Vector2(0.8f, 1.2f);
                        icicle.AddComponent<FallingHazard>();
                    }

                    CreateMovingPlat(46, 3.5f, PlatformType.Horizontal, 5f);
                    CreateMovingPlat(87, 4f, PlatformType.Vertical, 3f);
                    CreateCheckpoint(94, 2.5f);

                    CreateGoomba(25);
                    CreateKoopa(60);
                    CreateGoomba(102);

                    CreateStairs(175, 8, true);
                    CreateGoal(198.5f);
                    break;

                case 14: // Blizzard Challenge
                    CreateGround(0, 22);
                    CreateGround(28, 18);
                    CreateGround(54, 20);
                    CreateGround(82, 22);
                    CreateGround(112, 24);
                    CreateGround(144, 70);

                    CreateMovingPlat(23, 3f, PlatformType.Vertical, 4f);
                    CreateMovingPlat(75, 4f, PlatformType.Horizontal, 6f);
                    CreateMovingPlat(105, 3.5f, PlatformType.Falling);

                    CreateCheckpoint(100, 2.5f);
                    CreateParatroopa(40, 7f);
                    CreateParatroopa(70, 7f);
                    CreateParatroopa(125, 7f);

                    CreateStairs(175, 8, true);
                    CreateGoal(198.5f);
                    break;

                case 15: // Ice Castle Boss: Frost Titan
                    CreateGround(0, 140);
                    for (int cx = 0; cx < 140; cx += 2) CreateBrick(cx, 10);

                    CreateCheckpoint(75, 2.5f);
                    CreateGoomba(32);
                    CreateKoopa(62);

                    // Boss Arena
                    CreateGround(145, 50);
                    for (int cx = 145; cx < 195; cx += 2) CreateBrick(cx, 11);
                    for (int y = 2; y <= 10; y++) { CreateBrick(144, y); CreateBrick(195, y); }

                    GameObject fObj = new GameObject("FrostTitanBoss");
                    fObj.transform.position = new Vector3(175f, 3.5f, 0f);
                    var fSr = fObj.AddComponent<SpriteRenderer>();
                    fSr.sprite = sKoopaWalk1;
                    fSr.color = new Color(0.3f, 0.8f, 1f);
                    fSr.sortingOrder = 4;
                    var fCol = fObj.AddComponent<BoxCollider2D>();
                    fCol.size = new Vector2(1.8f, 2.2f);
                    var fRb = fObj.AddComponent<Rigidbody2D>();
                    fRb.freezeRotation = true;
                    fRb.gravityScale = 3f;
                    var fComp = fObj.AddComponent<World3Boss>();
                    fComp.ActivateBoss();
                    break;

                // ==================== WORLD 4: DARK VOLCANO WORLD ====================
                case 16: // Volcanic Valley
                    CreateGround(0, 48);
                    CreateGround(56, 36);
                    CreateGround(98, 42);
                    CreateGround(148, 65);

                    // Rising lava bubbles (Podoboos)
                    GameObject podo1 = new GameObject("Podoboo_50");
                    podo1.transform.position = new Vector3(52f, 1f, 0f);
                    var p1Sr = podo1.AddComponent<SpriteRenderer>();
                    p1Sr.sprite = sFireball;
                    p1Sr.color = new Color(1f, 0.4f, 0.1f);
                    podo1.AddComponent<FirePodoboo>();

                    GameObject podo2 = new GameObject("Podoboo_94");
                    podo2.transform.position = new Vector3(94f, 1f, 0f);
                    var p2Sr = podo2.AddComponent<SpriteRenderer>();
                    p2Sr.sprite = sFireball;
                    p2Sr.color = new Color(1f, 0.4f, 0.1f);
                    podo2.AddComponent<FirePodoboo>();

                    CreateQuestion(25, 5, BlockContent.FireFlower);
                    CreateQuestion(75, 6, BlockContent.Mushroom);

                    CreateGoomba(24);
                    CreateKoopa(65);
                    CreateGoomba(110);

                    CreateStairs(175, 7, true);
                    CreateGoal(198.5f);
                    break;

                case 17: // Burning Bridges
                    CreateGround(0, 28);
                    CreateGround(36, 22);
                    CreateGround(66, 24);
                    CreateGround(98, 25);
                    CreateGround(132, 80);

                    CreateMovingPlat(29, 3.5f, PlatformType.Falling);
                    CreateMovingPlat(59, 4f, PlatformType.Horizontal, 6f);
                    CreateMovingPlat(91, 3.5f, PlatformType.Falling);
                    CreateMovingPlat(124, 4f, PlatformType.Horizontal, 7f);

                    CreateCheckpoint(98, 2.5f);
                    CreateParatroopa(50, 7f);
                    CreateParatroopa(115, 7f);

                    CreateStairs(175, 8, true);
                    CreateGoal(198.5f);
                    break;

                case 18: // Dark Underground Magma Cavern
                    CreateGround(0, 42);
                    CreateGround(48, 35);
                    CreateGround(90, 35);
                    CreateGround(132, 80);

                    for (int cx = 0; cx < 205; cx += 2) CreateBrick(cx, 10);

                    CreateCheckpoint(90, 2.5f);
                    CreateMovingPlat(43, 3.5f, PlatformType.Horizontal, 4f);
                    CreateMovingPlat(84, 4f, PlatformType.Vertical, 3f);

                    CreateQuestion(20, 5, BlockContent.Mushroom);
                    CreateQuestion(60, 6, BlockContent.FireFlower);

                    CreateGoomba(24);
                    CreateKoopa(55);
                    CreateGoomba(102);

                    CreateStairs(175, 8, true);
                    CreateGoal(198.5f);
                    break;

                case 19: // Ultimate Challenge (Multi-Mechanic Gauntlet)
                    CreateGround(0, 22);
                    CreateGround(28, 16);
                    CreateGround(50, 18);
                    CreateGround(74, 20);
                    CreateGround(100, 22);
                    CreateGround(128, 24);
                    CreateGround(158, 65);

                    CreateMovingPlat(23, 3f, PlatformType.Vertical, 4f);
                    CreateMovingPlat(45, 4f, PlatformType.Horizontal, 4f);
                    CreateMovingPlat(69, 3.5f, PlatformType.Falling);
                    CreateMovingPlat(95, 4f, PlatformType.Vertical, 4f);
                    CreateMovingPlat(123, 4f, PlatformType.Horizontal, 4f);

                    CreateCheckpoint(74, 2.5f);
                    CreateCheckpoint(128, 2.5f);

                    CreateParatroopa(35, 7f);
                    CreateParatroopa(85, 7f);
                    CreateParatroopa(140, 7f);

                    CreateStairs(175, 8, true);
                    CreateGoal(198.5f);
                    break;

                case 20: // FINAL BOSS: KING OF THE VOLCANO (Mega Bowser)
                    CreateGround(0, 130);
                    for (int cx = 0; cx < 130; cx += 2) CreateBrick(cx, 11);

                    CreateCheckpoint(75, 2.5f);
                    CreateQuestion(25, 5, BlockContent.FireFlower);
                    CreateQuestion(50, 5, BlockContent.Mushroom);

                    CreateGoomba(30);
                    CreateKoopa(60);

                    // Grand Castle Arena (x = 135 to 205)
                    CreateGround(135, 10); // Entrance platform
                    for (int y = 2; y <= 11; y++) CreateBrick(134, y);

                    // Castle bridge over boiling lava pit
                    GameObject bridge = new GameObject("CastleBridge");
                    bridge.transform.parent = envRoot.transform;
                    for (int bx = 145; bx <= 185; bx++)
                    {
                        GameObject bPart = new GameObject($"BPart_{bx}");
                        bPart.transform.parent = bridge.transform;
                        bPart.transform.position = new Vector3(bx + 0.5f, 2.5f, 0f);
                        var sr = bPart.AddComponent<SpriteRenderer>();
                        sr.sprite = sBrick;
                        sr.color = new Color(0.4f, 0.1f, 0.1f);
                        sr.sortingOrder = 3;
                    }
                    var brCol = bridge.AddComponent<BoxCollider2D>();
                    brCol.size = new Vector2(41f, 1f);
                    brCol.offset = new Vector2(165.5f, 2.5f);

                    // Far landing with Bridge Axe Switch
                    CreateGround(186, 25);
                    for (int y = 2; y <= 11; y++) CreateBrick(210, y);
                    for (int cx = 135; cx <= 210; cx += 2) CreateBrick(cx, 11);

                    // Spawn Final Boss: Volcano King
                    GameObject megaBoss = new GameObject("VolcanoKingBoss");
                    megaBoss.transform.position = new Vector3(175f, 4.0f, 0f);
                    var mbSr = megaBoss.AddComponent<SpriteRenderer>();
                    mbSr.sprite = sKoopaWalk1;
                    mbSr.color = new Color(1f, 0.15f, 0.05f);
                    mbSr.sortingOrder = 4;
                    var mbCol = megaBoss.AddComponent<BoxCollider2D>();
                    mbCol.size = new Vector2(2.2f, 2.6f);
                    var mbRb = megaBoss.AddComponent<Rigidbody2D>();
                    mbRb.freezeRotation = true;
                    mbRb.gravityScale = 3f;

                    var finalBossComp = megaBoss.AddComponent<World4FinalBoss>();
                    finalBossComp.bridgeObject = bridge;
                    finalBossComp.ActivateBoss();

                    // Bridge Axe Switch at x = 195
                    GameObject axeObj = new GameObject("BridgeAxeSwitch");
                    axeObj.transform.parent = envRoot.transform;
                    axeObj.transform.position = new Vector3(195f, 3.5f, 0f);
                    var axeSr = axeObj.AddComponent<SpriteRenderer>();
                    axeSr.sprite = sQ1;
                    axeSr.color = new Color(1f, 0.85f, 0.1f);
                    axeSr.sortingOrder = 4;
                    var axeCol = axeObj.AddComponent<BoxCollider2D>();
                    axeCol.isTrigger = true;
                    axeCol.size = new Vector2(1f, 1.5f);
                    var axeComp = axeObj.AddComponent<BridgeAxeSwitch>();
                    axeComp.boss = finalBossComp;
                    break;
            }

            if (mario != null)
            {
                mario.EnsureCoinsAndPipes();
            }

            Debug.Log($"<color=green>[SuperUnity] Successfully Loaded Level {levelNumber}: {GameManager.LevelTitles[levelNumber - 1]}!</color>");
        }
    }
}
