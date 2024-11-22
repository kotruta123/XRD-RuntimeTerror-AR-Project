using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public GameObject goblinPrefab; // Assign the goblin prefab in the Inspector
    public Transform spawnPoint; // Assign the spawn point in the Inspector
    public Transform targetLine; // Assign the target line in the Inspector
    public TextMeshProUGUI waveNotificationText; // Assign the UI Text for wave notifications
    public TextMeshProUGUI gameOverText; // Assign the UI Text for game over messages
    public TextMeshProUGUI goblinsRemainingText; // Assign the UI Text for goblin count
    public int goblinsPerWave = 5; // Number of goblins in the first wave
    public float spawnInterval = 0.5f; // Delay between spawns
    public int totalWaves = 5; // Total number of waves

    private List<GameObject> activeGoblins = new List<GameObject>(); // List to track active goblins
    private int currentWave = 0; // Tracks the current wave
    private bool isGameOver = false; // To prevent further waves after the game ends

    public delegate void GameOverDelegate(string message); // Delegate for game end messages
    public event GameOverDelegate OnGameEnd; // Event triggered when the game ends

    private void Start()
    {
        UpdateGoblinsRemainingText(0); // Set initial goblin count
        StartCoroutine(SpawnWave());
        UpdateGameOverText(""); // Clear game-over text at the start
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

        // Add randomness to the spawn position
        Vector3 randomOffset = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(0f, 2f));
        Vector3 spawnPosition = spawnPoint.position + randomOffset;

        GameObject goblin = Instantiate(goblinPrefab, spawnPosition, Quaternion.identity);

        Debug.Log($"Goblin : {goblinPrefab}");

        // Log the spawn position for debugging
        Debug.Log($"Spawning goblin at {spawnPosition}");

        // Add the goblin to the active list
        activeGoblins.Add(goblin);

        // Update goblin count UI
        UpdateGoblinsRemainingText(activeGoblins.Count);

        // Calculate direction to the target
        Vector3 directionToTarget = (targetLine.position - spawnPosition).normalized;

        // Rotate the goblin to face the target
        goblin.transform.rotation = Quaternion.LookRotation(directionToTarget);

        GoblinMovement goblinMovement = goblin.GetComponent<GoblinMovement>();
        if (goblinMovement != null)
        {
            // Assign the target position slightly offset as well
            Vector3 randomTargetOffset = new Vector3(Random.Range(-1f, 1f), 0f, 0f);
            goblinMovement.SetTarget(targetLine.position + randomTargetOffset);

            goblinMovement.OnReachTarget += () => EndGame("Game Over! A goblin reached the target.", Color.red);
            goblinMovement.OnDie += RemoveGoblinFromWave;
        }

        Debug.Log("Spawned a goblin!");
    }

    private void RemoveGoblinFromWave(GameObject goblin)
    {
        activeGoblins.Remove(goblin);

        // Update goblin count UI
        UpdateGoblinsRemainingText(activeGoblins.Count);
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

            gameOverText.color = textColor.Value; // Update the text color

        }
    }

    private void UpdateGoblinsRemainingText(int count)
    {
        if (goblinsRemainingText != null)
        {
            goblinsRemainingText.text = $"Goblins Remaining: {count}";
        }
    }
}
