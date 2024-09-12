using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    // 아이템을 먹었으면 창이 떠야 하는데 여러 개 먹으면 창이 밀리면서 떠야한다.
    // 근데 어떤걸 원하는지 모르니까 두 가지 만들어서 보여주려 했는데 Pass
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
