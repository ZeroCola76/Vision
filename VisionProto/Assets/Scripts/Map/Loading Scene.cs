using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour, IListener
{
    public Image progressBar;

    // 생각해보자.
    // Loading UI를 Instanitate로 띄울거야.
    // string 전달을 어떻게 할 거야 
    // event manager로 Instanitage로 생성하고 그 다음에 notify로 알린다.
    // 이거 어떤데? 굿

    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddEvent(EventType.LoadingScene, OnEvent);
        progressBar.fillAmount = 0;
    }

    // 음 0 ~ 100퍼까지 채우는 것을 보여주고 싶은데 지금은 잠깐 들렸다 가는 것 같아.
    // 됐다 
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
