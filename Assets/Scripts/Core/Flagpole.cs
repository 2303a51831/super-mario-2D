using UnityEngine;
using System.Collections;
using SuperUnity.Player;

namespace SuperUnity.Core
{
    public class Flagpole : MonoBehaviour
    {
        [Header("Components")]
        public Transform poleTop;
        public Transform poleBottom;
        public Transform flag;
        public Transform castleDoor;

        [Header("Settings")]
        public float flagLowerSpeed = 12.0f;
        private bool hasTriggered = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (hasTriggered) return;

            MarioController mario = collision.GetComponent<MarioController>();
            if (mario != null && !mario.IsDead)
            {
                hasTriggered = true;
                StartCoroutine(FlagSequenceCoroutine(mario));
            }
        }

        private IEnumerator FlagSequenceCoroutine(MarioController mario)
        {
            // Calculate height bonus based on where Mario grabbed the pole
            float poleHeight = (poleTop != null && poleBottom != null) ? (poleTop.position.y - poleBottom.position.y) : 8.0f;
            float touchY = mario.transform.position.y - (poleBottom != null ? poleBottom.position.y : transform.position.y);
            float ratio = Mathf.Clamp01(touchY / poleHeight);

            int bonus = 100;
            if (ratio > 0.8f) bonus = 5000;
            else if (ratio > 0.6f) bonus = 2000;
            else if (ratio > 0.4f) bonus = 800;
            else if (ratio > 0.2f) bonus = 400;

            // Start lowering the flag
            StartCoroutine(LowerFlagCoroutine());

            // Slide Mario down
            bool slideDone = false;
            mario.SlideDownFlagpole(poleBottom != null ? poleBottom : transform, () => {
                slideDone = true;
            });

            while (!slideDone)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.15f);

            // Play stage clear music
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayStageClear();
            }

            // Mario walks to castle
            if (castleDoor != null)
            {
                bool reachedCastle = false;
                mario.WalkToCastle(castleDoor, () => {
                    reachedCastle = true;
                });

                while (!reachedCastle)
                {
                    yield return null;
                }
            }

            yield return new WaitForSeconds(0.3f);

            // Notify GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReachGoal(bonus);
                // Display Stage Clear modal with stars and score; auto-advance after fanfare if player hasn't clicked
                yield return new WaitForSeconds(3.5f);
                if (GameManager.Instance.isLevelClear)
                {
                    GameManager.Instance.NextLevel();
                }
            }
        }

        private IEnumerator LowerFlagCoroutine()
        {
            if (flag == null) yield break;

            float targetY = (poleBottom != null ? poleBottom.position.y : transform.position.y) + 1.0f;
            while (flag.position.y > targetY)
            {
                flag.position = Vector3.MoveTowards(flag.position, new Vector3(flag.position.x, targetY, flag.position.z), flagLowerSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
