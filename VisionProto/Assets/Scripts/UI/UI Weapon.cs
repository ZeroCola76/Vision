using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWeapon : MonoBehaviour, IListener
{
    // ���� �ʿ��ұ�?
    // �ִ� ź�� ����
    // ���� ź�� ���� ������ ���� �ȴ�!

    public TMP_Text textCurrentBullet;
    public TMP_Text textMaxBullet;
    
    private float currentBullet;    // ���� ź�� ��
    private float maxBullet;        // �ִ� ź�� ��

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

        // �̰� �Ẹ�� �ʹ�
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
                    // Update�� �ʿ� ���� ���� �־�.
                    ChangeBullet();
                }
                break;
        }
    }
}
