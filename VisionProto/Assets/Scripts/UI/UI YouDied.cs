using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI OnClick�� �ְ� ����ϸ� �ȴ�.
/// You Died Canvas�� �ְ� ����ϸ� �ȴ�.
/// </summary>
public class UIYouDied : MonoBehaviour
{
    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void Respawn()
    {
        // �ϴ��� Scene�� ���������.
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
        //             // ���� �ȵ� ������ ���� �ؾ� ��.
        //             StartCoroutine(EndOfFrameRoutine());
        //         }
        //         else Debug.Log("None Prefab");

    }


    /// <summary>
    /// ��¥ �����Ⱑ �ƴҰ� �ƴϾ� Ÿ��Ʋ�� ���� �׷��� �ƴұ�?
    /// </summary>
    public void GameExit()
    {
        SceneController.Instance.GameExit();

//         EventManager.Instance.ChangeScene();
//         SceneController.Instance.LoadScene("Prototype UI");
    }

    IEnumerator EndOfFrameRoutine()
    {
        // ���� �������� ���� ������ ���
        yield return new WaitForEndOfFrame();

        // ���⿡�� �ڵ带 ����
        EventManager.Instance.NotifyEvent(EventType.LoadingScene, "Prototype ver 3");
        EventManager.Instance.RemoveAllEvent();
    }
}
