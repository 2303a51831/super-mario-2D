using UnityEngine;
using SuperUnity.Player;

namespace SuperUnity.Items
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Mushroom : MonoBehaviour
    {
        public float moveSpeed = 3.0f;
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
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Reverse on walls
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.x > 0.6f) movingRight = true;
                else if (contact.normal.x < -0.6f) movingRight = false;
            }

            MarioController mario = collision.gameObject.GetComponent<MarioController>();
            if (mario != null)
            {
                mario.Grow();
                if (GameManager.Instance != null) GameManager.Instance.AddScore(scoreValue);
                Destroy(gameObject);
            }
        }
    }
}
