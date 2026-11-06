using UnityEngine;
using SuperUnity.Player;

namespace SuperUnity.Levels
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class FallingHazard : MonoBehaviour
    {
        public float triggerDistanceX = 2.0f;
        public float fallDelay = 0.35f;
        public float fallSpeed = 12.0f;
        private bool isTriggered = false;
        private bool isFalling = false;
        private Vector3 startPos;
        private float timer = 0f;

        private void Awake()
        {
            startPos = transform.position;
        }

        private void Update()
        {
            if (isFalling)
            {
                transform.position += Vector3.down * fallSpeed * Time.deltaTime;
                if (transform.position.y < -10f)
                {
                    Destroy(gameObject);
                }
                return;
            }

            if (isTriggered)
            {
                timer += Time.deltaTime;
                float shake = Mathf.Sin(Time.time * 45f) * 0.08f;
                transform.position = startPos + new Vector3(shake, 0f, 0f);

                if (timer >= fallDelay)
                {
                    isFalling = true;
                }
                return;
            }

            var mario = FindAnyObjectByType<MarioController>();
            if (mario != null && !mario.IsDead)
            {
                float dx = Mathf.Abs(mario.transform.position.x - transform.position.x);
                float dy = transform.position.y - mario.transform.position.y;
                if (dx <= triggerDistanceX && dy > 0f && dy < 10f)
                {
                    isTriggered = true;
                    timer = 0f;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            MarioController mario = collision.GetComponent<MarioController>();
            if (mario != null && !mario.IsDead)
            {
                mario.TakeDamage();
                Destroy(gameObject);
            }
        }
    }
}
