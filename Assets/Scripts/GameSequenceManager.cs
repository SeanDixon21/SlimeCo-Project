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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceSequence();
        }
    }

    public void SlimeEntered(bool isCorrect)
    {
        if (currentStep < sequence.Count)
        {
            //proceed sequence as usual
            AdvanceSequence();
            return;
        }

        if (isCorrect)
        {
            //play good line
            dialogueManager.QueueDialogue(successLines[UnityEngine.Random.Range(0, successLines.Count)]);
        }
        else
        {
            //play bad line
            dialogueManager.QueueDialogue(failureLines[UnityEngine.Random.Range(0, failureLines.Count)]);
        }
    }

    public void AdvanceSequence(string s)
    {
        AdvanceSequence();
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
