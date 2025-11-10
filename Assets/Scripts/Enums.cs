// Enums.cs
public enum PlayerChoice
{
    // Start choices (0-2)
    ScanVictim = 0,
    InterviewWitnesses = 1,
    ExamineScene = 2,

    // Witness choices (3-6)
    InterviewJames = 3,
    InterviewSarah = 4,
    ReviewSecurityFootage = 5,
    MakeFinalReport = 6,

    // Echo interpretation choices (7-9)
    InterpretMurder = 7,
    InterpretAccident = 8,
    InterpretSuicide = 9,

    // Verdict choices (10-13)
    SubmitAccident = 10,
    SubmitMurder = 11,
    SubmitSuicide = 12,
    RequestExtension = 13
}

public enum InvestigationState
{
    Start,
    Scanning,
    WitnessInterview,
    FinalReport
}

public enum Verdict
{
    Accident,
    Murder,
    Suicide,
    Inconclusive
}