using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource sfxSource;
    [SerializeField] public AudioSource deathSource; // New AudioSource for death sound

    public AudioClip drawCard;
    public AudioClip playSpell;
    public AudioClip shoot;
    public AudioClip battleMusic;
    public AudioClip menuMusic;
    public AudioClip deathMusic;
    public AudioClip addBullet;
    public AudioClip revoReload;
    public AudioClip purchaseCard;
    public AudioClip noMoney;
    public AudioClip death;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        PlayMusic(menuMusic);
    }

    public void PlaySfx(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
        musicSource.loop = true;
    }

    public void PlayDeath()
    {
        StartCoroutine(PlayDeathCoroutine());
    }

    private IEnumerator PlayDeathCoroutine()
    {
        musicSource.Pause();
        deathSource.clip = deathMusic;
        deathSource.Play();
        yield return new WaitForSeconds(deathMusic.length);
        PlayMusic(menuMusic); // Change to menu music and start from the beginning
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSfx()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SfxVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
