using UnityEngine;
using SuperUnity.Player;

namespace SuperUnity.Core
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DeathPit : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            MarioController mario = collision.GetComponent<MarioController>();
            if (mario != null && !mario.IsDead)
            {
                mario.Die();
            }
            else
            {
                // Destroy fallen enemies or items
                Destroy(collision.gameObject);
            }
        }
    }
}
