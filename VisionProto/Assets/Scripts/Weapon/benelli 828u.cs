using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

/// <summary>
/// ���� ������ benelli828u��.
/// </summary>
public class benelli828u : MonoBehaviour, IWeapon, IListener
{
    [SerializeField]
    private WeaponInformation weaponInformation;

    [SerializeField]
    private OutlineWeapon outlineWeapon;

    // ���� ���� �Ѿ� Bullet
    public GameObject shotGunRealBulletFactory;

    // �ѱ� ��ġ
    public Transform bulletTransform;
    private Transform playerRotation;
    private CameraShake cameraShake;

    public WeaponCollider weaponCollider;

    // �� �߻�� ī�޶� ��鸲
    public float amplitude;
    public float frequency;

    // ������ ��ġ
    private Transform gunContainer;

    // �ѱⰡ ��ġ�ؾ��� ����
    public Transform gunTransform;
    private Vector3 throwDirection;

    // ���� ����� �� ���� ũ�⸦ ������� ���� �����ѹ���. �ϴ���..
    private readonly Vector3 position = new Vector3(-0.018f, -0.7f, -2.44f);
    private readonly Vector3 rotation = new Vector3(0f, 177.255f, -90f);
    private readonly Vector3 scale = new Vector3(1339.226f, 20.04176f, 213.8984f);

    private Rigidbody rigidbody;
    private MeshCollider meshCollider;

    // Ray Research�� �ʿ��� ������
    private Vector3 direction;
    private Vector3 targetPoint;

    [SerializeField]
    private float spreadX;
    [SerializeField]
    private float spreadY;
    [SerializeField]
    private int bulletCount;

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

    private UIBulletInformation bulletInfomation;

    private int throwingGunLayer;
    private int stackingLayer;
    private int gunLayer;

    private AnimationInformation Sinfo;


    // Start is called before the first frame update
    void Start()
    {
        InitalizeRecoilSetting();
        Pickup();
    }

    private void FixedUpdate()
    {
  
    }

    // Update is called once per frame
    void Update()
    {
        if (this.enabled)
            Pickup();

        if (Input.GetMouseButtonDown(0))
        {
            Sinfo.stateName = "SAttack";
            Sinfo.layer = -1;
            Sinfo.normalizedTime = 0f;
            EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Sinfo);
            if (isEmpty)
                Drop();

            if (isShoting)
                return;

            SaveFovValue();

            if (cameraShake.gunRecoil)
            {
                Shot();
                RecoilFire();
            }
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

        if (Input.GetMouseButtonUp(0))
        {
            RecoilRecovery();
        }







        //         if (isEmpty)
        //             return;

        // ���簡 �Ұ����ϳ�
        //         if (Input.GetMouseButton(0))
        //         {
        //             totalTime += Time.deltaTime;
        //             if (totalTime > shotDelay)
        //             {
        //                 Shot();
        //                 RecoilFire();
        //                 totalTime = 0f;
        //             }
        //         }

    }


    // �ʹݿ� �ʿ��� �ݵ� ����
    public void InitalizeRecoilSetting()
    {
        // Weapon information setting
        weaponInformation = new WeaponInformation();
        weaponInformation.xRecoil = 0f;
        weaponInformation.yRecoil = 5f;
        weaponInformation.snappiness = 10f;
        weaponInformation.returnSpeed = 50f;
        weaponInformation.recoilSpeed = 1000f;

        weaponInformation.bulletSpeed = 500f;
        weaponInformation.distance = 800f;
        weaponInformation.maxBullet = 2;
        weaponInformation.shotDelay = 0.5f;
        weaponInformation.throwSpeed = 200f;

        changeThrowingSpeed = 10f;

        bulletCount = 10;

        spreadX = 0.3f;
        spreadY = 0.2f;

        // cameraShake setting
        cameraShake = FindObjectOfType<CameraShake>();
        cameraShake.weaponInformation = weaponInformation;
        playerRotation = GameObject.Find("Direction").GetComponent<Transform>();

        gunContainer = GameObject.Find("GunContainer").GetComponent<Transform>();

        meshCollider = GetComponentInChildren<MeshCollider>();
        rigidbody = GetComponentInChildren<Rigidbody>();

        currentBullet = 0;
        shotCount = 0;

        // �� ��ũ��Ʈ�� Ȱ��ȭ �Ǿ��ٴ� ���� Ȱ��ȭ�� �Ǿ��ٴ� ���̴�.
        isEquipped = false;
        isEmpty = false;

        EventManager.Instance.AddEvent(EventType.Pickup, OnEvent);

        amplitude = 1f;
        frequency = 1f;

        bulletInfomation = new UIBulletInformation();
        bulletInfomation.currentBullet = currentBullet;
        bulletInfomation.maxBullet = weaponInformation.maxBullet;

        Sinfo = new AnimationInformation();

        int throwLayer = LayerMask.GetMask("ThrowWeapon");
        throwingGunLayer = (int)(Mathf.Log(throwLayer) / Mathf.Log(2));

        int stacking = LayerMask.GetMask("StackingCamera");
        stackingLayer = (int)(Mathf.Log(stacking) / Mathf.Log(2));

        int gun = LayerMask.GetMask("Gun");
        gunLayer = (int)(Mathf.Log(gun) / Mathf.Log(2));
    }

