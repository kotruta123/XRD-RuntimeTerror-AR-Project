using UnityEngine;

public class FireballScript : MonoBehaviour
{
    public float lifeTime = 3f;

    void Start()
    {
        // Destroy the fireball after a few seconds
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Destroy the fireball when it collides with something
        Destroy(gameObject);

        // (Optional) Add explosion or damage logic here
    }
}
