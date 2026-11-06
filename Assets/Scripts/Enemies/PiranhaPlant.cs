using UnityEngine;
using System.Collections;
using SuperUnity.Player;

namespace SuperUnity.Enemies
{
    public class PiranhaPlant : MonoBehaviour
    {
        [Header("Movement")]
        public float moveDistance = 1.8f;
        public float moveSpeed = 2.0f;
        public float waitTopTime = 1.8f;
        public float waitBottomTime = 1.8f;
        public float playerDetectionRadius = 2.5f;

        [Header("Sprites & Visuals")]
        public SpriteRenderer spriteRenderer;
        public Sprite openMouthSprite;
        public Sprite closedMouthSprite;

        private Vector3 bottomPosition;
        private Vector3 topPosition;
        private MarioController mario;

        private void Awake()
        {
            bottomPosition = transform.position;
            topPosition = bottomPosition + Vector3.up * moveDistance;
            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            mario = FindAnyObjectByType<MarioController>();
            StartCoroutine(PiranhaCycleCoroutine());
        }

        private IEnumerator PiranhaCycleCoroutine()
        {
            // Initial delay
            yield return new WaitForSeconds(Random.Range(0.5f, 2.0f));

            while (true)
            {
                // Wait at bottom
                yield return new WaitForSeconds(waitBottomTime);

                // Check if Mario is too close above the pipe
                while (mario != null && Vector2.Distance(transform.position, mario.transform.position) < playerDetectionRadius)
                {
                    yield return new WaitForSeconds(0.5f);
                }

                // Rise Up
                while (transform.position.y < topPosition.y)
                {
                    transform.position = Vector3.MoveTowards(transform.position, topPosition, moveSpeed * Time.deltaTime);
                    yield return null;
                }

                // Wait at top
                yield return new WaitForSeconds(waitTopTime);

                // Lower Down
                while (transform.position.y > bottomPosition.y)
                {
                    transform.position = Vector3.MoveTowards(transform.position, bottomPosition, moveSpeed * Time.deltaTime);
                    yield return null;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            MarioController hitMario = other.GetComponent<MarioController>();
            if (hitMario != null)
            {
                hitMario.TakeDamage();
            }
        }

        public void HitByFireball()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(200);
            }
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayKick();
            }
            Destroy(gameObject);
        }
    }
}
