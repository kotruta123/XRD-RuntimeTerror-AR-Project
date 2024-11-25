using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject startButton;    // Reference to the Start Button
    public GameObject restartButton; // Reference to the Restart Button
    public WaveManager waveManager;  // Reference to the WaveManager
    public TargetLine targetLine;    // Reference to the TargetLine for game-over detection

    private bool isGameOver = false;

    private void Start()
    {
        // Ensure buttons are in the correct state at the start
        if (startButton != null)
        {
            startButton.SetActive(true);
        }

        if (restartButton != null)
        {
            restartButton.SetActive(false);
        }

        // Pause the WaveManager at the start
        if (waveManager != null)
        {
            waveManager.enabled = false;
        }

        // Subscribe to the TargetLine destruction event
        if (targetLine != null)
        {
            targetLine.OnTargetLineDestroyed += HandleGameOver;
        }
    }

    public void StartGame()
    {
        // Hide the Start Button
        if (startButton != null)
        {
            startButton.SetActive(false);
        }

        // Enable the WaveManager to start spawning waves
        if (waveManager != null)
        {
            waveManager.enabled = true;
        }

        Debug.Log("Game Started!");
    }

    public void GameOver(string message)
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log(message);

        if (restartButton != null)
        {
            restartButton.SetActive(true); // Show the Restart Button
        }

        if (waveManager != null)
        {
            waveManager.enabled = false; // Stop further wave spawning
        }

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Debug.Log("Restarting game...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }

    private void HandleGameOver()
    {
        // Trigger Game Over when the target line is destroyed
        GameOver("Game Over! The treasure was destroyed!");
    }
}
