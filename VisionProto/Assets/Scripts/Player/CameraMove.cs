using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float mouseSpeed = 5f;
    float mouseX = 0f;
    float mouseY = 0f;

    Transform direction;
    public CinemachineVirtualCamera virtualCamera;
    CinemachinePOV pov;
    // Start is called before the first frame update
    void Start()
    {
        direction = GameObject.Find("Direction").GetComponent<Transform>();
        pov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X")*mouseSpeed;
        mouseY = Input.GetAxis("Mouse Y") * mouseSpeed;

        pov.m_HorizontalAxis.Value += mouseX;
        direction.Rotate(Vector3.up,mouseX);
        pov.m_VerticalAxis.Value += -mouseY;
    }
}
