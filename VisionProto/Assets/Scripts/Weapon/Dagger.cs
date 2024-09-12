using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;

/// <summary>
/// ���⸦ ������ ���� �ʴٸ� �ܰ��� Ȱ��ȭ �ȴ�.
/// </summary>
public class Dagger : MonoBehaviour, IListener
{
    // �ִϸ��̼��� ����Ǹ� ���� ����� �� �ʿ� ����.
    // �ִϸ����Ϳ��� true false�� �ϸ�ȴ�. -> �װ� �ƴϴ�
    public GameObject dagger;
    public GameObject gunContainer;

    // ���� ������
    public int cutDamage = 10;
    // ��� ������
    public int pierceDamage = 20;
    // ���� �ݰ� ����
    public int cutRange = 120;
    // ���� �ӵ�
    public float cuttingSpeed = 2;
    // ��� �ӵ�
    public float pireceSpeed = 2;
    // ��� �ݰ� ���� (Dagger�� Z ������ �Ǿ� �ִ�.)
    public float pireceRange;
 
    private float startCutDegree;
    private float endCutDegree;

    private Vector3 initPosition;


    // �ܰ� ���� Collider�� ����� ���� �״��ұ�?
    private bool isCutting;
    private bool isPiercing;
    private float totalTime;

    private bool nextAttack;

    private bool isEquiped;

    AnimationInformation Dinfo;

    void Start()
    {
        totalTime = 0;
        startCutDegree = -cutRange * 0.5f;
        endCutDegree = cutRange * 0.5f;
        initPosition = this.transform.localPosition;
        pireceRange = dagger.transform.localScale.z;
        nextAttack = false;

       // Dinfo = new AnimationInformation();

        EventManager.Instance.AddEvent(EventType.isEquiped, OnEvent);
    }

    enum eWeapon
    {
        none,
        Weapon1,
        Weapon2,
    }

    eWeapon currentWeapon = eWeapon.none;
    eWeapon oldWeapon;

    private void Update()
    {
        //���� ����ִ� �� 
        


        KnifeController();
        ActionKnife();
        DelayKnife();
    }

    bool isEmptyGunContainer()
    {
        // Gun Container �ڽ��� Ȯ���ߴµ� ��� ������? ���� �������� ���� ���̴�.
        // ���� ������ ������ �ڵ� �ݱ���� �����ϸ� �׳� 1�� Ȯ���ؼ� Į�� Ȱ��ȭ �ߴ� ���� �ϴ°� �� ���� �� �ִ�.
        // �ִϸ��̼Ǳ����� �����ؾ� �� �� ������

        // �ڽ��� �ִٸ� True ���ٸ� false
        return gunContainer.transform.childCount > 0;
    }

    // Į�� ���ų� ��ų�
    void KnifeController()
    {
        // ��, ��� ���� �� �Է� ���ϰ�
        if (isCutting || isPiercing)
            return;

        // ��Ŭ������ �� ����
        if (Input.GetMouseButtonDown(0) && !nextAttack)
        {
            Dinfo.stateName = "DSlice";
            Dinfo.layer = -1;
            Dinfo.normalizedTime = 0f;
            EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Dinfo);
            // Cutting �� 
            isCutting = true;
            nextAttack = true;

            // �ܰ��� ���� ������ ������ Collider ������ �ѱ��.
            DaggerInformation daggerInformation = SettingDaggerInformation(true, true, cutDamage);
            EventManager.Instance.NotifyEvent(EventType.DaggerInformation, daggerInformation);
            //SoundManager.Instance.PlayEffectSound(SFX.Knife_Swing, this.transform);
            // Direction�� �޾ƿ;� �ǳ�

            // ������ �� �ٷ� start Degree�� �̵�
            this.transform.localRotation = Quaternion.Euler(0f, startCutDegree, 0f);
            return;
        }

        // ��Ŭ�� �ι� ���� �� ���
        if (Input.GetMouseButtonDown(0) && nextAttack)
        {
            Dinfo.stateName = "DSting";
            Dinfo.layer = -1;
            Dinfo.normalizedTime = 0f;
            EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Dinfo);
            // Pierce ��
            isPiercing = true;
            nextAttack = false;

