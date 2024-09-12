using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


[DefaultExecutionOrder(-1)]
public class UIHP : MonoBehaviour, IListener
{
    public Image imageHP;
    public TMP_Text textHP;

    public float HPSpeed = 0.1f;
    public float HPSubSpeed = 10f;

    private float modelHP;   // model(실제 data) Player 체력
    private float viewHP;    // view(UI) Player 체력 

    private bool isChange;

    private bool isInjury;      // 부상
    private bool isRecovery;    // 회복

    private bool isInvincible;  // 무적 상태

    // Start is called before the first frame update
    void Start()
    {
        // 초기 Player HP는 100이지?
        modelHP = 100;
        viewHP = 100;
        EventManager.Instance.AddEvent(EventType.PlayerHPUI, OnEvent);

        Initalize();
        isInvincible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8))
        {
            isInvincible = true;
        }

        // Update를 많이 안 거치는게 좋으니까 change 할 때만 들어오도록 하자.
        if (!isChange)
            return;

        // view HP
        if (modelHP != viewHP)
        {
            float currentHP = viewHP - modelHP;

            if (currentHP > 0)
                isInjury = true;
            else
                isRecovery = true;
        }

        if (isRecovery)
        {
            viewHP += Time.deltaTime * HPSpeed;
            imageHP.fillAmount += HPSpeed * Time.deltaTime;
            textHP.text = (Mathf.Ceil(imageHP.fillAmount * 100)).ToString();

            if (viewHP >= modelHP)
            {
                if (viewHP > 100)
                {
                    viewHP = 100f;
                    modelHP = 100f;
                }
                imageHP.fillAmount = modelHP * 0.01f;
                viewHP = modelHP;
                isRecovery = false;
                isChange = false;
            }
        }

        if (isInjury)
        {
            viewHP -= Time.deltaTime * HPSpeed;
            imageHP.fillAmount -= HPSpeed * Time.deltaTime;
            textHP.text = (Mathf.Ceil(imageHP.fillAmount * 100)).ToString();

            if ((Mathf.Ceil(imageHP.fillAmount * 100)) <= modelHP)
            {
                if (viewHP < 0)
                {
                    viewHP = 0;
                    modelHP = 0;
                }
                imageHP.fillAmount = DowntotheSecondDecimalPlace(modelHP * 0.01f);
                viewHP = modelHP;
                textHP.text = modelHP.ToString();
                isInjury = false;
                isChange = false;
            }

            if (!isInvincible)
            {
                /// 일단 체력 0되면 You Died UI가 안 뜨게 설정해놓자.
                if (imageHP.fillAmount <= 0.0f)
                {
                    viewHP = 0;
                    modelHP = 0;
                    Time.timeScale = 0;
                    GameObject canvas = GameObject.Find("Canvas");

                    GameObject youDiedPrefab = Resources.Load<GameObject>("UI/You Died");
                    Cursor.lockState = CursorLockMode.None;

                    if (youDiedPrefab != null)
                    {
                        Instantiate(youDiedPrefab, canvas.transform);
                    }
                    else Debug.Log("None Prefab");
                }
            }

        }
    }

    void Initalize()
    {
        imageHP.fillAmount = 1.0f;
        imageHP.type = Image.Type.Filled;
        imageHP.fillMethod = Image.FillMethod.Horizontal;
        imageHP.fillOrigin = (int)Image.OriginHorizontal.Left;
    }

    // HP
    void ChangeHP(int hp)
    {
        isChange = true;
        modelHP = hp;

        // 체력이 한번에 까인다? 체력이 다는게 보인다?
        //textHP.text = hp.ToString();
    }

    private float DowntotheSecondDecimalPlace(float value)
    {
        value *= 100;

        value = Mathf.Floor(value);

        value *= 0.01f;

        return value;
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.PlayerHPUI:
                {


                    ChangeHP((int)param);
                }
                break;
        }
    }

}
