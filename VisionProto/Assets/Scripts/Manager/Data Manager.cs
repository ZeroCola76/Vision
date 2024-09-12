using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct PlayerInformation
{
    public Vector3 position;
    public Quaternion rotation;
}

/// <summary>
/// Save Load �����ϱ� ���� ���� ���̴�.
/// </summary>
public class DataManager : Singleton<DataManager>
{
    private string path;

    private void Awake()
    {
        path = Application.persistentDataPath + "/";  
    }
    

    // data�� ���� ���� �ʿ�� ����.
    public void SaveData(PlayerInformation _playerinfo)
    {
        string data = JsonUtility.ToJson(_playerinfo);

        File.WriteAllText(path + "PlayerInformation", data);
    }

    // ����� Player ������ Load �Ѵ�.
    public PlayerInformation LoadData()
    {
        string data = default;

        data = File.ReadAllText(path + "PlayerInformation");

        PlayerInformation dataInfomation = JsonUtility.FromJson<PlayerInformation>(data);

        return dataInfomation;
    }

    // Data���� ��� �ʿ��ұ�? -> ���� ��ġ ������ ��� ������ �ȴ� !
    // �׷��� Transform ����� ����? ���� �� ���µ�?
}
