using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWeapon : MonoBehaviour, IListener
{
    // 뭐가 필요할까?
    // 최대 탄알 개수
    // 현재 탄알 수만 가지고 오면 된다!

    public TMP_Text textCurrentBullet;
    public TMP_Text textMaxBullet;
    
    private float currentBullet;    // 현재 탄알 수
    private float maxBullet;        // 최대 탄알 수

    void Start()
    {
        textCurrentBullet.text = "0";
        textMaxBullet.text = "/ 0";
        EventManager.Instance.AddEvent(EventType.WeaponBullet, OnEvent);
    }

    private void ChangeBullet()
    {
        float remainBullet = maxBullet - currentBullet;
        
        textCurrentBullet.text = remainBullet.ToString();

        // 이거 써보고 싶다
        //StringBuilder stringBuilder = new StringBuilder();
//         stringBuilder.Append("/ ");
//         stringBuilder.Append(maxBullet);

        textMaxBullet.text = "/ "+maxBullet;
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType) 
        {
            case EventType.WeaponBullet:
                {
                    UIBulletInformation bulletInformation = (UIBulletInformation)param;
                    currentBullet = bulletInformation.currentBullet;
                    maxBullet = bulletInformation.maxBullet;
                    // Update가 필요 없을 수도 있어.
                    ChangeBullet();
                }
                break;
        }
    }
}