    // �Ѿ��� ���� �� �ʱ� ������.
    public void SaveFovValue()
    {
        cameraShake.isMouseDown = true;
        cameraShake.isRecoil = false;
        cameraShake.mouseDistance = Vector3.zero;
    }

    // ������� ���� �ݵ�
    public void RecoilFire()
    {
        // Camera Shake�� �ݵ��� ���� ī�޶��� �̵��� �ؾ��Ѵ�.
        // �ݵ��� ����� ���⼭ ������.

        float recoilX = weaponInformation.xRecoil;
        float recoilY = weaponInformation.yRecoil;
        float recoilZ = 0f;
        cameraShake.targetRotaion += new Vector3(recoilZ, recoilY, recoilX);
        cameraShake.isRecoil = true;

        shotCount++;
    }

    // �Ѿ��� ���󰣴�.
    public void Shot()
    {
        if (isEmpty) return;
        isShoting = true;

        cameraShake.SetCameraNoise(amplitude, frequency);

        // bullet ����
        //GameObject realBullet = Instantiate(shotGunRealBulletFactory, bulletTransform.position, bulletTransform.rotation);

        GameObject realBullet;

        // ��� �Ѿ� ������ ���� �� �ִ�.
        Dictionary<GameObject, Rigidbody> bullets = new Dictionary<GameObject, Rigidbody>();

        float distance = weaponInformation.distance;
        float bulletSpeed = weaponInformation.bulletSpeed;

        if (currentBullet < weaponInformation.maxBullet)
        {
            realBullet = PoolManager.Instance.GetPooledObject("PRealBenelli828u");
            realBullet.transform.position = bulletTransform.position;
            realBullet.transform.rotation = bulletTransform.rotation;

            for (int i = 0; i < bulletCount; i++)
            {
                //GameObject imageBullet = Instantiate(shotGunImageBulletFactory, bulletTransform.position, bulletTransform.rotation);

                GameObject imageBullet = PoolManager.Instance.GetPooledObject("PImageBenelli828u", bulletTransform.position, Quaternion.identity);
                imageBullet.transform.position = bulletTransform.position;
                //imageBullet.transform.rotation = bulletTransform.rotation;

                //Collider bulletCollider = imageBullet.GetComponent<Collider>();
                Rigidbody bulletRigidbody = imageBullet.GetComponent<Rigidbody>();
                // �̷��� ���� �°� ������°� �Ұ����ѵ�? Collider�� ���� �׷���. ������ �̷����ϰ�
                // Shotgun image Bullet�� ���� ������ ��� ���� ���̳� �ٴڿ� ������ ������� �ϴ� ��ũ��Ʈ �ϳ� ���� ����.
                //bulletCollider.enabled = false;
                bullets.Add(imageBullet, bulletRigidbody);
            }

            // world * local ���� Shot Bullet�̴�. Player�� �ٶ󺸴� �������� Bullet�� ���� �Ѵ�.
            realBullet.transform.rotation = playerRotation.rotation * shotGunRealBulletFactory.transform.rotation;

            // Real Bullet Rigidbody component
        }
        else realBullet = null;

        Rigidbody realBulletRigidBody;

        if (realBullet != null)
            realBulletRigidBody = realBullet.GetComponent<Rigidbody>();
        else realBulletRigidBody = null;

        RayResearch();

        if (currentBullet < weaponInformation.maxBullet && realBulletRigidBody != null)
        {
            foreach (var bullet in bullets)
            {
                Vector3 randomDirection = direction
                    + new Vector3(
                    Random.Range(-spreadX, spreadX),
                    Random.Range(-spreadY, spreadY),
                    0);

                bullet.Key.transform.rotation = Quaternion.LookRotation(randomDirection);
                bullet.Value.velocity = randomDirection * bulletSpeed;
            }
            EventManager.Instance.NotifyEvent(EventType.playerShot, isShoot = true);
            realBulletRigidBody.velocity = direction * bulletSpeed;
        }

        currentBullet++;

        // Bullet ����
        bulletInfomation.currentBullet = currentBullet;
        bulletInfomation.maxBullet = weaponInformation.maxBullet;

        SoundManager.Instance.PlayEffectSound(SFX.Shotgun, this.transform);
        EffectManager.Instance.ExecutionEffect(Effect.PistolShot, bulletTransform);
        EventManager.Instance.NotifyEvent(EventType.WeaponBullet, bulletInfomation);
    }

