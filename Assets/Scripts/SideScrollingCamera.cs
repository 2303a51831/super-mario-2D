using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SideScrollingCamera : MonoBehaviour
{
    public Transform trackedObject;
    public float height = 6.5f;
    public float undergroundHeight = -9.5f;
    public float undergroundThreshold = 0f;

    private void Awake()
    {
        FindTrackedObject();
    }

    private void FindTrackedObject()
    {
        if (trackedObject == null) {
            Player player = FindObjectOfType<Player>();
            if (player != null) {
                trackedObject = player.transform;
            }
        }
    }

    private void LateUpdate()
    {
        if (trackedObject == null) {
            FindTrackedObject();
            if (trackedObject == null) return;
        }

        Vector3 cameraPosition = transform.position;
        cameraPosition.x = Mathf.Max(cameraPosition.x, trackedObject.position.x);
        transform.position = cameraPosition;
    }

    public void SetUnderground(bool underground)
    {
        Vector3 cameraPosition = transform.position;
        cameraPosition.y = underground ? undergroundHeight : height;
        transform.position = cameraPosition;
    }

}
