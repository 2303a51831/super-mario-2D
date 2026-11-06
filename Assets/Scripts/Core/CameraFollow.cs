using UnityEngine;
using SuperUnity.Player;

namespace SuperUnity.Core
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target & Tracking")]
        public Transform target;
        public float smoothSpeed = 18.0f;
        public Vector3 offset = new Vector3(2.5f, 0f, -10f);

        [Header("Camera Constraints")]
        public bool allowScrollBackwards = false;
        public float minX = 0f;
        public float maxX = 210f;
        public float fixedY = 6.0f;

        private float highestX = 0f;

        private void Start()
        {
            if (target == null)
            {
                var mario = FindAnyObjectByType<MarioController>();
                if (mario != null) target = mario.transform;
            }

            highestX = transform.position.x;
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                var mario = FindAnyObjectByType<MarioController>();
                if (mario != null) target = mario.transform;
                else return;
            }

            float targetX = target.position.x + offset.x;

            if (!allowScrollBackwards)
            {
                if (targetX < highestX)
                {
                    targetX = highestX;
                }
                else
                {
                    highestX = targetX;
                }
            }

            targetX = Mathf.Clamp(targetX, minX, maxX);

            Vector3 desiredPosition = new Vector3(targetX, fixedY, offset.z);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }

        public void SnapToTarget(Vector3 targetPos)
        {
            float targetX = Mathf.Clamp(targetPos.x + offset.x, minX, maxX);
            highestX = targetX;
            transform.position = new Vector3(targetX, fixedY, offset.z);
        }
    }
}
