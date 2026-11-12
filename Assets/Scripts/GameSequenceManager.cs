using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

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
        AdvanceSequence();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceSequence();
        }
    }

    public void SlimeEntered(bool isCorrect)
    {
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
