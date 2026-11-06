using UnityEngine;
using SuperUnity.Player;

namespace SuperUnity.Levels
{
    public enum PlatformType
    {
        Horizontal,
        Vertical,
        Falling
    }

    [RequireComponent(typeof(BoxCollider2D))]
    public class MovingPlatform : MonoBehaviour
    {
        public PlatformType platformType = PlatformType.Horizontal;
        public float moveDistance = 5.0f;
        public float moveSpeed = 2.5f;
        public float fallDelay = 0.8f;

        private Vector3 startPos;
        private Vector3 targetPos;
        private bool movingToTarget = true;
        private bool isFalling = false;
        private float fallTimer = 0f;
        private Rigidbody2D rb;

        private void Awake()
        {
            startPos = transform.position;
            if (platformType == PlatformType.Horizontal)
            {
                targetPos = startPos + Vector3.right * moveDistance;
            }
            else if (platformType == PlatformType.Vertical)
            {
                targetPos = startPos + Vector3.up * moveDistance;
            }

            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
            }
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.freezeRotation = true;
        }

        private void FixedUpdate()
        {
            if (platformType == PlatformType.Falling)
            {
                if (isFalling)
                {
                    fallTimer += Time.fixedDeltaTime;
                    if (fallTimer >= fallDelay)
                    {
                        rb.bodyType = RigidbodyType2D.Dynamic;
                        rb.gravityScale = 3f;
                    }
                    else
                    {
                        // Shake effect before falling
                        float shake = Mathf.Sin(Time.time * 40f) * 0.08f;
                        transform.position = startPos + new Vector3(shake, 0f, 0f);
                    }
                }
                return;
            }

            // Ping-pong movement
            Vector3 dest = movingToTarget ? targetPos : startPos;
            Vector3 newPos = Vector3.MoveTowards(transform.position, dest, moveSpeed * Time.fixedDeltaTime);
            transform.position = newPos;

            if (Vector3.Distance(transform.position, dest) < 0.05f)
            {
                movingToTarget = !movingToTarget;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            MarioController mario = collision.gameObject.GetComponent<MarioController>();
            if (mario != null)
            {
                // Check if Mario is standing on top of platform
                foreach (ContactPoint2D contact in collision.contacts)
                {
                    if (contact.normal.y < -0.5f)
                    {
                        collision.transform.SetParent(transform);
                        if (platformType == PlatformType.Falling && !isFalling)
                        {
                            isFalling = true;
                            fallTimer = 0f;
                        }
                        break;
                    }
                }
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            MarioController mario = collision.gameObject.GetComponent<MarioController>();
            if (mario != null)
            {
                if (collision.transform.parent == transform)
                {
                    collision.transform.SetParent(null);
                }
            }
        }
    }
}
