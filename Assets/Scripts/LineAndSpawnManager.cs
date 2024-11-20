using UnityEngine;

public class LineAndSpawnManager : MonoBehaviour
{
    public Transform targetLine; // Assign the TargetLine GameObject
    public Transform spawnPoint; // Assign the SpawnPoint GameObject
    public Camera mainCamera; // Reference to the Main Camera
    public float targetLineDistance = 10f; // Distance of the TargetLine from the camera
    public float spawnPointDistance = 20f; // Distance of the SpawnPoint from the camera

    private void Start()
    {
        PositionTargetLine();
        PositionSpawnPoint();
    }

    private void PositionTargetLine()
    {
        if (mainCamera != null && targetLine != null)
        {
            // Calculate position directly in front of the camera
            Vector3 linePosition = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, targetLineDistance));
            targetLine.position = new Vector3(linePosition.x, 0f, linePosition.z); // Align to ground (y=0)
        }
    }

    private void PositionSpawnPoint()
    {
        if (mainCamera != null && spawnPoint != null)
        {
            // Calculate position farther in front of the camera
            Vector3 spawnPosition = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, spawnPointDistance));
            spawnPoint.position = new Vector3(spawnPosition.x, 0f, spawnPosition.z); // Align to ground (y=0)
        }
    }
}
