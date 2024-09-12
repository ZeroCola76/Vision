using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BGM
{
    MainTitle_BGM,
    Ambience,
    VP_Am,
};

public enum SFX
{
    // Player State
    Player_Walk,
    Player_Run,
    Player_Jump,
    Player_Slide,
    Player_sit,
    Player_Hurt,
    Player_Death,

    Grappling_Start,
    Grappling_End,
    Weapon_Throwing,
    Weapon_Draw,
    Change, 
    Heal,

    // Enemy
    Enemy_Run,
    Enemy_Jump,
    Enemy_Hurt,
    Enemy_Death,

    Weapon_Drop,

    // Weapon
    Pistol,
    Shotgun,
    Rifle,
    lazergun,
    Knife_Swing,
    Knife_Stab,

    // etc
    Interact,
    Object_Destroy,
    Open_Door,
    Close_Door,
    OpenDoor_Big,
    Gunhit,


    END,
};


/// <summary>
/// Sound Manager ����
/// ��� Sound���� �����Ѵ�.
/// ����ϱ� ���ϰ��ϱ� ���� Enum���� ����
/// 0620 �̿뼺
/// </summary>
public class SoundManager : Singleton<SoundManager>
{
    // ��ȹ���� ������ CSV���� Sound ���� ������ ������ �ϸ� �� �� ����. - Enum�� ������� ���Ѵ�.

    // SoundClip ���� SoundSource�� ����.
    // BGM ����, SFX ���� ���� ������ �� �� ����.

    private AudioSource[] bgmAudio;
    private AudioSource[] sfxAudio;
    private GameObject[] testObject;

    private Dictionary<BGM, AudioSource> activeBGMSounds = new Dictionary<BGM, AudioSource>();
    private Dictionary<SFX, AudioSource> activeSFXSounds = new Dictionary<SFX, AudioSource>();

    private List<AudioSource> activeSound = new List<AudioSource>();

    private void Awake()
    {
        CreateSound();
    }

    public void CreateSound()
    {
        /// Prefab �ȿ� �ִ� ���� �����ϰ� ��ȸ�ؼ� bgmAudio, sfxAudio�� �ְ� �ʹ�.
        testObject = Resources.LoadAll<GameObject>("SoundPrefab");

        // TestObject���� Children Component���� ��ȸ�ؼ� �迭�� �ִ´�.
        bgmAudio = testObject[0].GetComponentsInChildren<AudioSource>();
        sfxAudio = testObject[1].GetComponentsInChildren<AudioSource>();

    }


    /// <summary>
    /// ��ȹ���� �� �׼����� �����͵��� ���ĵǾ� �����ٵ� �ű⼭ �޾ƿ��� �� �����̴�!
    /// ������ �ӽ÷�
    /// </summary>
    /// <param name="bgmSound"></param>
    public void PlayBGMSound(BGM bgmSound)
    {
        AudioSource playBGMAudio = null;

        if (activeBGMSounds.ContainsKey(bgmSound))
        {
            Debug.LogWarning("Sound already playing : " + bgmSound.ToString());
            return;
        }

        switch (bgmSound)
        {
            case BGM.MainTitle_BGM:
                playBGMAudio = FindBGMName("MainTitle_BGM");
                break;
            case BGM.Ambience:
                playBGMAudio = FindBGMName("Ambience");
                break;
            case BGM.VP_Am:
                playBGMAudio = FindBGMName("VP_Am");
                break;
        }

        AudioSource bgm = Object.Instantiate(playBGMAudio);
        activeSound.Add(bgm);

        activeBGMSounds.Add(bgmSound, playBGMAudio);

        // �ߺ� Sound�� �鸮�� �ʰ� ó���ߴ�.
        StartCoroutine(RemoveDuplicationSound(bgmSound, playBGMAudio));
    }

