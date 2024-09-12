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

    private float modelHP;   // model(���� data) Player ü��
    private float viewHP;    // view(UI) Player ü�� 

    private bool isChange;

    private bool isInjury;      // �λ�
    private bool isRecovery;    // ȸ��

    private bool isInvincible;  // ���� ����

    // Start is called before the first frame update
    void Start()
    {
        // �ʱ� Player HP�� 100����?
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

        // Update�� ���� �� ��ġ�°� �����ϱ� change �� ���� �������� ����.
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
                /// �ϴ� ü�� 0�Ǹ� You Died UI�� �� �߰� �����س���.
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

        // ü���� �ѹ��� ���δ�? ü���� �ٴ°� ���δ�?
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
