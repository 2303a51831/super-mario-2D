using UnityEngine;
using System.Collections;
using SuperUnity.Player;

namespace SuperUnity.Bosses
{
    public class World2Boss : BossBase
    {
        [Header("Pharaoh Sand Golem")]
        public float moveSpeed = 3.0f;
        public float leapForce = 14.0f;
        private bool movingRight = false;
        private float attackTimer = 0f;
        private bool isBurrowing = false;

        protected override void Awake()
        {
            bossName = "PHARAOH SAND GOLEM - DESERT LORD";
            maxHealth = 4;
            scoreReward = 7500;
            base.Awake();
        }

        private void Update()
        {
            if (isDefeated || !isActivated) return;

            attackTimer += Time.deltaTime;

            if (isBurrowing) return;

            // Track Mario
            var mario = FindAnyObjectByType<MarioController>();
            if (mario != null)
            {
                movingRight = mario.transform.position.x > transform.position.x;
            }

            rb.linearVelocity = new Vector2((movingRight ? 1f : -1f) * moveSpeed, rb.linearVelocity.y);

            // Special attack: Sand Burrow teleport slam!
            if (attackTimer >= 4.0f)
            {
                attackTimer = 0f;
                StartCoroutine(BurrowAttackCoroutine(mario));
            }
        }

        private IEnumerator BurrowAttackCoroutine(MarioController mario)
        {
            isBurrowing = true;
            isInvulnerable = true;

            // Sink into ground
            float elapsed = 0f;
            Vector3 origPos = transform.position;
            while (elapsed < 0.8f)
            {
                transform.position += Vector3.down * 3f * Time.deltaTime;
                elapsed += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(0.8f);

            // Emerge near Mario with leap slam!
            if (mario != null)
            {
                transform.position = new Vector3(mario.transform.position.x + (Random.value > 0.5f ? 4f : -4f), origPos.y, origPos.z);
            }

            isInvulnerable = false;
            isBurrowing = false;
            rb.linearVelocity = new Vector2((movingRight ? 1f : -1f) * 6f, leapForce);
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
