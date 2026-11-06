using UnityEngine;
using System.Collections;
using SuperUnity.Player;
using SuperUnity.Core;

namespace SuperUnity.Bosses
{
    public abstract class BossBase : MonoBehaviour
    {
        [Header("Boss Identity & Health")]
        public string bossName = "Boss";
        public int maxHealth = 3;
        public int currentHealth = 3;
        public int scoreReward = 5000;

        [Header("State")]
        public bool isInvulnerable = false;
        public bool isDefeated = false;
        public bool isActivated = false;

        protected SpriteRenderer spriteRenderer;
        protected Rigidbody2D rb;
        protected Collider2D bossCollider;

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
            bossCollider = GetComponent<Collider2D>();
        }

        protected virtual void Start()
        {
            UpdateBossHealthUI();
        }

        public virtual void ActivateBoss()
        {
            isActivated = true;
            UpdateBossHealthUI();
        }

        public virtual void TakeDamage(int amount = 1)
        {
            if (isInvulnerable || isDefeated || !isActivated) return;

            currentHealth -= amount;
            UpdateBossHealthUI();

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayStomp();
            }

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                DefeatBoss();
            }
            else
            {
                StartCoroutine(InvulnerabilityCoroutine());
                OnDamaged();
            }
        }

        protected virtual void OnDamaged()
        {
        }

        protected virtual IEnumerator InvulnerabilityCoroutine()
        {
            isInvulnerable = true;
            float duration = 1.8f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = !spriteRenderer.enabled;
                }
                yield return new WaitForSeconds(0.08f);
                elapsed += 0.08f;
            }

            if (spriteRenderer != null) spriteRenderer.enabled = true;
            isInvulnerable = false;
        }

        protected virtual void DefeatBoss()
        {
            isDefeated = true;
            isInvulnerable = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(scoreReward);
            }

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayStageClear();
            }

            StartCoroutine(DefeatAnimationCoroutine());
        }

        protected virtual IEnumerator DefeatAnimationCoroutine()
        {
            float elapsed = 0f;
            while (elapsed < 2.0f)
            {
                transform.Rotate(0f, 0f, 360f * Time.deltaTime * 2f);
                transform.position += Vector3.down * 4f * Time.deltaTime;
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Spawn goal / finish level
            OnDefeatComplete();
            Destroy(gameObject);
        }

        protected virtual void OnDefeatComplete()
        {
            // Trigger Level Clear modal with victory rewards and Next Level progression
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReachGoal(scoreReward);
            }
        }

        protected void UpdateBossHealthUI()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UpdateBossHealth(bossName, currentHealth, maxHealth);
            }
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            MarioController mario = collision.gameObject.GetComponent<MarioController>();
            if (mario != null && !mario.IsDead)
            {
                // Check if Mario stomped the boss from above
                bool stomped = false;
                foreach (ContactPoint2D contact in collision.contacts)
                {
                    if (contact.normal.y < -0.5f && mario.transform.position.y > transform.position.y + 0.5f)
                    {
                        stomped = true;
                        break;
                    }
                }

                if (stomped && !isInvulnerable)
                {
                    mario.Bounce(12f);
                    TakeDamage(1);
                }
                else if (!isDefeated)
                {
                    mario.TakeDamage();
                }
            }
        }
    }
}
