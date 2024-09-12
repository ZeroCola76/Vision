using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        // Tag로 Player면 저장하자.
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInformation playerInfo = new PlayerInformation();
            playerInfo.position = collision.gameObject.transform.position;
            playerInfo.rotation = collision.gameObject.transform.rotation;

            DataManager.Instance.SaveData(playerInfo);
        }
    }
}
