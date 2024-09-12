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
/// Save Load 구현하기 위해 만든 것이다.
/// </summary>
public class DataManager : Singleton<DataManager>
{
    private string path;

    private void Awake()
    {
        path = Application.persistentDataPath + "/";  
    }
    

    // data를 따로 만들 필요는 없다.
    public void SaveData(PlayerInformation _playerinfo)
    {
        string data = JsonUtility.ToJson(_playerinfo);

        File.WriteAllText(path + "PlayerInformation", data);
    }

    // 저장된 Player 정보를 Load 한다.
    public PlayerInformation LoadData()
    {
        string data = default;

        data = File.ReadAllText(path + "PlayerInformation");

        PlayerInformation dataInfomation = JsonUtility.FromJson<PlayerInformation>(data);

        return dataInfomation;
    }

    // Data에는 어떤게 필요할까? -> 현재 위치 정보만 들고 있으면 된다 !
    // 그러면 Transform 말고는 없나? 딱히 뭐 없는듯?
}
