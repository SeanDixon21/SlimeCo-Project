
using UnityEngine;
using UnityEngine.InputSystem;

public class SlimeDrag : MonoBehaviour
{
    [Header("Drag Target")]
    public Collider2D centerCollider;
    public Rigidbody2D centerBody;

    private Camera mainCamera;
    private bool isDragging;
    private Vector2 dragOffset;

    private RigidbodyType2D originalBodyType;
    private Vector2 originalVelocity;
    private float originalAngularVelocity;

    void Awake()
    {
        mainCamera = Camera.main;

        if (centerCollider != null && centerBody == null)
            centerBody = centerCollider.attachedRigidbody;
    }

    void Update()
    {
        if (mainCamera == null)
            return;

        Vector2 mouseWorld = GetMouseWorldPosition();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (centerCollider != null &&
                centerBody != null &&
                centerCollider.OverlapPoint(mouseWorld))
            {
                isDragging = true;

                dragOffset = centerBody.position - mouseWorld;

                originalBodyType = centerBody.bodyType;
                originalVelocity = centerBody.linearVelocity;
                originalAngularVelocity = centerBody.angularVelocity;

                centerBody.linearVelocity = Vector2.zero;
                centerBody.angularVelocity = 0f;
                centerBody.bodyType = RigidbodyType2D.Kinematic;
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) &&
            isDragging)
        {
            isDragging = false;

            centerBody.bodyType = originalBodyType;
            centerBody.linearVelocity = originalVelocity;
            centerBody.angularVelocity = originalAngularVelocity;
        }
    }

    void FixedUpdate()
    {
        if (!isDragging || centerBody == null)
            return;

        centerBody.MovePosition(GetMouseWorldPosition() + dragOffset);
    }

    Vector2 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        return new Vector2(worldPosition.x, worldPosition.y);
    }
}


