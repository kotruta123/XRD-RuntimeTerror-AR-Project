using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public GameObject goblinPrefab;          // Assign the goblin prefab in the Inspector
    public Transform spawnPoint;            // Assign the spawn point in the Inspector
    public Transform targetLine;            // Assign the target line in the Inspector
    public TargetLine targetLineScript;     // Reference to the TargetLine script
    public TextMeshProUGUI waveNotificationText; // Assign the UI Text for wave notifications
    public TextMeshProUGUI gameOverText;    // Assign the UI Text for game over messages
    public TextMeshProUGUI goblinsRemainingText; // Assign the UI Text for goblin count
    public GameObject healthBarUI;          // The health bar UI
    public GameObject healthTextUI;         // The health text UI
    public int goblinsPerWave = 5;          // Number of goblins in the first wave
    public float spawnInterval = 0.5f;      // Delay between spawns
    public int totalWaves = 5;              // Total number of waves

    private List<GameObject> activeGoblins = new List<GameObject>(); // List to track active goblins
    private int currentWave = 0;            // Tracks the current wave
    private bool isGameOver = false;        // To prevent further waves after the game ends

    public delegate void GameOverDelegate(string message); // Delegate for game end messages
    public event GameOverDelegate OnGameEnd;              // Event triggered when the game ends

    public float speedMultiplier = 0.1f; // Multiplier to increase goblin speed each wave
    private float currentSpeedMultiplier = 1f; // Tracks the current speed multiplier


    private void Start()
    {
        UpdateGoblinsRemainingText(0); // Set initial goblin count
        StartCoroutine(SpawnWave());
        UpdateGameOverText(""); // Clear game-over text at the start

        healthBarUI.SetActive(true);
        healthTextUI.SetActive(true);

        // Subscribe to TargetLine destruction event
        if (targetLineScript != null)
        {
            targetLineScript.OnTargetLineDestroyed += HandleTargetLineDestroyed;
        }
    }

    private IEnumerator SpawnWave()
    {
        while (!isGameOver && currentWave < totalWaves)
        {
            currentWave++;
            Debug.Log($"Starting wave {currentWave} with {goblinsPerWave} goblins!");

            // Show wave notification
            UpdateWaveNotification($"Wave {currentWave} Started!");

            // Spawn goblins for the current wave
            for (int i = 0; i < goblinsPerWave; i++)
            {
                SpawnGoblin();
                yield return new WaitForSeconds(spawnInterval);
            }

            UpdateWaveNotification(""); // Clear the notification

            // Wait for all goblins in the wave to die before starting the next wave
            yield return new WaitUntil(() => activeGoblins.Count == 0);

            // Increment goblins per wave for the next wave
            goblinsPerWave += 2;

            // Increase the speed multiplier for the next wave
            currentSpeedMultiplier *= speedMultiplier;

            Debug.Log($"Wave {currentWave} cleared!");
            UpdateWaveNotification($"Wave {currentWave} Cleared!");

            // Show "Wave Cleared" message for a short time
            yield return new WaitForSeconds(2f);
            UpdateWaveNotification(""); // Clear the notification
        }

        if (!isGameOver)
        {
            // Player wins if all waves are cleared
            EndGame("Congratulations! You cleared all waves!", Color.cyan);
        }
    }

    private void SpawnGoblin()
    {
        // Check if the prefab is valid
        if (goblinPrefab == null)
        {
            Debug.LogError("Goblin Prefab is missing or destroyed!");
            return;
        }

        // Small spawn area around the spawn point
        float spawnRangeX = 1f; 
        float spawnRangeZ = 0.5f; 
        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnRangeX, spawnRangeX), 
            0f, 
            Random.Range(-spawnRangeZ, spawnRangeZ)
        );

        Vector3 spawnPosition = spawnPoint.position + randomOffset;

        GameObject goblin = Instantiate(goblinPrefab, spawnPosition, Quaternion.identity);

        Debug.Log($"Spawning goblin at {spawnPosition}");

        // Add the goblin to the active list
        activeGoblins.Add(goblin);

        // Update goblin count UI
        UpdateGoblinsRemainingText(activeGoblins.Count);

        GoblinMovement goblinMovement = goblin.GetComponent<GoblinMovement>();
        if (goblinMovement != null)
        {
            // Assign a dynamic spread target along the wall
            Vector3 spreadTarget = CalculateSpreadTarget();

            // Apply speed multiplier to the goblin
            goblinMovement.SetSpeedMultiplier(currentSpeedMultiplier);

            // Assign the target position slightly offset
            goblinMovement.SetTarget(spreadTarget);

            // When goblins reach the target, damage it
            goblinMovement.OnReachTarget += () =>
            {
                if (targetLineScript != null)
                {
                    targetLineScript.TakeDamage(10); // Adjust the damage value
                    Debug.Log("Target Line Damaged!");
                }

                // Remove and destroy the goblin
                RemoveGoblinFromWave(goblin);
                Destroy(goblin);
            };

            goblinMovement.OnDie += RemoveGoblinFromWave;
        }
    }

    private void RemoveGoblinFromWave(GameObject goblin)
    {
        activeGoblins.Remove(goblin);

        // Update goblin count UI
        UpdateGoblinsRemainingText(activeGoblins.Count);
    }

    private void HandleTargetLineDestroyed()
    {
        // End the game when the target line is destroyed
        EndGame("Game Over! The wall was destroyed!", Color.red);
    }

    private void EndGame(string message, Color textColor)
    {
        if (isGameOver) return; // Prevent multiple game end calls
        isGameOver = true;

        Debug.Log(message);

        // Show game-over notification
        UpdateGameOverText(message, textColor);

        goblinsRemainingText.text = "";

        // Trigger the game end event (for UI or other systems)
        OnGameEnd?.Invoke(message);

        // Show the Game Over UI
        FindObjectOfType<GameManager>()?.GameOver(message);

        // Stop spawning waves and clear active goblins
        StopAllCoroutines();
        foreach (GameObject goblin in activeGoblins)
        {
            if (goblin != null)
                Destroy(goblin);
        }
        activeGoblins.Clear();
    }

    private void UpdateWaveNotification(string message)
    {
        if (waveNotificationText != null)
        {
            waveNotificationText.text = message; // Update the UI text
        }
    }

    private void UpdateGameOverText(string message, Color? textColor = null)
    {
        if (gameOverText != null)
        {
            gameOverText.text = message; // Update the game-over text

            if (textColor.HasValue)
            {
                gameOverText.color = textColor.Value; // Update the text color
            }
        }
    }

    private void UpdateGoblinsRemainingText(int count)
    {
        if (goblinsRemainingText != null)
        {
            goblinsRemainingText.text = $"Goblins Remaining: {count}";
        }
    }

    private Vector3 CalculateSpreadTarget()
    {
        // Define the spread width relative to the wall
        float spreadWidth = 5f; 

        // Offset from the center of the wall
        float offset = Random.Range(-spreadWidth / 2f, spreadWidth / 2f);

        // Use the wall's local orientation to calculate the spread position
        Vector3 localOffset = targetLine.right * offset;

        // Calculate the target position in world space
        Vector3 spreadTarget = targetLine.position + localOffset;

        return spreadTarget;
    }

}