    /// <summary>
    /// ��ȹ���� �� �׼����� �����͵��� ���ĵǾ� �����ٵ� �ű⼭ �޾ƿ��� �� �����̴�!
    /// ������ �ӽ÷�
    /// </summary>
    /// <param name="bgmSound"></param>
    public AudioSource PlayAudioSourceBGMSound(BGM bgmSound)
    {
        AudioSource playBGMAudio = null;

        switch (bgmSound)
        {
            case BGM.MainTitle_BGM:
                playBGMAudio = FindBGMName("MainTitle_BGM");
                break;
            case BGM.Ambience:
                playBGMAudio = FindBGMName("Ambience");
                break;
            case BGM.VP_Am:
                playBGMAudio = FindBGMName("VP_Am");
                break;
        }

        AudioSource bgm = Object.Instantiate(playBGMAudio);

        return bgm;
    }

    // Switch�� �ϳ��� ����.


    /// <summary>
    /// Effect Sound�� Enum���� ã�Ƽ� ���ϴ� Transform�� ����Ѵ�.
    /// </summary>
    /// <param name="effectSound"></param>
    public void PlayEffectSound(SFX effectSound, Transform _transform)
    {
        AudioSource playEffectAudio = null;

        // �̰� �ߺ� Sound ó���� �ȵǰ� ���� �Ŵ�. �װ� �ƴϾ�.

        if (activeSFXSounds.ContainsKey(effectSound))
        {
            Debug.LogWarning("Sound already playing : " + effectSound.ToString());
            return;
        }

        switch (effectSound)
        {
            case SFX.Player_Walk:
                playEffectAudio = FindSFXName("Player_Walk");
                break;
            case SFX.Player_Run:
                playEffectAudio = FindSFXName("Player_Run");
                break;
            case SFX.Player_Jump:
                playEffectAudio = FindSFXName("Player_Jump");
                break;
            case SFX.Player_Slide:
                playEffectAudio = FindSFXName("Player_Slide");
                break;
            case SFX.Player_sit:
                playEffectAudio = FindSFXName("Player_Sit");
                break;
            case SFX.Player_Hurt:
                playEffectAudio = FindSFXName("Player_Hurt");
                break;
            case SFX.Player_Death:
                playEffectAudio = FindSFXName("Player_Death");
                break;
            case SFX.Grappling_Start:
                playEffectAudio = FindSFXName("Grappling_Start");
                break;
            case SFX.Grappling_End:
                playEffectAudio = FindSFXName("Grappling_End");
                break;
            case SFX.Weapon_Throwing:
                playEffectAudio = FindSFXName("Weapon_Throwing");
                break;
            case SFX.Weapon_Draw:
                playEffectAudio = FindSFXName("Weapon_Draw");
                break;
            case SFX.Change:
                playEffectAudio = FindSFXName("Change");
                break;
            case SFX.Heal:
                playEffectAudio = FindSFXName("Heal");
                break;
            case SFX.Enemy_Run:
                playEffectAudio = FindSFXName("Enemy_Run");
                break;
            case SFX.Enemy_Jump:
                playEffectAudio = FindSFXName("Enemy_Jump");
                break;
            case SFX.Enemy_Hurt:
                playEffectAudio = FindSFXName("Enemy_Hurt");
                break;
            case SFX.Enemy_Death:
                playEffectAudio = FindSFXName("Enemy_Death");
                break;
            case SFX.Weapon_Drop:
                playEffectAudio = FindSFXName("Weapon_Drop");
                break;
            case SFX.Pistol:
                playEffectAudio = FindSFXName("Pistol");
                break;
            case SFX.Shotgun:
                playEffectAudio = FindSFXName("Shotgun");
                break;
            case SFX.Rifle:
                playEffectAudio = FindSFXName("Rifle");
                break;
            case SFX.lazergun:
                playEffectAudio = FindSFXName("Lazergun");
                break;
            case SFX.Knife_Swing:
                playEffectAudio = FindSFXName("Knife_Swing");
                break;
            case SFX.Knife_Stab:
                playEffectAudio = FindSFXName("Knife_Stab");
                break;
            case SFX.Interact:
                playEffectAudio = FindSFXName("Interact");
                break;
            case SFX.Object_Destroy:
                playEffectAudio = FindSFXName("Object_Destroy");
                break;
            case SFX.Open_Door:
                playEffectAudio = FindSFXName("Open_Door");
                break;
            case SFX.Close_Door:
                playEffectAudio = FindSFXName("Close_Door");
                break;
            case SFX.OpenDoor_Big:
                playEffectAudio = FindSFXName("OpenDoor_Big");
                break;
            case SFX.Gunhit:
                playEffectAudio = FindSFXName("Gunhit");
                break;
        }

        AudioSource sfx = Object.Instantiate(playEffectAudio, _transform);

    }

