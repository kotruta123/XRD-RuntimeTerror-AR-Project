using UnityEngine;

public class GoblinSpawner : MonoBehaviour
{
    public GameObject goblin;
    public BoundaryManager boundaryManager;

    public void SpawnGoblin()
    {
        if (boundaryManager == null)
        {
            Debug.LogError("Boundary Manager is not assigned!");
            return;
        }

        Vector3 spawnPosition = boundaryManager.GetRandomPositionInsideRoom();

        if (spawnPosition == Vector3.zero)
        {
            Debug.LogWarning("No valid spawn position found inside the room!");
            return;
        }

        if (goblin != null)
        {
            Instantiate(goblin, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Goblin prefab is not assigned in the Inspector!");
        }
    }
}