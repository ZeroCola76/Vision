using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorAType : MonoBehaviour
{
    public GameObject leftDoor;
    public GameObject rightDoor;

    // �� ���� Open Door
    Vector3 openLeftDoor;
    Vector3 openRightDoor;

    // ȸ���Ǿ� �ִ� ���� ���� ����� ã�ƺ���.
    bool isOpen;
    float deltaTime;

    public float count;

    string playerTag = "Player";

    // Start is called before the first frame update
    void Start()
    {
        // Door�� ���� Rotation�� �޴´�. 
        // Rotation�� ���� ���� ������ Position�� �ٲ�� �ϱ� ���� �ʿ��� �غ� �۾�
        count = 2.5f;
        deltaTime = 0;
        /// ���ʰ� ������ ���� �ϳ� �����ؼ� ���� ȸ���� ������ �����´�.
        Vector3 doorRotation = leftDoor.transform.rotation.eulerAngles;
        float theta = doorRotation.y;

        /// Mathf.Sin, Math.Cos�� ����ϱ� ���� Radian�� ����ؾ� �ؼ� Degree -> Radian���� �ٲ۴�.
        float radian = theta * Mathf.Deg2Rad;

        /// Door�� Position�� �����صд�.
        Vector3 openDoorPosition = new Vector3(12f, 0f, 0);
        openLeftDoor = leftDoor.transform.localPosition - openDoorPosition;
        openRightDoor = rightDoor.transform.localPosition + openDoorPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen)
        {
            if (deltaTime == 0)
                SoundManager.Instance.PlayEffectSound(SFX.OpenDoor_Big, this.transform);
            leftDoor.transform.localPosition = Vector3.Lerp(leftDoor.transform.localPosition, openLeftDoor, Time.deltaTime * count);
            rightDoor.transform.localPosition = Vector3.Lerp(rightDoor.transform.localPosition, openRightDoor, Time.deltaTime * count);
            deltaTime += Time.deltaTime;
            // �ʱ� Transform�� ���� Transform�� ����ؼ� ��ȭ����ŭ ���� �ߴٸ� ��
            if (deltaTime > 3.0f)
            {
                deltaTime = 0f;
                isOpen = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isOpen = true;
        }
    }
}