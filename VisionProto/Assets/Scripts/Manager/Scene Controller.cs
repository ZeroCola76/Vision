using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Map
{
    AmbientLight,
    Cascades,
    CCTV,
    EmissiveMaterial,
    SettingLightProbe,
    SampleScene,
    PrefabAudioTest,
}

/// <summary>
/// Scene Controller�� ���� ������ Untiy�� Scene Manager�� �־ �׷���.
/// Scene���� �����, ���� �������� �̵��� ����Ѵ�.
/// ����ϱ� ���ϰ��ϱ� ���� Enum���� ����
/// 0620 �̿뼺
/// </summary>
public class SceneController : Singleton<SceneController>
{
    List<string> paths;

    private void Awake()
    {
        ListAllScenes();
    }

    /// <summary>
    /// Enum���� ã�� (���� Scene���� ��� �̻��)
    /// </summary>
    /// <param name="SceneMap"></param>
    public void LoadScene(Map SceneMap)
    {
        string sceneName = "";

        // Scene Name�� �޾ƿ��� �ʹ�. 
        // ���� �����Ѱ� �׳� String Name �ְ� �ϴ°� ���� �������� �ϴ� �׽�Ʈ�غ�
        // �� �ȴ� ��

        switch (SceneMap)
        {
            case Map.AmbientLight:
                {
                    sceneName = "Ambient Light";
                    SceneManager.LoadScene(sceneName);
                }
                break;
            case Map.Cascades:
                {
                    sceneName = "Cascades";
                    SceneManager.LoadScene(sceneName);
                }
                break;
            case Map.CCTV:
                {
                    sceneName = "CCTV";
                    SceneManager.LoadScene(sceneName);
                }
                break;
            case Map.EmissiveMaterial:
                {
                    sceneName = "Emissive Material";
                    SceneManager.LoadScene(sceneName);
                }
                break;
            case Map.SettingLightProbe:
                {
                    sceneName = "Setting Light Probe";
                    SceneManager.LoadScene(sceneName);
                }
                break;
            case Map.SampleScene:
                {
                    sceneName = "SampleScene";
                    SceneManager.LoadScene(sceneName);
                }
                break;
            case Map.PrefabAudioTest:
                {
                    sceneName = "Prefab Audio Test";
                    SceneManager.LoadScene(sceneName);
                }
                break;
        }
    }


    /// <summary>
    /// Scene �̸����� �����ϴ� �Լ�
    /// </summary>
    /// <param name="sceneName">Please Writing Scene Title Name</param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // ���ϰ� ������ �����
    // Project�� �ִ� ��� Scene���� �ϳ��� ��Ƽ� �����Ѵ�.
    // ���߿� ������ ���� �ϸ� ���� �� ����. -> enum ��������.

    void ListAllScenes()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        paths = new List<string>();

        for (int i = 0; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            paths.Add(path);
        }
    }

    public void GameStart()
    {
        LoadScene("ProtoType ver 3");
        PlayerInformation playerInfo = new PlayerInformation();

        // Game Init Position
        playerInfo.position = new Vector3(125.9f, 14.5f, -629.2f);
        playerInfo.rotation = Quaternion.identity;

        DataManager.Instance.SaveData(playerInfo);
    }

    /// <summary>
    /// �� ����
    /// </summary>
    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
