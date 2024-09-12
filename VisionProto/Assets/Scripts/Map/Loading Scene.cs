using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour, IListener
{
    public Image progressBar;

    // �����غ���.
    // Loading UI�� Instanitate�� ���ž�.
    // string ������ ��� �� �ž� 
    // event manager�� Instanitage�� �����ϰ� �� ������ notify�� �˸���.
    // �̰� ���? ��

    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddEvent(EventType.LoadingScene, OnEvent);
        progressBar.fillAmount = 0;
    }

    // �� 0 ~ 100�۱��� ä��� ���� �����ְ� ������ ������ ��� ��ȴ� ���� �� ����.
    // �ƴ� 
    IEnumerator LoadSceneProcess(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float progress = 0;

        while(!op.isDone)
        {
            progress = Mathf.MoveTowards(progress, op.progress, Time.deltaTime);
            progressBar.fillAmount = progress;

            if(progress >= 0.9f)
            {
                progressBar.fillAmount = 1;
                op.allowSceneActivation = true;
                EventManager.Instance.RemoveEvent(EventType.LoadingScene);
            }
            yield return null;
        }
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType) 
        {
            case EventType.LoadingScene:
                {
                    string sceneName = (string)param;

                    StartCoroutine(LoadSceneProcess(sceneName));
                }
                break;
        }
    }

}
