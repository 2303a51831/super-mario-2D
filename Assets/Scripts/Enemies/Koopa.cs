using UnityEngine;
using SuperUnity.Player;

namespace SuperUnity.Enemies
{
    public class Koopa : EnemyBase
    {
        [Header("Koopa Sprites")]
        public Sprite walkSprite1;
        public Sprite walkSprite2;
        public Sprite shellSprite;

        [Header("Shell Mechanics")]
        public bool inShell = false;
        public bool isShellMoving = false;
        public float shellSpeed = 12.0f;

        [Header("Animation")]
        public float animSpeed = 4.0f;
        private float animTimer = 0f;
        private bool frameToggle = false;

        protected override void Awake()
        {
            base.Awake();
            scoreValue = 200;
        }

        protected override void Update()
        {
            if (isDefeated) return;

            if (!inShell)
            {
                base.Update();
                if (isActivated) AnimateWalk();
            }
            else
            {
                if (isShellMoving)
                {
                    float dir = isFacingLeft ? -1f : 1f;
                    rb.linearVelocity = new Vector2(dir * shellSpeed, rb.linearVelocity.y);
                }
                else
                {
                    rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                }
            }
        }

        private void AnimateWalk()
        {
            if (walkSprite1 == null || walkSprite2 == null || spriteRenderer == null) return;

            animTimer += Time.deltaTime * animSpeed;
            if (animTimer >= 1.0f)
            {
                animTimer = 0f;
                frameToggle = !frameToggle;
                spriteRenderer.sprite = frameToggle ? walkSprite2 : walkSprite1;
            }
        }

        public override void OnStomped(MarioController mario)
        {
            if (!inShell)
            {
                EnterShell();
                if (GameManager.Instance != null) GameManager.Instance.AddScore(scoreValue);
            }
            else
            {
                if (isShellMoving)
                {
                    // Stop the shell
                    isShellMoving = false;
                    if (SoundManager.Instance != null) SoundManager.Instance.PlayStomp();
                }
                else
                {
                    // Kick the shell
                    KickShell(mario.transform.position.x < transform.position.x);
                }
            }
        }

        private void EnterShell()
        {
            inShell = true;
            isShellMoving = false;
            rb.linearVelocity = Vector2.zero;

            if (shellSprite != null && spriteRenderer != null)
            {
                spriteRenderer.sprite = shellSprite;
            }
        }

        public void KickShell(bool movingRight)
        {
            inShell = true;
            isShellMoving = true;
            isFacingLeft = !movingRight;

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayKick();
            }
        }

        protected override void HandleMarioCollision(MarioController mario, Collision2D collision)
        {
            if (isDefeated || mario.IsDead) return;

            // Check stomp
            bool stomped = false;
            foreach (ContactPoint2D contact in collision.contacts)
            {
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
                return;
            }

            if (inShell)
            {
                if (!isShellMoving)
                {
                    // Mario kicks the stationary shell
                    KickShell(mario.transform.position.x < transform.position.x);
                }
                else
                {
                    // Moving shell hurts Mario
                    mario.TakeDamage();
                }
            }
            else
            {
                // Walking Koopa hurts Mario
                mario.TakeDamage();
            }
        }

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            base.OnCollisionEnter2D(collision);

            if (inShell && isShellMoving)
            {
                // Shell destroys other enemies on contact
                EnemyBase otherEnemy = collision.gameObject.GetComponent<EnemyBase>();
                if (otherEnemy != null && otherEnemy != this)
                {
                    otherEnemy.HitByShell();
                }
            }
        }
    }
}
