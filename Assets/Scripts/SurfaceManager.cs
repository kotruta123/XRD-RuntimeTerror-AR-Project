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
    public GameObject fixedJoystick;        // UI Joystick to control the dragon
    public GameObject startGameButton;      // UI Button to start the game
    public GameObject healthBarUI;          // The health bar UI
    public GameObject healthTextUI;         // The health text UI
    public float targetLineOffset = 1f;     // Offset distance from the plane center for the target line
    public float spawnPointOffset = 5f;     // Offset distance from the plane center for the spawn point
    public float cameraHeightOffset = 10f;   // Height offset for the camera

    private ARPlane detectedPlane = null;   // The detected AR plane or mock plane
    private ARPlaneManager planeManager;    // ARPlaneManager for detecting real AR planes
    private ARAnchorManager anchorManager;  // ARAnchorManager for fixing content to the plane
    private ARAnchor gameAnchor = null;     // Anchor for the game content
    private bool gamePlaced = false;        // Tracks if the game has been placed

    private void Start()
    {
        // Get the ARPlaneManager and ARAnchorManager components
        planeManager = FindObjectOfType<ARPlaneManager>();
        anchorManager = FindObjectOfType<ARAnchorManager>();

        // Ensure initial UI state
        placeGameButton.SetActive(false);
        findSurfaceText.SetActive(true);
        startGameButton.gameObject.SetActive(false);
        fixedJoystick.SetActive(false);
        gameContent.SetActive(false);
        healthBarUI.SetActive(false);
        healthTextUI.SetActive(false);
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
            // Create an anchor at the detected plane's position
            Pose planePose = new Pose(detectedPlane.center, Quaternion.identity);
            gameAnchor = anchorManager.AttachAnchor(detectedPlane, planePose);
            if (gameAnchor == null)
            {
                Debug.LogError("Failed to create anchor. Game placement aborted.");
                return;
            }

            // Parent the game content to the anchor
            gameContent.transform.SetParent(gameAnchor.transform, worldPositionStays: false);

            // Adjust game content to align with the plane
            gameContent.transform.localPosition = Vector3.zero;
            gameContent.transform.localRotation = Quaternion.identity;

            // Ensure the target line and spawn point are aligned to the plane
            AlignGameElementsToPlane();

            // Adjust the camera for a better view
            AdjustCameraPosition(gameAnchor.transform.position);

            // Activate the game content
            gameContent.SetActive(true);

            // Hide the Place Game button
            placeGameButton.SetActive(false);

            // Mark the game as placed
            gamePlaced = true;

            // Activate UI elements
            startGameButton.gameObject.SetActive(true);
            fixedJoystick.SetActive(true);
            healthBarUI.SetActive(true);
            healthTextUI.SetActive(true);

            // Optionally hide plane visuals
            DisablePlanes();
        }
    }

    private void AlignGameElementsToPlane()
    {
        Vector3 planePosition = detectedPlane.transform.position;

        // Calculate forward direction based on the camera
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0; // Keep direction on the horizontal plane
        cameraForward.Normalize();

        // Position and rotate the target line
        Vector3 targetLinePosition = planePosition + cameraForward * targetLineOffset;
        targetLine.position = new Vector3(targetLinePosition.x, planePosition.y, targetLinePosition.z);
        targetLine.rotation = Quaternion.LookRotation(planePosition - targetLine.position);

        // Position and rotate the spawn point
        Vector3 spawnPointPosition = planePosition + cameraForward * spawnPointOffset;
        spawnPoint.position = new Vector3(spawnPointPosition.x, planePosition.y, spawnPointPosition.z);
        spawnPoint.rotation = Quaternion.LookRotation(targetLine.position - spawnPoint.position);
    }



    private ARPlane GetFirstPlane()
    {
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

    private void AdjustCameraPosition(Vector3 planePosition)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Position the camera above and slightly behind the game content
            float optimalDistance = 10f;
            mainCamera.transform.position = new Vector3(
                planePosition.x,
                planePosition.y + cameraHeightOffset, // Height offset
                planePosition.z - optimalDistance    // Distance offset
            );

            // Rotate the camera to look directly at the game content
            mainCamera.transform.LookAt(planePosition + Vector3.up * 0.5f); // Look slightly above the plane center
        }
    }
}
