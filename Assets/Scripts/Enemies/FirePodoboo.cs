using UnityEngine;
using SuperUnity.Player;

namespace SuperUnity.Enemies
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class FirePodoboo : MonoBehaviour
    {
        public float leapHeight = 7.0f;
        public float leapSpeed = 9.0f;
        public float restTime = 1.5f;

        private Vector3 startPos;
        private float timer = 0f;
        private bool isLeaping = false;
        private float verticalVelocity = 0f;

        private void Awake()
        {
            startPos = transform.position;
            var col = GetComponent<BoxCollider2D>();
            col.isTrigger = true;
        }

        private void Update()
        {
            if (!isLeaping)
            {
                timer += Time.deltaTime;
                if (timer >= restTime)
                {
                    isLeaping = true;
                    timer = 0f;
                    verticalVelocity = leapSpeed;
                }
                return;
            }

            // Up and down trajectory
            verticalVelocity -= 22.0f * Time.deltaTime;
            transform.position += Vector3.up * verticalVelocity * Time.deltaTime;

            if (transform.position.y <= startPos.y)
            {
                transform.position = startPos;
                isLeaping = false;
                timer = 0f;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            MarioController mario = collision.GetComponent<MarioController>();
            if (mario != null && !mario.IsDead)
            {
                mario.TakeDamage();
            }
        }
    }
}
