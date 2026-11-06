using UnityEngine;
using System.Collections;
using SuperUnity.Player;

namespace SuperUnity.Bosses
{
    public class World1Boss : BossBase
    {
        [Header("Bowser Jr Settings")]
        public float moveSpeed = 3.5f;
        public float chargeSpeed = 8.0f;
        public float jumpForce = 12.0f;
        private bool movingRight = false;
        private float stateTimer = 0f;
        private bool isSpinning = false;

        protected override void Awake()
        {
            bossName = "BOWSER JR. - GRASSLAND GUARDIAN";
            maxHealth = 3;
            scoreReward = 5000;
            base.Awake();
        }

        private void Update()
        {
            if (isDefeated || !isActivated) return;

            stateTimer += Time.deltaTime;

            if (isSpinning)
            {
                // Rapid spin rush
                transform.Rotate(0f, 0f, 720f * Time.deltaTime);
                rb.linearVelocity = new Vector2((movingRight ? 1f : -1f) * chargeSpeed, rb.linearVelocity.y);

                if (stateTimer >= 3.5f)
                {
                    isSpinning = false;
                    transform.rotation = Quaternion.identity;
                    stateTimer = 0f;
                }
                return;
            }

            // Normal combat pattern: pace and jump
            rb.linearVelocity = new Vector2((movingRight ? 1f : -1f) * moveSpeed, rb.linearVelocity.y);

            if (stateTimer >= 2.5f)
            {
                // Leap towards Mario
                var mario = FindAnyObjectByType<MarioController>();
                if (mario != null)
                {
                    movingRight = mario.transform.position.x > transform.position.x;
                }
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                stateTimer = 0f;

                // After every hit, trigger shell spin
                if (currentHealth < maxHealth && Random.value > 0.4f)
                {
                    isSpinning = true;
                }
            }
        }

        protected override void OnDamaged()
        {
            base.OnDamaged();
            // Start spinning counterattack
            isSpinning = true;
            stateTimer = 0f;
        }

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            // Reverse direction on walls
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.x > 0.6f) movingRight = true;
                else if (contact.normal.x < -0.6f) movingRight = false;
            }

            base.OnCollisionEnter2D(collision);
        }
    }
}
