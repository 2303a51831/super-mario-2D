using System.Collections;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    public Transform connection;
    public KeyCode enterKeyCode = KeyCode.S;
    public Vector3 enterDirection = Vector3.down;
    public Vector3 exitDirection = Vector3.zero;

    [Header("Level Exit Settings")]
    public bool isLevelExit = false;
    public int nextWorld = 1;
    public int nextStage = 3;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out Player player))
        {
            if (isLevelExit)
            {
                if (Input.GetKey(enterKeyCode)) {
                    StartCoroutine(EnterExitPipe(player));
                }
            }
            else if (connection != null)
            {
                if (Input.GetKey(enterKeyCode)) {
                    StartCoroutine(Enter(player));
                }
            }
        }
    }

    private IEnumerator EnterExitPipe(Player player)
    {
        player.movement.enabled = false;

        Vector3 enteredPosition = transform.position + enterDirection;
        Vector3 enteredScale = Vector3.one * 0.5f;

        yield return Move(player.transform, enteredPosition, enteredScale);
        yield return new WaitForSeconds(1f);

        if (GameManager.Instance != null) {
            int timeBonus = Mathf.CeilToInt(GameManager.Instance.timeRemaining) * 50;
            GameManager.Instance.AddScore(1000 + timeBonus);
            GameManager.Instance.LoadLevel(nextWorld, nextStage);
        }
    }

    private IEnumerator Enter(Player player)
    {
        player.movement.enabled = false;

        Vector3 enteredPosition = transform.position + enterDirection;
        Vector3 enteredScale = Vector3.one * 0.5f;

        yield return Move(player.transform, enteredPosition, enteredScale);
        yield return new WaitForSeconds(1f);

        var sideScrolling = Camera.main != null ? Camera.main.GetComponent<SideScrollingCamera>() : null;
        if (sideScrolling != null && connection != null) {
            sideScrolling.SetUnderground(connection.position.y < sideScrolling.undergroundThreshold);
        }

        if (exitDirection != Vector3.zero && connection != null)
        {
            player.transform.position = connection.position - exitDirection;
            yield return Move(player.transform, connection.position + exitDirection, Vector3.one);
        }
        else if (connection != null)
        {
            player.transform.position = connection.position;
            player.transform.localScale = Vector3.one;
        }

        player.movement.enabled = true;
    }

    private IEnumerator Move(Transform player, Vector3 endPosition, Vector3 endScale)
    {
        float elapsed = 0f;
        float duration = 1f;

        Vector3 startPosition = player.position;
        Vector3 startScale = player.localScale;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            player.position = Vector3.Lerp(startPosition, endPosition, t);
            player.localScale = Vector3.Lerp(startScale, endScale, t);
            elapsed += Time.deltaTime;

            yield return null;
        }

        player.position = endPosition;
        player.localScale = endScale;
    }

}
