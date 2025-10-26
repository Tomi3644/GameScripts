using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManagerScript : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    public UnityEvent pause;
    public UnityEvent resume;

    void Awake()
    {
        Time.timeScale = 1f;
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
            if (PlayerPrefs.GetInt("Training") == 0) PlayerPrefs.SetInt("CurrentLevelID", SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void Update()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            if (GameObject.Find("Player").GetComponent<PlayerInput>().actions["Pause"].triggered && SceneManager.GetActiveScene().buildIndex != 0)
            {
                Pause();
            }
        }
    }
    public void Pause()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        Cursor.visible = true;
        MuteMusic();
        pause.Invoke();
    }

    public void RestartLevel()
    {
        if (PlayerPrefs.GetInt("Training") == 0) PlayerPrefs.SetInt("CurrentLevelID", SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        Cursor.visible = false;
        resume.Invoke();
        UnMuteMusic();
    }
    public void Menu()
    {
        if (PlayerPrefs.GetInt("Training") == 0) PlayerPrefs.SetInt("CurrentLevelID", SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.SetInt("Training", 0);
        PlayerPrefs.Save();
        UnMuteMusic();
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }

    public void NextLevel()
    {
        /*if (SceneManager.GetActiveScene().buildIndex == 10) GameObject.Find("MusicManager").GetComponent<MusicManager>().PlayMusic(4);
        else if (SceneManager.GetActiveScene().buildIndex == 2) GameObject.Find("MusicManager").GetComponent<MusicManager>().PlayMusic(3);*/
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void FinishGame(int finishType)
    {
        PlayerPrefs.SetInt("CurrentLevelID", 0);
        PlayerPrefs.SetInt("GameType", 0);
        PlayerPrefs.SetInt("IsBroken", 0);
        if (finishType == 1)
        {
            PlayerPrefs.SetInt("normalUnlocked", 1);
            PlayerPrefs.SetInt("GameType", 2);
            //GameObject.Find("MusicManager").GetComponent<MusicManager>().PlayMusic(3);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (finishType == 2)
        {
            PlayerPrefs.SetInt("hardcoreUnlocked", 1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (finishType == 3)
        {
            PlayerPrefs.SetInt("hardcoreFinished", 1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else SceneManager.LoadScene(0);
    }

    public void GameEnd()
    {
        UnMuteMusic();
        if (PlayerPrefs.GetInt("GameType") == 2)
        {
            FinishGame(2);
        }
        else if (PlayerPrefs.GetInt("GameType") == 3)
        {
            FinishGame(3);
        }
    }

    public void MuteMusic()
    {
        try
        {
            GameObject Music = GameObject.Find("MusicManager");
            Music.GetComponent<MusicManager>().MuteMusic();
        }
        catch (NullReferenceException e)
        {
            print(e);
        }
    }
    public void UnMuteMusic()
    {
        try
        {
            GameObject Music = GameObject.Find("MusicManager");
            Music.GetComponent<MusicManager>().UnMuteMusic();
        }
        catch (NullReferenceException e)
        {
            print(e);
        }
    }
}
