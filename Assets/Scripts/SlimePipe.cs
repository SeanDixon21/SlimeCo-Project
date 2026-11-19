using System;
using UnityEngine;
using UnityEngine.Events;



public class SlimePipe : MonoBehaviour
{

    public UnityEvent PipeTriggerd;
    private enum SlimeTagEnum
    {
        GoodSlime,
        BadSlime,
    }

    [Header("Pipe Settings")]

    [SerializeField] private SlimeTagEnum slimeTag;
    public AudioSource PipeThwop;

    public static event Action<bool, string> onSlimeEntered;
    //public static event Action<bool> recordSlime;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("SlimeTrigger"))
        {
            // Check whether the entering object is a slime.
            SlimeCollision slime = other.GetComponentInParent<SlimeCollision>();

            if (slime == null)
                return;

            // Store the slime GameObject before destroying it.
            GameObject slimeObject = slime.gameObject;

            bool isCorrectSlime = other.gameObject.CompareTag(slimeTag.ToString());
            

            // Trigger the custom event.
            onSlimeEntered.Invoke(isCorrectSlime, slimeTag.ToString());


            // Destroy the slime.
            Destroy(slimeObject);
            PipeThwop.Play();
            PipeTriggerd.Invoke();
        }
    }
}