    /// <summary>
    /// Effect Sound�� Enum���� ã�Ƽ� ���ϴ� Transform�� ����Ѵ�.
    /// </summary>
    /// <param name="effectSound"></param>
    public AudioSource PlayAudioSourceEffectSound(SFX effectSound, Transform _transform)
    {
        AudioSource playEffectAudio = null;

        // �̰� �ߺ� Sound ó���� �ȵǰ� ���� �Ŵ�.
        //         if (activeSFXSounds.ContainsKey(effectSound))
        //         {
        //             Debug.LogWarning("Sound already playing : " + effectSound.ToString());
        //             return;
        //         }

        switch (effectSound)
        {
            case SFX.Player_Walk:
                playEffectAudio = FindSFXName("Player_Walk");
                break;
            case SFX.Player_Run:
                playEffectAudio = FindSFXName("Player_Run");
                break;
            case SFX.Player_Jump:
                playEffectAudio = FindSFXName("Player_Jump");
                break;
            case SFX.Player_Slide:
                playEffectAudio = FindSFXName("Player_Slide");
                break;
            case SFX.Player_sit:
                playEffectAudio = FindSFXName("Player_Sit");
                break;
            case SFX.Player_Hurt:
                playEffectAudio = FindSFXName("Player_Hurt");
                break;
            case SFX.Player_Death:
                playEffectAudio = FindSFXName("Player_Death");
                break;
            case SFX.Grappling_Start:
                playEffectAudio = FindSFXName("Grappling_Start");
                break;
            case SFX.Grappling_End:
                playEffectAudio = FindSFXName("Grappling_End");
                break;
            case SFX.Weapon_Throwing:
                playEffectAudio = FindSFXName("Weapon_Throwing");
                break;
            case SFX.Weapon_Draw:
                playEffectAudio = FindSFXName("Weapon_Draw");
                break;
            case SFX.Change:
                playEffectAudio = FindSFXName("Change");
                break;
            case SFX.Heal:
                playEffectAudio = FindSFXName("Heal");
                break;
            case SFX.Enemy_Run:
                playEffectAudio = FindSFXName("Enemy_Run");
                break;
            case SFX.Enemy_Jump:
                playEffectAudio = FindSFXName("Enemy_Jump");
                break;
            case SFX.Enemy_Hurt:
                playEffectAudio = FindSFXName("Enemy_Hurt");
                break;
            case SFX.Enemy_Death:
                playEffectAudio = FindSFXName("Enemy_Death");
                break;
            case SFX.Weapon_Drop:
                playEffectAudio = FindSFXName("Weapon_Drop");
                break;
            case SFX.Pistol:
                playEffectAudio = FindSFXName("Pistol");
                break;
            case SFX.Shotgun:
                playEffectAudio = FindSFXName("Shotgun");
                break;
            case SFX.Rifle:
                playEffectAudio = FindSFXName("Rifle");
                break;
            case SFX.lazergun:
                playEffectAudio = FindSFXName("lazergun");
                break;
            case SFX.Knife_Swing:
                playEffectAudio = FindSFXName("Knife_Swing");
                break;
            case SFX.Knife_Stab:
                playEffectAudio = FindSFXName("Knife_Stab");
                break;
            case SFX.Interact:
                playEffectAudio = FindSFXName("Interact");
                break;
            case SFX.Object_Destroy:
                playEffectAudio = FindSFXName("Object_Destroy");
                break;
            case SFX.Open_Door:
                playEffectAudio = FindSFXName("Open_Door");
                break;
            case SFX.Close_Door:
                playEffectAudio = FindSFXName("Close_Door");
                break;
            case SFX.OpenDoor_Big:
                playEffectAudio = FindSFXName("OpenDoor_Big");
                break;
            case SFX.Gunhit:
                playEffectAudio = FindSFXName("Gunhit");
                break;
        }

        AudioSource sfx = Object.Instantiate(playEffectAudio, _transform);

        return sfx;
    }

