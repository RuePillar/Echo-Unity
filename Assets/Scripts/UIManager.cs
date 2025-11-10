using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Panels")]
    public GameObject dialoguePanel;
    public GameObject choicePanel;
    public GameObject verdictPanel;

    [Header("Text Elements")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerText;
    public Button[] choiceButtons;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Hide choice panel initially until we're ready to show choices
        choicePanel.SetActive(false);
    }

    public void ShowDialogue(string text, string speaker = "")
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = text;
        speakerText.text = speaker;
    }

    public void ShowDetectiveDialogue(string dialogue)
    {
        ShowDialogue(dialogue, "Detective");
    }

    public void ShowWitnessDialogue(string witnessName, string dialogue)
    {
        ShowDialogue(dialogue, witnessName);
    }

    public void ShowEchoDialogue(string echoText)
    {
        ShowDialogue(echoText, "Alex (Echo)");
    }

    public void ShowChoices(string[] choices)
    {
        Debug.Log($"Showing {choices.Length} choices"); // For debugging

        // Hide dialogue temporarily to focus on choices, or keep it visible
        // dialoguePanel.SetActive(false);
        choicePanel.SetActive(true);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                TextMeshProUGUI buttonText = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = choices[i];
                }

                // IMPORTANT: Set up button click event
                int choiceIndex = i; // Capture the index for the closure
                choiceButtons[i].onClick.RemoveAllListeners(); // Clear previous listeners
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(choiceIndex));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void ShowWitnessSelection()
    {
        string[] choices = {
            "Interview James (claims accident)",
            "Interview Sarah (claims murder)",
            "Review security footage",
            "Make final report"
        };
        ShowChoices(choices);
    }

    public void ShowEchoInterpretationOptions()
    {
        string[] choices = {
            "The push felt intentional",
            "He simply lost his balance",
            "There was hesitation... like he jumped"
        };
        ShowChoices(choices);
    }

    public void ShowVerdictOptions()
    {
        string[] choices = {
            "ACCIDENT: Slipped in crowd chaos",
            "MURDER: Pushed by security",
            "SUICIDE: Intentional jump",
            "INCONCLUSIVE: Need more time"
        };
        ShowChoices(choices);
    }

    // Button event handler - UPDATED WITH MAPPING LOGIC
    public void OnChoiceSelected(int choiceIndex)
    {
        Debug.Log($"Button {choiceIndex} clicked!"); // For debugging

        choicePanel.SetActive(false);
        dialoguePanel.SetActive(true); // Show dialogue panel again

        if (GameManager.Instance != null)
        {
            // Map button index to correct PlayerChoice based on current context
            PlayerChoice choice = MapChoiceIndexToEnum(choiceIndex);
            GameManager.Instance.ProcessPlayerChoice(choice);
        }
        else
        {
            Debug.LogError("GameManager.Instance is null!");
        }
    }

    // ADD THIS METHOD: Map button indices to correct enum values
    private PlayerChoice MapChoiceIndexToEnum(int choiceIndex)
    {
        InvestigationState currentState = GameManager.Instance.GetCurrentState();

        Debug.Log($"Mapping choice index {choiceIndex} in state {currentState}");

        switch (currentState)
        {
            case InvestigationState.Start:
                switch (choiceIndex)
                {
                    case 0: return PlayerChoice.ScanVictim;
                    case 1: return PlayerChoice.InterviewWitnesses;
                    case 2: return PlayerChoice.ExamineScene;
                    default:
                        Debug.LogWarning($"Unknown choice index {choiceIndex} for Start state");
                        return PlayerChoice.ScanVictim;
                }

            case InvestigationState.WitnessInterview:
                switch (choiceIndex)
                {
                    case 0: return PlayerChoice.InterviewJames;
                    case 1: return PlayerChoice.InterviewSarah;
                    case 2: return PlayerChoice.ReviewSecurityFootage;
                    case 3: return PlayerChoice.MakeFinalReport;
                    default:
                        Debug.LogWarning($"Unknown choice index {choiceIndex} for WitnessInterview state");
                        return PlayerChoice.InterviewJames;
                }

            case InvestigationState.Scanning:
                switch (choiceIndex)
                {
                    case 0: return PlayerChoice.InterpretMurder;
                    case 1: return PlayerChoice.InterpretAccident;
                    case 2: return PlayerChoice.InterpretSuicide;
                    default:
                        Debug.LogWarning($"Unknown choice index {choiceIndex} for Scanning state");
                        return PlayerChoice.InterpretMurder;
                }

            case InvestigationState.FinalReport:
                switch (choiceIndex)
                {
                    case 0: return PlayerChoice.SubmitAccident;
                    case 1: return PlayerChoice.SubmitMurder;
                    case 2: return PlayerChoice.SubmitSuicide;
                    case 3: return PlayerChoice.RequestExtension;
                    default:
                        Debug.LogWarning($"Unknown choice index {choiceIndex} for FinalReport state");
                        return PlayerChoice.SubmitAccident;
                }

            default:
                Debug.LogWarning($"Unknown state {currentState}");
                return PlayerChoice.ScanVictim;
        }
    }

    public void HideAllPanels()
    {
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        verdictPanel.SetActive(false);
    }
}