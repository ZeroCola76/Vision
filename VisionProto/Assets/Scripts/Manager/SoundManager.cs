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
/// Sound Manager 제작
/// 모든 Sound들을 관리한다.
/// 사용하기 편하게하기 위해 Enum으로 관리
/// 0620 이용성
/// </summary>
public class SoundManager : Singleton<SoundManager>
{
    // 기획에서 정리한 CSV파일 Sound 들을 가지고 정리를 하면 될 것 같다. - Enum을 사용하지 못한다.

    // SoundClip 보다 SoundSource로 하자.
    // BGM 전용, SFX 전용 따로 만들어야 될 것 같아.

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
        /// Prefab 안에 있는 것을 선택하고 순회해서 bgmAudio, sfxAudio에 넣고 싶다.
        testObject = Resources.LoadAll<GameObject>("SoundPrefab");

        // TestObject에서 Children Component들을 순회해서 배열에 넣는다.
        bgmAudio = testObject[0].GetComponentsInChildren<AudioSource>();
        sfxAudio = testObject[1].GetComponentsInChildren<AudioSource>();

    }


    /// <summary>
    /// 기획에서 준 액셀에서 데이터들이 정렬되어 있을텐데 거기서 받아오게 할 예정이다!
    /// 지금은 임시로
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

        // 중복 Sound가 들리지 않게 처리했다.
        StartCoroutine(RemoveDuplicationSound(bgmSound, playBGMAudio));
    }

    /// <summary>
    /// 기획에서 준 액셀에서 데이터들이 정렬되어 있을텐데 거기서 받아오게 할 예정이다!
    /// 지금은 임시로
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

    // Switch를 하나로 몰자.


    /// <summary>
    /// Effect Sound를 Enum으로 찾아서 원하는 Transform에 출력한다.
    /// </summary>
    /// <param name="effectSound"></param>
    public void PlayEffectSound(SFX effectSound, Transform _transform)
    {
        AudioSource playEffectAudio = null;

        // 이게 중복 Sound 처리가 안되게 막는 거다. 그건 아니야.

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
    /// Effect Sound를 Enum으로 찾아서 원하는 Transform에 출력한다.
    /// </summary>
    /// <param name="effectSound"></param>
    public AudioSource PlayAudioSourceEffectSound(SFX effectSound, Transform _transform)
    {
        AudioSource playEffectAudio = null;

        // 이게 중복 Sound 처리가 안되게 막는 거다.
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
    /// 필요 없는 Sound는 삭제 해야한다.
    /// </summary>
    /// <param name="destroyedObject"></param>
    public void DestroyObject(GameObject destroyedObject)
    {
        Destroy(destroyedObject);
    }


    /// <summary>
    /// 현재 Play 되고 있는 Sound를 List에서 제거한다.
    /// </summary>
    /// <param name="audioSource"></param>
    public void RemoveActiveSound(AudioSource audioSource)
    {
        activeSound.Remove(audioSource);
    }

    /// <summary>
    /// BGM Sound 중복 방지
    /// </summary>
    /// <param name="soundName">BGM Sound</param>
    /// <param name="audioSource">AudioSource</param>
    /// <returns></returns>
    private IEnumerator RemoveDuplicationSound(BGM soundName, AudioSource audioSource)
    {
        // Sound 길이만큼 Effect Sound가 들어오지 않게 한다.
        yield return new WaitForSeconds(audioSource.clip.length);
        activeBGMSounds.Remove(soundName);
    }

    /// <summary>
    /// Effect Sound 중복 방지
    /// </summary>
    /// <param name="soundName">SFX Sound</param>
    /// <param name="audioSource">AudioSource</param>
    /// <returns></returns>
    private IEnumerator RemoveDuplicationSound(SFX soundName, AudioSource audioSource)
    {
        // Sound 길이만큼 Effect Sound가 들어오지 않게 한다.
        yield return new WaitForSeconds(audioSource.clip.length);
        activeSFXSounds.Remove(soundName);
    }

    /// <summary>
    /// Instatiate 되기 전의 Sound의 Volume을 조절한다.
    /// </summary>
    /// <param name="addVolume">Volume을 더한다.</param>
    public void BGMSoundControl(float addVolume)
    {
        foreach (var bgm in bgmAudio)
        {
            bgm.volume += addVolume;
        }
    }

    /// <summary>
    /// 이미 Instatiate된 Sound의 Volume 조절한다.
    /// </summary>
    /// <param name="addVolume">Volume을 더한다</param>
    public void ActiveSoundControl(float addVolume)
    {
        foreach (var sound in activeSound)
        {
            sound.volume += addVolume;
        }
    }

    /// <summary>
    /// Instatiate 되기 전의 Sound의 Volume을 조절한다.
    /// </summary>
    /// <param name="addVolume">Volume을 더한다</param>
    public void SFXSoundControl(float addVolume)
    {
        foreach (var sfx in sfxAudio)
        {
            sfx.volume += addVolume;
        }
    }


    /// <summary>
    /// 이름을 넣어서 BGM Sound를 찾는다.
    /// </summary>
    /// <param name="soundName">BGM 이름</param>
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
    /// 이름을 넣어서 Effect Sound를 찾는다.
    /// </summary>
    /// <param name="soundName">SFX 이름</param>
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
