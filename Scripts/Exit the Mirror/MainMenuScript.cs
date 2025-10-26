using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] GameObject mainButtons, levelsButtons;

    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Levels()
    {
        mainButtons.SetActive(false);
        levelsButtons.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LevelSelected(TMP_Text buttonText)
    {
        SceneManager.LoadScene(Convert.ToInt32(buttonText.text));
    }

    public void Back()
    {
        mainButtons.SetActive(true);
        levelsButtons.SetActive(false);
    }
}
