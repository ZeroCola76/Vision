using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;

/// <summary>
/// 무기를 가지고 있지 않다면 단검이 활성화 된다.
/// </summary>
public class Dagger : MonoBehaviour, IListener
{
    // 애니메이션이 진행되면 내가 힘들게 할 필요 없다.
    // 애니메이터에서 true false로 하면된다. -> 그건 아니다
    public GameObject dagger;
    public GameObject gunContainer;

    // 베기 데미지
    public int cutDamage = 10;
    // 찌르기 데미지
    public int pierceDamage = 20;
    // 베기 반경 범위
    public int cutRange = 120;
    // 베는 속도
    public float cuttingSpeed = 2;
    // 찌르는 속도
    public float pireceSpeed = 2;
    // 찌르기 반경 범위 (Dagger의 Z 범위로 되어 있다.)
    public float pireceRange;
 
    private float startCutDegree;
    private float endCutDegree;

    private Vector3 initPosition;


    // 단검 전용 Collider를 만들고 끄고 켰다할까?
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
        //현재 들고있는 총 
        


        KnifeController();
        ActionKnife();
        DelayKnife();
    }

    bool isEmptyGunContainer()
    {
        // Gun Container 자식을 확인했는데 비어 있으면? 총을 장착하지 않은 것이다.
        // 여러 생각은 들지만 자동 줍기까지 생각하면 그냥 1번 확인해서 칼을 활성화 했다 껐다 하는게 더 편할 수 있다.
        // 애니메이션까지도 생각해야 할 것 같은데

        // 자식이 있다면 True 없다면 false
        return gunContainer.transform.childCount > 0;
    }

    // 칼을 벨거냐 찌를거냐
    void KnifeController()
    {
        // 컷, 찌르기 중일 때 입력 못하게
        if (isCutting || isPiercing)
            return;

        // 좌클릭했을 때 베기
        if (Input.GetMouseButtonDown(0) && !nextAttack)
        {
            Dinfo.stateName = "DSlice";
            Dinfo.layer = -1;
            Dinfo.normalizedTime = 0f;
            EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Dinfo);
            // Cutting 중 
            isCutting = true;
            nextAttack = true;

            // 단검의 베기 데미지 정보를 Collider 쪽으로 넘긴다.
            DaggerInformation daggerInformation = SettingDaggerInformation(true, true, cutDamage);
            EventManager.Instance.NotifyEvent(EventType.DaggerInformation, daggerInformation);
            //SoundManager.Instance.PlayEffectSound(SFX.Knife_Swing, this.transform);
            // Direction을 받아와야 되나

            // 시작할 때 바로 start Degree로 이동
            this.transform.localRotation = Quaternion.Euler(0f, startCutDegree, 0f);
            return;
        }

        // 좌클릭 두번 했을 때 찌르기
        if (Input.GetMouseButtonDown(0) && nextAttack)
        {
            Dinfo.stateName = "DSting";
            Dinfo.layer = -1;
            Dinfo.normalizedTime = 0f;
            EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Dinfo);
            // Pierce 중
            isPiercing = true;
            nextAttack = false;

            // 단검의 찌르기 데미지 정보를 Collider 쪽으로 넘긴다.
            DaggerInformation daggerInformation = SettingDaggerInformation(true, true, pierceDamage);
            EventManager.Instance.NotifyEvent(EventType.DaggerInformation, daggerInformation);
            //SoundManager.Instance.PlayEffectSound(SFX.Knife_Stab, this.transform);

            // 시작할 때 초기 위치로 이동
            this.transform.position = initPosition;
            return;
        }
    }
 
    // 입력을 받았으니 실행해야지
    void ActionKnife()
    {
        // 애니메이션이 있다면 실행, 아니라면 직접 위치 조절을 해야지
        // 서든을 보니 애니메이션은 그 언저리로 긋고 실제 타격은 따로 있다.
        
        // 시간이 끝나기 전에 -60 ~ 60도를 끝내야 한다.
        if (isCutting)
        {
            // Dagger -60 ~ 60 도로 지나가고 다시 원점으로 돌아온다.
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

            // Dagger Z 방향을 조절해서 찌르기를 구현한다.
            float currentPosition = Mathf.Lerp(initPosition.z, initPosition.z + pireceRange, pireceSpeed * time);
            transform.localPosition = new Vector3 (0, 0, currentPosition);

            float roundPosition = (float)System.Math.Round(currentPosition, 2);
            float pirecePosition = (float)System.Math.Round(initPosition.z + pireceRange, 2);
            if (roundPosition == pirecePosition)
            {
                // 찌르기가 끝날 때 실행
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
        // 베기일 때 실행
        if (isCutting)
        {
            totalTime += Time.deltaTime;
        }
        // 찌르기일 때 실행
        else if (isPiercing)
        {
            totalTime += Time.deltaTime;        
        }
    }

    /// <summary>
    /// 단검 정보를 넘긴다.
    /// </summary>
    /// <param name="enable">Collider를 활성화 할거냐?</param>
    /// <param name="trigger">트리거를 활성화 할거냐?</param>
    /// <param name="damaged">데미지 정보</param>
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
