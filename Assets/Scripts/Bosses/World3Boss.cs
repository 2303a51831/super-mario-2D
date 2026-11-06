using UnityEngine;
using System.Collections;
using SuperUnity.Player;

namespace SuperUnity.Bosses
{
    public class World3Boss : BossBase
    {
        [Header("Frost Titan")]
        public float moveSpeed = 4.0f;
        public float rushSpeed = 10.0f;
        private bool movingRight = false;
        private float timer = 0f;
        private bool isRushing = false;

        protected override void Awake()
        {
            bossName = "FROST TITAN - ICE KING";
            maxHealth = 5;
            scoreReward = 10000;
            base.Awake();
        }

        private void Update()
        {
            if (isDefeated || !isActivated) return;

            timer += Time.deltaTime;

            var mario = FindAnyObjectByType<MarioController>();
            if (mario != null && !isRushing)
            {
                movingRight = mario.transform.position.x > transform.position.x;
            }

            if (isRushing)
            {
                rb.linearVelocity = new Vector2((movingRight ? 1f : -1f) * rushSpeed, rb.linearVelocity.y);
                if (timer >= 2.0f)
                {
                    isRushing = false;
                    timer = 0f;
                }
                return;
            }

            rb.linearVelocity = new Vector2((movingRight ? 1f : -1f) * moveSpeed, rb.linearVelocity.y);

            // Trigger Ice Rush or Blizzard leap
            if (timer >= 3.5f)
            {
                timer = 0f;
                if (Random.value > 0.5f)
                {
                    isRushing = true;
                }
                else
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 13.0f);
                }
            }
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
