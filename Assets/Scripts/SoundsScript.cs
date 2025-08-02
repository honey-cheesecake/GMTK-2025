using UnityEngine;

public class SoundsScript : MonoBehaviour
{
    public static SoundsScript Instance;

    AudioSource audioSource;

    [SerializeField] public AudioSource yippie;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //yippie.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayCatSound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, 0.5f);
        }
    }

}
