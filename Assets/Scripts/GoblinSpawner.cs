using UnityEngine;

public class GoblinSpawner : MonoBehaviour
{
    public GameObject goblin;

    public void SpawnGoblin()
    {
    
        float spawnPointX = Random.Range(0.1f, 3f);
        float spawnPointY = 0f;
        float spawnPointZ = Random.Range(0.1f, 3f);

        Vector3 spawnPosition = new Vector3(spawnPointX, spawnPointY, spawnPointZ);

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
