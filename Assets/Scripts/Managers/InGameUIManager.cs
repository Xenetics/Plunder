﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour 
{
    private static InGameUIManager instance = null;
    public static InGameUIManager Instance { get { return instance; } }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        maxBarLength = visionBar.rectTransform.sizeDelta.x;
        originalBarPos = visionBar.rectTransform.localPosition.x;
    }

    public bool paused { get; set; }
    [SerializeField]
    private Canvas UICanvas;
    [SerializeField]
    private Image transitionLayer;
    private float transitionSpeed = 5.0f;
    private bool transitioning = false;
    private bool transitioningIn = true;
    [SerializeField]
    private Image visionBar;
    private float maxBarLength;
    private float originalBarPos;
    [SerializeField]
    private Text timerText;
    private float time = 0f;

    [SerializeField]
    private Canvas PausedCanvas;

    [SerializeField]
    private Canvas GameOverCanvas;

    [SerializeField]
    private Canvas InstructionCanvas;
    [SerializeField]
    private Image InstructionImage;
    private bool tutDone = false;

    [SerializeField]
    private Text scoreText;
    private int _Treasure = 0;

    [SerializeField]
    private Text doorText;

    public void AddTreasure()
    {
        _Treasure++;
        scoreText.text = "Treasure: " + _Treasure;
    }

    public void OpenDoor()
    {
        doorText.text = "Door: Open"; 
    }

    public void DoorReset()
    {
        doorText.text = "Door: Closed"; 
    }

	void Start () 
    {

	}
	
	void Update () 
    {
        Transitions();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                Paused(false);
            }
            else
            {
                Paused(true);
            }
        }

        if (!paused)
        {
            if (GameManager.WhatState() == "playing")
            {
                
            }
            Timer(); // move into the above IF
            //VisionBar();
        }
	}

    private void VisionBar()
    {
        //float scale = RunnerPlayerController.Instance.VisionScale();

        //Vector3 newLength = new Vector3(maxBarLength * scale, visionBar.rectTransform.sizeDelta.y, 1);
        //visionBar.rectTransform.sizeDelta = newLength;
        //Vector3 newPosition = new Vector3(originalBarPos - (maxBarLength - visionBar.rectTransform.sizeDelta.x) * 0.5f, visionBar.rectTransform.localPosition.y, 0);
        //visionBar.rectTransform.localPosition = newPosition;
    }

    private void Timer()
    {
        time += Time.deltaTime;

        int temp = Mathf.FloorToInt(time);

        int tempMinute = Mathf.FloorToInt(time / 60);
        int tempSecond;
        if (time > 60)
        {
            tempSecond = Mathf.FloorToInt(time % (tempMinute * 60));
        }
        else if (time < 60)
        {
            tempSecond = Mathf.FloorToInt(time);
        }
        else
        {
            tempSecond = 00;
        }

        string minute;
        string second;

        if (tempMinute < 10)
        {
            minute = "0" + tempMinute;
        }
        else
        {
            minute = tempMinute.ToString();
        }

        if (tempSecond < 10)
        {
            second = "0" + tempSecond;
        }
        else
        {
            second = tempSecond.ToString();
        }

        timerText.text = minute + ":" + second;
    }

    public void Paused(bool isPaused)
    {
        if (isPaused)
        {
            paused = true;
            Time.timeScale = 0.0f;
            //UICanvas.gameObject.SetActive(false);
            PausedCanvas.gameObject.SetActive(true);
        }
        else
        {
            paused = false;
            Time.timeScale = 1.0f;
            PausedCanvas.gameObject.SetActive(false);
            //UICanvas.gameObject.SetActive(true);
        }
    }

    public void Reset()
    {
        AudioManager.Instance.PlaySound("button");
        Paused(false);
        time = 0;
        GameOverCanvas.gameObject.SetActive(false);
        UICanvas.gameObject.SetActive(true);
        GameManager.Instance.NewGameState(GameManager.Instance.stateGamePlaying);
    }

    public void Quit()
    {
        AudioManager.Instance.PlaySound("button");
        Paused(false);
        GameManager.Instance.NewGameState(GameManager.Instance.stateGameMenu);
        Application.LoadLevel("menu");
    }

    public void EndGame()
    {
        GameManager.Instance.NewGameState(GameManager.Instance.stateGameLost);
        transitioning = true;
    }

    private void TallyScore()
    {
        
    }

    private void SaveScore()
    {

    }

    private void Transitions()
    {
        if (transitioning)
        {
            // ADD TRANSITION HERE
            transitionLayer.color = Color.Lerp(transitionLayer.color, Color.white, Time.deltaTime * transitionSpeed);

            if (Vector4.Distance(transitionLayer.color, Color.white) < 0.1f)
            {
                transitionLayer.color = Color.white;
                transitioning = false;
                GameManager.Instance.NewGameState(GameManager.Instance.stateGameMenu);
                Application.LoadLevel("menu");
            }
        }

        if (transitioningIn)
        {
            transitionLayer.color = Color.Lerp(transitionLayer.color, new Color(0.0f, 0.0f, 0.0f, 0.0f), Time.deltaTime * transitionSpeed);

            if (Vector4.Distance(transitionLayer.color, new Color(0.0f, 0.0f, 0.0f, 0.0f)) < 0.05f)
            {
                transitionLayer.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                transitioningIn = false;
            }
        }
    }
}
