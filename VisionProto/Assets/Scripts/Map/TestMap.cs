using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMap : MonoBehaviour
{
    GameObject testGameObject;
    AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        //SoundManager.Instance.PlayBGMSound(BGM.MainTitle_BGM);
        audio = SoundManager.Instance.PlayAudioSourceBGMSound(BGM.Ambience);
        testGameObject = audio.gameObject;
    }

    private void Update()
    {
        // 특정 Sound를 삭제하고 싶을 때 사용하는 방법
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Destroy(testGameObject);
            audio = SoundManager.Instance.PlayAudioSourceBGMSound(BGM.VP_Am);
            testGameObject = audio.gameObject;
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Destroy(testGameObject);
            audio = SoundManager.Instance.PlayAudioSourceBGMSound(BGM.Ambience); 
            testGameObject = audio.gameObject;
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Destroy(testGameObject);
            audio = SoundManager.Instance.PlayAudioSourceBGMSound(BGM.MainTitle_BGM);
            testGameObject = audio.gameObject;
        }

        // Sound 삭제 없이 사용하는 방법
        if(Input.GetKeyDown(KeyCode.F4))
        {
            SoundManager.Instance.PlayEffectSound(SFX.Shotgun, this.transform);
        }
    }

}
