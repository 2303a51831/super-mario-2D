using UnityEngine;
using System.Collections;

namespace SuperUnity.Items
{
    public class CoinEffect : MonoBehaviour
    {
        public float popHeight = 2.0f;
        public float duration = 0.5f;

        private void Start()
        {
            StartCoroutine(AnimateCoin());
        }

        private IEnumerator AnimateCoin()
        {
            Vector3 startPos = transform.position;
            Vector3 peakPos = startPos + Vector3.up * popHeight;
            float elapsed = 0f;

            while (elapsed < duration * 0.5f)
            {
                transform.position = Vector3.Lerp(startPos, peakPos, elapsed / (duration * 0.5f));
                elapsed += Time.deltaTime;
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < duration * 0.5f)
            {
                transform.position = Vector3.Lerp(peakPos, startPos, elapsed / (duration * 0.5f));
                elapsed += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
