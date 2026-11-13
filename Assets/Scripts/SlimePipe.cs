using System;
using UnityEngine;
using UnityEngine.Events;

public class SlimePipe : MonoBehaviour
{
    [Header("Pipe Settings")]
    public string slimeTag = "Slime";

    public AudioSource PipeThwop;

    public static event Action<bool> onSlimeEntered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("SlimeTrigger"))
        {
            // Check whether the entering object is a slime.
            SlimeCollision slime =
            other.GetComponentInParent<SlimeCollision>();

            if (slime == null)
                return;

            // Store the slime GameObject before destroying it.
            GameObject slimeObject = slime.gameObject;

            bool isCorrectSlime = other.gameObject.CompareTag(slimeTag);

            // Trigger the custom event.
            onSlimeEntered.Invoke(isCorrectSlime);

            // Destroy the slime.
            Destroy(slimeObject);
            PipeThwop.Play();
        }
    }
}
