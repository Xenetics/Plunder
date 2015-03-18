using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour 
{
    [SerializeField]
    private Rigidbody entity;
    [SerializeField]
    private float speed = 150.0f;
    private float vSpeed;
    private float hSpeed;


    private bool isAlive = true;

	void Start () 
    {
	
	}
	
	void LateUpdate () 
    {
        vSpeed = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        hSpeed = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        vSpeed *= Time.deltaTime;
        hSpeed *= Time.deltaTime;
        transform.Translate(hSpeed, 0, vSpeed, null);
	}
}
