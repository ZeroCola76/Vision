using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Effect
{
    BulletDecal,    // �Ѿ� ����
    PistolShot,     // ������ ���� ��
    RipleShot,      // �������� ���� ��
    ShotGunShot,    // ������ ���� ��
    Scanner,        // ���� ������ �� ����� Wave Effect
    GunHit,         // ���� ���̳� �ٴڿ� �¾��� ��
}

/// <summary>
/// Effect�� ���õ� Resource�� ��� �ִ� Ŭ����
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

    // �ѹ��� ��°� ���� ���ϴ� �̸��� Effect�� ��Ƶδ� ���� ���� ������?
    // Effect�� �� �� ������ �ѹ��� �� ��Ƶΰ� ����.
    void CreateEffect()
    {
        // Sound�� ����ϰ� �س���.
        effectObjects = Resources.LoadAll<GameObject>("EffectPrefab");

        // �̷������� �ϴ���, �ƴϸ� ������ �� �ް� �迭 �ȿ� ���� �͵��� �ϳ��� Ȯ���ؼ� �ִ���
        bulletDecal = Resources.Load<GameObject>("EffectPrefab/Bullet Decal");
        scanner = Resources.Load<GameObject>("EffectPrefab/Scanner");

        // GunShot Effect�� �������� �ڽĵ��� ��ȸ�ؼ� �ִ´�.
        effectGunShotResource = Resources.Load<GameObject>("EffectPrefab/GunShot");
        gunShot = effectGunShotResource.GetComponentsInChildren<WeaponMuzzleEffect>();

        // Gun Hit Effect�� �������� �ڽĵ��� ��ȸ�ؼ� �ִ´�.
        GameObject effectGunHitResource = Resources.Load<GameObject>("EffectPrefab/GunHit");
        gunHit = effectGunHitResource.GetComponentsInChildren<WeaponHitEffect>();

    }
    
    // ����Ʈ�� �����ϱ� ���� �Լ�
    public void ExecutionEffect(Effect effect, Transform _transform)
    {
        SwitchEffectResource(effect);

        // Effect ����
        Object.Instantiate(effectResource, _transform);
    }

    // ����Ʈ�� �θ� Transform�� �����ϴ� �Լ�
    public void ExecutionEffect(Effect effect, Vector3 position, Quaternion quaternion, Transform parent)
    {
        SwitchEffectResource(effect);

        // Effect ����
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
    /// ���� ���� �� �� ����Ʈ�� �ٸ� ���̱� ������ �̸����� ã��.
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
