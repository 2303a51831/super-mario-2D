using UnityEngine;
using System.Collections;
using SuperUnity.Player;

namespace SuperUnity.Blocks
{
    public enum BlockContent
    {
        Coin,
        Mushroom,
        FireFlower,
        OneUp
    }

    [RequireComponent(typeof(BoxCollider2D))]
    public class QuestionBlock : MonoBehaviour
    {
        [Header("Block Configuration")]
        public BlockContent content = BlockContent.Coin;
        public int coinCount = 1;
        public bool isUsed = false;

        [Header("Prefabs to Spawn")]
        public GameObject coinEffectPrefab;
        public GameObject mushroomPrefab;
        public GameObject fireFlowerPrefab;

        [Header("Visuals & Sprites")]
        public SpriteRenderer spriteRenderer;
        public Sprite[] activeFrames;
        public Sprite emptySprite;
        public float animSpeed = 4f;

        private Vector3 originalPosition;
        private float animTimer = 0f;
        private int currentFrame = 0;
        private bool isBouncing = false;

        private void Awake()
        {
            originalPosition = transform.position;
            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Update()
        {
            if (!isUsed && activeFrames != null && activeFrames.Length > 1 && spriteRenderer != null)
            {
                animTimer += Time.deltaTime * animSpeed;
                if (animTimer >= 1.0f)
                {
                    animTimer = 0f;
                    currentFrame = (currentFrame + 1) % activeFrames.Length;
                    spriteRenderer.sprite = activeFrames[currentFrame];
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (isUsed || isBouncing) return;

            MarioController mario = collision.gameObject.GetComponent<MarioController>();
            if (mario != null)
            {
                // Check if Mario bumped the bottom of the block
                foreach (ContactPoint2D contact in collision.contacts)
                {
                    if (contact.normal.y > 0.5f && mario.transform.position.y < transform.position.y - 0.4f)
                    {
                        HitBlock(mario);
                        break;
                    }
                }
            }
        }

        public void HitBlock(MarioController mario)
        {
            if (isUsed || isBouncing) return;

            StartCoroutine(BounceBlockCoroutine());
            SpawnContent(mario);

            coinCount--;
            if (coinCount <= 0)
            {
                isUsed = true;
                if (spriteRenderer != null && emptySprite != null)
                {
                    spriteRenderer.sprite = emptySprite;
                }
            }
        }

        private void SpawnContent(MarioController mario)
        {
            BlockContent spawnType = content;

            // If it's a powerup and Mario is already Super, spawn Fire Flower instead of Mushroom
            if (spawnType == BlockContent.Mushroom && mario.currentState != MarioState.Small)
            {
                spawnType = BlockContent.FireFlower;
            }

            switch (spawnType)
            {
                case BlockContent.Coin:
                    if (GameManager.Instance != null) GameManager.Instance.AddCoin(1);
                    if (SoundManager.Instance != null) SoundManager.Instance.PlayCoin();

                    if (coinEffectPrefab != null)
                    {
                        Instantiate(coinEffectPrefab, transform.position + Vector3.up * 0.8f, Quaternion.identity);
                    }
                    break;

                case BlockContent.Mushroom:
                    if (mushroomPrefab != null)
                    {
                        GameObject item = Instantiate(mushroomPrefab, transform.position, Quaternion.identity);
                        StartCoroutine(EmergeItemCoroutine(item));
                    }
                    if (SoundManager.Instance != null) SoundManager.Instance.PlayPowerUp();
                    break;

                case BlockContent.FireFlower:
                    if (fireFlowerPrefab != null)
                    {
                        GameObject item = Instantiate(fireFlowerPrefab, transform.position, Quaternion.identity);
                        StartCoroutine(EmergeItemCoroutine(item));
                    }
                    if (SoundManager.Instance != null) SoundManager.Instance.PlayPowerUp();
                    break;
            }
        }

        private IEnumerator EmergeItemCoroutine(GameObject item)
        {
            if (item == null) yield break;

            Collider2D col = item.GetComponent<Collider2D>();
            Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
            if (col != null) col.enabled = false;
            if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;

            Vector3 startPos = transform.position;
            Vector3 endPos = transform.position + Vector3.up * 1.0f;
            float elapsed = 0f;
            float duration = 0.5f;

            while (elapsed < duration)
            {
                if (item == null) yield break;
                item.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (item != null)
            {
                item.transform.position = endPos;
                if (col != null) col.enabled = true;
                if (rb != null) rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }

        private IEnumerator BounceBlockCoroutine()
        {
            isBouncing = true;
            if (SoundManager.Instance != null) SoundManager.Instance.PlayBump();

            Vector3 peakPos = originalPosition + Vector3.up * 0.35f;
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
