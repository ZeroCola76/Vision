using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    // �������� �Ծ����� â�� ���� �ϴµ� ���� �� ������ â�� �и��鼭 �����Ѵ�.
    // �ٵ� ��� ���ϴ��� �𸣴ϱ� �� ���� ���� �����ַ� �ߴµ� Pass
    private List<Image> images;
    public Vector3 uiPosition;

    private void Start()
    {
        images = new List<Image>();
        uiPosition = new Vector3(0.0f, -40.0f, 0.0f);
    }

    public void AddPickup(Image _backGround)
    {
        images.Add(_backGround);
        PrintUI();
        Invoke("RemovePickup", 3.0f);
    }

    private void PrintUI()
    {
        foreach (var image in images) 
        {
            image.transform.position += uiPosition;
            image.gameObject.SetActive(true);
        }
    }

    private void RemovePickup()
    {
        images[0].gameObject.SetActive(false);
        images.RemoveAt(0);
    }

    public void GameStart()
    {
        SceneController.Instance.GameStart();
    }
}
