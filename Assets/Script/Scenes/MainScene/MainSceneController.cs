using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneController : MonoBehaviour
{
    [SerializeField] GameObject Canvas;

    public void StartButton()
    {
        // ���ӿ�����Ʈ�� �����ؼ� �����ֱ� & ������ ������Ʈ�� �� ���� ���� �������� �� Ȱ��ȭ�Ǽ� Ʃ�� �̹��� ���� �ڽ� �����ϱ�
        GameManager.Data.DeckClear();
        Destroy(GameManager.Instance.gameObject);

        bool isfirst = true;
        if(isfirst)
        {
            SceneChanger.SceneChange("CutScene");
            isfirst = false;
        }
        else
        {
            SceneChanger.SceneChange("StageSelectScene");
        }
    }

    public void OptionButton()
    {
        UI_Option go = GameManager.UI.ShowPopup<UI_Option>();
        //GameObject go = Resources.Load<GameObject>("Prefabs/UI/Popup/UI_Option");
        //GameObject.Instantiate(go, Canvas.transform);
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}