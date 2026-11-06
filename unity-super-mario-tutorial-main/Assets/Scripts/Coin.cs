using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    public int points = 200;
    public float bobSpeed = 3f;
    public float bobHeight = 0.12f;
    public float spinSpeed = 4f;

    private Vector3 basePosition;
    private bool collected = false;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        basePosition = transform.position;
    }

    private void Update()
    {
        if (collected) return;

        // Gentle floating bobbing
        float newY = basePosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // 3D coin spin illusion via X-scale oscillation
        float scaleX = Mathf.Cos(Time.time * spinSpeed);
        transform.localScale = new Vector3(scaleX, 1f, 1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!collected && other.CompareTag("Player") && other.TryGetComponent(out Player player))
        {
            StartCoroutine(CollectSequence());
        }
    }

    private IEnumerator CollectSequence()
    {
        collected = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) {
            col.enabled = false;
        }

        if (GameManager.Instance != null) {
            GameManager.Instance.AddCoin();
            GameManager.Instance.AddScore(points);
        }

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * 1.5f;
        Color startColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.localScale = new Vector3(Mathf.Cos(elapsed * 25f), 1f, 1f);

            if (spriteRenderer != null) {
                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
