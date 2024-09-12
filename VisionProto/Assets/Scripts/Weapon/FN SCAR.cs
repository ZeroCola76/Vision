using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UIElements;

public class FNSCAR : MonoBehaviour, IWeapon, IListener
{
    [SerializeField]
    private WeaponInformation weaponInformation;

    [SerializeField]
    private OutlineWeapon outlineWeapon;

    public Transform bulletTransform;
    private CameraShake cameraShake;

    public WeaponCollider weaponCollider;

    // 총 발사시 카메라 흔들림
    public float amplitude;
    public float frequency;

    // 장착할 위치
    private Transform gunContainer;

    // 총기가 위치해야할 공간
    public Transform gunTransform;
    private Vector3 throwDirection;

    // 총을 잡았을 때 총의 크기를 원래대로 돌릴 매직넘버다. 일단은..
    private readonly Vector3 position = new Vector3(0.1943f, 0.4998903f, -2.035f);
    private readonly Vector3 rotation = new Vector3(0f, 177.255f, -90f);
    private readonly Vector3 scale = new Vector3(1339.226f, 20.04176f, 213.8984f);

    private Rigidbody rigidbody;
    private MeshCollider meshCollider;

    // Ray Research에 필요한 변수들
    private Vector3 direction;
    private Vector3 targetPoint;


    private float totalTime;
    private int shotCount;
    private int currentBullet;

    private bool isEquipped;
    private bool isPickup;
    private bool isEmpty;

    private bool isChanging;
    private bool isShoting;

    [SerializeField]
    private float changeThrowingSpeed;

    private bool isShoot;

    UIBulletInformation bulletInfomation;

    private int throwingGunLayer;
    private int stackingLayer;
    private int gunLayer;

    //애니메이션 관련 변수
    AnimationInformation Rinfo;


    // Start is called before the first frame update
    void Start()
    {
        InitalizeRecoilSetting();
        Pickup();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.enabled)
            Pickup();

        if (Input.GetMouseButtonDown(0))
        {
            if (isEmpty)
                Drop();

            if (isShoting)
                return;

            SaveFovValue();

            Shot();
            RecoilFire();
        }

        if (isShoting && !isEmpty)
        {
            totalTime += Time.deltaTime;

            if (totalTime > weaponInformation.shotDelay)
            {
                isShoting = false;
                totalTime = 0f;
            }
        }

        if (currentBullet >= weaponInformation.maxBullet)
        {
            isEmpty = true;
            RecoilRecovery();
        }
        else
            isEmpty = false;

        if (isEmpty)
            return;

        if (Input.GetMouseButton(0))
        {
            Rinfo.stateName = "RAttack";
            Rinfo.layer = -1;
            Rinfo.normalizedTime = 0f;
            EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Rinfo);
            if (isShoting)
                return;

