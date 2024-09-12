using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Effect
{
    BulletDecal,    // 총알 흔적
    PistolShot,     // 권총을 쐈을 때
    RipleShot,      // 라이플을 쐈을 때
    ShotGunShot,    // 샷건을 쐈을 때
    Scanner,        // 점프 공격할 때 생기는 Wave Effect
    GunHit,         // 총이 벽이나 바닥에 맞았을 때
}

/// <summary>
/// Effect와 관련된 Resource를 담고 있는 클래스
/// </summary>
public class EffectManager : Singleton<EffectManager>
{
    private GameObject[] effectObjects;

    private GameObject bulletDecal;
    private GameObject scanner;
    private WeaponMuzzleEffect[] gunShot;
    private WeaponHitEffect[] gunHit;

    private GameObject effectGunShotResource;

    private GameObject effectResource;

    private void Awake()
    {
        CreateEffect();
    }

    // 한번에 담는것 보다 원하는 이름의 Effect를 담아두는 것이 맞지 않을까?
    // Effect도 몇 개 없으면 한번에 다 담아두고 있자.
    void CreateEffect()
    {
        // Sound와 비슷하게 해놨다.
        effectObjects = Resources.LoadAll<GameObject>("EffectPrefab");

        // 이런식으로 하던가, 아니면 위에서 다 받고 배열 안에 들은 것들을 하나씩 확인해서 넣던가
        bulletDecal = Resources.Load<GameObject>("EffectPrefab/Bullet Decal");
        scanner = Resources.Load<GameObject>("EffectPrefab/Scanner");

        // GunShot Effect를 가져오고 자식들을 순회해서 넣는다.
        effectGunShotResource = Resources.Load<GameObject>("EffectPrefab/GunShot");
        gunShot = effectGunShotResource.GetComponentsInChildren<WeaponMuzzleEffect>();

        // Gun Hit Effect를 가져오고 자식들을 순회해서 넣는다.
        GameObject effectGunHitResource = Resources.Load<GameObject>("EffectPrefab/GunHit");
        gunHit = effectGunHitResource.GetComponentsInChildren<WeaponHitEffect>();

    }
    
    // 이펙트를 실행하기 위한 함수
    public void ExecutionEffect(Effect effect, Transform _transform)
    {
        SwitchEffectResource(effect);

        // Effect 생성
        Object.Instantiate(effectResource, _transform);
    }

    // 이펙트를 부모 Transform에 실행하는 함수
    public void ExecutionEffect(Effect effect, Vector3 position, Quaternion quaternion, Transform parent)
    {
        SwitchEffectResource(effect);

        // Effect 생성
        Object.Instantiate(effectResource, position, quaternion, parent);
    }

    void SwitchEffectResource(Effect _effect)
    {
        switch(_effect) 
        {
            case Effect.BulletDecal:
                effectResource = bulletDecal;
                break;
            case Effect.PistolShot:
                effectResource = FindGunShotEffectName("Muzzle 1");
                break;
            case Effect.ShotGunShot:
                effectResource = FindGunShotEffectName("Muzzle 2");
                break;
            case Effect.RipleShot:
                effectResource = FindGunShotEffectName("Muzzle 3");
                break;
            case Effect.Scanner:
                effectResource = scanner;
                break;
            case Effect.GunHit:
                effectResource = gunHit[0].gameObject;
                break;
            default:
                effectResource = null;
                break;
        }
    }

    /// <summary>
    /// 총을 쐈을 때 각 이펙트가 다를 것이기 때문에 이름으로 찾자.
    /// </summary>
    /// <param name="effectName"></param>
    /// <returns></returns>
    GameObject FindGunShotEffectName(string effectName)
    {
        GameObject effect = null;

        foreach (var obj in gunShot) 
        {
            if (obj.name == effectName)
                return obj.gameObject;
        }

        return effect;
    }

}
