using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

/// <summary>
/// �� ���� ���۰� ���õ� ����
/// ���� ������ ó���� Collider ó�� -> like Dagger Collider
/// �ܰ˰� VP Weapon�� 
/// </summary>
public class VPWeapon : MonoBehaviour
{
    public GameObject vpWeapon;
    
    // gunContainer�� Ȯ������ ?

    // ������ ������
    public int scratchDamage = 30;

    // ������ �ݰ� ����
    public int scratchRange = 240;

    // ������ �ӵ�
    public float scratchSpeed = 2;

    private float startScratchDegree;
    private float endScratchDegree;

    // ��Ŭ�� ��Ŭ�� ������ ������ �ٸ��ϱ�
    private bool isLeftScratch;
    private bool isRightScratch;
    private bool isNextAttack;

    private float totalTime;

    // ��Ŭ�� ��Ŭ�� ���� 240���� �����ϴ� �Ŵ�. 
    // �ִϸ��̼Ǹ� �޶��� �� �� ����.
    private bool isVPState;

    void Start()
    {
        totalTime = 0f;
        startScratchDegree = -scratchRange * 0.5f;
        endScratchDegree = scratchRange * 0.5f;
        isNextAttack = false; 
    }

    void Update()
    {
        VPWeaponController();
        ActionVPWeapon();
        DelayVPWeapon();


        /// VP Weapon ����
        // VP Weapon ������ ���� 
        // 1. Auto pickup�� ������. 
        // 2. ������ �ִ� ����� ��� ���� ? ��ȹ�� ������ �� ���� �װ� ������ ����. -> �׳� �ٴڿ� �����°��ϵ�
        // 3. Dagger �ʵ� �׷� VP State���� �˾ƾ� �ϳ�. Dagger�� Ȱ��ȭ �Ǹ� �� �Ǵϱ�
        // 4. ������ �����ϸ� �� ��
        // �ٵ� ���� ������ �� �� �� ���� �����ϸ� ������ ���� �� ������? Collider�� ó���ϴϱ�
        // �׸�ŭ map ũ�� ��� ������ Map Size�� �ø��ٰ� �����ϱ� -> ���� �װ� ��ȹ���� �ֳ�

        /// VP Weapon�� ���� ��Ȳ
        // �׷��� Collider �κп��� Player ���� �Ÿ��� Ȯ���ϰ� �������� ������ �Ѵٴ� �ǵ�?
        // Screen ���� or Player - Enemy ���� ray�� ���� ������ ���� �ƴϸ� ���� ����
        // �Ѵ� �غ��� �������� ����.
        // �տ����� ��¦�� �������� ������ �� ������ �����Ѵ�.
        // �ڿ����� �� �������� ������ ���� �ȵɰ� ������

        /// ó��
        // VWC : VP Weapon Collider , VW : WP Weapon
        // �׷��� VWC Collider -> VWC NPC ���� -> VW ���� �� Ray üũ (�Լ��� ����ؼ� VMC���� ����) -> VWC ������ ó��
        // �ٵ� ������ ó���� ���ʿ��� ����ϰ� ������? Collider �ʿ��� ����ϰ� �;� 
        // �ִϸ��̼��̳� ������ �̰� ������ ������ ���ʿ� ó������.
    }

    private void VPWeaponController()
    {
        if (isLeftScratch || isRightScratch)
            return;

        if (Input.GetMouseButtonDown(0) && !isNextAttack)
        {
            isLeftScratch = true;
            isNextAttack = true;

            // VP ���� ������ 
            VPWeaponInformation vpWeaponInformation = SettingVPWeaponInformation(true, true, scratchDamage);
            EventManager.Instance.NotifyEvent(EventType.VPWeaponInformation, vpWeaponInformation);


            // VP ���� Collider ������ �ѱ���.
            this.transform.localRotation = Quaternion.Euler(0f, startScratchDegree, 0f);
            return;
        }

        if (Input.GetMouseButtonDown(0) && isNextAttack)
        {
            isRightScratch = true;
            isNextAttack = false;

            // VP ���� ������ 
            VPWeaponInformation vpWeaponInformation = SettingVPWeaponInformation(true, true, scratchDamage);
            EventManager.Instance.NotifyEvent(EventType.VPWeaponInformation, vpWeaponInformation);

            this.transform.localRotation = Quaternion.Euler(0f, endScratchDegree, 0f);
            return;
        }
    }

    private void ActionVPWeapon()
    {
        if(isLeftScratch)
        {
            float time = totalTime;
            time = Mathf.Clamp01(time);

            float currentRotaion = Mathf.Lerp(startScratchDegree, endScratchDegree, time * scratchSpeed);
            transform.localRotation = Quaternion.Euler(0f, currentRotaion, 0f);

       
            if(currentRotaion == endScratchDegree)
            {
                // ���� ����
                VPWeaponInformation vpWeaponInformation = SettingVPWeaponInformation(false, true, default);
                EventManager.Instance.NotifyEvent(EventType.VPWeaponInformation, vpWeaponInformation);
                transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                isLeftScratch = false;
                totalTime = 0f;
            }
        }
        else if(isRightScratch)
        {
            float time = totalTime;
            time = Mathf.Clamp01(time);

            float currentRotaion = Mathf.Lerp(endScratchDegree, startScratchDegree, time * scratchSpeed);
            transform.localRotation = Quaternion.Euler(0f, currentRotaion, 0f);

            if(currentRotaion == startScratchDegree)
            {
                // ���� ���� 
                VPWeaponInformation vpWeaponInformation = SettingVPWeaponInformation(false, true, default);
                EventManager.Instance.NotifyEvent(EventType.VPWeaponInformation, vpWeaponInformation);
                transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                isRightScratch = false;
                totalTime = 0f;
            }
        }
    }

    private void DelayVPWeapon()
    {
        if (isLeftScratch)
            totalTime += Time.deltaTime;
        else if(isRightScratch)
            totalTime += Time.deltaTime;
    }


    private VPWeaponInformation SettingVPWeaponInformation(bool enable, bool trigger, int damaged)
    {
        VPWeaponInformation vpWeaponInformation = new VPWeaponInformation();
        vpWeaponInformation.isTrueTrigger = trigger;
        vpWeaponInformation.isEnableCollider = enable;
        vpWeaponInformation.damaged = damaged;

        return vpWeaponInformation;
    }

    /// VP ���� ó�� ���
    // VP State ��������? Ȯ���Ϸ��� Player state���� �޾ƿ��� ���� �� ����.
    // Event manager�� �����ϸ� �ǰڴ� 1:1 �̴ϱ�


}
