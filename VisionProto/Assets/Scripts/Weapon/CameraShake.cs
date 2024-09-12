using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class CameraShake : MonoBehaviour
{
    // 반동 ㄱㄱ 카메라도 같이 흔들려야 하니까 부모에서 받아와야겠다.
    public CinemachineVirtualCamera playerCamera;
    private CinemachinePOV pov;
    private Vector3 currentRotation;

    // 카메라 흔들림
    private CinemachineBasicMultiChannelPerlin noise;
    public NoiseSettings noiseSettings;

    // 밖에서 더해준다.
    public Vector3 targetRotaion;   /// 인터페이스로 변경할 때 필요한 변수
    
    // Mouse 누르기 전의 FOV 값을 저장해야 한다.
    public bool isRecoil;       /// 인터페이스로 변경할 때 필요한 변수
    public bool isMouseDown;    /// 인터페이스로 변경할 때 필요한 변수
    public Vector3 mouseDistance;   /// 인터페이스로 변경할 때 필요한 변수
    public bool gunRecoil;      /// 인터페이스로 변경할 때 필요한 변수

    // 이전 좌표 Y,Z (Y,X) 좌표 값
    private float preRecoilYPosition;
    private float preRecoilZPosition;

    // 비어 있으면 Update가 실행되면 안 된다.
    public WeaponInformation weaponInformation;


    private void Start()
    {
        pov = playerCamera.GetCinemachineComponent<CinemachinePOV>();
        noise = playerCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        if (weaponInformation.IsEmpty())
            return;

        if(isMouseDown) 
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            mouseDistance.x += mouseX;
            mouseDistance.y += mouseY;

            // 반동 제어하면 Rotation이 Zero가 되서 그 위치에 고정한다.
            //if (mouseX != 0 || mouseY != 0)
            if (mouseY != 0)
                targetRotaion = Vector3.zero;
        }

        // 에임이 회복됐으면 Zero이고 아니라면 Zero가 아니다.
        if (targetRotaion == Vector3.zero)
            gunRecoil = true;
        else gunRecoil = false;

        if (isRecoil)
        {
            // 여기가 다시 돌아오는 곳을 담당하는 곳 같다.
            float returnSpeed = weaponInformation.returnSpeed;
            targetRotaion = Vector3.Lerp(targetRotaion, Vector3.zero, returnSpeed * Time.deltaTime);
        }

        float snappiness = weaponInformation.snappiness;
        float recoilSpeed = weaponInformation.recoilSpeed;

        // 현재 있던 Rotation 에서 target Rotation 이동.
        currentRotation = Vector3.Slerp(currentRotation, targetRotaion, snappiness * Time.deltaTime);

        // Local Rotation 좌표가 변경된다. 근데 우리는 cinemachine value를 조절해서 카메라를 움직여야한다.   
        Quaternion recoilGun = Quaternion.Euler(currentRotation);

        // 변화량을 더 해서 넣어야한다. -> 핵심, 근데 이거 아님 다른거 해야해

        // 현재 값
        float recoilYPosition = recoilGun.y;
        float recoilZPosition = recoilGun.z;

        // 현재 - 과거
        float diffentYPosition = recoilYPosition - preRecoilYPosition;
        float diffentZPosition = recoilZPosition - preRecoilZPosition;

        pov.m_VerticalAxis.Value -= diffentYPosition * recoilSpeed;
        pov.m_HorizontalAxis.Value -= diffentZPosition * recoilSpeed;

        // 과거 값
        preRecoilYPosition = recoilGun.y;
        preRecoilZPosition = recoilGun.z;
    }

    public void SetCameraNoise(float amplitude, float frequency)
    {
        //noise.m_NoiseProfile = noiseSettings;

        noise.m_AmplitudeGain = amplitude;
        noise.m_FrequencyGain = frequency;
    }

}
