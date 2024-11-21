using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARPlaceOnPlane : MonoBehaviour
{
    public GameObject dragonPrefab; // Dragon to be instantiated
    private GameObject spawnedDragon; // Reference to the instantiated dragon
    public ARRaycastManager raycastManager; // Reference to ARRaycastManager
    public ARPlaneManager planeManager; // Reference to ARPlaneManager

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        if (raycastManager == null || planeManager == null)
        {
            Debug.LogError("Raycast Manager or Plane Manager is not assigned!");
            return;
        }

        // Detect touch input for dragon placement
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Check if the dragon is already in the scene
            if (spawnedDragon == null && GameObject.Find("Purple(Clone)") == null) 
            {
                if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;
                    spawnedDragon = Instantiate(dragonPrefab, hitPose.position, hitPose.rotation);
                    spawnedDragon.name = "Purple"; // Ensure consistent naming
                    Debug.Log("Dragon spawned at position: " + hitPose.position);
                }
            }
        }
    }





    void PreventExitingBounds()
    {
        foreach (var plane in planeManager.trackables)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp || plane.alignment == PlaneAlignment.Vertical)
            {
                Vector3 dragonPosition = spawnedDragon.transform.position;

                // Horizontal planes (ground)
                if (plane.alignment == PlaneAlignment.HorizontalUp)
                {
                    Vector2 planeCenter = new Vector2(plane.center.x, plane.center.z);
                    float planeExtentX = plane.extents.x;
                    float planeExtentZ = plane.extents.y;

                    // Clamp the dragon's position within the plane's boundary
                    dragonPosition.x = Mathf.Clamp(dragonPosition.x, planeCenter.x - planeExtentX, planeCenter.x + planeExtentX);
                    dragonPosition.z = Mathf.Clamp(dragonPosition.z, planeCenter.y - planeExtentZ, planeCenter.y + planeExtentZ);
                }

                // Vertical planes (walls)
                if (plane.alignment == PlaneAlignment.Vertical)
                {
                    Vector3 wallPosition = plane.transform.position;

                    // Prevent the dragon from passing through walls (example logic)
                    if (Vector3.Distance(dragonPosition, wallPosition) < 0.5f)
                    {
                        dragonPosition = spawnedDragon.transform.position; // Stop movement
                    }
                }

                spawnedDragon.transform.position = dragonPosition;
            }
        }
    }
    
    ARPlane GetClosestHorizontalPlane()
    {
        float closestDistance = Mathf.Infinity;
        ARPlane closestPlane = null;

        foreach (var plane in planeManager.trackables)
        {
            Debug.Log($"Detected Plane: {plane.trackableId}, Alignment: {plane.alignment}");

            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                float distance = Vector3.Distance(transform.position, plane.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlane = plane;
                }
            }
        }

        if (closestPlane == null)
        {
            Debug.LogWarning("No horizontal planes found.");
        }

        return closestPlane;
    }


    
}
