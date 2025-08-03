using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] float currentMasterVolume;
    [SerializeField] float previousVolume = 1f;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioMixerGroup sfxMixerGroup;
    [SerializeField] AudioMixerGroup musicMixerGroup;
    
    [Header("Music")]
    [SerializeField] AudioClip backgroundMusic;
    
    private AudioSource sfxAudioSource;  // For sound effects
    private AudioSource musicAudioSource; // For background music

    

    AudioSource audioSource;

    void Awake()
    {
        // Singleton pattern - keep only one audioManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            LoadVolumeSettings();
            SetupAudioSources();
            StartBackgroundMusic();
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void SetupAudioSources()
    {
        
        AudioSource[] sources = GetComponents<AudioSource>();
        
        //if not audio source make both
        if (sources.Length == 0)
        {
            sfxAudioSource = gameObject.AddComponent<AudioSource>();
            musicAudioSource = gameObject.AddComponent<AudioSource>();
        }
        //if none for music make one for music
        else if (sources.Length == 1)
        {
            sfxAudioSource = sources[0];
            musicAudioSource = gameObject.AddComponent<AudioSource>();
        }
       
        else
        {
            sfxAudioSource = sources[0];
            musicAudioSource = sources[1];
        }
        

        if (sfxMixerGroup != null)
            sfxAudioSource.outputAudioMixerGroup = sfxMixerGroup;
        

        if (musicMixerGroup != null)
            musicAudioSource.outputAudioMixerGroup = musicMixerGroup;
        
        musicAudioSource.loop = true; 
        musicAudioSource.playOnAwake = false;
    }

    void StartBackgroundMusic()
    {
        if (backgroundMusic != null && musicAudioSource != null && !musicAudioSource.isPlaying)
        {
            musicAudioSource.clip = backgroundMusic;
            musicAudioSource.Play();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeSlider();
        ApplyCurrentVolume();

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitializeSlider()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = currentMasterVolume;
            // Connect slider to the volume change method
            masterVolumeSlider.onValueChanged.AddListener(ChangeMasterVolume);
        }
    }

    public void ChangeMasterVolume(float newVolume)
    {
        currentMasterVolume = ChangeVolume("MasterVolume", newVolume);
        SaveVolumeSettings();
    }

    float ChangeVolume(string volumeSource, float sliderValue)
    {
        audioMixer.SetFloat(volumeSource, Mathf.Log10(sliderValue) * 20); // log operation is to compensate for how audio mixing works to allow for a smoother volume change
        return sliderValue;

        //audioMixer.SetFloat()
    }

    public void UpdateSliders()
    {
        masterVolumeSlider.value = currentMasterVolume;

    }

    public void muteMasterVolume()
    {
        if (currentMasterVolume != 0)
        {
            previousVolume = currentMasterVolume;
            currentMasterVolume = ChangeVolume("MasterVolume", 0.0001f);
            currentMasterVolume = 0;
        }
        else
        {
            currentMasterVolume = ChangeVolume("MasterVolume", previousVolume);
        }
        UpdateSliders();
        SaveVolumeSettings();
    }

    void ApplyCurrentVolume()
    {
        ChangeVolume("MasterVolume", currentMasterVolume);
    }

    void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", currentMasterVolume);
        PlayerPrefs.SetFloat("PreviousVolume", previousVolume);
        PlayerPrefs.Save();
    }

    void LoadVolumeSettings()
    {
        currentMasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        previousVolume = PlayerPrefs.GetFloat("PreviousVolume", 1f);
    }
    
    public void PlayCatSound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

}
