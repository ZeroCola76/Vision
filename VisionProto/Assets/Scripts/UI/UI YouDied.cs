using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI OnClick에 넣고 사용하면 된다.
/// You Died Canvas에 넣고 사용하면 된다.
/// </summary>
public class UIYouDied : MonoBehaviour
{
    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void Respawn()
    {
        // 일단은 Scene을 재시작하자.
        SceneController.Instance.LoadScene("LevelDesign");
        
        EventManager.Instance.RemoveAllEvent();
        Time.timeScale = 1;

        //         GameObject canvas = GameObject.Find("Canvas");
        // 
        //         GameObject youDiedPrefab = Resources.Load<GameObject>("UI/Loading");
        //         Cursor.lockState = CursorLockMode.None;
        // 
        //         if (youDiedPrefab != null)
        //         {
        //             Instantiate(youDiedPrefab, canvas.transform);
        //             // ㄴㄴ 안돼 끝나고 나서 해야 해.
        //             StartCoroutine(EndOfFrameRoutine());
        //         }
        //         else Debug.Log("None Prefab");

    }


    /// <summary>
    /// 진짜 나가기가 아닐거 아니야 타이틀로 가기 그런거 아닐까?
    /// </summary>
    public void GameExit()
    {
        SceneController.Instance.GameExit();

//         EventManager.Instance.ChangeScene();
//         SceneController.Instance.LoadScene("Prototype UI");
    }

    IEnumerator EndOfFrameRoutine()
    {
        // 현재 프레임이 끝날 때까지 대기
        yield return new WaitForEndOfFrame();

        // 여기에서 코드를 실행
        EventManager.Instance.NotifyEvent(EventType.LoadingScene, "Prototype ver 3");
        EventManager.Instance.RemoveAllEvent();
    }
}
