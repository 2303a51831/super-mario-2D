using UnityEngine;
using SuperUnity.Player;
using SuperUnity.Core;

namespace SuperUnity.Levels
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Checkpoint : MonoBehaviour
    {
        [Header("Checkpoint State")]
        public bool isActivated = false;
        public SpriteRenderer flagRenderer;
        public Color activeColor = new Color(0.2f, 1f, 0.4f);
        public Color inactiveColor = new Color(0.8f, 0.8f, 0.8f);

        private void Awake()
        {
            var col = GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            if (flagRenderer == null) flagRenderer = GetComponentInChildren<SpriteRenderer>();
            UpdateVisuals();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isActivated) return;

            MarioController mario = collision.GetComponent<MarioController>();
            if (mario != null && !mario.IsDead)
            {
                ActivateCheckpoint();
            }
        }

        public void ActivateCheckpoint()
        {
            isActivated = true;
            UpdateVisuals();

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayOneUp();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetCheckpoint(transform.position + Vector3.up * 0.5f);
            }
        }

        private void UpdateVisuals()
        {
            if (flagRenderer != null)
            {
                flagRenderer.color = isActivated ? activeColor : inactiveColor;
            }
        }
    }
}
