using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour 
{
    [SerializeField]
    private GameObject lid;
    private bool state = false;

    private bool opening = false;
    private float angleOpen = -75.0f;
    private float speed = 1.0f;

    private bool closing = false;
    private float angleClosed = 0;

    private bool isOpen = false;

	void Start () 
    {
	}
	
	void Update () 
    {
	    if(opening)
        {
            Quaternion openAngle = Quaternion.Euler(angleOpen, lid.transform.localRotation.y, lid.transform.localRotation.z);
            lid.transform.localRotation = Quaternion.Slerp(lid.transform.localRotation, openAngle, speed * Time.deltaTime);
        }
        else if(closing)
        {
            Quaternion openAngle = Quaternion.Euler(angleClosed, lid.transform.localRotation.y, lid.transform.localRotation.z);
            lid.transform.localRotation = Quaternion.Slerp(lid.transform.localRotation, openAngle, speed * Time.deltaTime);
        }
	}

    public void ToggleOpen()
    {
        if(state)
        {
            state = false;
        }
        else
        {
            state = true;
        }

        if(state)
        {
            opening = true;
            closing = false;
        }
        else
        {
            closing = true;
            opening = false;
        }
    }

    public void ForceOpen()
    {
        opening = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            isOpen = true;
            opening = true;
            if (isOpen)
            {
                InGameUIManager.Instance.AddTreasure();
            }
            if(gameObject.name == "Chest_W")
            {
                LevelManager.Instance.Unlock();
            }
        }
    }
}
