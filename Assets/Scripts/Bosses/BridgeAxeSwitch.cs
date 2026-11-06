using UnityEngine;
using SuperUnity.Player;
using SuperUnity.Core;

namespace SuperUnity.Bosses
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class BridgeAxeSwitch : MonoBehaviour
    {
        public World4FinalBoss boss;
        private bool hasTriggered = false;

        private void Awake()
        {
            var col = GetComponent<BoxCollider2D>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (hasTriggered) return;

            MarioController mario = collision.GetComponent<MarioController>();
            if (mario != null && !mario.IsDead)
            {
                hasTriggered = true;
                if (boss != null)
                {
                    boss.TriggerBridgeAxeCollapse();
                }
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayBrickBreak();
                }
                Destroy(gameObject);
            }
        }
    }
}
