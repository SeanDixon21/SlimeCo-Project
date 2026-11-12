using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlimeSpawner : MonoBehaviour
{
    [Header("Spawn Sound")]
    public AudioSource audioSource;
    public AudioClip spawnSound;

    [Header("Slime Prefabs")]
    public GameObject[] slimePrefabs = new GameObject[5];

    [Header("Spawn Point")]
    public Transform spawnPoint;

    private readonly List<SlimeCollision> spawnedSlimes =
        new List<SlimeCollision>();

    void Update()
    {
        // Spacebar spawns the first prefab.
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            //SpawnSlime(0);
        }
    }

    // PUBLIC FUNCTION - Can be called by UI buttons.
    public void SpawnSlime(int prefabIndex)
    {
        if (prefabIndex < 0 || prefabIndex >= slimePrefabs.Length)
        {
            Debug.LogError("Invalid slime prefab index!");
            return;
        }

        GameObject prefab = slimePrefabs[prefabIndex];

        if (prefab == null)
        {
            Debug.LogError("Slime prefab is missing!");
            return;
        }

        Vector3 position = spawnPoint != null
            ? spawnPoint.position
            : transform.position;

        Quaternion rotation = spawnPoint != null
            ? spawnPoint.rotation
            : transform.rotation;

        GameObject newSlimeObject =
            Instantiate(prefab, position, rotation);

        SlimeCollision newSlime =
            newSlimeObject.GetComponent<SlimeCollision>();

        if (newSlime == null)
        {
            Debug.LogError("Slime prefab needs a SlimeCollision component!");

            Destroy(newSlimeObject);
            return;
        }

        Collider2D[] newColliders = newSlime.AllColliders;

        foreach (SlimeCollision oldSlime in spawnedSlimes)
        {
            if (oldSlime == null) continue;

            Collider2D[] oldColliders = oldSlime.AllColliders;

            foreach (Collider2D a in newColliders)
            {
                foreach (Collider2D b in oldColliders)
                {
                    // Allow collision if either collider is special.
                    bool allowCollision =
                        a == newSlime.specialCollider ||
                        b == oldSlime.specialCollider;

                    Physics2D.IgnoreCollision(
                        a, b, !allowCollision
                    );
                }
            }
        }

        spawnedSlimes.Add(newSlime);

        // Play spawn sound.
        if (audioSource != null && spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }
    }
}
