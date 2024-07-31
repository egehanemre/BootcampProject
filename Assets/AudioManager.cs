using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    public AudioClip drawCard;
    public AudioClip playSpell;
    public AudioClip shoot;
    public AudioClip background;
    public AudioClip addBullet;
    public AudioClip revoReload;

    private void Start()
    {
        musicSource.clip = background;
        //musicSource.Play();
    }

    public void PlaySfx(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
