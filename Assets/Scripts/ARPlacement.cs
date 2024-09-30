using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARPlaceOnPlane : MonoBehaviour
{
    public GameObject dragonPrefab;  // The dragon prefab
    private GameObject spawnedDragon;
    public ARRaycastManager raycastManager;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        // Detect if user is touching the screen
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;

                if (spawnedDragon == null)
                {
                    // If no dragon exists yet, spawn one at the detected plane
                    spawnedDragon = Instantiate(dragonPrefab, hitPose.position, hitPose.rotation);
                }
                else
                {
                    // If dragon is already placed, move it to the new position
                    spawnedDragon.transform.position = hitPose.position;
                }
            }
        }
    }
}