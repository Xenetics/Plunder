using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour 
{
    [SerializeField]
    private GameObject entity;

	// Update is called once per frame
	void Update () 
    {
        this.gameObject.transform.LookAt(entity.transform);
	}
}
