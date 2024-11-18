using UnityEngine;
using UnityEngine.XR.ARFoundation; // Required for ARPlane and ARPlaneManager
using UnityEngine.XR.ARSubsystems;

public class DragonMovement : MonoBehaviour
{
    public Joystick joystick; // Reference to the joystick
    public Animator dragonAnimator; // Animator for the dragon
    public float groundSpeed = 2f; // Speed for walking
    public float flySpeed = 3f; // Speed for flying
    public float heightSpeed = 2f; // Speed for height control
    private bool isFlying = true; // State of the dragon
    private Rigidbody rb; // Rigidbody for physics
    public ARPlaneManager planeManager; // Reference to ARPlaneManager

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Fly(); // Start in flying mode
    }

    void Update()
    {
        float moveX = joystick.Horizontal; // Horizontal joystick movement
        float moveZ = joystick.Vertical; // Vertical joystick movement

        if (isFlying)
        {
            FlyMovement(moveX, moveZ);
        }
        else
        {
            WalkMovement(moveX, moveZ);
        }
    }

    void WalkMovement(float moveX, float moveZ)
    {
        Vector3 direction = new Vector3(moveX, 0, moveZ).normalized;

        if (direction.magnitude > 0.1f)
        {
            Vector3 targetPosition = transform.position + direction * groundSpeed * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * groundSpeed);
            dragonAnimator.SetBool("isWalking", true);
        }
        else
        {
            dragonAnimator.SetBool("isWalking", false);
        }
    }

    void FlyMovement(float moveX, float moveZ)
    {
        Vector3 horizontalMovement = new Vector3(moveX, 0, moveZ).normalized * flySpeed * Time.deltaTime;
        float verticalMovement = joystick.Vertical * heightSpeed * Time.deltaTime;

        Vector3 targetPosition = transform.position + horizontalMovement + new Vector3(0, verticalMovement, 0);

        transform.position = targetPosition;
        dragonAnimator.SetBool("isFlying", true);
    }

    public void Fly()
    {
        isFlying = true;
        rb.useGravity = false; // Disable gravity during flight
        dragonAnimator.SetBool("isFlying", true);
    }

    public void Land()
    {
        isFlying = false;
        rb.useGravity = true; // Enable gravity for walking
        dragonAnimator.SetBool("isFlying", false);

        ARPlane closestPlane = GetClosestHorizontalPlane();
        if (closestPlane != null)
        {
            Vector3 dragonPosition = transform.position;
            transform.position = new Vector3(dragonPosition.x, closestPlane.transform.position.y, dragonPosition.z);
            Debug.Log($"Dragon landed on plane: ID = {closestPlane.trackableId}, Position = {closestPlane.transform.position}");
        }
        else
        {
            Debug.LogWarning("No horizontal plane detected. Landing at default height.");
            transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
        }
    }


    ARPlane GetClosestHorizontalPlane()
    {
        float closestDistance = Mathf.Infinity;
        ARPlane closestPlane = null;

        foreach (var plane in planeManager.trackables)
        {
            Debug.Log($"Plane detected: ID = {plane.trackableId}, Alignment = {plane.alignment}, Center = {plane.center}");

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
        else
        {
            Debug.Log($"Closest horizontal plane: ID = {closestPlane.trackableId}, Distance = {closestDistance}");
        }

        return closestPlane;
    }

}
