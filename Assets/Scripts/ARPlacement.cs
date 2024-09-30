using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARPlaceOnPlane : MonoBehaviour
{
    public GameObject dragonPrefab;
    private GameObject spawnedDragon;
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        // Detect if user is touching the screen to place the dragon
        if (Input.touchCount > 0 && spawnedDragon == null)
        {
            Touch touch = Input.GetTouch(0);
            if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                spawnedDragon = Instantiate(dragonPrefab, hitPose.position, hitPose.rotation);
            }
        }
        
        if (spawnedDragon != null)
        {
            PreventExitingBounds();
        }
    }

    void PreventExitingBounds()
    {
        foreach (var plane in planeManager.trackables)
        {
            if (plane.alignment == PlaneAlignment.Vertical)
            {
                // Handle wall detection and boundaries here
            }
        }
    }
}