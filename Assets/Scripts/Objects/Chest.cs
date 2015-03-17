using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour 
{
    [SerializeField]
    private GameObject lid;
    [SerializeField]
    private bool opening = false;
    private float openSpeed = 1.0f;

	void Start () 
    {
	    
	}
	
	void Update () 
    {
	    if(opening)
        {
            if(lid.transform.rotation.x > -30)
            {
                Quaternion openAngle = Quaternion.Euler(-30, lid.transform.rotation.y, lid.transform.rotation.z);
                lid.transform.rotation = Quaternion.Slerp(lid.transform.rotation, openAngle, openSpeed * Time.deltaTime);
            }
        }
	}

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            opening = true;
        }
    }
}
