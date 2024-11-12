using UnityEngine;

public class DragonController : MonoBehaviour
{
    public GameObject dragonPrefab;
    private GameObject spawnedDragon;
    public float distanceFromCamera = 2.0f;

    void Start()
    {
        Vector3 initialPosition = Camera.main.transform.position + Camera.main.transform.forward * distanceFromCamera;
        spawnedDragon = Instantiate(dragonPrefab, initialPosition, Quaternion.identity);
        Invoke("StartFlying", 3f);  // Start flying after 3 seconds
    }

    void StartFlying()
    {
        if (spawnedDragon != null)
        {
            DragonMovement dragonMovement = spawnedDragon.GetComponent<DragonMovement>();
            dragonMovement?.Fly();
        }
    }
}