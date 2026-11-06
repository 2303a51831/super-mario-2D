using UnityEngine;
using SuperUnity.Enemies;

namespace SuperUnity.Items
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Fireball : MonoBehaviour
    {
        public float speed = 12.0f;
        public float bounceForce = 7.5f;
        public float lifeTime = 3.0f;
        private Rigidbody2D rb;
        private float direction = 1f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 3.5f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            Destroy(gameObject, lifeTime);
        }

        public void Launch(float dir)
        {
            direction = Mathf.Sign(dir);
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            rb.linearVelocity = new Vector2(direction * speed, -2f);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Check ground bounce
            bool hitGround = false;
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    hitGround = true;
                    break;
                }
            }

            if (hitGround)
            {
                rb.linearVelocity = new Vector2(direction * speed, bounceForce);
                return;
            }

            // Check enemy hit
            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.HitByFireball();
                Explode();
                return;
            }

            PiranhaPlant piranha = collision.gameObject.GetComponent<PiranhaPlant>();
            if (piranha != null)
            {
                piranha.HitByFireball();
                Explode();
                return;
            }

            // Hit wall
            Explode();
        }

        private void Explode()
        {
            // Quick explosion / disappear
            Destroy(gameObject);
        }
    }
}
