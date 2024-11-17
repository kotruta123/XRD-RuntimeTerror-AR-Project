using System.Diagnostics;
using UnityEngine;

public class FireballSpawner : MonoBehaviour
{
    public GameObject fireballPrefab;   // Assign Fireball Prefab here
    public Transform fireballSpawnPoint;  // Assign Spawn Point here
    public float fireballSpeed = 10f;

    public void ShootFireball()
    {
        // Instantiate a fireball
        GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, fireballSpawnPoint.rotation);

        // Add velocity to the fireball
        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = fireballSpawnPoint.forward.normalized * fireballSpeed;
        }
    }
}