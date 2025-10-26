using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    public AudioClip[] musics;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex >= 5)
        {
            audioSource.clip = musics[0];
            audioSource.Play();
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            audioSource.clip = musics[1];
            audioSource.Play();
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            audioSource.clip = musics[2];
            audioSource.Play();
        }
        else if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            audioSource.clip = musics[3];
            audioSource.Play();
        }
        else if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            audioSource.clip = musics[4];
            audioSource.Play();
        }
    }

    public void MuteMusic()
    {
        audioSource.mute = true;
    }
    public void UnMuteMusic()
    {
        audioSource.mute = false;
    }
}
