using UnityEngine;
using System;

public class GoblinMovement : MonoBehaviour
{
    public float minSpeed = 0.2f; // Minimum movement speed
    public float maxSpeed = 0.3f; // Maximum movement speed
    private float movementSpeed; // Randomized movement speed
    private float wobbleAmount; // How much the goblin deviates while moving
    private float wobbleSpeed; // Speed of the wobbling effect
    private Vector3 targetPosition;
    private Animation animationComponent; // Reference to the legacy Animation component
    private bool isDead = false; // Prevent further movement after death
    private bool hasReachedTarget = false; // Prevent multiple calls to target logic

    // Events for goblin actions
    public Action OnReachTarget; // Triggered when goblin reaches the target
    public Action<GameObject> OnDie; // Triggered when goblin dies

    private void Start()
    {
        // Get the Animation component
        animationComponent = GetComponent<Animation>();

        // Assign random speed and wobble behavior for each goblin
        movementSpeed = UnityEngine.Random.Range(minSpeed, maxSpeed);
        wobbleAmount = UnityEngine.Random.Range(0.2f, 0.5f);
        wobbleSpeed = UnityEngine.Random.Range(1f, 2f);
    }

    public void SetTarget(Vector3 target)
    {
        targetPosition = target; // Set the goblin's target
    }

    private void Update()
    {
        if (isDead || hasReachedTarget) return; // Stop movement if dead or target reached

        // Calculate distance to the target
        float distance = Vector3.Distance(transform.position, targetPosition);

        // Rotate to face the target dynamically
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToTarget), Time.deltaTime * 5f);

        // Switch to walk animation
        animationComponent.CrossFade("walk");
        MoveTowardsTarget(movementSpeed); // Walk at normal speed

        // Check if the goblin has reached the target
        if (distance < 0.1f)
        {
            ReachTarget();
        }
    }

    private void MoveTowardsTarget(float currentSpeed)
    {
        // Add a wobble effect to the movement
        Vector3 wobble = new Vector3(Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount, 0f, 0f);

        // Move towards the target with wobble applied
        transform.position = Vector3.MoveTowards(transform.position, targetPosition + wobble, currentSpeed * Time.deltaTime);
    }

    public void TriggerDeath()
    {
        isDead = true; // Stop further movement

        // Notify WaveManager that this goblin is dead
        OnDie?.Invoke(gameObject);
    }

    private void ReachTarget()
    {
        if (isDead || hasReachedTarget) return; // Don't trigger target logic if dead or already processed

        hasReachedTarget = true; // Mark as having reached the target
        Debug.Log("Goblin reached the target!");

        OnReachTarget?.Invoke(); // Trigger the event to notify WaveManager
        Destroy(gameObject); // Destroy the goblin
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        movementSpeed *= multiplier; // Scale the movement speed
    }

}