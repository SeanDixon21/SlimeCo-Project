using System;
using UnityEngine;

[CreateAssetMenu(menuName = "New Dialogue")]
public class DialogueLines : ScriptableObject
{
    public LineLine[] lines;

    public static event Action OnDialogueComplete;

    public void Complete()
    {
        OnDialogueComplete?.Invoke();
    }

    [Serializable]
    public struct LineLine
    {
        [TextArea(2,4)]
        public string text;
        public AudioClip audio;
    }
}
