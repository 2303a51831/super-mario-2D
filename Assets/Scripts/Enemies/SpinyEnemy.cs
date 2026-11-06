using UnityEngine;
using SuperUnity.Player;

namespace SuperUnity.Enemies
{
    public class SpinyEnemy : EnemyBase
    {
        protected override void Awake()
        {
            base.Awake();
            scoreValue = 200;
        }

        public override void OnStomped(MarioController mario)
        {
            // Spiny cannot be stomped - hurts Mario unless Mario is invincible!
            if (mario != null && mario.isInvincible)
            {
                DefeatWithFlip();
            }
            else if (mario != null)
            {
                mario.TakeDamage();
            }
        }
    }
}
