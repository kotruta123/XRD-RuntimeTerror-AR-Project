using UnityEngine;

public class FireballScript : MonoBehaviour
{
    public GameObject explosionEffect; // The explosion effect prefab
    private Transform target; // The target the fireball will move toward
    private Rigidbody rb; // The Rigidbody of the fireball
    private float speed; // The speed of the fireball
    private System.Action onFireballDestroyed; // Callback to notify the spawner when the fireball is destroyed
    private AudioSource _audioSource;

    public float closeRange = 3f; // Distance considered "close" to the goblin
    public float closeRangeTimeout = 1.5f; // Time before fireball disappears if near the goblin
    private float closeRangeTimer = 0f; // Timer for how long fireball has been near the goblin

    private void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        if (target != null)
        {
            MoveTowardsTarget(); // Move the fireball toward the target
        }
        else
        {
            DestroyFireball(); // Destroy immediately if no target
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource != null)
        {
            _audioSource.Play();
        }
        
    }

    public void SetTarget(Transform newTarget, float fireballSpeed, System.Action destroyCallback)
    {
        // Assign the target and speed for the fireball
        target = newTarget;
        speed = fireballSpeed;
        onFireballDestroyed = destroyCallback;
    }

    private void Update()
    {
        if (target == null)
        {
            Debug.Log("Target disappeared. Destroying fireball.");
            DestroyFireball();
            return;
        }

        // Check if the fireball is close to the target
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget < closeRange)
        {
            Debug.Log($"Fireball close to target: Distance {distanceToTarget}");
            closeRangeTimer += Time.deltaTime;

            // Destroy the fireball if it has been close for too long
            if (closeRangeTimer > closeRangeTimeout)
            {
                Debug.Log("Fireball missed goblin and is being destroyed due to timeout.");
                DestroyFireball();
            }
        }
        else
        {
            // Reset the timer if the fireball is no longer close
            closeRangeTimer = 0f;
        }
    }

    private void MoveTowardsTarget()
    {
        // Calculate the direction to the target and set the velocity
        if (target != null)
        {
            // Adjust the target position to aim for the goblin's center
            Vector3 targetPosition = target.position + Vector3.up * 0.6f;

            Vector3 direction = (targetPosition - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the fireball collides with an object tagged as "Goblin"
        if (collision.collider.CompareTag("Goblin"))
        {
            Debug.Log("Fireball collided with Goblin!");
            Explode(); // Trigger the explosion effect
            DestroyFireball(); // Destroy the fireball
        }
    }

    private void DestroyFireball()
    {
        // Notify the spawner that the fireball has been destroyed
        onFireballDestroyed?.Invoke();
        Destroy(gameObject);
    }

    private void Explode()
    {
        // Instantiate the explosion effect at the fireball's position
        if (explosionEffect != null)
        {
            // Instantiate the explosion effect
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);

            // Destroy the explosion effect after 1 second
            Destroy(explosion, 1f);
        }
    }
}