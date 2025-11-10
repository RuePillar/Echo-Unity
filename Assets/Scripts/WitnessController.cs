using UnityEngine;

public class WitnessController : MonoBehaviour
{
    public enum WitnessType { James, Sarah }
    public WitnessType witnessType;

    public Animator animator;
    private bool hasBeenInterviewed = false;

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void Interview()
    {
        hasBeenInterviewed = true;

        switch (witnessType)
        {
            case WitnessType.James:
                PlayJamesTestimony();
                break;
            case WitnessType.Sarah:
                PlaySarahTestimony();
                break;
        }
    }

    private void PlayJamesTestimony()
    {
        animator.SetTrigger("Talk");
        string dialogue = "I was right next to him. The pavement was wet. He slipped, I'm sure of it!";
        UIManager.Instance.ShowWitnessDialogue("James", dialogue);

        // FIX: Use GameManager.Instance instead of static access
        if (GameManager.Instance != null)
        {
            GameManager.Instance.witness_james_credible = true;
        }
    }

    private void PlaySarahTestimony()
    {
        animator.SetTrigger("TalkAngry");
        string dialogue = "The corporate security guard pushed him! Number 247! This was murder!";
        UIManager.Instance.ShowWitnessDialogue("Sarah", dialogue);

        // FIX: Use GameManager.Instance instead of static access
        if (GameManager.Instance != null)
        {
            GameManager.Instance.witness_sarah_believable = true;
        }
    }

    public void ReactToVerdict(Verdict verdict)
    {
        switch (verdict)
        {
            case Verdict.Accident:
                if (witnessType == WitnessType.James)
                    animator.SetTrigger("Relieved");
                else
                    animator.SetTrigger("Angry");
                break;
            case Verdict.Murder:
                if (witnessType == WitnessType.Sarah)
                    animator.SetTrigger("Relieved");
                else
                    animator.SetTrigger("Worried");
                break;
        }
    }
}