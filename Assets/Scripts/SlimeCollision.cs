using UnityEngine;

public class SlimeCollision : MonoBehaviour
{
    [Header("Slime Colliders")]
    public Collider2D specialCollider;

    public Collider2D[] AllColliders =>
        GetComponentsInChildren<Collider2D>();

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip[] floorSounds;
    public AudioClip[] slimeSounds;

    [Header("Sound Settings")]
    public float soundCooldown = 0.15f;

    private float nextFloorSoundTime;
    private float nextSlimeSoundTime;

    void Awake()
    {
        // Add a collision listener to every child collider.
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>(true);

        foreach (Collider2D col in colliders)
        {
            SlimeChildCollisionListener listener =
                col.gameObject.GetComponent<SlimeChildCollisionListener>();

            if (listener == null)
                listener = col.gameObject.AddComponent<SlimeChildCollisionListener>();

            listener.Initialize(this);
        }
    }

    public void HandleContact(Collider2D other)
    {
        if (other == null)
            return;

        Debug.Log($"{name}: contact with {other.name}");

        if (audioSource == null)
        {
            Debug.LogWarning($"{name}: No AudioSource assigned!");
            return;
        }

        // Check the collider's object and its parents for the Ground tag.
        bool isGround =
            other.CompareTag("Ground") ||
            (other.GetComponentInParent<Transform>() != null &&
             other.transform.root.CompareTag("Ground"));

        if (isGround)
        {
            Debug.Log($"{name}: Ground detected!");

            if (Time.time >= nextFloorSoundTime)
            {
                PlayRandomSound(floorSounds);
                nextFloorSoundTime = Time.time + soundCooldown;
            }

            return;
        }

        SlimeCollision otherSlime =
            other.GetComponentInParent<SlimeCollision>();

        if (otherSlime != null && otherSlime != this)
        {
            Debug.Log($"{name}: Another slime detected!");

            if (Time.time >= nextSlimeSoundTime)
            {
                PlayRandomSound(slimeSounds);
                nextSlimeSoundTime = Time.time + soundCooldown;
            }
        }
    }

    void PlayRandomSound(AudioClip[] clips)
    {
        if (audioSource == null || clips == null || clips.Length == 0)
        {
            Debug.LogWarning($"{name}: No sound clips assigned!");
            return;
        }

        AudioClip clip = clips[Random.Range(0, clips.Length)];

        if (clip == null)
            return;

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(clip);
    }
}

public class SlimeChildCollisionListener : MonoBehaviour
{
    private SlimeCollision owner;

    public void Initialize(SlimeCollision slime)
    {
        owner = slime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (owner != null)
            owner.HandleContact(collision.collider);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (owner != null)
            owner.HandleContact(other);
    }
}