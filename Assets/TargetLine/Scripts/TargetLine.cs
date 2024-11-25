using UnityEngine;

public class TargetLine : MonoBehaviour
{
    public int maxHealth = 100;         // Maximum health of the target line
    private int currentHealth;         // Current health

    public TargetLineHealthUI healthUI; // Reference to the Target Line Health UI

    public delegate void TargetLineDestroyed();
    public event TargetLineDestroyed OnTargetLineDestroyed;

    private void Start()
    {
        currentHealth = maxHealth;

        // Initialize the health UI
        if (healthUI != null)
        {
            healthUI.Initialize(maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Clamp health to avoid negative values
        currentHealth = Mathf.Max(currentHealth, 0);

        // Update the health UI
        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHealth);
        }

        // Check if health has reached 0
        if (currentHealth <= 0)
        {
            OnTargetLineDestroyed?.Invoke(); // Notify GameManager about destruction
            Debug.Log("Target Line Destroyed!");
        }
    }
}
