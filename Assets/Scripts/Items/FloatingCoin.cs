using UnityEngine;
using SuperUnity.Player;

namespace SuperUnity.Items
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class FloatingCoin : MonoBehaviour
    {
        [Header("Sprites for Spin Animation")]
        public Sprite[] frames;
        public float animSpeed = 6.0f;
        private SpriteRenderer sr;
        private Vector3 basePos;
        private float bobOffset;

        private static Sprite cachedCoinSprite;

        public static Sprite GetOrCreateCoinSprite()
        {
            if (cachedCoinSprite != null) return cachedCoinSprite;

            int size = 16;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            Color yellow = new Color(1f, 0.85f, 0.05f, 1f);
            Color darkGold = new Color(0.85f, 0.55f, 0.05f, 1f);
            Color highlight = new Color(1f, 1f, 0.8f, 1f);
            Color clear = Color.clear;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = (x - 7.5f) / 5.5f;
                    float dy = (y - 7.5f) / 7.5f;
                    float dist = dx * dx + dy * dy;

                    if (dist > 1f) tex.SetPixel(x, y, clear);
                    else if (dist > 0.72f) tex.SetPixel(x, y, darkGold);
                    else if (x >= 5 && x <= 7 && y >= 6 && y <= 12) tex.SetPixel(x, y, highlight);
                    else tex.SetPixel(x, y, yellow);
                }
            }
            tex.Apply();
            cachedCoinSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 16f);
            return cachedCoinSprite;
        }

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();
            if (sr.sprite == null) sr.sprite = GetOrCreateCoinSprite();
            sr.color = Color.white;
            sr.sortingOrder = 5;

            var col = GetComponent<CircleCollider2D>();
            if (col == null) col = gameObject.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.4f;

            basePos = transform.position;
            bobOffset = Random.Range(0f, Mathf.PI * 2f);
        }

        private void Update()
        {
            // 3D spin rotation via cosine scale
            float scaleX = Mathf.Cos(Time.time * 6.5f + bobOffset);
            transform.localScale = new Vector3(Mathf.Abs(scaleX) < 0.1f ? 0.1f : scaleX, 1f, 1f);

            // Floating bobbing up and down
            transform.position = basePos + new Vector3(0f, Mathf.Sin(Time.time * 4f + bobOffset) * 0.12f, 0f);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            MarioController mario = collision.GetComponent<MarioController>();
            if (mario != null && !mario.IsDead)
            {
                if (GameManager.Instance != null) GameManager.Instance.AddCoin(1);
                if (SoundManager.Instance != null) SoundManager.Instance.PlayCoin();
                Destroy(gameObject);
            }
        }
    }
}
