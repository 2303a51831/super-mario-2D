using UnityEngine;

namespace SuperUnity.Player
{
    [RequireComponent(typeof(MarioController))]
    public class MarioAnimation : MonoBehaviour
    {
        private MarioController controller;
        private SpriteRenderer spriteRenderer;

        [Header("Small Mario Sprites")]
        public Sprite smallIdle;
        public Sprite[] smallRun;
        public Sprite smallJump;
        public Sprite smallSkid;
        public Sprite smallDie;

        [Header("Super Mario Sprites")]
        public Sprite superIdle;
        public Sprite[] superRun;
        public Sprite superJump;
        public Sprite superSkid;
        public Sprite superCrouch;

        [Header("Fire Mario Sprites (Can tint or custom)")]
        public Sprite fireIdle;
        public Sprite[] fireRun;
        public Sprite fireJump;
        public Sprite fireSkid;
        public Sprite fireCrouch;

        [Header("Animation Settings")]
        public float animationSpeedMultiplier = 2.5f;
        private float frameTimer = 0f;
        private int currentFrame = 0;

        private void Awake()
        {
            controller = GetComponent<MarioController>();
            spriteRenderer = controller.spriteRenderer;
        }

        private void LateUpdate()
        {
            if (spriteRenderer == null) return;

            // Handle facing direction
            spriteRenderer.flipX = !controller.IsFacingRight;

            if (!controller.isInvincible && controller.currentState != MarioState.Dead)
            {
                switch (controller.activeModel)
                {
                    case CharacterModel.Luigi:
                        spriteRenderer.color = new Color(0.25f, 1f, 0.45f); // Emerald Luigi tint
                        break;
                    case CharacterModel.FireKnight:
                        spriteRenderer.color = new Color(1f, 0.4f, 0.2f); // Fiery Crimson Knight
                        break;
                    case CharacterModel.CyberNeon:
                        float glow = 0.82f + Mathf.PingPong(Time.time * 5f, 0.28f);
                        spriteRenderer.color = new Color(0.1f * glow, 0.95f * glow, 1f * glow);
                        break;
                    default:
                        if (controller.currentState == MarioState.Fire && fireIdle == null)
                        {
                            spriteRenderer.color = new Color(1f, 0.95f, 0.7f); // Fire palette glow
                        }
                        else
                        {
                            spriteRenderer.color = Color.white;
                        }
                        break;
                }
            }

            // Determine appropriate sprite based on current action & state
            spriteRenderer.sprite = GetCurrentSprite();
        }

        private Sprite GetCurrentSprite()
        {
            if (controller.IsDead)
            {
                return smallDie != null ? smallDie : smallIdle;
            }

            MarioState state = controller.currentState;
            bool grounded = controller.isGrounded;
            bool skidding = controller.IsSkidding;
            bool crouching = controller.IsCrouching;
            float speedX = Mathf.Abs(controller.Rb.linearVelocity.x);

            switch (state)
            {
                case MarioState.Small:
                    if (!grounded) return smallJump != null ? smallJump : smallIdle;
                    if (skidding) return smallSkid != null ? smallSkid : smallIdle;
                    if (speedX > 0.2f) return GetAnimatedRunSprite(smallRun, smallIdle, speedX);
                    return smallIdle;

                case MarioState.Super:
                    if (crouching) return superCrouch != null ? superCrouch : superIdle;
                    if (!grounded) return superJump != null ? superJump : superIdle;
                    if (skidding) return superSkid != null ? superSkid : superIdle;
                    if (speedX > 0.2f) return GetAnimatedRunSprite(superRun, superIdle, speedX);
                    return superIdle;

                case MarioState.Fire:
                    if (fireIdle != null)
                    {
                        if (crouching) return fireCrouch != null ? fireCrouch : fireIdle;
                        if (!grounded) return fireJump != null ? fireJump : fireIdle;
                        if (skidding) return fireSkid != null ? fireSkid : fireIdle;
                        if (speedX > 0.2f) return GetAnimatedRunSprite(fireRun, fireIdle, speedX);
                        return fireIdle;
                    }
                    else
                    {
                        // Fallback to super sprites with tint
                        if (crouching) return superCrouch != null ? superCrouch : superIdle;
                        if (!grounded) return superJump != null ? superJump : superIdle;
                        if (skidding) return superSkid != null ? superSkid : superIdle;
                        if (speedX > 0.2f) return GetAnimatedRunSprite(superRun, superIdle, speedX);
                        return superIdle;
                    }

                default:
                    return smallIdle;
            }
        }

        private Sprite GetAnimatedRunSprite(Sprite[] frames, Sprite fallback, float speed)
        {
            if (frames == null || frames.Length == 0) return fallback;

            frameTimer += Time.deltaTime * speed * animationSpeedMultiplier;
            if (frameTimer >= 1.0f)
            {
                frameTimer = 0f;
                currentFrame = (currentFrame + 1) % frames.Length;
            }

            return frames[currentFrame] != null ? frames[currentFrame] : fallback;
        }
    }
}
