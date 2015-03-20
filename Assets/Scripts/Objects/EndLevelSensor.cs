using UnityEngine;
using System.Collections;

public class EndLevelSensor : MonoBehaviour 
{
    void OnTriggerEnter(Collider other)
    {
        LevelManager.Instance.NextLevel();
    }
}
