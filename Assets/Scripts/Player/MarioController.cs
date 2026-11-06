using UnityEngine;
using System.Collections;

namespace SuperUnity.Player
{
    public enum MarioState
    {
        Small,
        Super,
        Fire,
        Dead
    }

    public enum CharacterModel
    {
        ClassicMario,
        Luigi,
        FireKnight,
        CyberNeon
    }

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class MarioController : MonoBehaviour
    {
        [Header("Character Model & Skin")]
        public CharacterModel activeModel = CharacterModel.ClassicMario;
        public bool canDoubleJump = true;
        private float afterimageTimer = 0f;

        public string ModelDisplayName => activeModel switch
        {
            CharacterModel.ClassicMario => "Mario (Classic)",
            CharacterModel.Luigi => "Luigi (High Flutter)",
            CharacterModel.FireKnight => "Fire Knight (Crimson)",
            CharacterModel.CyberNeon => "Cyber Neon (Hyper Dash)",
            _ => "Mario"
        };

        public Color ModelColor => activeModel switch
        {
            CharacterModel.Luigi => new Color(0.25f, 1f, 0.45f),
            CharacterModel.FireKnight => new Color(1f, 0.35f, 0.15f),
            CharacterModel.CyberNeon => new Color(0.1f, 0.95f, 1f),
            _ => Color.white
        };

        [Header("Movement Settings")]
        public float walkSpeed = 8.5f;
        public float runSpeed = 14.0f;
        public float acceleration = 45.0f;
        public float deceleration = 30.0f;
        public float skidDeceleration = 50.0f;

        [Header("Jump Settings (Tuned +18% Higher & Responsive Air Control)")]
        public float jumpForce = 20.2f;
        public float bounceForce = 13.5f;
        public float gravityMultiplier = 3.5f;
        public float fallGravityMultiplier = 4.6f;
        public float lowJumpMultiplier = 7.5f;
        public float airControlMultiplier = 1.15f;

        [Header("Death & Respawn Settings")]
        public float respawnDelay = 2.0f;
        public float postRespawnGraceTime = 1.5f;
        private bool isDying = false;

        [Header("Power States")]
        public MarioState currentState = MarioState.Small;
        public bool isInvincible = false;
        public float invincibilityDuration = 2.5f;

        [Header("Ground Detection")]
        public LayerMask groundLayer;
        public Transform groundCheck;
        public Vector2 groundCheckSize = new Vector2(0.6f, 0.1f);
        public bool isGrounded = false;

        [Header("Combat & Abilities")]
        public GameObject fireballPrefab;
        public Transform fireballSpawnPoint;
        public float fireballCooldown = 0.25f;
        private float lastFireballTime = 0f;

        [Header("Sprites & Rendering")]
        public SpriteRenderer spriteRenderer;

        // Internal physics & state
        private Rigidbody2D rb;
        private BoxCollider2D boxCollider;
        private float horizontalInput = 0f;
        private bool jumpPressed = false;
        private bool jumpHeld = false;
        private bool runHeld = false;
        private bool crouchHeld = false;
        private bool isSkidding = false;
        private bool isFacingRight = true;
        private bool isControllable = true;
        private float jumpBufferCounter = 0f;
        private float coyoteTimeCounter = 0f;

        public bool IsDead => currentState == MarioState.Dead || isDying;
        public bool IsFacingRight => isFacingRight;
        public bool IsSkidding => isSkidding;
        public bool IsCrouching => crouchHeld && (currentState == MarioState.Super || currentState == MarioState.Fire);
        public Rigidbody2D Rb => rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
            var noFriction = new PhysicsMaterial2D("MarioFrictionless") { friction = 0f, bounciness = 0f };
            boxCollider.sharedMaterial = noFriction;
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                }
            }

            walkSpeed = 8.5f;
            runSpeed = 14.0f;
            acceleration = 45.0f;
            deceleration = 30.0f;
            skidDeceleration = 50.0f;
            jumpForce = 20.2f;
            bounceForce = 13.5f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.gravityScale = gravityMultiplier;

            if (groundLayer == 0)
            {
                groundLayer = LayerMask.GetMask("Default", "Ground");
                if (groundLayer == 0) groundLayer = ~0; // fallback to everything except triggers
            }

            ApplyStateCollider();
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.gameState != GameState.Playing)
            {
                horizontalInput = 0f;
                return;
            }

            if (!isControllable || IsDead) return;

            // Handle Input
            horizontalInput = Input.GetAxisRaw("Horizontal");
            jumpPressed = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
            jumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
            runHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.X);
            crouchHeld = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

            // Facing direction
            if (horizontalInput > 0.1f) isFacingRight = true;
            else if (horizontalInput < -0.1f) isFacingRight = false;

            // Jump with Coyote Time & Jump Buffering
            if (isGrounded)
            {
                coyoteTimeCounter = 0.15f;
                canDoubleJump = true;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }

            if (jumpPressed) jumpBufferCounter = 0.2f;
            else jumpBufferCounter -= Time.deltaTime;

            if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f && !IsCrouching)
            {
                Jump();
                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;
            }
            else if (jumpPressed && canDoubleJump && !isGrounded && rb.linearVelocity.y < 8f)
            {
                // Double Jump / Air Dash!
                canDoubleJump = false;
                float airDashDir = (isFacingRight ? 1f : -1f) * (runHeld ? 5f : 3f);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.7f + airDashDir, jumpForce * 0.95f);
                if (SoundManager.Instance != null) SoundManager.Instance.PlayJump();
                SpawnAfterimage(ModelColor);
            }

            // Switch Character Model with C key
            if (Input.GetKeyDown(KeyCode.C))
            {
                SwitchCharacterModel();
            }

            // Afterimage ghost trail when sprinting at turbo speed or during double jump
            if (Mathf.Abs(rb.linearVelocity.x) > 10.0f || !canDoubleJump)
            {
                afterimageTimer += Time.deltaTime;
                if (afterimageTimer >= 0.08f)
                {
                    afterimageTimer = 0f;
                    SpawnAfterimage(ModelColor);
                }
            }

            // Shoot fireball
            if ((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.Z)) && (currentState == MarioState.Fire || activeModel == CharacterModel.FireKnight))
            {
                ShootFireball();
            }

            // Skid detection
            isSkidding = isGrounded && ((horizontalInput > 0.1f && rb.linearVelocity.x < -0.5f) || (horizontalInput < -0.1f && rb.linearVelocity.x > 0.5f));
        }

        private void FixedUpdate()
        {
            if (IsDead) return;

            CheckGrounded();

            if (isControllable)
            {
                HandleHorizontalMovement();
                HandleJumpPhysics();
            }
        }

        private void CheckGrounded()
        {
            Vector2 checkPos = groundCheck != null ? (Vector2)groundCheck.position : (Vector2)transform.position + Vector2.down * (boxCollider.size.y * 0.5f);
            Collider2D col = Physics2D.OverlapBox(checkPos, groundCheckSize, 0f, groundLayer);
            isGrounded = col != null && col.gameObject != gameObject;
        }

        private void HandleHorizontalMovement()
        {
            if (IsCrouching && isGrounded)
            {
                // Decelerate quickly when crouching
                float crouchDecel = deceleration * 2.5f * Time.fixedDeltaTime;
                rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, 0, crouchDecel), rb.linearVelocity.y);
                return;
            }

            float targetSpeed = (runHeld ? runSpeed : walkSpeed) * horizontalInput;
            float currentX = rb.linearVelocity.x;
            float chosenAccel = isGrounded ? acceleration : (acceleration * airControlMultiplier);

            if (isSkidding && isGrounded)
            {
                chosenAccel = skidDeceleration;
            }
            else if (Mathf.Abs(horizontalInput) < 0.05f)
            {
                chosenAccel = isGrounded ? deceleration : (deceleration * 0.75f);
                targetSpeed = 0f;
            }

            float newX = Mathf.MoveTowards(currentX, targetSpeed, chosenAccel * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
        }

        private void HandleJumpPhysics()
        {
            // Mario variable jump height physics
            if (rb.linearVelocity.y < -0.1f)
            {
                // Falling faster - satisfying snappy landing
                rb.gravityScale = fallGravityMultiplier;
            }
            else if (rb.linearVelocity.y > 0.1f && !jumpHeld)
            {
                // Variable jump: cut jump short immediately when button released early
                rb.gravityScale = lowJumpMultiplier;
            }
            else
            {
                // Ascending with button held - reach full peak height
                rb.gravityScale = gravityMultiplier;
            }
        }

        public void Jump()
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayJump();
            }
        }

        public void SwitchCharacterModel(int modelIndex = -1)
        {
            if (modelIndex >= 0)
            {
                activeModel = (CharacterModel)(modelIndex % 4);
            }
            else
            {
                activeModel = (CharacterModel)(((int)activeModel + 1) % 4);
            }

            ApplyModelStats();

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayPowerUp();
            }

            SpawnAfterimage(ModelColor);
        }

        public void ApplyModelStats()
        {
            switch (activeModel)
            {
                case CharacterModel.Luigi:
                    walkSpeed = 8.5f;
                    runSpeed = 14.5f;
                    jumpForce = 22.5f; // High flutter jump!
                    bounceForce = 14.5f;
                    break;
                case CharacterModel.FireKnight:
                    walkSpeed = 9.0f;
                    runSpeed = 15.0f;
                    jumpForce = 20.5f;
                    bounceForce = 13.5f;
                    fireballCooldown = 0.12f; // Rapid fire!
                    break;
                case CharacterModel.CyberNeon:
                    walkSpeed = 10.0f;
                    runSpeed = 17.5f; // Hyper speed!
                    jumpForce = 20.8f;
                    bounceForce = 13.5f;
                    break;
                default: // Classic Mario (+18% higher jumps)
                    walkSpeed = 8.5f;
                    runSpeed = 14.0f;
                    jumpForce = 20.2f;
                    bounceForce = 13.5f;
                    fireballCooldown = 0.25f;
                    break;
            }
        }

        private void SpawnAfterimage(Color color)
        {
            if (spriteRenderer == null || spriteRenderer.sprite == null) return;

            GameObject ghost = new GameObject("Afterimage");
            ghost.transform.position = transform.position;
            ghost.transform.localScale = transform.localScale;

            var sr = ghost.AddComponent<SpriteRenderer>();
            sr.sprite = spriteRenderer.sprite;
            sr.flipX = spriteRenderer.flipX;
            sr.sortingOrder = spriteRenderer.sortingOrder - 1;
            sr.color = new Color(color.r, color.g, color.b, 0.45f);

            StartCoroutine(FadeAndDestroy(ghost, sr));
        }

        private IEnumerator FadeAndDestroy(GameObject obj, SpriteRenderer sr)
        {
            float duration = 0.25f;
            float elapsed = 0f;
            Color startColor = sr.color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / duration);
                if (sr != null) sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }

            if (obj != null) Destroy(obj);
        }

        public void Bounce(float force = -1f)
        {
            float f = force > 0 ? force : bounceForce;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, f);
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayStomp();
            }
        }

        public void ShootFireball()
        {
            if (Time.time < lastFireballTime + fireballCooldown) return;
            lastFireballTime = Time.time;

            if (fireballPrefab != null)
            {
                Vector3 spawnPos = fireballSpawnPoint != null ? fireballSpawnPoint.position : transform.position + (isFacingRight ? Vector3.right : Vector3.left) * 0.6f;
                GameObject fb = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);
                var fireballComp = fb.GetComponent<Items.Fireball>();
                if (fireballComp != null)
                {
                    fireballComp.Launch(isFacingRight ? 1f : -1f);
                }
            }

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayFireball();
            }
        }

        public void Grow()
        {
            if (currentState == MarioState.Small)
            {
                currentState = MarioState.Super;
                ApplyStateCollider();
                StartCoroutine(GrowthEffectCoroutine());
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayPowerUp();
                }
            }
            else
            {
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayCoin();
                }
            }
        }

        public void GetFireFlower()
        {
            currentState = MarioState.Fire;
            ApplyStateCollider();
            StartCoroutine(GrowthEffectCoroutine());
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayPowerUp();
            }
        }

        public void TakeDamage()
        {
            if (isInvincible || IsDead) return;

            if (currentState == MarioState.Fire)
            {
                currentState = MarioState.Super;
                StartCoroutine(InvincibilityCoroutine());
                if (SoundManager.Instance != null) SoundManager.Instance.PlayPowerDown();
            }
            else if (currentState == MarioState.Super)
            {
                currentState = MarioState.Small;
                ApplyStateCollider();
                StartCoroutine(InvincibilityCoroutine());
                if (SoundManager.Instance != null) SoundManager.Instance.PlayPowerDown();
            }
            else
            {
                Die();
            }
        }

        public void Die()
        {
            if (IsDead) return;

            isDying = true;
            currentState = MarioState.Dead;
            isControllable = false;
            boxCollider.enabled = false;
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;

            StopAllCoroutines();
            StartCoroutine(DeathSequenceCoroutine());
        }

        private IEnumerator DeathSequenceCoroutine()
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayDie();
            }

            // If Mario fell deep below the camera, snap to bottom of screen so death pop is visible
            var cam = Camera.main;
            if (cam != null && transform.position.y < cam.transform.position.y - 4.5f)
            {
                transform.position = new Vector3(transform.position.x, cam.transform.position.y - 3.8f, transform.position.z);
            }

            // Dramatic freeze in place (0.35s)
            yield return new WaitForSeconds(0.35f);

            // Pop up into air and arc downwards off-screen
            rb.gravityScale = 3.6f;
            rb.linearVelocity = new Vector2(0f, 11.5f);

            // Total respawn delay is 2.0s
            float remainingDelay = Mathf.Max(0.5f, respawnDelay - 0.35f);
            yield return new WaitForSeconds(remainingDelay);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnMarioDied();
            }
        }

        private IEnumerator GrowthEffectCoroutine()
        {
            // Flash between sizes
            Time.timeScale = 0.05f; // classic freeze frame
            yield return new WaitForSecondsRealtime(0.35f);
            Time.timeScale = 1.0f;
        }

        private IEnumerator InvincibilityCoroutine()
        {
            isInvincible = true;
            float elapsed = 0f;
            while (elapsed < invincibilityDuration)
            {
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = !spriteRenderer.enabled;
                }
                yield return new WaitForSeconds(0.08f);
                elapsed += 0.08f;
            }

            if (spriteRenderer != null) spriteRenderer.enabled = true;
            isInvincible = false;
        }

        public void TriggerInvincibility(float duration)
        {
            StartCoroutine(StarInvincibilityCoroutine(duration));
        }

        private IEnumerator StarInvincibilityCoroutine(float duration)
        {
            isInvincible = true;
            float elapsed = 0f;
            Color[] rainbow = new Color[] { Color.red, Color.yellow, Color.green, Color.cyan, Color.magenta };

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                int cIndex = (int)(elapsed * 12f) % rainbow.Length;
                if (spriteRenderer != null) spriteRenderer.color = rainbow[cIndex];
                yield return null;
            }

            if (spriteRenderer != null) spriteRenderer.color = ModelColor;
            isInvincible = false;
        }

        private void ApplyStateCollider()
        {
            if (boxCollider == null) return;

            if (currentState == MarioState.Small)
            {
                boxCollider.size = new Vector2(0.7f, 0.9f);
                boxCollider.offset = new Vector2(0f, 0.45f);
            }
            else
            {
                boxCollider.size = new Vector2(0.75f, 1.8f);
                boxCollider.offset = new Vector2(0f, 0.9f);
            }
        }

        public void SlideDownFlagpole(Transform poleBase, System.Action onComplete)
        {
            StartCoroutine(FlagpoleSlideCoroutine(poleBase, onComplete));
        }

        private IEnumerator FlagpoleSlideCoroutine(Transform poleBase, System.Action onComplete)
        {
            isControllable = false;
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            boxCollider.enabled = false;

            float slideSpeed = 10.0f;
            float targetY = poleBase.position.y + 0.5f;

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayFlagpole();
            }

            while (transform.position.y > targetY)
            {
                transform.position = new Vector3(poleBase.position.x - 0.25f, Mathf.MoveTowards(transform.position.y, targetY, slideSpeed * Time.deltaTime), transform.position.z);
                yield return null;
            }

            // Flip to other side of the pole
            transform.position = new Vector3(poleBase.position.x + 0.35f, targetY, transform.position.z);
            isFacingRight = true;
            yield return new WaitForSeconds(0.12f);

            // Re-enable gravity to drop to ground
            rb.gravityScale = 3f;
            boxCollider.enabled = true;

            onComplete?.Invoke();
        }

        public void WalkToCastle(Transform castleDoor, System.Action onReachedCastle)
        {
            StartCoroutine(WalkToCastleCoroutine(castleDoor, onReachedCastle));
        }

        private IEnumerator WalkToCastleCoroutine(Transform castleDoor, System.Action onReachedCastle)
        {
            isControllable = false;
            isFacingRight = true;

            while (transform.position.x < castleDoor.position.x)
            {
                rb.linearVelocity = new Vector2(6.5f, rb.linearVelocity.y);
                yield return null;
            }

            rb.linearVelocity = Vector2.zero;
            if (spriteRenderer != null) spriteRenderer.enabled = false; // Enter castle door
            onReachedCastle?.Invoke();
        }

        private void Start()
        {
            EnsureCoinsAndPipes();
        }

        public void EnsureCoinsAndPipes()
        {
            // Remove friction from all pipes so jumping over pipes is silky smooth
            var noFriction = new PhysicsMaterial2D("NoFriction") { friction = 0f, bounciness = 0f };
            if (boxCollider != null) boxCollider.sharedMaterial = noFriction;

            var allColliders = FindObjectsByType<BoxCollider2D>();
            foreach (var col in allColliders)
            {
                if (col.gameObject.name.StartsWith("Pipe"))
                {
                    col.sharedMaterial = noFriction;
                }
            }

            // Spawn Starting Coins right in front of Mario if they don't exist yet!
            if (GameObject.Find("StartingCoin_4") == null)
            {
                GameObject coinsRoot = GameObject.Find("--- STARTING COINS ---");
                if (coinsRoot == null) coinsRoot = new GameObject("--- STARTING COINS ---");

                for (int x = 4; x <= 10; x++)
                {
                    CreateWorldCoin(coinsRoot.transform, new Vector3(x + 0.5f, 2.7f, 0f), $"StartingCoin_{x}");
                }

                // Coins over pipes
                CreateWorldCoin(coinsRoot.transform, new Vector3(28.5f, 4.8f, 0f), "PipeCoin_28_1");
                CreateWorldCoin(coinsRoot.transform, new Vector3(29.5f, 4.8f, 0f), "PipeCoin_28_2");
                CreateWorldCoin(coinsRoot.transform, new Vector3(38.5f, 5.8f, 0f), "PipeCoin_38_1");
                CreateWorldCoin(coinsRoot.transform, new Vector3(39.5f, 5.8f, 0f), "PipeCoin_38_2");
                CreateWorldCoin(coinsRoot.transform, new Vector3(46.5f, 6.8f, 0f), "PipeCoin_46_1");
                CreateWorldCoin(coinsRoot.transform, new Vector3(47.5f, 6.8f, 0f), "PipeCoin_46_2");
            }
        }

        private GameObject CreateWorldCoin(Transform parent, Vector3 pos, string name)
        {
            GameObject coin = new GameObject(name);
            coin.transform.parent = parent;
            coin.transform.position = pos;

            var sr = coin.AddComponent<SpriteRenderer>();
            sr.color = Color.white;
            sr.sortingOrder = 5;

            var col = coin.AddComponent<CircleCollider2D>();
            col.radius = 0.4f;
            col.isTrigger = true;

            coin.AddComponent<SuperUnity.Items.FloatingCoin>();
            return coin;
        }

        public void RespawnAt(Vector3 spawnPos)
        {
            StopAllCoroutines();
            transform.position = spawnPos;

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.gravityScale = gravityMultiplier;
            }

            if (boxCollider != null)
            {
                boxCollider.enabled = true;
                boxCollider.sharedMaterial = new PhysicsMaterial2D("NoFriction") { friction = 0f, bounciness = 0f };
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
            }

            currentState = MarioState.Small;
            ApplyStateCollider();
            ApplyModelStats();

            isDying = false;
            isControllable = true;
            isFacingRight = true;
            canDoubleJump = true;

            var cam = Camera.main;
            if (cam != null)
            {
                var cf = cam.GetComponent<SuperUnity.Core.CameraFollow>();
                if (cf != null)
                {
                    cf.SnapToTarget(spawnPos);
                }
                else
                {
                    cam.transform.position = new Vector3(spawnPos.x, 6.0f, -10f);
                }
            }

            // Post-respawn grace invincibility
            StartCoroutine(PostRespawnGraceCoroutine());
        }

        private IEnumerator PostRespawnGraceCoroutine()
        {
            isInvincible = true;
            float elapsed = 0f;
            while (elapsed < postRespawnGraceTime)
            {
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = !spriteRenderer.enabled;
                }
                yield return new WaitForSeconds(0.1f);
                elapsed += 0.1f;
            }

            if (spriteRenderer != null) spriteRenderer.enabled = true;
            isInvincible = false;
        }

        public void ResetForNextLevel(Vector3 startPos)
        {
            StopAllCoroutines();
            transform.position = startPos;

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.gravityScale = gravityMultiplier;
            }

            if (boxCollider != null)
            {
                boxCollider.enabled = true;
                boxCollider.sharedMaterial = new PhysicsMaterial2D("NoFriction") { friction = 0f, bounciness = 0f };
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
            }

            currentState = MarioState.Small;
            ApplyStateCollider();
            ApplyModelStats();

            isDying = false;
            isControllable = true;
            isFacingRight = true;
            canDoubleJump = true;

            var cam = Camera.main;
            if (cam != null)
            {
                var cf = cam.GetComponent<SuperUnity.Core.CameraFollow>();
                if (cf != null)
                {
                    cf.SnapToTarget(startPos);
                }
                else
                {
                    cam.transform.position = new Vector3(startPos.x + 0.5f, 6.0f, -10f);
                }
            }

            EnsureCoinsAndPipes();
        }
}
}
