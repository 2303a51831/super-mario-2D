using UnityEngine;
using System.Collections;
using SuperUnity.Player;
using SuperUnity.Core;

namespace SuperUnity.Bosses
{
    public class World4FinalBoss : BossBase
    {
        [Header("Volcano King Settings")]
        public float moveSpeed = 4.0f;
        public float chargeSpeed = 11.0f;
        public GameObject bridgeAxeTrigger;
        public GameObject bridgeObject;

        private bool movingRight = false;
        private float phaseTimer = 0f;
        private bool isEnraged = false;
        private bool isCharging = false;

        protected override void Awake()
        {
            bossName = "VOLCANO KING - LORD OF THE ABYSS";
            maxHealth = 6;
            currentHealth = 6;
            scoreReward = 25000;
            base.Awake();
        }

        private void Update()
        {
            if (isDefeated || !isActivated) return;

            phaseTimer += Time.deltaTime;

            // Enrage phase when below half health
            if (currentHealth <= 3 && !isEnraged)
            {
                isEnraged = true;
                moveSpeed = 5.5f;
                chargeSpeed = 13.0f;
                if (spriteRenderer != null) spriteRenderer.color = new Color(1f, 0.2f, 0.1f);
            }

            var mario = FindAnyObjectByType<MarioController>();
            if (mario != null && !isCharging)
            {
                movingRight = mario.transform.position.x > transform.position.x;
            }

            if (isCharging)
            {
                rb.linearVelocity = new Vector2((movingRight ? 1f : -1f) * chargeSpeed, rb.linearVelocity.y);
                if (phaseTimer >= 1.8f)
                {
                    isCharging = false;
                    phaseTimer = 0f;
                }
                return;
            }

            // Normal movement
            rb.linearVelocity = new Vector2((movingRight ? 1f : -1f) * moveSpeed, rb.linearVelocity.y);

            // Phase attacks: Leap slam or charge
            if (phaseTimer >= (isEnraged ? 2.5f : 3.8f))
            {
                phaseTimer = 0f;
                if (Random.value > 0.45f)
                {
                    // Fire Charge!
                    isCharging = true;
                }
                else
                {
                    // Volcano Leap!
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 15.0f);
                }
            }
        }

        public void TriggerBridgeAxeCollapse()
        {
            if (isDefeated) return;

            // Bridge collapses!
            if (bridgeObject != null)
            {
                Destroy(bridgeObject);
            }

            // Instant defeat!
            DefeatBoss();
        }

        protected override void DefeatBoss()
        {
            isDefeated = true;
            isInvulnerable = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(scoreReward);
            }

            StartCoroutine(FinalVictorySequenceCoroutine());
        }

        private IEnumerator FinalVictorySequenceCoroutine()
        {
            // Fall into lava abyss
            float elapsed = 0f;
            while (elapsed < 2.5f)
            {
                transform.Rotate(0f, 0f, 540f * Time.deltaTime);
                transform.position += Vector3.down * 6f * Time.deltaTime;
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Trigger complete game win celebration
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameWon();
            }

            Destroy(gameObject);
        }

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.x > 0.6f) movingRight = true;
                else if (contact.normal.x < -0.6f) movingRight = false;
            }

            base.OnCollisionEnter2D(collision);
        }
    }
}
