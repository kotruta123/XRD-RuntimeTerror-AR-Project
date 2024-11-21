using System.Collections.Generic;
using UnityEngine;

public class GoblinSpawner : MonoBehaviour
{

    public GameObject goblinPrefab; // The goblin prefab to spawn
    public float minDistance = 2f; // Minimum distance between spawn positions
    public float spawnDistance = 5f; // Distance from the camera
    public int maxRetryAttempts = 100; // Maximum retries to prevent infinite loops
    private List<Vector3> usedPositions = new List<Vector3>(); // List to track recent spawn positions

    public void SpawnGoblin()
    {
        // Check if the goblin prefab is assigned
        if (goblinPrefab != null)
        {
            Vector3 spawnPosition = GetUniquePosition();
            Instantiate(goblinPrefab, spawnPosition, Quaternion.identity); // Instantiate the goblin
        }
        else
        {
            Debug.LogError("Goblin prefab is not assigned in the Inspector!");
        }
    }

}


    private Vector3 GetUniquePosition()
    {
        Camera mainCamera = Camera.main;

        // Manually define spawn bounds
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, spawnDistance));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, spawnDistance));

        for (int attempt = 0; attempt < maxRetryAttempts; attempt++)
        {
            // Generate a random position within the bounds
            float randomX = Random.Range(bottomLeft.x, topRight.x);
            float randomZ = Random.Range(bottomLeft.z, topRight.z);
            float floorY = 0f; // Assume ground level at Y=0
            Vector3 randomPosition = new Vector3(randomX, floorY, randomZ);

            // Check if the position is far enough from previously used positions
            bool isFarEnough = true;
            foreach (Vector3 usedPosition in usedPositions)
            {
                if (Vector3.Distance(randomPosition, usedPosition) < minDistance)
                {
                    isFarEnough = false;
                    break;
                }
            }

            if (isFarEnough)
            {
                usedPositions.Add(randomPosition); // Add the position to the list
                return randomPosition; // Return the valid position
            }
        }

        // If no position is found within max retries, log a warning and pick a random position
        Debug.LogWarning("Failed to find a unique position within the retry limit. Using a random position.");
        return new Vector3(Random.Range(bottomLeft.x, topRight.x), 0f, Random.Range(bottomLeft.z, topRight.z));
    }
}

