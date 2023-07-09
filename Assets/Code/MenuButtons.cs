using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip buttonClickClip;

    public void PlayGame()
    {
        audioSource.clip = buttonClickClip;
        audioSource.Play();
        SceneManager.LoadScene("Game");
    }
}
