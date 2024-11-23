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
    public float targetLineOffset = 1f;     // Offset distance from the plane center for the target line
    public float spawnPointOffset = 5f;     // Offset distance from the plane center for the spawn point
    public float cameraHeightOffset = 4f;   // Height offset for the camera

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
            gameAnchor = anchorManager.AttachAnchor(detectedPlane, new Pose(detectedPlane.center, Quaternion.identity));
            if (gameAnchor == null)
            {
                Debug.LogError("Failed to create anchor. Game placement aborted.");
                return;
            }

            // Attach the game content to the anchor
            gameContent.transform.SetParent(gameAnchor.transform);

            // Adjust the game content position relative to the plane
            gameContent.transform.localPosition = Vector3.zero;
            gameContent.transform.localRotation = Quaternion.identity;

            // Set the dragon's height explicitly
            Vector3 adjustedPosition = gameContent.transform.position;
            adjustedPosition.y = detectedPlane.transform.position.y + 0.7f; // Add height offset
            gameContent.transform.position = adjustedPosition;

            // Calculate forward direction from the camera
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0; // Keep it on the horizontal plane
            cameraForward.Normalize();

            // Position and rotate the target line
            Vector3 targetLinePosition = detectedPlane.center + cameraForward * targetLineOffset;
            targetLine.position = new Vector3(targetLinePosition.x, detectedPlane.transform.position.y, targetLinePosition.z);
            targetLine.rotation = Quaternion.LookRotation(detectedPlane.center - targetLine.position);

            // Position and rotate the spawn point
            Vector3 spawnPointPosition = detectedPlane.center + cameraForward * spawnPointOffset;
            spawnPoint.position = new Vector3(spawnPointPosition.x, detectedPlane.transform.position.y, spawnPointPosition.z);
            spawnPoint.rotation = Quaternion.LookRotation(targetLine.position - spawnPoint.position);

            // Adjust the camera for a better view
            AdjustCameraPosition(detectedPlane.transform.position);

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
            mainCamera.transform.position = new Vector3(
                planePosition.x,
                planePosition.y + cameraHeightOffset, // Adjust the height
                planePosition.z - 3f                  // Adjust the distance
            );

            // Rotate the camera to look directly at the game content
            mainCamera.transform.LookAt(planePosition + Vector3.up * 0.5f); // Look slightly above the plane center
        }
    }
}
