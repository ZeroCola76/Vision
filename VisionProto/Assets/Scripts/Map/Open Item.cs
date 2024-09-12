using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenItem : MonoBehaviour
{
    public GameObject cover;
    public bool isControl;
    public float openSpeed;

    private Quaternion openItemDegree;

    // Start is called before the first frame update
    void Start()
    {
        openItemDegree = Quaternion.Euler(0, 90f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(isControl)
        {
            cover.transform.localRotation = Quaternion.Lerp(cover.transform.localRotation, openItemDegree, Time.deltaTime * openSpeed);
        }
    }
}
