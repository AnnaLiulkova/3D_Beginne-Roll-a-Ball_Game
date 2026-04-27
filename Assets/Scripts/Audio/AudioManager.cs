using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip clickSound;
    public AudioClip pickupNormalSound;
    public AudioClip pickupSuperSound;
    public AudioClip winSound;
    public AudioClip loseSound;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true; 
            musicSource.Play();
        }
        
        float savedMusicVol = PlayerPrefs.GetFloat("MusicVol", 1f);
        float savedSFXVol = PlayerPrefs.GetFloat("SFXVol", 1f);
        SetMusicVolume(savedMusicVol);
        SetSFXVolume(savedSFXVol);

        musicSource.mute = PlayerPrefs.GetInt("MusicMute", 1) == 0;
        sfxSource.mute = PlayerPrefs.GetInt("SFXMute", 1) == 0;
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVol", volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVol", volume);
    }

    public void SetMusicMute(bool isMuted) 
    { 
        musicSource.mute = isMuted; 
    }

    public void SetSFXMute(bool isMuted) 
    { 
        sfxSource.mute = isMuted; 
    }

    public void PlaySFX(AudioClip clip) 
    { 
        if (clip != null) sfxSource.PlayOneShot(clip); 
    }

    public void PlayClick() => PlaySFX(clickSound);
    public void PlayPickupNormal() => PlaySFX(pickupNormalSound);
    public void PlayPickupSuper() => PlaySFX(pickupSuperSound);
    public void PlayWin() => PlaySFX(winSound);
    public void PlayLose() => PlaySFX(loseSound);
}