using UnityEngine;

public class DragonController : MonoBehaviour
{
    public GameObject dragonPrefab; // Reference to the Dragon prefab
    private GameObject spawnedDragon; // Tracks the spawned Dragon instance
    public float distanceFromCamera = 2.0f; // Initial distance from the camera

    void Start()
    {
        // Check if a dragon already exists
        spawnedDragon = GameObject.Find("Purple") ?? GameObject.FindWithTag("Dragon");

        if (spawnedDragon == null)
        {
            // Instantiate the dragon at the camera's forward position initially
            Vector3 initialPosition = Camera.main.transform.position + Camera.main.transform.forward * distanceFromCamera;
            spawnedDragon = Instantiate(dragonPrefab, initialPosition, Quaternion.identity);
            spawnedDragon.name = "Purple"; // Assign a unique name
            Debug.Log("Dragon instantiated.");
        }

        RemoveDuplicates(); // Ensure only one dragon exists
    }

    public void StartFlying()
    {
        if (spawnedDragon != null)
        {
            DragonMovement dragonMovement = spawnedDragon.GetComponent<DragonMovement>();
            dragonMovement?.Fly(); // Trigger the flying animation or logic
        }
    }

    void RemoveDuplicates()
    {
        GameObject[] dragons = GameObject.FindGameObjectsWithTag("Dragon"); // Ensure the prefab and existing dragons share this tag
        if (dragons.Length > 1)
        {
            for (int i = 1; i < dragons.Length; i++) // Keep the first one, destroy others
            {
                Destroy(dragons[i]);
            }
        }
    }
}
