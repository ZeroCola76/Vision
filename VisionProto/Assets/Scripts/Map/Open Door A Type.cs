using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorAType : MonoBehaviour
{
    public GameObject leftDoor;
    public GameObject rightDoor;

    // 이 문은 Open Door
    Vector3 openLeftDoor;
    Vector3 openRightDoor;

    // 회전되어 있는 문을 여는 방법을 찾아보자.
    bool isOpen;
    float deltaTime;

    public float count;

    string playerTag = "Player";

    // Start is called before the first frame update
    void Start()
    {
        // Door의 현재 Rotation을 받는다. 
        // Rotation에 따라 문이 열리는 Position이 바뀌게 하기 위해 필요한 준비 작업
        count = 2.5f;
        deltaTime = 0;
        /// 왼쪽과 오른쪽 문중 하나 선택해서 현재 회전한 각도를 가져온다.
        Vector3 doorRotation = leftDoor.transform.rotation.eulerAngles;
        float theta = doorRotation.y;

        /// Mathf.Sin, Math.Cos을 사용하기 위해 Radian을 사용해야 해서 Degree -> Radian으로 바꾼다.
        float radian = theta * Mathf.Deg2Rad;

        /// Door의 Position을 저장해둔다.
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
            // 초기 Transform과 나중 Transform을 계산해서 변화량만큼 도달 했다면 끝
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