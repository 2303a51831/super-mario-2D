using UnityEngine;
using System.Collections;
using SuperUnity.Player;

namespace SuperUnity.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public abstract class EnemyBase : MonoBehaviour
    {
        [Header("Base Enemy Settings")]
        public float moveSpeed = 2.0f;
        public int scoreValue = 100;
        public bool isDefeated = false;
        public bool isFacingLeft = true;

        [Header("Activation Distance")]
        public float activationDistance = 16.0f;
        protected bool isActivated = false;

        protected Rigidbody2D rb;
        protected Collider2D enemyCollider;
        protected SpriteRenderer spriteRenderer;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            enemyCollider = GetComponent<Collider2D>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        protected virtual void Update()
        {
            if (isDefeated) return;

            // Activate when Mario gets within range
            if (!isActivated)
            {
                var mario = FindAnyObjectByType<MarioController>();
                if (mario != null && Mathf.Abs(mario.transform.position.x - transform.position.x) <= activationDistance)
                {
                    isActivated = true;
                }
                else
                {
                    rb.linearVelocity = Vector2.zero;
                    return;
                }
            }

            Move();
        }

        protected virtual void Move()
        {
            float dir = isFacingLeft ? -1f : 1f;
            rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
        }

        public virtual void ReverseDirection()
        {
            isFacingLeft = !isFacingLeft;
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = !isFacingLeft;
            }
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (isDefeated) return;

            // Check if hit wall or another enemy
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.x > 0.6f)
                {
                    isFacingLeft = false;
                }
                else if (contact.normal.x < -0.6f)
                {
                    isFacingLeft = true;
                }
            }

            // Check collision with Mario
            MarioController mario = collision.gameObject.GetComponent<MarioController>();
            if (mario != null)
            {
                HandleMarioCollision(mario, collision);
            }
        }

        protected virtual void HandleMarioCollision(MarioController mario, Collision2D collision)
        {
            if (isDefeated || mario.IsDead) return;

            // Check if Mario stomped on top
            bool stomped = false;
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // Normal pointing up means Mario hit top of enemy
                if (contact.normal.y < -0.4f || (mario.transform.position.y > transform.position.y + 0.4f && mario.Rb.linearVelocity.y <= 0.5f))
                {
                    stomped = true;
                    break;
                }
            }

            if (stomped)
            {
                mario.Bounce();
                OnStomped(mario);
            }
            else
            {
                mario.TakeDamage();
            }
        }

        public abstract void OnStomped(MarioController mario);

        public virtual void HitByFireball()
        {
            DefeatWithFlip();
        }

        public virtual void HitByShell()
        {
            DefeatWithFlip();
        }

        public virtual void DefeatWithFlip()
        {
            if (isDefeated) return;
            isDefeated = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(scoreValue);
            }

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayKick();
            }

            StartCoroutine(FlipAndFallCoroutine());
        }

        protected IEnumerator FlipAndFallCoroutine()
        {
            if (enemyCollider != null) enemyCollider.enabled = false;
            if (spriteRenderer != null) spriteRenderer.flipY = true;

            rb.linearVelocity = new Vector2(isFacingLeft ? 2f : -2f, 8f);
            rb.gravityScale = 3f;

            yield return new WaitForSeconds(2.0f);
            Destroy(gameObject);
        }
    }
}
