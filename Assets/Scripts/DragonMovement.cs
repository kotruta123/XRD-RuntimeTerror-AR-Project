using UnityEngine;

public class DragonMovement : MonoBehaviour
{
    public Joystick joystick;
    public Animator dragonAnimator;
    public float groundSpeed = 2f;
    public float flySpeed = 3f;
    private bool isFlying = false;
    private AudioSource _audioSource;

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
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource != null)
        {
            _audioSource.pitch = 0.85f;
            _audioSource.Play();
        }
    }

    public void Land()
    {
        isFlying = false;
        dragonAnimator.SetBool("isFlying", false);
        _audioSource.Stop();
    }
}