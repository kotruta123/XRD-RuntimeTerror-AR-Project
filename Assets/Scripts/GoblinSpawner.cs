using UnityEngine;

public class GoblinSpawner : MonoBehaviour
{
    public GameObject goblinPrefab; // The goblin prefab to spawn

    public void SpawnGoblin()
    {
        // Check if the goblin prefab is assigned
        if (goblinPrefab != null)
        {
            Vector3 spawnPosition = GetRandomPositionOnGround(); // Get a random position
            Instantiate(goblinPrefab, spawnPosition, Quaternion.identity); // Instantiate the goblin
        }
        else
        {
            // Log an error if the goblin prefab is not assigned
            Debug.LogError("Goblin prefab is not assigned in the Inspector!");
        }
    }

    private Vector3 GetRandomPositionOnGround()
    {
        Camera mainCamera = Camera.main;

        // Manually define spawn bounds at a fixed distance from the camera
        float spawnDistance = 5f; // Distance from the camera
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, spawnDistance));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, spawnDistance));

        // Randomize positions within the visible area
        float randomX = Random.Range(bottomLeft.x, topRight.x);
        float randomZ = Random.Range(bottomLeft.z, topRight.z);

        float groundY = 0f; // Y-coordinate for the ground level
        return new Vector3(randomX, groundY, randomZ);
    }

}
