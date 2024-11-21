using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class BoundaryManager : MonoBehaviour
{
    public ARPlaneManager planeManager;

    private List<Bounds> roomBounds = new List<Bounds>();

    void Update()
    {
        UpdateRoomBounds();
    }

    // Collect bounds of all detected horizontal and vertical planes
    void UpdateRoomBounds()
    {
        roomBounds.Clear();

        foreach (var plane in planeManager.trackables)
        {
            if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalUp || plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.Vertical)
            {
                Bounds bounds = new Bounds(plane.center, new Vector3(plane.extents.x * 2, 0.1f, plane.extents.y * 2));
                roomBounds.Add(bounds);
            }
        }
    }

    // Check if a position is inside the room boundaries
    public bool IsInsideRoom(Vector3 position)
    {
        foreach (var bound in roomBounds)
        {
            if (bound.Contains(new Vector3(position.x, bound.center.y, position.z))) // Ignore Y-axis
            {
                return true;
            }
        }
        return false;
    }

    // Get a random position inside the room boundaries
    public Vector3 GetRandomPositionInsideRoom()
    {
        if (roomBounds.Count == 0)
        {
            Debug.LogWarning("No room boundaries detected!");
            return Vector3.zero;
        }

        Bounds selectedBound = roomBounds[Random.Range(0, roomBounds.Count)];
        float x = Random.Range(selectedBound.min.x, selectedBound.max.x);
        float z = Random.Range(selectedBound.min.z, selectedBound.max.z);

        return new Vector3(x, selectedBound.center.y, z); // Keep the Y position consistent
    }
}