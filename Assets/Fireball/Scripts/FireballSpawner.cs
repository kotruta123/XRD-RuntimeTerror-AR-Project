using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class FireballSpawner : MonoBehaviour
{
    public GameObject fireballPrefab; // The fireball prefab to instantiate
    public Transform fireballSpawnPoint; // The point where the fireball will be spawned
    public float fireballSpeed = 1f; // Speed of the fireball

    private PlayerInputActions inputActions; // Stores the player's input actions
    private GameObject activeFireball; // Tracks the currently active fireball
    
    private void Awake()
    {
        // Initialize input actions
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        // Enable input actions and register the click event
        inputActions.Player.Enable();
        inputActions.Player.Click.performed += OnClickPerformed;
    }

    private void OnDisable()
    {
        // Disable input actions and unregister the click event
        inputActions.Player.Click.performed -= OnClickPerformed;
        inputActions.Player.Disable();
    }

    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        // Check if the pointer is over a UI element, and ignore clicks on UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Pointer is over a UI element. Raycast ignored.");
            return;
        }

        // Prevent shooting another fireball if one is already active
        if (activeFireball != null)
        {
            Debug.Log("A fireball is already in flight!");
            return;
        }

        // Create a Raycast from the camera through the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);

        RaycastHit hit;

        int layerMask = LayerMask.GetMask("Goblins");
        // Check if the Raycast hits any object
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            // Check if the clicked object is tagged as "Goblin"
            if (hit.collider.CompareTag("Goblin"))
            {
                Debug.Log($"Goblin clicked: {hit.collider.name}");
                ShootFireball(hit.collider.transform); // Shoot a fireball at the goblin
            }
        }
        else
        {
            // Log if the Raycast does not hit anything
            Debug.Log("Raycast did not hit anything.");
        }
    }

    private void ShootFireball(Transform target)
    {
        // Instantiate the fireball at the spawn point
        activeFireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, fireballSpawnPoint.rotation);

        // Get the FireballScript component and assign the target and speed
        FireballScript fireballScript = activeFireball.GetComponent<FireballScript>();
        if (fireballScript != null)
        {
            fireballScript.SetTarget(target, fireballSpeed, OnFireballDestroyed);
        }
    }

    // Callback to reset the fireball tracker when the fireball is destroyed
    private void OnFireballDestroyed()
    {
        activeFireball = null;
    }
}
