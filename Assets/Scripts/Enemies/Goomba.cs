using UnityEngine;
using System.Collections;
using SuperUnity.Player;

namespace SuperUnity.Enemies
{
    public class Goomba : EnemyBase
    {
        [Header("Goomba Sprites")]
        public Sprite walkSprite1;
        public Sprite walkSprite2;
        public Sprite squishedSprite;

        [Header("Animation")]
        public float animSpeed = 4.0f;
        private float animTimer = 0f;
        private bool frameToggle = false;

        protected override void Awake()
        {
            base.Awake();
            scoreValue = 100;
        }

        protected override void Update()
        {
            base.Update();

            if (!isDefeated && isActivated)
            {
                AnimateWalk();
            }
        }

        private void AnimateWalk()
        {
            if (walkSprite1 == null || walkSprite2 == null || spriteRenderer == null) return;

            animTimer += Time.deltaTime * animSpeed;
            if (animTimer >= 1.0f)
            {
                animTimer = 0f;
                frameToggle = !frameToggle;
                spriteRenderer.sprite = frameToggle ? walkSprite2 : walkSprite1;
            }
        }

        public override void OnStomped(MarioController mario)
        {
            if (isDefeated) return;
            isDefeated = true;

            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            if (enemyCollider != null) enemyCollider.enabled = false;

            if (squishedSprite != null && spriteRenderer != null)
            {
                spriteRenderer.sprite = squishedSprite;
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(scoreValue);
            }

            StartCoroutine(SquishAndDestroyCoroutine());
        }

        private IEnumerator SquishAndDestroyCoroutine()
        {
            yield return new WaitForSeconds(0.45f);
            Destroy(gameObject);
        }
    }
}