            // �ܰ��� ��� ������ ������ Collider ������ �ѱ��.
            DaggerInformation daggerInformation = SettingDaggerInformation(true, true, pierceDamage);
            EventManager.Instance.NotifyEvent(EventType.DaggerInformation, daggerInformation);
            //SoundManager.Instance.PlayEffectSound(SFX.Knife_Stab, this.transform);

            // ������ �� �ʱ� ��ġ�� �̵�
            this.transform.position = initPosition;
            return;
        }
    }
 
    // �Է��� �޾����� �����ؾ���
    void ActionKnife()
    {
        // �ִϸ��̼��� �ִٸ� ����, �ƴ϶�� ���� ��ġ ������ �ؾ���
        // ������ ���� �ִϸ��̼��� �� �������� �߰� ���� Ÿ���� ���� �ִ�.
        
        // �ð��� ������ ���� -60 ~ 60���� ������ �Ѵ�.
        if (isCutting)
        {
            // Dagger -60 ~ 60 ���� �������� �ٽ� �������� ���ƿ´�.
            float time = totalTime;
            time = Mathf.Clamp01(time);

            float currentRotation = Mathf.Lerp(startCutDegree, endCutDegree, time * cuttingSpeed);
            transform.localRotation = Quaternion.Euler(0f, currentRotation, 0f);

            if(currentRotation == endCutDegree)
            {
                DaggerInformation daggerInformation = SettingDaggerInformation(false, true, default);
                EventManager.Instance.NotifyEvent(EventType.DaggerInformation, daggerInformation);
                transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                isCutting = false;
                totalTime = 0;
            }
                             
        }
        else if(isPiercing) 
        {
            float time = totalTime;
            time = Mathf.Clamp01(time);

            // Dagger Z ������ �����ؼ� ��⸦ �����Ѵ�.
            float currentPosition = Mathf.Lerp(initPosition.z, initPosition.z + pireceRange, pireceSpeed * time);
            transform.localPosition = new Vector3 (0, 0, currentPosition);

            float roundPosition = (float)System.Math.Round(currentPosition, 2);
            float pirecePosition = (float)System.Math.Round(initPosition.z + pireceRange, 2);
            if (roundPosition == pirecePosition)
            {
                // ��Ⱑ ���� �� ����
                DaggerInformation daggerInformation = SettingDaggerInformation(false, true, default);
                EventManager.Instance.NotifyEvent(EventType.DaggerInformation, daggerInformation);
                transform.localPosition = initPosition;

                isPiercing = false;
                totalTime = 0;
            }
            
        }
    }

    void DelayKnife()
    {
        // ������ �� ����
        if (isCutting)
        {
            totalTime += Time.deltaTime;
        }
        // ����� �� ����
        else if (isPiercing)
        {
            totalTime += Time.deltaTime;        
        }
    }

    /// <summary>
    /// �ܰ� ������ �ѱ��.
    /// </summary>
    /// <param name="enable">Collider�� Ȱ��ȭ �Ұų�?</param>
    /// <param name="trigger">Ʈ���Ÿ� Ȱ��ȭ �Ұų�?</param>
    /// <param name="damaged">������ ����</param>
    /// <returns></returns>
    DaggerInformation SettingDaggerInformation(bool enable, bool trigger, int damaged)
    {
        DaggerInformation daggerInformation = new DaggerInformation();
        daggerInformation.isTrueTrigger = trigger;
        daggerInformation.isEnableCollider = enable;
        daggerInformation.damaged = damaged;

        return daggerInformation;
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch(eventType) 
        {
            case EventType.isEquiped:
                {
                    isEquiped = (bool)param;

                    if (!isEquiped)
                    {
                        dagger.gameObject.SetActive(true);
                        Dinfo.stateName = "DIdle";
                        Dinfo.layer = -1;
                        Dinfo.normalizedTime = 0f;
                        EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Dinfo);
                    }
                    else
                        dagger.gameObject.SetActive(false);
                }
                break;
        }
    }

}