            Shot();
            RecoilFire();
        }

        if (Input.GetMouseButtonUp(0))
        {
            RecoilRecovery();
        }

    }

    // 초반에 필요한 반동 세팅
    public void InitalizeRecoilSetting()
    {
        // Weapon information setting
        weaponInformation = new WeaponInformation();
        weaponInformation.xRecoil = 0.3f;
        weaponInformation.yRecoil = 0.3f;
        weaponInformation.snappiness = 10f;
        weaponInformation.returnSpeed = 50f;
        weaponInformation.recoilSpeed = 1000f;

        weaponInformation.bulletSpeed = 300;
        weaponInformation.distance = 1000f;
        weaponInformation.maxBullet = 30;
        weaponInformation.shotDelay = 0.2f;
        weaponInformation.throwSpeed = 200f;

        changeThrowingSpeed = 10f;

        // cameraShake setting
        cameraShake = FindObjectOfType<CameraShake>();
        cameraShake.weaponInformation = weaponInformation;

        // Gun bullet Setting
        gunContainer = GameObject.Find("GunContainer").GetComponent<Transform>();

        meshCollider = GetComponentInChildren<MeshCollider>();
        rigidbody = GetComponentInChildren<Rigidbody>();

        currentBullet = 0;
        shotCount = 0;

        // 이 스크립트가 활성화 되었다는 것은 활성화가 되었다는 뜻이다.
        isEquipped = false;
        isEmpty = false;

        EventManager.Instance.AddEvent(EventType.Pickup, OnEvent);

        amplitude = 1f;
        frequency = 1f;

        bulletInfomation = new UIBulletInformation();
        bulletInfomation.currentBullet = currentBullet;
        bulletInfomation.maxBullet = weaponInformation.maxBullet;

        int throwLayer = LayerMask.GetMask("ThrowWeapon");
        throwingGunLayer = (int)(Mathf.Log(throwLayer) / Mathf.Log(2));

        int stacking = LayerMask.GetMask("StackingCamera");
        stackingLayer = (int)(Mathf.Log(stacking) / Mathf.Log(2));

        int gun = LayerMask.GetMask("Gun");
        gunLayer = (int)(Mathf.Log(gun) / Mathf.Log(2));
    }

    // 총알을 쐈을 때 초기 설정값.
    public void SaveFovValue()
    {
        cameraShake.isMouseDown = true;
        cameraShake.isRecoil = false;
        cameraShake.mouseDistance = Vector3.zero;
    }

    // 사격했을 때의 반동
    public void RecoilFire()
    {
        // 이건 Random 값으로 정할 수 있지만 우리는 고정값이니까 보류
        //targetRotaion += new Vector3(recoilX, Random.Range(-recoilY, 0), Random.Range(-recoilZ, recoilZ));
        float recoilX = weaponInformation.xRecoil;
        float recoilY = weaponInformation.yRecoil;


        if (shotCount < 15)
        {
            recoilX = weaponInformation.xRecoil;
            recoilY = weaponInformation.yRecoil;
            cameraShake.targetRotaion += new Vector3(0f, recoilY, Random.Range(-recoilX, recoilX));
            //targetRotaion += new Vector3(recoilZ, recoilY, recoilX);
        }
        else if (shotCount >= 15 && shotCount < 20)
        {
            recoilX = weaponInformation.xRecoil;
            recoilY = 0.0f;
            cameraShake.targetRotaion += new Vector3(0f, recoilY, recoilX);
        }
        else if (shotCount >= 20)
        {
            recoilX = -weaponInformation.xRecoil;
            recoilY = 0.0f;
            cameraShake.targetRotaion += new Vector3(0f, recoilY, recoilX);
        }

        shotCount++;
    }

    // 총알이 날라간다.
    public void Shot()
    {
        if (isEmpty) return;
        isShoting = true;

        cameraShake.SetCameraNoise(amplitude, frequency);


        Rigidbody bulletRigidBody;

      

        float bulletSpeed = weaponInformation.bulletSpeed;
        RayResearch();

        if (currentBullet < weaponInformation.maxBullet)
        {
            // bullet 생성
            //GameObject bullet = Instantiate(bulletFactory, bulletTransform.position, bulletTransform.rotation);
            GameObject bullet = PoolManager.Instance.GetPooledObject("PFNSCAR", bulletTransform.position, Quaternion.identity);
            bullet.transform.position = bulletTransform.position;
            //bullet.transform.rotation = Quaternion.identity;
            bullet.transform.rotation = Quaternion.LookRotation(direction);

            bulletRigidBody = bullet.GetComponent<Rigidbody>();
        }
        else
            bulletRigidBody = null;

        if (currentBullet < weaponInformation.maxBullet && bulletRigidBody != null)
        {
            EventManager.Instance.NotifyEvent(EventType.playerShot, isShoot = true);
            bulletRigidBody.velocity = direction * bulletSpeed;
        }
        else
        {
            isEmpty = true;
            RecoilRecovery();
        }
        currentBullet++;

        // Bullet 상태
        bulletInfomation.currentBullet = currentBullet;
        bulletInfomation.maxBullet = weaponInformation.maxBullet;

        EventManager.Instance.NotifyEvent(EventType.WeaponBullet, bulletInfomation);
        EffectManager.Instance.ExecutionEffect(Effect.PistolShot, bulletTransform);
        SoundManager.Instance.PlayEffectSound(SFX.Rifle, this.transform);
    }
    
    // 반동 회복
    public void RecoilRecovery()
    {
        cameraShake.SetCameraNoise(0f, 0f);

        cameraShake.isRecoil = true;
        cameraShake.isMouseDown = false;
        shotCount = 0;
        EventManager.Instance.NotifyEvent(EventType.playerShot, isShoot = false);
    }

    // 무기 주웠을 때 
    public void Pickup()
    {
        if (!isEquipped)
        {
            EventManager.Instance.AddEvent(EventType.Change, OnEvent);
            EventManager.Instance.AddEvent(EventType.GunInformation, OnEvent);
            EventManager.Instance.NotifyEvent(EventType.WeaponBullet, bulletInfomation);
            SoundManager.Instance.PlayEffectSound(SFX.Weapon_Draw, this.transform);
            EventManager.Instance.NotifyEvent(EventType.Hooking, "Riple");

            Rinfo.stateName = "RDraw";
            Rinfo.layer = -1;
            Rinfo.normalizedTime = 0f;
            EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Rinfo);
        }

        weaponCollider.isEquipped = true;
        weaponCollider.enabled = false;
        outlineWeapon.isDone = true;
        outlineWeapon.GunLayer();

        gunTransform.gameObject.layer = stackingLayer;
        cameraShake.weaponInformation = weaponInformation;
        isEquipped = true;
        isPickup = true;  

        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        meshCollider.convex = true;
        meshCollider.isTrigger = true;

        gunTransform.localPosition = position;
        gunTransform.localRotation = Quaternion.Euler(rotation);
        gunTransform.localScale = scale;

        gunContainer.transform.localPosition = new Vector3(-0.150000006f, 0.0500000007f, -0.119999997f);

    }

    // 무기 던졌을 때
    public void Drop()
    {
        EventManager.Instance.AddEvent(EventType.Throwing, OnEvent);
        EventManager.Instance.NotifyEvent(EventType.isEmptyBullet, this.isEmpty);
        SoundManager.Instance.PlayEffectSound(SFX.Weapon_Drop, this.transform);

        weaponCollider.enabled = true;
        weaponCollider.isEquipped = false;
        weaponCollider.boxCollider.isTrigger = false;
        weaponCollider.boxCollider.enabled = true;
        weaponCollider.isthrowing = true;

        outlineWeapon.isDone = true;
        gunTransform.gameObject.layer = throwingGunLayer;
        gunTransform.gameObject.tag = "ThrowWeapon";

        EventManager.Instance.NotifyEvent(EventType.isEquiped, weaponCollider.isEquipped);

        cameraShake.SetCameraNoise(0f, 0f);

        cameraShake.weaponInformation = default;
        isEquipped = false;
        isPickup = false;

        transform.SetParent(null);
        rigidbody.isKinematic = false;
        rigidbody.useGravity = false;
        meshCollider.convex = true;
        meshCollider.isTrigger = false;

        // 총의 local Transform을 돌린다.
        gunTransform.localPosition = Vector3.zero;
        gunTransform.localRotation = Quaternion.Euler(Vector3.zero);
        gunTransform.localScale = Vector3.one * 100f;

        // 총의 원본을 돌린다.
        this.transform.localRotation = Quaternion.Euler(Vector3.zero);
        this.transform.localScale = Vector3.one;

        // direction을 다시 찍어야해..
        RayResearch();
        rigidbody.velocity = throwDirection * weaponInformation.throwSpeed;

        float random = Random.Range(-1f, 1f);
        rigidbody.AddTorque(new Vector3(random, random, random) * 10f);

        this.enabled = false;

        Rinfo.stateName = "RThrow";
        Rinfo.layer = -1;
        Rinfo.normalizedTime = 0f;
        EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Rinfo);
    }

    void RayResearch()
    {
        // Rigidbody component
        Ray cameraRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        float distance = weaponInformation.distance;

        LayerMask layerMask = ~LayerMask.GetMask("Bullet", "Ignore Raycast", "Default");

        RaycastHit[] hits = Physics.RaycastAll(cameraRay, distance, layerMask);

        if (hits.Length > 0)
        {
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            RaycastHit closetHit = hits[0];
            float closetDistance = Vector3.Distance(bulletTransform.position, closetHit.point);

            foreach (var hit in hits)
            {
                float bulletDistance = Vector3.Distance(bulletTransform.position, hit.point);
                if (bulletDistance == closetDistance)
                {
                    closetHit = hit;
                    closetDistance = bulletDistance;
                }
            }

            targetPoint = closetHit.point;
            direction = (targetPoint - bulletTransform.position).normalized;
            throwDirection = direction;
        }
        else
        {
            direction = cameraRay.direction;
            throwDirection = direction;
        }
    }

    void ChangeWeapon()
    {
        EventManager.Instance.NotifyEvent(EventType.isEquiped, false);

        EventManager.Instance.NotifyEvent(EventType.isEmptyBullet, this.isEmpty);

        cameraShake.weaponInformation = default;
        outlineWeapon.isDone = false;

        gunTransform.gameObject.layer = gunLayer;

        weaponCollider.isEquipped = false;
        weaponCollider.boxCollider.enabled = true;
        weaponCollider.isthrowing = true;
        weaponCollider.enabled = false;

        isEquipped = false;
        isPickup = false;

        transform.SetParent(null);
        rigidbody.isKinematic = false;
        rigidbody.useGravity = false;
        meshCollider.convex = true;
        meshCollider.isTrigger = false;

        // 총의 local Transform을 돌린다.
        gunTransform.localPosition = Vector3.zero;
        gunTransform.localRotation = Quaternion.Euler(Vector3.zero);
        gunTransform.localScale = Vector3.one * 100f;

        // 총의 원본을 돌린다.
        this.transform.localRotation = Quaternion.Euler(Vector3.zero);
        this.transform.localScale = Vector3.one;

        // direction을 다시 찍어야해..
        RayResearch();
        //rigidbody.velocity = throwDirection * weaponInformation.throwSpeed;

        Vector3 velocity = throwDirection;
        rigidbody.velocity = velocity * changeThrowingSpeed;
        rigidbody.useGravity = true;

        float random = Random.Range(-1f, 1f);
        rigidbody.AddTorque(new Vector3(random, random, random) * 10f);

        // this.enabled = false;
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.Throwing:
                {
                    bool paramType = (bool)param;
                    isPickup = paramType;
                    rigidbody.useGravity = true;
                    isChanging = paramType;
                    EventManager.Instance.RemoveEvent(EventType.Throwing);
                    EventManager.Instance.RemoveEvent(EventType.Pickup);
                    gameObject.SetActive(paramType);
                    this.enabled = paramType;
                }
                break;
            case EventType.Pickup:
                {
                    if (!isChanging)
                        Pickup();
                }
                break;
            case EventType.Change:
                {
                    isChanging = true;
                    ChangeWeapon();
                    EventManager.Instance.RemoveEvent(EventType.Change);
                    this.enabled = false;
                }
                break;
            case EventType.GunInformation:
                {
                    GameObject gameObject = (GameObject)param;

                    string name2 = gameObject.transform.parent.gameObject.name;
                    string name3 = this.name;

                    if (name2 == name3)
                    {
                        // 이러면 전체적으로 다 바뀌고 바로 검사하지 않을가? 테스트 ㄱㄱ
                        // 그러네 전체적으로 다 바뀐 후에 
                        EventManager.Instance.NotifyEvent(EventType.isEmptyBullet, this.isEmpty);
                    }
                }
                break;
        }
    }
}
