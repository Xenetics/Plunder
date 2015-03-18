using UnityEngine;
using System.Collections;

public class Torch : MonoBehaviour 
{
    [SerializeField]
    private Light torchLight;

    private float flickerMin = 0.05f;
    private float flickerMax = 0.1f;
    private float flickerTime = 0.1f;

    private float intensityMin = 0.6f;
    private float intensityMax = 1.0f;
    
	
	void Update () 
    {
        flickerTime -= Time.deltaTime;
        if(flickerTime <= 0)
        {
            torchLight.intensity = Random.RandomRange(intensityMin, intensityMax);
            flickerTime = Random.RandomRange(flickerMin, flickerMax);
        }
	}
}
