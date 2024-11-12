using UnityEngine;

public class DragonMovement : MonoBehaviour
{
    public Joystick joystick;
    public Animator dragonAnimator;
    public float groundSpeed = 2f;
    public float flySpeed = 3f;
    private bool isFlying = false;

    void Update()
    {
        float moveX = joystick.Horizontal;
        float moveZ = joystick.Vertical;

        Vector3 direction = new Vector3(moveX, 0, moveZ).normalized;

        if (direction.magnitude > 0.1f && !isFlying)
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

    public void Fly()
    {
        isFlying = true;
        dragonAnimator.SetBool("isFlying", true);
    }

    public void Land()
    {
        isFlying = false;
        dragonAnimator.SetBool("isFlying", false);
    }
}