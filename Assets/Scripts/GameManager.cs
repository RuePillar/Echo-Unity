using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Evidence state variables
    public bool evidence_murder = false;
    public bool evidence_accident = false;
    public bool evidence_suicide = false;
    public bool ethics_bypassed = false;
    public bool witness_james_credible = false;
    public bool witness_sarah_believable = false;

    // Scene references (assign in Inspector)
    public DetectiveController detective;
    public WitnessController james;
    public WitnessController sarah;
    public VictimController victim;
    public UIManager uiManager;

    private InvestigationState currentState = InvestigationState.Start;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InitializeScene();
    }

    void InitializeScene()
    {
        // Show initial Captain Reyes dialogue
        uiManager.ShowDialogue(
            "Echo Agent. About time. We've got a protester dead - Alex Chen, 24. Corporate security says it was an accident. The crowd says otherwise. Scan what you need, but be careful. This area's politically charged.",
            "Captain Reyes"
        );

        // Position the detective and make them do something
        if (detective != null)
        {
            detective.SetPosition(new Vector3(0, 0, 0)); // Set appropriate position
            detective.Speak("I'm on the scene. Let me start investigating.");
        }
        else
        {
            Debug.LogWarning("Detective reference not set in GameManager!");
        }

        // Position witnesses
        if (james != null)
        {
            james.SetPosition(new Vector3(2, 0, 0)); // Position James to the right
        }
        if (sarah != null)
        {
            sarah.SetPosition(new Vector3(-2, 0, 0)); // Position Sarah to the left
        }

        // Show choices after a short delay so player can read the dialogue
        StartCoroutine(ShowInitialChoicesAfterDelay());
    }

    IEnumerator ShowInitialChoicesAfterDelay()
    {
        yield return new WaitForSeconds(2f); // Wait 2 seconds for player to read
        ShowInitialChoices();
    }

    void ShowInitialChoices()
    {
        string[] initialChoices = {
            "Scan the victim's echo",
            "Interview witnesses",
            "Examine the scene physically"
        };
        uiManager.ShowChoices(initialChoices);
    }

    public void ProcessPlayerChoice(PlayerChoice choice)
    {
        switch (currentState)
        {
            case InvestigationState.Start:
                HandleStartChoice(choice);
                break;
            case InvestigationState.Scanning:
                HandleScanChoice(choice);
                break;
            case InvestigationState.WitnessInterview:
                HandleWitnessChoice(choice);
                break;
            case InvestigationState.FinalReport:
                HandleVerdictChoice(choice);
                break;
        }
    }

    void HandleStartChoice(PlayerChoice choice)
    {
        switch (choice)
        {
            case PlayerChoice.ScanVictim:
                currentState = InvestigationState.Scanning;

                // Make detective perform scan animation
                if (detective != null)
                {
                    detective.PlayScanAnimation();
                    detective.Speak("Scanning victim's echo...");
                }
                else
                {
                    uiManager.ShowDialogue("Scanning victim's echo...");
                }

                StartCoroutine(ShowEchoResults());
                break;

            case PlayerChoice.InterviewWitnesses:
                currentState = InvestigationState.WitnessInterview;
                uiManager.ShowWitnessSelection();
                break;

            case PlayerChoice.ExamineScene:
                // Make detective examine the scene
                if (detective != null)
                {
                    detective.Speak("Examining the scene... the railing shows fresh scuff marks.");
                }
                else
                {
                    uiManager.ShowDialogue("You examine the scene... the railing shows fresh scuff marks.");
                }
                // After examining, show choices again
                ShowInitialChoices();
                break;
        }
    }

    void HandleScanChoice(PlayerChoice choice)
    {
        switch (choice)
        {
            case PlayerChoice.InterpretMurder:
                evidence_murder = true;
                uiManager.ShowDialogue("You interpret the echo as intentional murder.");
                currentState = InvestigationState.WitnessInterview;
                uiManager.ShowWitnessSelection();
                break;
            case PlayerChoice.InterpretAccident:
                evidence_accident = true;
                uiManager.ShowDialogue("You interpret the echo as a tragic accident.");
                currentState = InvestigationState.WitnessInterview;
                uiManager.ShowWitnessSelection();
                break;
            case PlayerChoice.InterpretSuicide:
                evidence_suicide = true;
                uiManager.ShowDialogue("You interpret the echo as possible suicide.");
                currentState = InvestigationState.WitnessInterview;
                uiManager.ShowWitnessSelection();
                break;
        }
    }

    void HandleWitnessChoice(PlayerChoice choice)
    {
        switch (choice)
        {
            case PlayerChoice.InterviewJames:
                // Interview James
                if (james != null)
                {
                    james.Interview();
                    // Show follow-up options after James testimony
                    StartCoroutine(ShowAfterWitnessOptions());
                }
                else
                {
                    uiManager.ShowDialogue("James is not available for interview.");
                    uiManager.ShowWitnessSelection();
                }
                break;

            case PlayerChoice.InterviewSarah:
                // Interview Sarah
                if (sarah != null)
                {
                    sarah.Interview();
                    // Show follow-up options after Sarah testimony
                    StartCoroutine(ShowAfterWitnessOptions());
                }
                else
                {
                    uiManager.ShowDialogue("Sarah is not available for interview.");
                    uiManager.ShowWitnessSelection();
                }
                break;

            case PlayerChoice.ReviewSecurityFootage:
                // Review security footage
                uiManager.ShowDialogue("Reviewing security footage... The footage is grainy but shows a struggle near the edge.");
                evidence_murder = true;
                StartCoroutine(ShowAfterWitnessOptions());
                break;

            case PlayerChoice.MakeFinalReport:
                // Move to final report
                currentState = InvestigationState.FinalReport;
                uiManager.ShowVerdictOptions();
                break;

            case PlayerChoice.ScanVictim:
                currentState = InvestigationState.Scanning;

                // Make detective perform scan animation
                if (detective != null)
                {
                    detective.PlayScanAnimation();
                    detective.Speak("Scanning victim's echo again...");
                }
                else
                {
                    uiManager.ShowDialogue("Scanning victim's echo...");
                }

                StartCoroutine(ShowEchoResults());
                break;

            default:
                uiManager.ShowDialogue("Interviewing witness...");
                // After witness interview, show choices again
                uiManager.ShowWitnessSelection();
                break;
        }
    }

    void HandleVerdictChoice(PlayerChoice choice)
    {
        Verdict finalVerdict = Verdict.Inconclusive;

        switch (choice)
        {
            case PlayerChoice.SubmitAccident:
                uiManager.ShowDialogue("You file the report: Death by misadventure.");
                finalVerdict = Verdict.Accident;
                break;
            case PlayerChoice.SubmitMurder:
                uiManager.ShowDialogue("You file the report: Homicide by security personnel.");
                finalVerdict = Verdict.Murder;
                break;
            case PlayerChoice.SubmitSuicide:
                uiManager.ShowDialogue("You file the report: Intentional self-termination.");
                finalVerdict = Verdict.Suicide;
                break;
            case PlayerChoice.RequestExtension:
                uiManager.ShowDialogue("Request denied. Make the call now.");
                uiManager.ShowVerdictOptions();
                return; // Don't proceed with verdict reaction
        }

        // Make witnesses react to the verdict
        ReactWitnessesToVerdict(finalVerdict);
    }

    // Make witnesses react to the final verdict
    void ReactWitnessesToVerdict(Verdict verdict)
    {
        if (james != null)
        {
            james.ReactToVerdict(verdict);
        }
        if (sarah != null)
        {
            sarah.ReactToVerdict(verdict);
        }

        // Show final thoughts from detective
        if (detective != null)
        {
            string finalThought = GetDetectiveFinalThought(verdict);
            detective.Speak(finalThought);
        }
    }

    // Get appropriate final thought based on verdict
    string GetDetectiveFinalThought(Verdict verdict)
    {
        switch (verdict)
        {
            case Verdict.Accident:
                return "Case closed. A tragic accident, but at least no foul play was involved.";
            case Verdict.Murder:
                return "Justice will be served. The security guard will face charges.";
            case Verdict.Suicide:
                return "A sad conclusion. The family deserves to know the truth.";
            default:
                return "The case is resolved, but questions remain...";
        }
    }

    // Show options after witness interview
    IEnumerator ShowAfterWitnessOptions()
    {
        yield return new WaitForSeconds(3f); // Wait for player to read witness testimony
        uiManager.ShowWitnessSelection(); // Show witness selection again
    }

    IEnumerator ShowEchoResults()
    {
        yield return new WaitForSeconds(3f); // Wait for scan animation to complete
        uiManager.ShowEchoInterpretationOptions();
    }

    // Allow UIManager to check current state
    public InvestigationState GetCurrentState()
    {
        return currentState;
    }
}