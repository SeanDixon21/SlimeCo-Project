using System.Collections;
using TMPro;
using UnityEngine;
using static DialogueLines;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public TMP_Text dialogueText;

    public DialogueLines currentDialogue;
    private int currentLine = 0;
    [SerializeField] private bool skippable;
    [SerializeField] private bool finishTyping = false;
    Coroutine C_Typer;
    public AudioSource source;
    public GameObject ContinueButton;

    // PUBLIC FUNCTION - Can be called by UI buttons.
    public void QueueDialogue(DialogueLines lines)
    {
        if (C_Typer != null) 
        {
            StopCoroutine(C_Typer);
            C_Typer = null;
        }
        dialogueText.text = "";
        currentDialogue = lines;
        currentLine = -1;
        AdvanceDialogue();
    }

    public void AdvanceDialogue()
    {
        ContinueButton.SetActive(false);
        skippable = false;
        if (currentDialogue == null) return;
        currentLine++;
        //advance currentdialogue to the next line here

        if (C_Typer != null)
        {
            return;
        }
        if (currentDialogue.lines.Length <= currentLine)
        {
            EndDialogue();
            return;
        }
        dialogueText.text = "";
        C_Typer = StartCoroutine(Typer(currentDialogue.lines[currentLine]));
    }

    IEnumerator Typer(LineLine line)
    {
        int i = 0;
        finishTyping = false;

        if (line.audio != null)
        {
            source.Stop();
            source.PlayOneShot(line.audio);
        }

        while (i < line.text.Length)
        {
            if (finishTyping)
            {
                dialogueText.text = line.text;
                break;
            }

            char c = line.text[i];
            dialogueText.text += c;

            yield return new WaitForSeconds(0.02f);
            i++;
        }

        C_Typer = null;
        skippable = true;
        ContinueButton.SetActive(true);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (C_Typer != null)
            {
                finishTyping = true;
            }
            else if (skippable)
            {
                AdvanceDialogue();
            }
        }
    }

    public void EndDialogue()
    {
        currentDialogue.Complete();
        currentDialogue = null;
        currentLine = 0;
    }
}