    /// <summary>
    /// �ʿ� ���� Sound�� ���� �ؾ��Ѵ�.
    /// </summary>
    /// <param name="destroyedObject"></param>
    public void DestroyObject(GameObject destroyedObject)
    {
        Destroy(destroyedObject);
    }


    /// <summary>
    /// ���� Play �ǰ� �ִ� Sound�� List���� �����Ѵ�.
    /// </summary>
    /// <param name="audioSource"></param>
    public void RemoveActiveSound(AudioSource audioSource)
    {
        activeSound.Remove(audioSource);
    }

    /// <summary>
    /// BGM Sound �ߺ� ����
    /// </summary>
    /// <param name="soundName">BGM Sound</param>
    /// <param name="audioSource">AudioSource</param>
    /// <returns></returns>
    private IEnumerator RemoveDuplicationSound(BGM soundName, AudioSource audioSource)
    {
        // Sound ���̸�ŭ Effect Sound�� ������ �ʰ� �Ѵ�.
        yield return new WaitForSeconds(audioSource.clip.length);
        activeBGMSounds.Remove(soundName);
    }

    /// <summary>
    /// Effect Sound �ߺ� ����
    /// </summary>
    /// <param name="soundName">SFX Sound</param>
    /// <param name="audioSource">AudioSource</param>
    /// <returns></returns>
    private IEnumerator RemoveDuplicationSound(SFX soundName, AudioSource audioSource)
    {
        // Sound ���̸�ŭ Effect Sound�� ������ �ʰ� �Ѵ�.
        yield return new WaitForSeconds(audioSource.clip.length);
        activeSFXSounds.Remove(soundName);
    }

    /// <summary>
    /// Instatiate �Ǳ� ���� Sound�� Volume�� �����Ѵ�.
    /// </summary>
    /// <param name="addVolume">Volume�� ���Ѵ�.</param>
    public void BGMSoundControl(float addVolume)
    {
        foreach (var bgm in bgmAudio)
        {
            bgm.volume += addVolume;
        }
    }

    /// <summary>
    /// �̹� Instatiate�� Sound�� Volume �����Ѵ�.
    /// </summary>
    /// <param name="addVolume">Volume�� ���Ѵ�</param>
    public void ActiveSoundControl(float addVolume)
    {
        foreach (var sound in activeSound)
        {
            sound.volume += addVolume;
        }
    }

    /// <summary>
    /// Instatiate �Ǳ� ���� Sound�� Volume�� �����Ѵ�.
    /// </summary>
    /// <param name="addVolume">Volume�� ���Ѵ�</param>
    public void SFXSoundControl(float addVolume)
    {
        foreach (var sfx in sfxAudio)
        {
            sfx.volume += addVolume;
        }
    }


    /// <summary>
    /// �̸��� �־ BGM Sound�� ã�´�.
    /// </summary>
    /// <param name="soundName">BGM �̸�</param>
    /// <returns></returns>
    AudioSource FindBGMName(string soundName)
    {
        AudioSource bgmAudioSource = null;

        foreach (var bgm in bgmAudio)
        {
            if (bgm.name == soundName)
                return bgm;
        }

        return bgmAudioSource;
    }

    /// <summary>
    /// �̸��� �־ Effect Sound�� ã�´�.
    /// </summary>
    /// <param name="soundName">SFX �̸�</param>
    /// <returns></returns>
    AudioSource FindSFXName(string soundName)
    {
        AudioSource sfxAudioSource = null;

        foreach (var sfx in sfxAudio)
        {
            if (sfx.name == soundName)
                return sfx;
        }

        return sfxAudioSource;
    }
}
