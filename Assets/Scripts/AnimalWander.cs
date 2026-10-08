using UnityEngine;

/// <summary>
/// Simple wandering AI for animals (Cat, Dog, Cow, etc.)
/// Moves animals within a defined area or along roads with random pauses and direction changes.
/// Compatible with or without an Animator component.
/// </summary>
public class AnimalWander : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Movement speed in units/second")]
    public float moveSpeed = 2.0f;

    [Tooltip("Rotation turning speed")]
    public float turnSpeed = 5.0f;

    [Tooltip("Maximum distance the animal will wander from its starting point")]
    public float wanderRadius = 15.0f;

    [Header("Behavior Timers")]
    [Tooltip("Minimum time spent walking before taking a rest")]
    public float minWalkTime = 3.0f;
    [Tooltip("Maximum time spent walking")]
    public float maxWalkTime = 7.0f;

    [Tooltip("Minimum time spent idling/resting")]
    public float minIdleTime = 2.0f;
    [Tooltip("Maximum time spent idling")]
    public float maxIdleTime = 5.0f;

    [Header("Optional Animation Parameter Names")]
    [Tooltip("Animator bool or float parameter name for walking (leave blank if not using Animator)")]
    public string walkAnimBool = "isWalking";

    private Vector3 originPosition;
    private Vector3 targetPosition;
    private bool isWalking = false;
    private float stateTimer = 0f;
    private Animator animator;
    private CharacterController characterController;

    private void Start()
    {
        originPosition = transform.position;
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        // Start by picking a destination
        PickNewDestination();
    }

    private void Update()
    {
        stateTimer -= Time.deltaTime;

        if (isWalking)
        {
            MoveTowardsTarget();

            // Check if reached destination or timer expired
            float distanceToTarget = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                                                      new Vector3(targetPosition.x, 0, targetPosition.z));

            if (distanceToTarget < 0.5f || stateTimer <= 0f)
            {
                SetIdle();
            }
        }
        else
        {
            // Idling
            if (stateTimer <= 0f)
            {
                PickNewDestination();
            }
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 direction = (targetPosition - transform.position);
        direction.y = 0; // Keep horizontal

        if (direction.sqrMagnitude > 0.001f)
        {
            // Smoothly rotate towards movement direction
            Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);

            Vector3 moveDelta = transform.forward * (moveSpeed * Time.deltaTime);

            if (characterController != null && characterController.enabled)
            {
                characterController.Move(moveDelta);
            }
            else
            {
                transform.position += moveDelta;
            }
        }
    }

    private void PickNewDestination()
    {
        // Random point on X-Z circle around origin
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        targetPosition = originPosition + new Vector3(randomCircle.x, 0, randomCircle.y);

        isWalking = true;
        stateTimer = Random.Range(minWalkTime, maxWalkTime);

        UpdateAnimatorState(true);
    }

    private void SetIdle()
    {
        isWalking = false;
        stateTimer = Random.Range(minIdleTime, maxIdleTime);

        UpdateAnimatorState(false);
    }

    private void UpdateAnimatorState(bool walking)
    {
        if (animator != null && !string.IsNullOrEmpty(walkAnimBool))
        {
            animator.SetBool(walkAnimBool, walking);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize wander area in Unity Scene View
        Vector3 center = Application.isPlaying ? originPosition : transform.position;
        Gizmos.color = new Color(0.2f, 0.8f, 0.3f, 0.35f);
        Gizmos.DrawWireSphere(center, wanderRadius);

        if (Application.isPlaying && isWalking)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetPosition);
            Gizmos.DrawSphere(targetPosition, 0.3f);
        }
    }
}
