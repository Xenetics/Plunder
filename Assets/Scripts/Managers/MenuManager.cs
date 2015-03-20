using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

// Used for all Main menu functionality and interaction with the game manager

public class MenuManager : MonoBehaviour 
{
    private float transitionSpeed = 2.0f;
    private float transitionTime = 2.0f;
    private bool transitioning = false;

    [SerializeField]
    private Canvas menuCanvas;
    [SerializeField]
    private Text soundText;
    [SerializeField]
    private GameObject creditPic;
    private bool credited = false;

    private string buttonPushed;

	void Start () 
    {
	}

	void Update() 
    {
        if(transitioning)
        {
            transitionTime -= Time.deltaTime;
            if(transitionTime <= 0.0f)
            {
                transitionTime = transitionSpeed;
                Transition(buttonPushed);
            }
        }
        CreditImage();
        SoundImage();
	}

    private void Transition(string button)  // swap enabled canvases
    {
        if (button == "play")
        {
            GameManager.Instance.NewGameState(GameManager.Instance.stateGamePlaying);
            Application.LoadLevel("game");
        }
    }

    public void PlayButton()
    {
        AudioManager.Instance.PlaySound("button");
        buttonPushed = "play";
        transitioning = true;
    }

    public void CreditButton()
    {
        AudioManager.Instance.PlaySound("button");
        credited = true;
    }

    public void OptionsButton()
    {
        AudioManager.Instance.PlaySound("button");
        transitioning = true;
    }

    public void SoundToggle()
    {
        AudioManager.Instance.PlaySound("button");
        AudioManager.Instance.ToggleMusic(!AudioManager.Instance.IsMusicOn());
        AudioManager.Instance.ToggleSound(!AudioManager.Instance.IsSoundOn());
    }

    private void SoundImage()
    {
        if(AudioManager.Instance.IsMusicOn())
        {
            soundText.color = Color.green;
            soundText.text = "Sound \n ON";
        }
        else
        {
            soundText.color = Color.red;
            soundText.text = "Sound \n OFF";
        }
    }

    private void CreditImage()
    {
        if(credited)
        {
            Vector3 pos = creditPic.transform.position;
            creditPic.transform.position = Vector3.Lerp(creditPic.transform.position, new Vector3(pos.x, 1.4f, pos.z), Time.deltaTime);
        }
    }
}