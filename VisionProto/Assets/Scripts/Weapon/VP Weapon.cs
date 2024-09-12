using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

/// <summary>
/// 이 곳은 조작과 관련된 공간
/// 실제 데미지 처리는 Collider 처리 -> like Dagger Collider
/// 단검과 VP Weapon은 
/// </summary>
public class VPWeapon : MonoBehaviour
{
    public GameObject vpWeapon;
    
    // gunContainer를 확인하자 ?

    // 할퀴기 데미지
    public int scratchDamage = 30;

    // 할퀴기 반경 범위
    public int scratchRange = 240;

    // 할퀴는 속도
    public float scratchSpeed = 2;

    private float startScratchDegree;
    private float endScratchDegree;

    // 좌클릭 우클릭 데미지 범위가 다르니까
    private bool isLeftScratch;
    private bool isRightScratch;
    private bool isNextAttack;

    private float totalTime;

    // 좌클릭 우클릭 전부 240도로 공격하는 거다. 
    // 애니메이션만 달라질 뿐 뭐 없다.
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


        /// VP Weapon 고찰
        // VP Weapon 상태일 때는 
        // 1. Auto pickup이 꺼진다. 
        // 2. 가지고 있던 무기는 어떻게 되지 ? 기획서 봐야할 듯 관련 그건 설정은 없다. -> 그냥 바닥에 버리는거일듯
        // 3. Dagger 쪽도 그럼 VP State인지 알아야 하네. Dagger가 활성화 되면 안 되니까
        // 4. 할퀴기 구현하면 될 듯
        // 근데 실제 데미지 할 때 위 보고 공격하면 데미지 입을 거 같은데? Collider로 처리하니까
        // 그만큼 map 크면 상관 없긴해 Map Size를 늘린다고 했으니까 -> 관련 그건 기획서에 있네

        /// VP Weapon의 예외 상황
        // 그러면 Collider 부분에서 Player 까지 거리를 확인하고 데미지를 입혀야 한다는 건데?
        // Screen 기준 or Player - Enemy 기준 ray를 쏴서 맞으면 공격 아니면 공격 ㄴㄴ
        // 둘다 해보고 괜찮은거 쓰자.
        // 앞에꺼는 살짝만 보여져도 공격이 될 것으로 예상한다.
        // 뒤에꺼는 다 보여지지 않으면 공격 안될거 같은데

        /// 처리
        // VWC : VP Weapon Collider , VW : WP Weapon
        // 그러면 VWC Collider -> VWC NPC 감지 -> VW 전달 후 Ray 체크 (함수를 사용해서 VMC에서 ㄱㄱ) -> VWC 데미지 처리
        // 근데 데미지 처리는 저쪽에서 담당하고 싶은데? Collider 쪽에서 담당하고 싶어 
        // 애니메이션이나 조작은 이곳 데미지 관련은 저쪽에 처리하자.
    }

    private void VPWeaponController()
    {
        if (isLeftScratch || isRightScratch)
            return;

        if (Input.GetMouseButtonDown(0) && !isNextAttack)
        {
            isLeftScratch = true;
            isNextAttack = true;

            // VP 좌측 할퀴기 
            VPWeaponInformation vpWeaponInformation = SettingVPWeaponInformation(true, true, scratchDamage);
            EventManager.Instance.NotifyEvent(EventType.VPWeaponInformation, vpWeaponInformation);


            // VP 상태 Collider 정보를 넘기자.
            this.transform.localRotation = Quaternion.Euler(0f, startScratchDegree, 0f);
            return;
        }

        if (Input.GetMouseButtonDown(0) && isNextAttack)
        {
            isRightScratch = true;
            isNextAttack = false;

            // VP 우측 할퀴기 
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
                // 공격 종료
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
                // 공격 종료 
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

    /// VP 상태 처리 방법
    // VP State 상태인지? 확인하려면 Player state에서 받아오면 좋을 거 같다.
    // Event manager로 관리하면 되겠다 1:1 이니까


}
