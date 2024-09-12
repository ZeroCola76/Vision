using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Center : MonoBehaviour
{
    private void Awake()
    {
        CoroutineRunner[] existManagers = FindObjectsOfType<CoroutineRunner>();

        foreach(var runner in existManagers)
        {
            if(runner != null && runner.gameObject != null)
            {
                Destroy(runner.gameObject);
            }
        }
 
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
    }

    private void Update()
    {

    }

}
