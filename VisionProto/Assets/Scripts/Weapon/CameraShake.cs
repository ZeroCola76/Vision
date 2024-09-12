using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class CameraShake : MonoBehaviour
{
    // �ݵ� ���� ī�޶� ���� ������ �ϴϱ� �θ𿡼� �޾ƿ;߰ڴ�.
    public CinemachineVirtualCamera playerCamera;
    private CinemachinePOV pov;
    private Vector3 currentRotation;

    // ī�޶� ��鸲
    private CinemachineBasicMultiChannelPerlin noise;
    public NoiseSettings noiseSettings;

    // �ۿ��� �����ش�.
    public Vector3 targetRotaion;   /// �������̽��� ������ �� �ʿ��� ����
    
    // Mouse ������ ���� FOV ���� �����ؾ� �Ѵ�.
    public bool isRecoil;       /// �������̽��� ������ �� �ʿ��� ����
    public bool isMouseDown;    /// �������̽��� ������ �� �ʿ��� ����
    public Vector3 mouseDistance;   /// �������̽��� ������ �� �ʿ��� ����
    public bool gunRecoil;      /// �������̽��� ������ �� �ʿ��� ����

    // ���� ��ǥ Y,Z (Y,X) ��ǥ ��
    private float preRecoilYPosition;
    private float preRecoilZPosition;

    // ��� ������ Update�� ����Ǹ� �� �ȴ�.
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

            // �ݵ� �����ϸ� Rotation�� Zero�� �Ǽ� �� ��ġ�� �����Ѵ�.
            //if (mouseX != 0 || mouseY != 0)
            if (mouseY != 0)
                targetRotaion = Vector3.zero;
        }

        // ������ ȸ�������� Zero�̰� �ƴ϶�� Zero�� �ƴϴ�.
        if (targetRotaion == Vector3.zero)
            gunRecoil = true;
        else gunRecoil = false;

        if (isRecoil)
        {
            // ���Ⱑ �ٽ� ���ƿ��� ���� ����ϴ� �� ����.
            float returnSpeed = weaponInformation.returnSpeed;
            targetRotaion = Vector3.Lerp(targetRotaion, Vector3.zero, returnSpeed * Time.deltaTime);
        }

        float snappiness = weaponInformation.snappiness;
        float recoilSpeed = weaponInformation.recoilSpeed;

        // ���� �ִ� Rotation ���� target Rotation �̵�.
        currentRotation = Vector3.Slerp(currentRotation, targetRotaion, snappiness * Time.deltaTime);

        // Local Rotation ��ǥ�� ����ȴ�. �ٵ� �츮�� cinemachine value�� �����ؼ� ī�޶� ���������Ѵ�.   
        Quaternion recoilGun = Quaternion.Euler(currentRotation);

        // ��ȭ���� �� �ؼ� �־���Ѵ�. -> �ٽ�, �ٵ� �̰� �ƴ� �ٸ��� �ؾ���

        // ���� ��
        float recoilYPosition = recoilGun.y;
        float recoilZPosition = recoilGun.z;

        // ���� - ����
        float diffentYPosition = recoilYPosition - preRecoilYPosition;
        float diffentZPosition = recoilZPosition - preRecoilZPosition;

        pov.m_VerticalAxis.Value -= diffentYPosition * recoilSpeed;
        pov.m_HorizontalAxis.Value -= diffentZPosition * recoilSpeed;

        // ���� ��
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
