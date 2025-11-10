using UnityEngine;

public class VictimController : MonoBehaviour
{
    public GameObject echoEffect;
    public AudioClip echoSound;

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void TriggerEchoScan()
    {
        StartCoroutine(PlayEchoSequence());
    }

    private System.Collections.IEnumerator PlayEchoSequence()
    {
        echoEffect.SetActive(true);
        AudioSource.PlayClipAtPoint(echoSound, transform.position);

        yield return new WaitForSeconds(2f);

        // Show echo dialogue
        UIManager.Instance.ShowEchoDialogue(
            "Wait, why is security pushing so hard? Someone's shoving from behind—I'm losing balance—the railing—"
        );

        yield return new WaitForSeconds(3f);
        echoEffect.SetActive(false);
    }
}