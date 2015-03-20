using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour 
{
    [SerializeField]
    private GameObject door;
    private float doorSpeed = 2.0f;

    private bool isLocked = true;

	void Start () 
    {
	    
	}
	
	void Update () 
    {
	    if(!isLocked)
        {
            Vector3 pos = door.transform.localPosition;
            door.transform.localPosition = Vector3.Lerp(door.transform.localPosition, new Vector3(pos.x, 2.0f, pos.z), doorSpeed * Time.deltaTime);
        }
	}

    public void ForceOpen()
    {
        isLocked = false;
    }
}
