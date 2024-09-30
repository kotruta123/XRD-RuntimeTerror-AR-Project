using UnityEngine;

public class DragonMovement : MonoBehaviour
{
    public Joystick joystick;       // Reference to the joystick in the UI
    public Animator dragonAnimator; // Reference to the dragon's Animator
    public float groundSpeed = 5f;  // Speed for walking/running
    public float flySpeed = 10f;    // Speed for flying

    private bool isFlying = false;

    void Update()
    {
        // Get joystick input for movement
        float moveX = joystick.Horizontal;
        float moveZ = joystick.Vertical;

        Vector3 movement = new Vector3(moveX, 0, moveZ).normalized;

        // Move the dragon on the ground if not flying
        if (movement.magnitude > 0.1f && !isFlying)
        {
            transform.Translate(movement * groundSpeed * Time.deltaTime, Space.World);
            dragonAnimator.SetBool("isWalking", true);
        }
        else
        {
            dragonAnimator.SetBool("isWalking", false);
        }
    }

    public void Fly() // Called by Fly Button
    {
        isFlying = true;
        dragonAnimator.SetBool("isFlying", true);
    }

    public void Land() // Called by Land Button
    {
        isFlying = false;
        dragonAnimator.SetBool("isFlying", false);
    }
}