    // �ݵ� ȸ��
    public void RecoilRecovery()
    {
        cameraShake.SetCameraNoise(0f, 0f);

        cameraShake.isRecoil = true;
        cameraShake.isMouseDown = false;
        shotCount = 0;
        EventManager.Instance.NotifyEvent(EventType.playerShot, isShoot = false);
    }

    // ���� �ֿ��� ��
    public void Pickup()
    {
        if (!isEquipped)
        {
            EventManager.Instance.AddEvent(EventType.Change, OnEvent);
            EventManager.Instance.AddEvent(EventType.GunInformation, OnEvent);
            EventManager.Instance.NotifyEvent(EventType.WeaponBullet, bulletInfomation);
            SoundManager.Instance.PlayEffectSound(SFX.Weapon_Draw, this.transform);
            EventManager.Instance.NotifyEvent(EventType.Hooking, "ShotGun");

            Sinfo.stateName = "SDraw";
            Sinfo.layer = -1;
            Sinfo.normalizedTime = 0f;
            EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Sinfo);
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

        gunTransform.localPosition = position;
        gunTransform.localRotation = Quaternion.Euler(rotation);
        gunTransform.localScale = scale;

        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        meshCollider.convex = true;
        meshCollider.isTrigger = true;

        gunContainer.transform.localPosition = new Vector3(-0.140000001f, 0.349999994f, -0.129999995f);
    }

    // ���� ������ ��
    public void Drop()
    {
        EventManager.Instance.AddEvent(EventType.Throwing, OnEvent);
        EventManager.Instance.NotifyEvent(EventType.isEmptyBullet, this.isEmpty);
        SoundManager.Instance.PlayEffectSound(SFX.Weapon_Drop, this.transform);
        EventManager.Instance.NotifyEvent(EventType.Hooking, "");

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

        // ���� local Transform�� ������.
        gunTransform.localPosition = Vector3.zero;
        gunTransform.localRotation = Quaternion.Euler(Vector3.zero);
        gunTransform.localScale = Vector3.one * 100f;

        // ���� ������ ������.
        this.transform.localRotation = Quaternion.Euler(Vector3.zero);
        this.transform.localScale = Vector3.one;

        // direction�� �ٽ� ������..
        RayResearch();
        rigidbody.velocity = throwDirection * weaponInformation.throwSpeed;

        float random = Random.Range(-1f, 1f);
        rigidbody.AddTorque(new Vector3(random, random, random) * 10f);

        this.enabled = false;

        Sinfo.stateName = "SThrow";
        Sinfo.layer = -1;
        Sinfo.normalizedTime = 0f;
        EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Sinfo);
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
        EventManager.Instance.NotifyEvent(EventType.Hooking, "");

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

        // ���� local Transform�� ������.
        gunTransform.localPosition = Vector3.zero;
        gunTransform.localRotation = Quaternion.Euler(Vector3.zero);
        gunTransform.localScale = Vector3.one * 100f;

        // ���� ������ ������.
        this.transform.localRotation = Quaternion.Euler(Vector3.zero);
        this.transform.localScale = Vector3.one;

        // direction�� �ٽ� ������..
        RayResearch();
        //rigidbody.velocity = throwDirection * weaponInformation.throwSpeed;

        Vector3 velocity = throwDirection;
        rigidbody.velocity = velocity * changeThrowingSpeed;
        rigidbody.useGravity = true;

        float random = Random.Range(-1f, 1f);
        rigidbody.AddTorque(new Vector3(random, random, random) * 10f);

        this.enabled = false;
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.Throwing:
                {
                    bool paramType = (bool)param;
                    isPickup = paramType;
                    isChanging = paramType;
                    rigidbody.useGravity = true;
                    EventManager.Instance.RemoveEvent(EventType.Throwing);
                    EventManager.Instance.RemoveEvent(EventType.Pickup);
                    //EventManager.Instance.RemoveEvent(EventType.GunInformation, OnEvent);
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
                    ChangeWeapon();
                    isChanging = true;
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
                        // �̷��� ��ü������ �� �ٲ�� �ٷ� �˻����� ������? �׽�Ʈ ����
                        // �׷��� ��ü������ �� �ٲ� �Ŀ� 
                        EventManager.Instance.NotifyEvent(EventType.isEmptyBullet, this.isEmpty);
                    }
                }
                break;
        }
    }
}
