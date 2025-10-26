using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using JetBrains.Annotations;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainButtons, levelsButtons, transitionCam, sheepCam, title;
    [SerializeField] bool gameOver, menu, end;

    public void Start()
    {
        Cursor.visible = true;
        if (gameOver)
        {
            StartCoroutine("GameOverTimer");
        }
        else if (menu)
        {
            StartCoroutine("MenuTimer");
        }
        else if (end)
        {
            StartCoroutine("EndTimer");
        }
    }
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
        StartCoroutine(AnimateLevel(buttonText));
    }

    public void Back()
    {
        mainButtons.SetActive(true);
        levelsButtons.SetActive(false);
    }

    public void Menu()
    {
        StartCoroutine(OpenScene(0));
    }
    public void Retry()
    {
        StartCoroutine(OpenScene(PlayerPrefs.GetInt("scene")));
    }

    public void StartAnimation(string coroutine)
    {
        StartCoroutine(coroutine);
    }
    public IEnumerator AnimatePlay()
    {
        yield return new WaitForSeconds(2f);
        Play();
    }
    public IEnumerator AnimateLevel(TMP_Text buttonText)
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(Convert.ToInt32(buttonText.text));
    }

    public IEnumerator GameOverTimer()
    {
        yield return new WaitForSeconds(0.05f);
        transitionCam.SetActive(false);
        yield return new WaitForSeconds(2f);
        levelsButtons.SetActive(true);
    }
    private IEnumerator MenuTimer()
    {
        yield return new WaitForSeconds(0.05f);
        transitionCam.SetActive(false);
        yield return new WaitForSeconds(2f);
        mainButtons.SetActive(true);
        title.SetActive(true);
    }
    private IEnumerator EndTimer()
    {
        yield return new WaitForSeconds(0.05f);
        sheepCam.SetActive(false);
        yield return new WaitForSeconds(2f);
        mainButtons.SetActive(true);
        title.SetActive(true);
    }

    private IEnumerator OpenScene(int scene)
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(scene);
    }
}
