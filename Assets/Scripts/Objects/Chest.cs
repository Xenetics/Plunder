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
            Quaternion openAngle = Quaternion.Euler(-30, lid.transform.localRotation.y, lid.transform.localRotation.z);
            lid.transform.localRotation = Quaternion.Slerp(lid.transform.localRotation, openAngle, openSpeed * Time.deltaTime);
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
