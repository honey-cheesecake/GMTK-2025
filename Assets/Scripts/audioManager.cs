using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] float currentMasterVolume = 1f; // Initialize to 1
    [SerializeField] float previousVolume = 1f;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioMixerGroup sfxMixerGroup;
    [SerializeField] AudioMixerGroup musicMixerGroup;
    
    [Header("Music")]
    [SerializeField] AudioClip backgroundMusic;
    
    private AudioSource sfxAudioSource;  // Made private - no need to serialize
    private AudioSource musicAudioSource; // Made private - no need to serialize

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

    void Start()
    {
        InitializeSlider();
        ApplyCurrentVolume();
    }

    void InitializeSlider()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = currentMasterVolume;
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
        // Prevent log(0) which causes -infinity and add proper clamping
        float clampedValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        float dbValue = Mathf.Log10(clampedValue) * 20f;
        
        // Clamp the dB value to reasonable range (-80 to 0)
        dbValue = Mathf.Clamp(dbValue, -80f, 0f);
        
        audioMixer.SetFloat(volumeSource, dbValue);
        Debug.Log($"Setting {volumeSource} to {dbValue}dB (slider value: {sliderValue})");
        
        return sliderValue;
    }

    public void UpdateSliders()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = currentMasterVolume;
        }
    }

    public void muteMasterVolume()
    {
        if (currentMasterVolume != 0)
        {
            // Store current volume before muting
            previousVolume = currentMasterVolume;
            // Set to minimum audible value for the mixer, but 0 for UI
            audioMixer.SetFloat("MasterVolume", -80f); // -80dB is effectively muted
            currentMasterVolume = 0;
            Debug.Log("Muted audio");
        }
        else
        {
            // Restore previous volume
            currentMasterVolume = previousVolume;
            ChangeVolume("MasterVolume", currentMasterVolume);
            Debug.Log($"Unmuted audio to {currentMasterVolume}");
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
        if (clip != null && sfxAudioSource != null)
        {
            sfxAudioSource.PlayOneShot(clip);
        }
    }
}