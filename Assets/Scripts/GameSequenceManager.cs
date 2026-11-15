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

    private int currentStep = 0;

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
        if (isCorrect && pipeExpectancy == "GoodSlime")
        {
            ResultsTracker.ShipGood++;
            Debug.Log(isCorrect);
            Debug.Log(pipeExpectancy);
            Debug.Log(ResultsTracker.ShipGood);
        }
        else if (!isCorrect && pipeExpectancy == "GoodSlime")
        {
            ResultsTracker.ShipBad++;
            Debug.Log(ResultsTracker.ShipBad);
        }
        else if (isCorrect && pipeExpectancy == "BadSlime")
        {
            ResultsTracker.RehabGood++;
            Debug.Log(ResultsTracker.RehabGood);
        }
        else if (!isCorrect && pipeExpectancy == "BadSlime")
        {
            ResultsTracker.RehabBad++;
            Debug.Log(ResultsTracker.RehabBad);
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

        if (isCorrect)
        {
            //play good line
            int index = UnityEngine.Random.Range(0, successLines.Count);
            dialogueManager.QueueDialogue(successLines[index]);
            successLinesUsed.Add(successLines[index]);
            successLines.RemoveAt(index);

        }
        else
        {
            //play bad line
            int index = UnityEngine.Random.Range(0, failureLines.Count);
            dialogueManager.QueueDialogue(failureLines[index]);
            failureLinesUsed.Add(failureLines[index]);
            failureLines.RemoveAt(index);
        }
/*        if (currentStep >= sequence.Count)
        {
            slimeSpawner.SpawnSlimeRandom();
        }*/
    }

    // PUBLIC FUNCTION - Can also be called by UI buttons.




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
}
