using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TargetLineHealthUI : MonoBehaviour
{
    public Slider healthBar;                // The health bar slider
    public TextMeshProUGUI healthText;      // The health text 

    public void Initialize(int maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"Wall Durability: {maxHealth}/{maxHealth}";
        }
    }

    public void UpdateHealth(int currentHealth)
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"Wall Durability: {currentHealth}/{healthBar.maxValue}";
        }
    }
}

