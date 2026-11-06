using UnityEngine;
using SuperUnity.Player;
using SuperUnity.Core;

namespace SuperUnity.Enemies
{
    public class FlyingParatroopa : EnemyBase
    {
        [Header("Flight Settings")]
        public float flySpeed = 2.5f;
        public float waveHeight = 1.8f;
        public float waveSpeed = 2.0f;
        public Sprite wingSprite;

        private float startY;
        private bool hasLostWings = false;

        protected override void Awake()
        {
            base.Awake();
            startY = transform.position.y;
            scoreValue = 300;
        }

        protected override void Update()
        {
            if (isDefeated) return;

            if (!hasLostWings)
            {
                // Wave motion in air
                float newY = startY + Mathf.Sin(Time.time * waveSpeed) * waveHeight;
                float dir = isFacingLeft ? -1f : 1f;
                transform.position = new Vector3(transform.position.x + dir * flySpeed * Time.deltaTime, newY, transform.position.z);
            }
            else
            {
                base.Update();
            }
        }

        public override void OnStomped(MarioController mario)
        {
            if (!hasLostWings)
            {
                // First stomp: lose wings, drop to ground as regular Koopa
                hasLostWings = true;
                rb.gravityScale = 3f;
                if (SoundManager.Instance != null) SoundManager.Instance.PlayStomp();
                if (GameManager.Instance != null) GameManager.Instance.AddScore(200);
            }
            else
            {
                // Second stomp: defeat with flip
                DefeatWithFlip();
            }
        }
    }
}
