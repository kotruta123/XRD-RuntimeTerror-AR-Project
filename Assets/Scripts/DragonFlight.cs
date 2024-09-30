using UnityEngine;

public class DragonFlight : MonoBehaviour
{
    public float flySpeed = 3f;      // Speed of flight
    private Vector3 flyDirection;    // Current direction of flight
    private Rigidbody rb;            // Reference to Rigidbody component

    void Start()
    {
        // Initialize flight direction to a random forward vector
        flyDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        rb = GetComponent<Rigidbody>();  // Get Rigidbody component
        rb.isKinematic = true;           // Ensure Rigidbody is kinematic for physics detection
    }

    void Update()
    {
        // Move the dragon forward in the fly direction
        transform.Translate(flyDirection * flySpeed * Time.deltaTime, Space.World);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Reflect direction upon hitting a wall
            Vector3 normal = collision.contacts[0].normal;
            flyDirection = Vector3.Reflect(flyDirection, normal).normalized;

            // Optionally trigger any animation changes when hitting walls
            GetComponent<Animator>().SetTrigger("HitWall");
        }
    }
}