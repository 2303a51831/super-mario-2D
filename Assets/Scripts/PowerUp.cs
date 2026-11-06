using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum Type
    {
        Coin,
        ExtraLife,
        MagicMushroom,
        Starpower,
    }

    public Type type;

    private Vector3 basePosition;
    private bool isCoin;
    private bool collected;

    private void Start()
    {
        isCoin = (type == Type.Coin);
        if (isCoin) {
            basePosition = transform.position;
        }
    }

    private void Update()
    {
        if (isCoin && !collected)
        {
            // Bob up and down gently
            float newY = basePosition.y + Mathf.Sin(Time.time * 3f) * 0.12f;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            // 3D coin spin effect via X-scale oscillation
            float scaleX = Mathf.Cos(Time.time * 4f);
            transform.localScale = new Vector3(scaleX, 1f, 1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!collected && other.CompareTag("Player") && other.TryGetComponent(out Player player)) {
            Collect(player);
        }
    }

    private void Collect(Player player)
    {
        collected = true;

        switch (type)
        {
            case Type.Coin:
                StartCoroutine(CollectCoin());
                return;

            case Type.ExtraLife:
                GameManager.Instance.AddLife();
                break;

            case Type.MagicMushroom:
                player.Grow();
                GameManager.Instance.AddScore(1000);
                break;

            case Type.Starpower:
                player.Starpower();
                GameManager.Instance.AddScore(1000);
                break;
        }

        Destroy(gameObject);
    }

    private IEnumerator CollectCoin()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        GameManager.Instance.AddCoin();

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * 1.5f;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color startColor = sr != null ? sr.color : Color.white;

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.localScale = new Vector3(Mathf.Cos(elapsed * 25f), 1f, 1f);

            if (sr != null) {
                sr.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);
            }

            yield return null;
        }

        Destroy(gameObject);
    }

}
