using System.Collections;
using TMPro;
using UnityEngine;
using static DialogueLines;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public TMP_Text dialogueText;

    public DialogueLines currentDialogue;
    [SerializeField]private int currentLine = 0;
    [SerializeField] private bool skippable;
    [SerializeField] private bool finishTyping = false;
    Coroutine C_Typer;
    public AudioSource source;
    public GameObject ContinueButton;
    [SerializeField] bool willAdvanceDialogue;

    // PUBLIC FUNCTION - Can be called by UI buttons.

    private void Start()
    {
        willAdvanceDialogue = true;
    }
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
        Debug.Log("AdvancingDialogue");
        AdvanceDialogue();


    }

    public void AdvanceDialogue()
    {
        if (willAdvanceDialogue)
        {
            ContinueButton.SetActive(false);
            skippable = false;
            if (currentDialogue == null)
            {
                Debug.Log("currentDialogue is null");
                return;
            } 
            currentLine++;
            Debug.Log("Increased Current Line +1");
            //advance currentdialogue to the next line here

            if (C_Typer != null)
            {
                return;
            }
            if (currentDialogue.lines.Length <= currentLine)
            {
                Debug.Log("This Dialogue is finished");
                EndDialogue();
                return;
            }
            dialogueText.text = "";
            C_Typer = StartCoroutine(Typer(currentDialogue.lines[currentLine]));
        }

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
        if (line.showContinueButton)
        {
            ContinueButton.SetActive(true);
        }
        else
        {
            skippable = false;
        }
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
        //ERROR - THIS TRIGGERS THE NEXT SEQUENCE SOME HOW WHICH CAUSES THE CURRENT DIALOGUE TO SET NULL AND NEW DIALOGUE TO PLAY JUST BEFORE SETTING NULL
        //currentDialogue = null;
        if (willAdvanceDialogue)
        {
            currentDialogue.Complete();
        }
        currentLine = 0;
    }
    public void SetAdvanceDialogue(bool Advance)
    {
        willAdvanceDialogue = Advance;
    }
}
