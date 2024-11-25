using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class FireballSpawner : MonoBehaviour
{
    public GameObject fireballPrefab; // The fireball prefab to instantiate
    public Transform fireballSpawnPoint; // The point where the fireball will be spawned
    public float fireballSpeed = 2f; // Speed of the fireball

    private GameObject activeFireball; // Tracks the currently active fireball

    private void Update()
    {
        // Detect inputs based on platform
#if UNITY_EDITOR
        HandleMouseInput(); // Editor: Handle mouse clicks
#else
        HandleTouchInput(); // Device: Handle touch input
#endif
    }

    private void HandleMouseInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) // Check for left mouse button press
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            // Check if the mouse click is over UI
            if (IsPointerOverSpecificUI(mousePosition))
            {
                Debug.Log("Mouse click is over the UI. Ignoring input.");
                return;
            }

            // Process the raycast from the mouse position
            ProcessClickOrTouch(mousePosition);
        }
    }


    // Handles touch input for devices
    private void HandleTouchInput()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

            // Check if the touch is over UI
            if (IsPointerOverSpecificUI(touchPosition))
            {
                Debug.Log("Touch is over the UI. Ignoring input.");
                return;
            }

            // Process the raycast from the touch position
            ProcessClickOrTouch(touchPosition);
        }
    }

    // Processes the raycast for either mouse or touch input
    private void ProcessClickOrTouch(Vector2 screenPosition)
    {
        // Prevent shooting another fireball if one is already active
        if (activeFireball != null)
        {
            Debug.Log("A fireball is already in flight!");
            return;
        }

        // Create a Raycast from the camera through the screen position
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // Check if the clicked/tapped object is tagged as "Goblin"
            if (hit.collider.CompareTag("Goblin"))
            {
                Debug.Log($"Goblin detected: {hit.collider.name}");
                ShootFireball(hit.collider.transform); // Shoot a fireball at the goblin
            }
            else
            {
                Debug.Log($"Raycast hit: {hit.collider.name}, but it's not a goblin.");
            }
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
        }
    }

    // Shoots the fireball at the given target
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

    // Checks if the pointer is over a UI element
    private bool IsPointerOverSpecificUI(Vector2 screenPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.CompareTag("Joystick")) // Replace with the tag of your specific UI
            {
                return true; // Pointer is over the joystick or specified UI
            }
        }

        return false; // Pointer is not over specific UI
    }
}
