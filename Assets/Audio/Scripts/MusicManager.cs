using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicSource;

    private void Start()
    {
        if (musicSource != null)
        {
            musicSource.Play();
        }
    }
}
