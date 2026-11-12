using System;
using UnityEngine;

[CreateAssetMenu(menuName = "New Dialogue")]
public class DialogueLines : ScriptableObject
{
    public LineLine[] lines;

    public string eventID;

    public static event Action<string> OnDialogueComplete;

    public void Complete()
    {
        if (eventID == "") return;
        OnDialogueComplete?.Invoke(eventID);
    }

    [Serializable]
    public struct LineLine
    {
        [TextArea(2,4)]
        public string text;
        public AudioClip audio;
    }
}
