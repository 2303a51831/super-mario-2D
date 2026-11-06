using System.Collections;
using UnityEngine;

public class FlagPole : MonoBehaviour
{
    public Transform flag;
    public Transform poleBottom;
    public Transform castle;
    public float speed = 6f;
    public int nextWorld = 1;
    public int nextStage = 2;

    private bool triggered = false;

    private void Awake()
    {
        ResolveReferences();
    }

    private void ResolveReferences()
    {
        if (flag == null) {
            Transform t = transform.Find("Flag");
            if (t != null) flag = t;
        }

        if (poleBottom == null) {
            Transform t = transform.Find("Bottom") ?? transform.Find("Pole/Bottom");
            if (t != null) poleBottom = t;
        }

        if (castle == null) {
            GameObject castleObj = GameObject.Find("Castle") ?? GameObject.FindWithTag("Finish");
            if (castleObj != null) {
                castle = castleObj.transform;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggered && other.CompareTag("Player") && other.TryGetComponent(out Player player))
        {
            triggered = true;
            ResolveReferences();

            Vector3 bottomPos = (poleBottom != null) ? poleBottom.position : (transform.position + Vector3.down * 4.5f);
            if (flag != null) {
                StartCoroutine(MoveTo(flag, bottomPos));
            }
            StartCoroutine(LevelCompleteSequence(player, bottomPos));
        }
    }

    private IEnumerator LevelCompleteSequence(Player player, Vector3 bottomPos)
    {
        player.movement.enabled = false;

        // 1. Slide down the pole
        yield return MoveTo(player.transform, bottomPos);

        // 2. Step off the pole to the right
        yield return MoveTo(player.transform, player.transform.position + Vector3.right * 1f);
        yield return MoveTo(player.transform, player.transform.position + Vector3.right * 0.5f + Vector3.down * 0.5f);

        // 3. Walk into the castle (or walk forward and disappear if castle is far/absent)
        if (castle != null) {
            yield return MoveTo(player.transform, castle.position);
            player.gameObject.SetActive(false);
        } else {
            yield return MoveTo(player.transform, player.transform.position + Vector3.right * 4f);
            player.gameObject.SetActive(false);
        }

        // 4. Award level clear bonus points
        if (GameManager.Instance != null) {
            int timeBonus = Mathf.CeilToInt(GameManager.Instance.timeRemaining) * 50;
            GameManager.Instance.AddScore(1000 + timeBonus);
        }

        yield return new WaitForSeconds(1.5f);

        // 5. Load next level
        if (GameManager.Instance != null)
        {
            if (nextStage > GameManager.Instance.stage) {
                GameManager.Instance.LoadLevel(nextWorld, nextStage);
            } else {
                GameManager.Instance.NextLevel();
            }
        }
    }

    private IEnumerator MoveTo(Transform subject, Vector3 position)
    {
        if (subject == null) yield break;

        while (Vector3.Distance(subject.position, position) > 0.125f)
        {
            subject.position = Vector3.MoveTowards(subject.position, position, speed * Time.deltaTime);
            yield return null;
        }

        subject.position = position;
    }

}
