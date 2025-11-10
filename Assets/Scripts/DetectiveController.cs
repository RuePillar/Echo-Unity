using UnityEngine;
using System.Collections; // Add this missing using directive

public class DetectiveController : MonoBehaviour
{
    public Animator animator;
    public GameObject scannerTool; // Change from Transform to GameObject for easier activation

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void PlayScanAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Scan");
            StartCoroutine(EnableScanner());
        }
        else
        {
            Debug.LogWarning("Animator not assigned in DetectiveController");
            // Fallback: just enable scanner without animation
            StartCoroutine(EnableScanner());
        }
    }

    private System.Collections.IEnumerator EnableScanner()
    {
        if (scannerTool != null)
        {
            scannerTool.SetActive(true);
            yield return new WaitForSeconds(3f);
            scannerTool.SetActive(false);
        }
        else
        {
            Debug.LogWarning("ScannerTool not assigned in DetectiveController");
        }
    }

    public void Speak(string dialogue)
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowDetectiveDialogue(dialogue);
        }
        else
        {
            Debug.LogWarning("UIManager.Instance is null");
        }
    }
}