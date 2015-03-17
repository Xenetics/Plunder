using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour 
{
    [SerializeField]
    private GameObject entity;

    private float transitionSpeed = 2.0f;

    void Awake()
    {
        
    }

	void Start () 
    {
        entity = FindObjectOfType<PlayerControls>().gameObject;
	}
	
	void Update () 
    {
        this.gameObject.transform.LookAt(entity.transform);

        Vector3 camZ = new Vector3(0, 0, this.transform.position.z);
        Vector3 playerZ = new Vector3(0, 0, entity.transform.position.z);
        Vector3 camX = new Vector3(this.transform.position.x, 0, 0);
        Vector3 playerX = new Vector3(entity.transform.position.x, 0, 0);

        if (Vector3.Distance(camZ, playerZ) < 5 || Vector3.Distance(camZ, playerZ) > 5)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(this.transform.position.x, this.transform.position.y, entity.transform.position.z - 5), transitionSpeed * Time.deltaTime);
        }
        
        if(Vector3.Distance(camX, playerX) > 1)
        {
            if (entity.transform.position.x > this.transform.position.x)
            {
                this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(entity.transform.position.x - 1, this.transform.position.y, this.transform.position.z), transitionSpeed * Time.deltaTime);
            }
            else
            {
                this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(entity.transform.position.x + 1, this.transform.position.y, this.transform.position.z), transitionSpeed * Time.deltaTime);
            }
        }
	}
}
