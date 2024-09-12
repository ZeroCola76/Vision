using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI Onclick에 넣고 사용하면 된다.
/// </summary>
public class UIOnclick : MonoBehaviour
{
    /// <summary>
    /// Title에서 최초로 게임이 시작될 때 사용하는 함수
    /// </summary>
    public void GameStart()
    {
        //SceneController.Instance.GameStart();

        GameObject canvas = GameObject.Find("Canvas");

        GameObject youDiedPrefab = Resources.Load<GameObject>("UI/Loading");
        Cursor.lockState = CursorLockMode.None;

        if (youDiedPrefab != null)
        {
            Instantiate(youDiedPrefab, canvas.transform);
            // ㄴㄴ 안돼 끝나고 나서 해야 해.
            StartCoroutine(EndOfFrameRoutine());
        }
        else Debug.Log("None Prefab");

        PlayerInformation playerInfo = new PlayerInformation();

        // Game Init Position
        playerInfo.position = new Vector3(125.9f, 14.5f, -629.2f);
        playerInfo.rotation = Quaternion.identity;

        DataManager.Instance.SaveData(playerInfo);
    }
    
    /// <summary>
    /// 게임을 종료하고 싶을 때 사용하는 함수
    /// </summary>
    public void GameExit()
    {
       SceneController.Instance.GameExit(); 
    }

    /// <summary>
    /// Setting으로 가고 싶을 때 사용하는 함수
    /// </summary>
    public void SettingUI()
    {
        // Canvas를 가져온다.
        GameObject canvas = GameObject.Find("Canvas");
        
        // SettingUI Prefab을 생성해서 불러오자.
        GameObject settingPrefab = Resources.Load<GameObject>("UI/Setting UI");

        if (settingPrefab != null)
        {
            Instantiate(settingPrefab, canvas.transform);
        }
        else Debug.Log("None Prefab");
    }

    /// <summary>
    /// Setting을 종료하고 싶을 때 사용하는 함수
    /// </summary>
    public void SettingExit()
    {
        Destroy(this.gameObject.transform.parent.gameObject);
    }

    IEnumerator EndOfFrameRoutine()
    {   
        // 현재 프레임이 끝날 때까지 대기
        yield return new WaitForEndOfFrame();

        // 여기에서 코드를 실행
        EventManager.Instance.NotifyEvent(EventType.LoadingScene, "Prototype ver 3");   
    }
}
