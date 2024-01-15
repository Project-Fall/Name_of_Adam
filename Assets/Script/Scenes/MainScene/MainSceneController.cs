using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneController : MonoBehaviour
{
    [SerializeField] GameObject Canvas;
    [SerializeField] GameObject ContinueBox;

    private void Start()
    {
        if (GameManager.SaveManager.SaveFileCheck())
            ContinueBox.SetActive(true);
        else
            ContinueBox.SetActive(false);
    }

    public void StartButton()
    {
        // 게임오브젝트를 생성해서 보내주기 & 생성한 오브젝트가 맵 선택 씬에 도달했을 때 활성화되서 튜토 이미지 띄우고 자신 삭제하기
        GameManager.Data.DeckClear();
        GameManager.Data.GameData.FallenUnits.AddRange(GameManager.Data.GameDataMain.DeckUnits);
        Destroy(GameManager.Instance.gameObject);

        GameManager.SaveManager.DeleteSaveData();
        GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");

        if (GameManager.OutGameData.isTutorialClear())
        {
            //GameManager.Data.HallDeckSet();
            //GameManager.Data.HallSelectedDeckSet();
            SceneChanger.SceneChange("DifficultySelectScene");
        }
        else
        {
            SceneChanger.SceneChange("CutScene");
        }
    }

    public void ContinueBotton()
    {
        GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
        if (GameManager.SaveManager.SaveFileCheck())
            SceneChanger.SceneChange("StageSelectScene");
    }
    public void ProgressButton()
    {
        GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
        SceneChanger.SceneChange("ProgressShopScene");
    }

    public void HallButton()
    {
        GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
        GameManager.Data.HallDeckSet();
        GameManager.UI.ShowPopup<UI_MyDeck>("UI_MyDeck").Init(false);
    }

    public void OptionButton()
    {
        GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
        UI_Option go = GameManager.UI.ShowPopup<UI_Option>();
        //GameObject go = Resources.Load<GameObject>("Prefabs/UI/Popup/UI_Option");
        //GameObject.Instantiate(go, Canvas.transform);
    }



    public void ExitButton()
    {
        GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
        Application.Quit();
    }
}