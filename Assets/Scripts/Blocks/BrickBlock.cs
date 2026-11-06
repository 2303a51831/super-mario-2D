using UnityEngine;
using System.Collections;
using SuperUnity.Player;

namespace SuperUnity.Blocks
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class BrickBlock : MonoBehaviour
    {
        [Header("Block Properties")]
        public bool containsCoin = false;
        public int coinCount = 1;
        public GameObject brickDebrisPrefab;
        public GameObject coinEffectPrefab;

        private Vector3 originalPosition;
        private bool isBouncing = false;

        private void Awake()
        {
            originalPosition = transform.position;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (isBouncing) return;

            MarioController mario = collision.gameObject.GetComponent<MarioController>();
            if (mario != null)
            {
                foreach (ContactPoint2D contact in collision.contacts)
                {
                    if (contact.normal.y > 0.5f && mario.transform.position.y < transform.position.y - 0.4f)
                    {
                        HitBrick(mario);
                        break;
                    }
                }
            }
        }

        public void HitBrick(MarioController mario)
        {
            if (containsCoin && coinCount > 0)
            {
                coinCount--;
                if (GameManager.Instance != null) GameManager.Instance.AddCoin(1);
                if (SoundManager.Instance != null) SoundManager.Instance.PlayCoin();

                if (coinEffectPrefab != null)
                {
                    Instantiate(coinEffectPrefab, transform.position + Vector3.up * 0.8f, Quaternion.identity);
                }

                StartCoroutine(BounceCoroutine());
                return;
            }

            if (mario.currentState == MarioState.Small)
            {
                // Small Mario bumps brick without breaking
                StartCoroutine(BounceCoroutine());
                if (SoundManager.Instance != null) SoundManager.Instance.PlayBump();
            }
            else
            {
                // Super or Fire Mario shatters the brick!
                BreakBrick();
            }
        }

        public void BreakBrick()
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayBrickBreak();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(50);
            }

            SpawnDebris();
            Destroy(gameObject);
        }

        private void SpawnDebris()
        {
            Vector2[] velocities = new Vector2[]
            {
                new Vector2(-3.5f, 10f),
                new Vector2(3.5f, 10f),
                new Vector2(-2.5f, 6.5f),
                new Vector2(2.5f, 6.5f)
            };

            for (int i = 0; i < 4; i++)
            {
                GameObject piece;
                if (brickDebrisPrefab != null)
                {
                    piece = Instantiate(brickDebrisPrefab, transform.position, Quaternion.identity);
                }
                else
                {
                    // Procedural debris piece
                    piece = new GameObject("BrickDebris");
                    piece.transform.position = transform.position;
                    piece.transform.localScale = Vector3.one * 0.4f;
                    var sr = piece.AddComponent<SpriteRenderer>();
                    var currentSr = GetComponent<SpriteRenderer>();
                    if (currentSr != null) sr.sprite = currentSr.sprite;
                    sr.color = new Color(0.85f, 0.4f, 0.1f);
                    sr.sortingOrder = 10;
                }

                var rb = piece.GetComponent<Rigidbody2D>();
                if (rb == null) rb = piece.AddComponent<Rigidbody2D>();
                rb.gravityScale = 3.5f;
                rb.linearVelocity = velocities[i];
                rb.angularVelocity = Random.Range(-400f, 400f);

                var col = piece.GetComponent<Collider2D>();
                if (col != null) col.enabled = false;

                Destroy(piece, 2.0f);
            }
        }

        private IEnumerator BounceCoroutine()
        {
            isBouncing = true;
            Vector3 peakPos = originalPosition + Vector3.up * 0.3f;
            float speed = 7.0f;

            while (transform.position.y < peakPos.y)
            {
                transform.position = Vector3.MoveTowards(transform.position, peakPos, speed * Time.deltaTime);
                yield return null;
            }

            while (transform.position.y > originalPosition.y)
            {
                transform.position = Vector3.MoveTowards(transform.position, originalPosition, speed * Time.deltaTime);
                yield return null;
            }

            transform.position = originalPosition;
            isBouncing = false;
        }
    }
}
