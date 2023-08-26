using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour 
{ 
    public static SoundManager Instance; // Singleton instance of the SoundManager
    [Header("Audio Sources")] 
    public AudioSource backgroundMusicSource; // Background music audio source
    public AudioSource sfxSource; // Sound effects audio source
    [Header("SoundFX")]
    public AudioClip playerJumpSound; // Sound clip for jumping
    public AudioClip playerLandSound; // Sound clip for landing
    public AudioClip playerRunSound; // Sound clip for running
    public AudioClip playerDeathByExplosionSound; // Sound clip for attacking
    public AudioClip gameOverSound; // Sound clip for game over
    public AudioClip winSound; // Sound clip for winning
    public AudioClip collectItemSound; // Sound clip for collecting items
    
    [Header("Music")]
    public AudioClip startGameSong; // Sound clip for starting the game
    public AudioClip level1Song; // Sound clip for level 1
    public AudioClip level2Song; // Sound clip for level 2
    // adding a way to change the volume of the music and the sound effects seperatly
    [Header("Volume")]
    [Range(0.0f, 1.0f)]
    public float musicVolume = 1.0f;
    [Range(0.0f, 1.0f)]
    public float sfxVolume = 1.0f;
    
    
    // Additional sound clips can be added here.
    
    private void Awake()
    {
        // Singleton pattern to ensure only one SoundManager instance exists.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Play the background music
    public void PlayBackgroundMusic(AudioClip musicClip)
    {
         backgroundMusicSource.clip = musicClip;
         backgroundMusicSource.loop = true;
         backgroundMusicSource.Play();
    }

    // Play a sound effect
    public void PlaySFX(AudioClip sfxClip)
    { 
        sfxSource.PlayOneShot(sfxClip);
    }
    
    // Call this method from the PlayerController when the player runs
    public void PlayRunSound()
    { 
        PlaySFX(playerRunSound);
    }
    
    // Call this method from the PlayerController when the player jumps
    public void PlayJumpSound()
    { 
        PlaySFX(playerJumpSound);
    }

    // Call this method from the PlayerController when the player lands
    public void PlayLandSound()
    { 
        PlaySFX(playerLandSound);
    }

    // Call this method from other scripts when an item is collected
    public void PlayCollectItemSound()
    { 
        PlaySFX(collectItemSound);
    }
    
    // call this method from the Death Manager when the player dies
    public void PlayPlayerDeathByExplosionSound()
    {
        PlaySFX(playerDeathByExplosionSound);
    }
    
    // set the volume of the music
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        backgroundMusicSource.volume = musicVolume;
    }
    
    // set the volume of the sound effects
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = sfxVolume;
    }
}
