using UnityEngine;

public class GoblinSpawner : MonoBehaviour
{
    public GameObject goblin;
    private AudioSource _audioSource;
    public void SpawnGoblin()
    {
    
        float spawnPointX = Random.Range(0.1f, 3f);
        float spawnPointY = 0f;
        float spawnPointZ = Random.Range(0.1f, 3f);

        _audioSource = GetComponent<AudioSource>();

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

    public void OnDestroy()
    {
        if (_audioSource != null)
        {
            _audioSource.volume = 1.0f;
            _audioSource.Play();
        }
        Destroy(gameObject);

    }
}
