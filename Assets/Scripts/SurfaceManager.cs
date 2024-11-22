using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SurfaceManager : MonoBehaviour
{
    public GameObject gameContent;          // The game content prefab
    public GameObject placeGameButton;      // UI Button to place the game
    public GameObject findSurfaceText;      // UI Text prompting to find a surface
    public Transform targetLine;            // The target line (e.g., kingdom wall)
    public Transform spawnPoint;            // The spawn point for goblins
    public GameObject fixedJoystick;        // UI Joystick to control daragon
    public GameObject startGameButton;      // UI Button to start the game
    public float targetLineOffset = 1f;     // Offset distance from the plane center for the target line
    public float spawnPointOffset = 5f;     // Offset distance from the plane center for the spawn point

    private ARPlane detectedPlane = null;   // The detected AR plane or mock plane
    private ARPlaneManager planeManager;    // ARPlaneManager for detecting real AR planes
    private bool gamePlaced = false;        // Tracks if the game has been placed

    private void Start()
    {
        // Get the ARPlaneManager component
        planeManager = FindObjectOfType<ARPlaneManager>();

        // Ensure initial UI state
        placeGameButton.SetActive(false);
        findSurfaceText.SetActive(true);
        startGameButton.gameObject.SetActive(false);
        fixedJoystick.SetActive(false);
        gameContent.SetActive(false);
    }

    private void Update()
    {
        if (gamePlaced)
        {
            return; // Stop further processing if the game has been placed
        }

        if (detectedPlane == null)
        {
            detectedPlane = GetFirstPlane(); // Detect a plane
        }

        if (detectedPlane != null && !placeGameButton.activeSelf)
        {
            findSurfaceText.SetActive(false);  // Hide the guide text
            placeGameButton.SetActive(true);   // Show the Place Game button
        }
    }

    public void PlaceGame()
    {
        if (detectedPlane != null && !gamePlaced)
        {
            // Position the game content
            gameContent.transform.position = detectedPlane.transform.position;
            gameContent.transform.rotation = detectedPlane.transform.rotation;

            // Position the targetLine relative to the detected plane
            Vector3 planeCenter = detectedPlane.transform.position;
            targetLine.position = planeCenter + detectedPlane.transform.forward * targetLineOffset;
            targetLine.position = new Vector3(targetLine.position.x, planeCenter.y, targetLine.position.z);

            // Position the spawnPoint relative to the detected plane
            spawnPoint.position = planeCenter + detectedPlane.transform.forward * spawnPointOffset;
            spawnPoint.position = new Vector3(spawnPoint.position.x, planeCenter.y, spawnPoint.position.z);

            // Activate the game content
            gameContent.SetActive(true);

            // Hide the Place Game button
            placeGameButton.SetActive(false);

            // Mark the game as placed
            gamePlaced = true;

            // Activate UI
            startGameButton.gameObject.SetActive(true);
            fixedJoystick.SetActive(true);

            // Optionally hide plane visuals
            DisablePlanes();
        }
    }

    private ARPlane GetFirstPlane()
    {
        //// Find the mock plane by its tag
        //GameObject mockPlane = GameObject.FindWithTag("MockPlane");

        //if (mockPlane != null)
        //{
        //    // Ensure the mock plane has an ARPlane component (to simulate AR functionality)
        //    ARPlane mockARPlane = mockPlane.GetComponent<ARPlane>();
        //    if (mockARPlane == null)
        //    {
        //        mockARPlane = mockPlane.AddComponent<ARPlane>();
        //    }
        //    return mockARPlane;
        //}

        //return null; // No mock plane found

        // Detect AR planes on a real device
        foreach (var plane in planeManager.trackables)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp) // Only consider horizontal planes
            {
                return plane;
            }
        }
        return null; // No AR planes found
    }

    private void DisablePlanes()
    {
        if (planeManager != null)
        {
            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(false); // Hide plane visuals
            }
            planeManager.enabled = false; // Stop further plane detection
        }
    }
}