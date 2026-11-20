using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEditor;
using TMPro;
using Unity.VisualScripting;

public class GameSequenceManager : MonoBehaviour
{
    public enum EventType
    {
        Dialogue,
        SpawnSlime,
        SetActive,
        CustomEvent
    }

    [Serializable]
    public class SequenceStep
    {
        [Header("Event Settings")]
        public EventType eventType;

        [Header("Dialogue")]
        public DialogueLines lines;

        [Header("Slime Spawning")]
        public int slimePrefabIndex;

        [Header("GameObject Activation")]
        public GameObject targetObject;
        public bool setActive = true;

        [Header("Custom Event")]
        public UnityEvent customEvent;
    }

    [Header("References")]
    public DialogueManager dialogueManager;
    public SlimeSpawner slimeSpawner;

    public List<TMP_Text> ListofTexts;
    public GameObject ResultsPanel;

    private struct results
    {
        public int ShipGood;
        public int ShipBad;
        public int RehabGood;
        public int RehabBad;


    }

    [Header("Sequence Settings")]
    public List<SequenceStep> sequence = new List<SequenceStep>();

    public List<DialogueLines> successLines, failureLines;
    [SerializeField] List<DialogueLines> successLinesUsed = new();
    [SerializeField] List<DialogueLines> failureLinesUsed = new();
    [SerializeField] private int InstructionsFollowed = 0;
    [SerializeField] private int InstructionsNotFollowed = 0;

    private int currentStep = 0;
    [SerializeField] GameObject CustomerWindow;
    [SerializeField] GameObject SlimeButons;
    [SerializeField] bool willRespond;
    [SerializeField] bool willAdvanceSequence = false;
    [SerializeField] public int slimeCount;

    private void OnEnable()
    {
        DialogueLines.OnDialogueComplete += AdvanceSequence;
        SlimePipe.onSlimeEntered += SlimeEntered;
    }

    private void OnDisable()
    {
        DialogueLines.OnDialogueComplete -= AdvanceSequence;
        SlimePipe.onSlimeEntered -= SlimeEntered;
    }

    void Start()
    {
        ResultsPanel.SetActive(false);
        AdvanceSequence();
        InstructionsFollowed = -1;
        InstructionsNotFollowed = -1;
        willAdvanceSequence = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //AdvanceSequence();
        }
    }
    results ResultsTracker = new();
    public void SlimeEntered(bool isCorrect, string pipeExpectancy)
    {
        slimeCount++;
        if (willRespond && isCorrect && pipeExpectancy == "GoodSlime")
        {
            ResultsTracker.ShipGood++;
            InstructionsFollowed++;
        }
        else if (willRespond && !isCorrect && pipeExpectancy == "GoodSlime")
        {
            ResultsTracker.ShipBad++;
            InstructionsNotFollowed++;
        }
        else if (willRespond && isCorrect && pipeExpectancy == "BadSlime")
        {
            ResultsTracker.RehabGood++;
            InstructionsFollowed++;
        }
        else if (willRespond && !isCorrect && pipeExpectancy == "BadSlime")
        {
            ResultsTracker.RehabBad++;
            InstructionsNotFollowed++;
        }



        if (successLines.Count == 0)
        {
            //successLines = successLinesUsed;
            successLines.AddRange(successLinesUsed);
            successLinesUsed.Clear();
        }
        if (failureLines.Count == 0)
        {
            //failureLines = failureLinesUsed;
            failureLines.AddRange(failureLinesUsed);
            failureLinesUsed.Clear();
        }

        if (willRespond && isCorrect && (InstructionsFollowed == 0 || InstructionsFollowed % 2 == 0))
        {
            PraisePlayer();
            Debug.Log("Praise Player");
        }
        else if (willRespond && !isCorrect && (InstructionsNotFollowed == 0 || InstructionsNotFollowed % 2 == 0))
        {
            WarnPlayer();
            Debug.Log("Warn Player");
        }
        else if (willAdvanceSequence)
        {
            AdvanceSequence();
        }
        /*        if (currentStep >= sequence.Count)
                {
                    slimeSpawner.SpawnSlimeRandom();
                }*/
    }

    // PUBLIC FUNCTION - Can also be called by UI buttons.

    public void PraisePlayer()
    {
        dialogueManager.QueueDialogue(successLines[0]);
        successLinesUsed.Add(successLines[0]);
        successLines.RemoveAt(0);
        dialogueManager.AdvanceDialogue();

        if (slimeCount == 1)
        {
            dialogueManager.ShowContinueButton();
            //dialogueManager.skippable = true;
        }
    }

    public void WarnPlayer()
    {

        dialogueManager.QueueDialogue(failureLines[0]);
        failureLinesUsed.Add(failureLines[0]);
        failureLines.RemoveAt(0);
        dialogueManager.AdvanceDialogue();
    }


    public void ShowResults()
    {
        Debug.Log("ResultsShown");
        ResultsPanel.SetActive(true);
        ListofTexts[0].SetText(ResultsTracker.ShipGood.ToString());
        ListofTexts[1].SetText(ResultsTracker.ShipBad.ToString());
        ListofTexts[2].SetText(ResultsTracker.RehabGood.ToString());
        ListofTexts[3].SetText(ResultsTracker.RehabBad.ToString());
        ResultsPanel.SetActive(true);
    }
    public void AdvanceSequence()
    {
        if (currentStep >= sequence.Count)
        {
            Debug.Log("Sequence completed!");
            return;
        }

        SequenceStep step = sequence[currentStep];

        switch (step.eventType)
        {
            case EventType.Dialogue:

                if (dialogueManager != null)
                {
                    //queue the next dialogue
                    dialogueManager.QueueDialogue(step.lines);
                }

                break;

            case EventType.SpawnSlime:

                if (slimeSpawner != null)
                {
                    slimeSpawner.SpawnSlime(
                        step.slimePrefabIndex
                    );
                }

                break;

            case EventType.SetActive:

                if (step.targetObject != null)
                {
                    step.targetObject.SetActive(step.setActive);
                }

                break;

            case EventType.CustomEvent:

                step.customEvent?.Invoke();

                break;
        }

        Debug.Log("Sequence Step Completed: " + (currentStep + 1));

        currentStep++;
    }

    // PUBLIC FUNCTION - Restart the sequence.
    public void RestartSequence()
    {
        currentStep = 0;
    }

    // PUBLIC FUNCTION - Check current progress.
    public int GetCurrentStep()
    {
        return currentStep;
    }

    public void StartCustomers()
    {
        CustomerWindow.SetActive(true);
        InstructionsFollowed = -1;
        InstructionsNotFollowed = -1;
    }
    public void ShowButtons()
    {
        SlimeButons.SetActive(true);
    }

    public void SetResponse(bool Respond)
    {
        willRespond = Respond;
    }
    public void SetAdvanceSequence(bool Advance)
    {
        willAdvanceSequence = Advance;
    }

    


}
