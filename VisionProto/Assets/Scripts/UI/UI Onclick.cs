using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI Onclick�� �ְ� ����ϸ� �ȴ�.
/// </summary>
public class UIOnclick : MonoBehaviour
{
    /// <summary>
    /// Title���� ���ʷ� ������ ���۵� �� ����ϴ� �Լ�
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
            // ���� �ȵ� ������ ���� �ؾ� ��.
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
    /// ������ �����ϰ� ���� �� ����ϴ� �Լ�
    /// </summary>
    public void GameExit()
    {
       SceneController.Instance.GameExit(); 
    }

    /// <summary>
    /// Setting���� ���� ���� �� ����ϴ� �Լ�
    /// </summary>
    public void SettingUI()
    {
        // Canvas�� �����´�.
        GameObject canvas = GameObject.Find("Canvas");
        
        // SettingUI Prefab�� �����ؼ� �ҷ�����.
        GameObject settingPrefab = Resources.Load<GameObject>("UI/Setting UI");

        if (settingPrefab != null)
        {
            Instantiate(settingPrefab, canvas.transform);
        }
        else Debug.Log("None Prefab");
    }

    /// <summary>
    /// Setting�� �����ϰ� ���� �� ����ϴ� �Լ�
    /// </summary>
    public void SettingExit()
    {
        Destroy(this.gameObject.transform.parent.gameObject);
    }

    IEnumerator EndOfFrameRoutine()
    {   
        // ���� �������� ���� ������ ���
        yield return new WaitForEndOfFrame();

        // ���⿡�� �ڵ带 ����
        EventManager.Instance.NotifyEvent(EventType.LoadingScene, "Prototype ver 3");   
    }
}
