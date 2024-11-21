using UnityEngine;

public class FireballScript : MonoBehaviour
{
    public GameObject explosionEffect; // The explosion effect prefab
    private Transform target; // The target the fireball will move toward
    private Rigidbody rb; // The Rigidbody of the fireball
    private float speed; // The speed of the fireball
    private System.Action onFireballDestroyed; // Callback to notify the spawner when the fireball is destroyed
    public float lifetime = 5f; // Time before fireball is destroyed if it misses
    private AudioSource _audioSource;

    private void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        if (target != null)
        {
            MoveTowardsTarget(); // Move the fireball toward the target
        }
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource != null) {
            _audioSource.Play();        
        }
        // Destroy the fireball after a few seconds
        Destroy(gameObject, lifeTime);
    }

    public void SetTarget(Transform newTarget, float fireballSpeed, System.Action destroyCallback)
    {
        // Assign the target and speed for the fireball
        target = newTarget;
        speed = fireballSpeed;
        onFireballDestroyed = destroyCallback;
    }

    private void MoveTowardsTarget()
    {
        // Calculate the direction to the target and set the velocity
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
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
            Destroy(gameObject); // Destroy the fireball
        }
    }

    private void OnDestroy()
    {
        // Notify the spawner that the fireball has been destroyed
        if (onFireballDestroyed != null)
        {
            onFireballDestroyed.Invoke();
        }
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
