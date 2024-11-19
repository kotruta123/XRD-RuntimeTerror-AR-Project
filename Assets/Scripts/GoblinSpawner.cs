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

        // Manually define spawn bounds
        float spawnDistance = 1f; // Depth from the camera
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, spawnDistance));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, spawnDistance));

        // Use bottom-left and top-right to calculate random positions
        float floorY = 0f;
        float randomX = UnityEngine.Random.Range(bottomLeft.x, topRight.x);
        float randomZ = UnityEngine.Random.Range(spawnDistance - 2f, spawnDistance + 1f); // Custom Z range

        return new Vector3(randomX, floorY, randomZ);
    }
}
