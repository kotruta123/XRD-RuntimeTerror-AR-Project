using UnityEngine;
using System.Collections;

public class GoblinDestroy : MonoBehaviour
{
    public int health = 3; // The health of the goblin
    private Animation animationComponent;
    private bool isDead = false;

    private void Start()
    {
        animationComponent = GetComponent<Animation>(); // Get the Animation component
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the goblin is hit by an object tagged as "Fireball"
        if (collision.gameObject.CompareTag("Fireball") && !isDead)
        {
            health--; // Decrease the goblin's health

            Destroy(collision.gameObject); // Destroy the fireball

            if (health <= 0)
            {
                Die(); // Destroy the goblin if health is 0 or less
            }
        }
    }

    private void Die()
    {
        isDead = true;
        GetComponent<Collider>().enabled = false; // Disable the collider

        GoblinMovement goblinMovement = GetComponent<GoblinMovement>();
        if (goblinMovement != null)
        {
            goblinMovement.TriggerDeath();
        }

        // Stop all animations
        animationComponent.Stop();

        // Play the death animation
        if (animationComponent["death"] != null)
        {
            Debug.Log("Playing death animation.");
            animationComponent.Play("death");
            StartCoroutine(DestroyAfterAnimation(animationComponent["death"].length));
        }
        else
        {
            Debug.LogWarning("Death animation is missing. Destroying goblin immediately.");
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyAfterAnimation(float delay)
    {
        Debug.Log($"Waiting {delay} seconds before destroying the goblin.");
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}