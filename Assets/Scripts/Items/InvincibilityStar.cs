using UnityEngine;
using SuperUnity.Player;
using SuperUnity.Core;

namespace SuperUnity.Items
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class InvincibilityStar : MonoBehaviour
    {
        public float moveSpeed = 4.0f;
        public float bounceForce = 8.5f;
        public int scoreValue = 1000;
        private Rigidbody2D rb;
        private bool movingRight = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.freezeRotation = true;
        }

        private void FixedUpdate()
        {
            rb.linearVelocity = new Vector2((movingRight ? 1f : -1f) * moveSpeed, rb.linearVelocity.y);

            // Gentle bounce when near ground
            if (Mathf.Abs(rb.linearVelocity.y) < 0.2f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.x > 0.6f) movingRight = true;
                else if (contact.normal.x < -0.6f) movingRight = false;
                if (contact.normal.y > 0.5f)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
                }
            }

            MarioController mario = collision.gameObject.GetComponent<MarioController>();
            if (mario != null && !mario.IsDead)
            {
                mario.TriggerInvincibility(12f);
                if (GameManager.Instance != null) GameManager.Instance.AddScore(scoreValue);
                if (SoundManager.Instance != null) SoundManager.Instance.PlayPowerUp();
                Destroy(gameObject);
            }
        }
    }
}
