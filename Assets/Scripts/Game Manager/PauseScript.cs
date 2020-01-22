﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class PauseScript : MonoBehaviour
{
    // Start is called before the first frame update
    bool isPaused = false;
    bool changedButton = false;
    public Canvas pauseCanvas;
    [HideInInspector] public bool canPause = true;

    [SerializeField] GameObject resumeButton;
    [SerializeField] GameObject quitButton;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause") && canPause)
        {
            btn_Resume();
        }

        if (isPaused)
        {
            if (!changedButton)
            {
                changedButton = true;
                EventSystem.current.SetSelectedGameObject(resumeButton);
                resumeButton.GetComponent<Button>().OnSelect(null); //allows the button to be selected again
            }
        }
    }

    void PauseGame()
    {

        pauseCanvas.gameObject.SetActive(true);

        isPaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0;
        GameObject.Find("Character").GetComponent<PlayerMovement>().StopMovement(true);

        pauseCanvas.enabled = true;
    }

    void ResumeGame()
    {
        pauseCanvas.gameObject.SetActive(false);
        changedButton = false;

        isPaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1;
        StartCoroutine(LetPlayerMove());

        pauseCanvas.enabled = false;
    }

    IEnumerator LetPlayerMove()
    {
        //this avoids player jumping right after RESUME GAME selection
        yield return new WaitForSeconds(0.5f);
        GameObject.Find("Character").GetComponent<PlayerMovement>().StopMovement(false);
    }

    public void btn_Resume()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void btn_Quit()
    {
        //This function now quits the app, but it should take you to main menu when it's done!
        Debug.Log("Quit has been pressed!");
        PlayerPrefs.DeleteKey("hasDoneMain");
        Application.Quit();
    }


}
