using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameClear : MonoBehaviour
{
    string playerTag = "Player";
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            SceneController.Instance.LoadScene("GameClear");

            EventManager.Instance.RemoveAllEvent();
        }
    }
}
