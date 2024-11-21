using UnityEngine;

public class DragonController : MonoBehaviour
{
    public GameObject dragonPrefab;
    private GameObject spawnedDragon;
    public float distanceFromCamera = 2.0f;

    void Start()
    {
        if (spawnedDragon == null && GameObject.Find("Purple(Clone)") == null)
        {
            Vector3 initialPosition = Camera.main.transform.position + Camera.main.transform.forward * 2f;
            spawnedDragon = Instantiate(dragonPrefab, initialPosition, Quaternion.identity);
            spawnedDragon.name = "Purple";
            Debug.Log("Dragon instantiated.");
        }
    }

    void StartFlying()
    {
        if (spawnedDragon != null)
        {
            DragonMovement dragonMovement = spawnedDragon.GetComponent<DragonMovement>();
            dragonMovement?.Fly();
        }
    }
    
    void RemoveDuplicates()
    {
        GameObject[] dragons = GameObject.FindGameObjectsWithTag("Dragon"); // Use appropriate tag
        if (dragons.Length > 1)
        {
            for (int i = 1; i < dragons.Length; i++) // Keep the first one, destroy others
            {
                Destroy(dragons[i]);
            }
        }
    }

}