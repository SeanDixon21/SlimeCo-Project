using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections;
using static DialogueLines;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public TMP_Text dialogueText;

    [TextArea(2, 5)]
    public DialogueLines currentDialogue;
    private int currentLine = 0;
    Coroutine C_Typer;
    public AudioSource source;

    // PUBLIC FUNCTION - Can be called by UI buttons.
    public void QueueDialogue(DialogueLines lines)
    {
        dialogueText.text = "";
        currentDialogue = lines;
        currentLine = -1;
        AdvanceDialogue();
    }

    public void AdvanceDialogue()
    {
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
        //play audio here
        if (line.audio != null) source.PlayOneShot(line.audio);
        while (i < line.text.Length)
        {
            char c = line.text[i];
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f);
            i++;
        }
        C_Typer = null;
    }

    public void EndDialogue()
    {
        dialogueText.text = "";
        currentDialogue.Complete();
        currentDialogue = null;
        currentLine = 0;
    }
}
