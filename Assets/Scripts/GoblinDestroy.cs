using UnityEngine;

public class GoblinDestroy : MonoBehaviour
{
    public int health = 1; // The health of the goblin

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the goblin is hit by an object tagged as "Fireball"
        if (collision.gameObject.CompareTag("Fireball"))
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
        // Destroy the goblin game object
        Destroy(gameObject);
    }
}