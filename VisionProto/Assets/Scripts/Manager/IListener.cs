// �̺�Ʈ Ÿ���� ���Ѵ�.
// �̺�Ʈ Ÿ���� �߰��ϰ� ������ ���⿡ �߰��Ѵ�.

public enum EventType
{
    Hooking,
    detected,
    Throwing,
    isEquiped,
    Pickup, 
    Change,
    PlayerPosition,
    DaggerInformation,
    isEmptyBullet,
    GunInformation,
    playerShot,
    PlayerHPUI,
    WeaponBullet,
    PlayerAnimator,
    NPCHit,
    LoadingScene,
    VPState,
    VPWeaponInformation,
    END,
}


public interface IListener
{
    public void OnEvent(EventType eventType, object param = null);
}
