using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWeaponCabinet : MonoBehaviour
{
    public GameObject leftDoor;
    public GameObject rightDoor;

    public bool isControl;

    private Quaternion leftDoorOpen;
    private Quaternion rightDoorOpen;

    public float doorSpeed;

    private void Start()
    {
        leftDoorOpen = Quaternion.Euler(0, 90f, 0);
        rightDoorOpen = Quaternion.Euler(0, -90f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(isControl)
        {
            // left door´Â 90
            leftDoor.transform.localRotation = Quaternion.Lerp(leftDoor.transform.localRotation, leftDoorOpen, Time.deltaTime * (doorSpeed));
            // right door´Â -90
            rightDoor.transform.localRotation = Quaternion.Lerp(rightDoor.transform.localRotation, rightDoorOpen, Time.deltaTime * (doorSpeed));
        }
    }
}
