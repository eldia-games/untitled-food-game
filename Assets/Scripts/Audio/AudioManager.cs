using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private float musicFade;
    [SerializeField] private AudioSource sfxUIAudioSource, musicAudioSource, sfxEnemyAudioSource, sfxPlayerAudioSource, otherAudioSource;
    [SerializeField] private AudioClip[] sfxClips, enemyWalkClips, enemyRunClips, enemyAttackClips;
    [SerializeField] private AudioClip[] backgroundMusicClips;
    [SerializeField] private AudioMixer masterAudioMixer;

    private bool isMusicPlaying;
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Se intento crear una segunda instancia del objeto Audio Manager");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        
    }

    // Method to play clip by sound
    public void PlaySoundByClip(AudioClip clip) { 
        otherAudioSource.PlayOneShot(clip);
    }

    // Method get a bool of music playing
    public bool GetIsMusicPlayingBool() { 
        return isMusicPlaying;
    }

    // Method to mute background music
    public void ToggleMusic()
    {
        musicAudioSource.mute = !musicAudioSource.mute;
    }
    public void UnuteMusic()
    {
        musicAudioSource.mute= false;
    }
    public void MuteMusic()
    {
        musicAudioSource.mute = true;
    }

    public void ChangeMasterVolume(float volume)
    {
        masterAudioMixer.SetFloat("MasterVol", volume);
    }
    public void ChangeMusicVolume(float volume)
    {
        masterAudioMixer.SetFloat("MusicVol", volume);
    }
    public void ChangeSFXVolume(float volume)
    {
        masterAudioMixer.SetFloat("SfxVol", volume);
    }

    // Method to stop background music
    public void StopBackgroundMusic()
    {
        musicAudioSource.Stop();
        isMusicPlaying = false;
    }

    // Method to pause background music
    public void PauseBackgroundMusic()
    {
        musicAudioSource.Pause();
        isMusicPlaying = false;
    }

    // Method to resume background music
    public void ResumeBackgroundMusic()
    {
        musicAudioSource.UnPause();
        isMusicPlaying = true;
    }
    public void PlayMenuMusic()
    {
        if (!isMusicPlaying)
            PlayMusicByIndex(0);
        else
        {
            ChangeBackgroundMusic(0);
        }
    }

    public void PlayLobbyMusic()
    {
        ChangeBackgroundMusic(1);
    }

    public void PlayMapMusic()
    {
        ChangeBackgroundMusic(2);
    }

    public void PlayChamberMusic()
    {
        ChangeBackgroundMusic(3);
    }

    public void PlayEndGameMusic()
    {
        ChangeBackgroundMusic(4);
    }

    public void PlaySFXClick()
    {
        PlaySFXByIndex(0);
    }

    public void PlaySFXBack()
    {
        PlaySFXByIndex(1);
    }

    public void PlaySFXClose()
    {
        PlaySFXByIndex(2);
    }

    public void PlaySFXConfirmation()
    {
        PlaySFXByIndex(3);
    }

    public void PlaySFXOpen()
    {
        PlaySFXByIndex(4);
    }

    public void PlaySFXSelect()
    {
        PlaySFXByIndex(5);
    }


    // Method to play sfx by index
    private void PlaySFXByIndex(int sfxIndex)
    {
        if (sfxIndex >= 0 && sfxIndex < sfxClips.Length)
        {
            sfxUIAudioSource.PlayOneShot(sfxClips[sfxIndex]);
        }
        else
        {
            Debug.LogWarning("SFX index out of range.");
        }
    }

    // Method to play music by index
    private void PlayMusicByIndex(int musicIndex)
    {
        if (musicIndex >= 0 && musicIndex < backgroundMusicClips.Length)
        {
            musicAudioSource.PlayOneShot(backgroundMusicClips[musicIndex]);
            isMusicPlaying = true;
        }
        else
        {
            Debug.LogWarning("Music index out of range.");
        }
    }

    // Method to change background music with a fase
    private void ChangeBackgroundMusic(int musicIndex)
    {
        if (musicIndex >= 0 && musicIndex < backgroundMusicClips.Length)
        {
            AudioClip clip = backgroundMusicClips[musicIndex];
            if (clip != null && clip != musicAudioSource.clip)
            {
                StartCoroutine(FadeOutAndPlay(clip, musicFade));
            }
        }
        else
        {
            Debug.LogWarning("Music index out of range.");
        }
    }

    // Enumerator to control de fade effect
    private IEnumerator FadeOutAndPlay(AudioClip newClip, float fadeDuration)
    {
        float startVolume = musicAudioSource.volume;

        // Fade out
        while (musicAudioSource.volume > 0)
        {
            musicAudioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicAudioSource.Stop();
        musicAudioSource.clip = newClip;
        musicAudioSource.Play();

        // Fade in
        while (musicAudioSource.volume < startVolume)
        {
            musicAudioSource.volume += startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }
    }

}
