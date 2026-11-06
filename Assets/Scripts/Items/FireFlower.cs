using UnityEngine;
using SuperUnity.Player;

namespace SuperUnity.Items
{
    [RequireComponent(typeof(Collider2D))]
    public class FireFlower : MonoBehaviour
    {
        public int scoreValue = 1000;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            MarioController mario = collision.GetComponent<MarioController>();
            if (mario != null)
            {
                mario.GetFireFlower();
                if (GameManager.Instance != null) GameManager.Instance.AddScore(scoreValue);
                Destroy(gameObject);
            }
        }
    }
}
