using TMPro;
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

        if (GameManager.OutGameData.Data.YohrnClear && !GameManager.OutGameData.Data.IsOnMainTooltipForYohrn)
        {
            GameManager.OutGameData.Data.IsOnMainTooltipForYohrn = true;
            GameManager.UI.ShowPopup<UI_SystemInfo>().Init("YohrnClear", "YohrnTooltip");
        }
        else if (GameManager.OutGameData.Data.SaviorClear && !GameManager.OutGameData.Data.IsOnMainTooltipForSavior)
        {
            GameManager.OutGameData.Data.IsOnMainTooltipForSavior = true;
            GameManager.UI.ShowPopup<UI_SystemInfo>().Init("SaviorClear", "SaviorTooltip");
        }
        else if (GameManager.OutGameData.Data.PhanuelClear && !GameManager.OutGameData.Data.IsOnMainTooltipForPhanuel)
        {

            GameManager.OutGameData.Data.IsOnMainTooltipForPhanuel = true;
            GameManager.UI.ShowPopup<UI_SystemInfo>().Init("PhanuelClear", "PhanuelTooltip");
        }

        GameManager.OutGameData.SaveData();
    }

    public void NewGameButton()
    {
        GameManager.Sound.Play("UI/UISFX/UIImportantButtonSFX");

        if (GameManager.SaveManager.SaveFileCheck())
        {
            GameManager.UI.ShowPopup<UI_SystemSelect>().Init("Restart", ResetAlertYesButton);
        }
        else
        {
            GameManager.Data.DeckClear();
            GameManager.Data.SetDeck(GameManager.OutGameData.SetHallDeck());

            if (GameManager.OutGameData.Data.TutorialClear)
            {
                GameManager.Data.HallDeckSet();
                SceneChanger.SceneChange("ActSelectScene");
            }
            else
            {
                GameManager.Data.GameData.FallenUnits.AddRange(GameManager.Data.GameData.DeckUnits);
                SceneChanger.SceneChangeToCutScene(CutSceneType.Main);
            }

            GameManager.SaveManager.DeleteSaveData();
            GameManager.Data.Init();
        }
    }

    public void ContinueBotton()
    {
        GameManager.Sound.Play("UI/UISFX/UIImportantButtonSFX");

        if (GameManager.SaveManager.SaveFileCheck())
        {
            SceneChanger.SceneChange("StageSelectScene");
            GameManager.SaveManager.LoadGame();
        }
    }

    public void DivineHallButton()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");

        GameManager.Data.SetDeck(GameManager.OutGameData.SetHallDeck());
        UI_MyDeck myDeck = GameManager.UI.ShowPopup<UI_MyDeck>();
        myDeck.Init();
        myDeck.EventInit((DeckUnit) =>
        {
            string infoTooltipKey = null;

            if (DeckUnit.PrivateKey.Contains("Origin"))
            {
                infoTooltipKey = "HallDeleteCannotOriginTooltip";
            }
            else if (GameManager.OutGameData.Data.HallUnit.Find(unit => unit.PrivateKey == DeckUnit.PrivateKey).IsMainDeck)
            {
                infoTooltipKey = "HallDeleteCannotSelectedTooltip";
            }
            else if (DeckUnit.PrivateKey.Contains("OnlyUnit"))
            {
                infoTooltipKey = "HallDeleteCannotOnlyUnitTooltip";
            }

            if (infoTooltipKey != null)
            {
                UI_SystemInfo systemInfo = GameManager.UI.ShowPopup<UI_SystemInfo>();
                systemInfo.Init("HallDeleteCannotInfo", infoTooltipKey);
            }
            else
            {
                UI_SystemSelect systemSelect = GameManager.UI.ShowPopup<UI_SystemSelect>();
                systemSelect.Init("HallDeleteInfo", () =>
                {
                    GameManager.OutGameData.RemoveHallUnit(DeckUnit.PrivateKey);
                    GameManager.OutGameData.SaveData();
                    GameManager.UI.ClosePopup();

                    GameManager.Data.GetDeck().Remove(DeckUnit);
                    myDeck.Init();

                    GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
                });
            }
        }, CurrentEvent.Hall_Delete);
    }

    public void SanctumButton()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");

        SceneChanger.SceneChange("ProgressShopScene");
    }

    public void OptionButton()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");

        GameManager.UI.ShowPopup<UI_Option>();
    }

    public void ResetAlertYesButton()
    {
        // 게임오브젝트를 생성해서 보내주기 & 생성한 오브젝트가 맵 선택 씬에 도달했을 때 활성화되서 튜토 이미지 띄우고 자신 삭제하기
        GameManager.Sound.Play("UI/UISFX/UIImportantButtonSFX");

        GameManager.Data.DeckClear();
        GameManager.Data.SetDeck(GameManager.OutGameData.SetHallDeck());

        if (GameManager.OutGameData.Data.TutorialClear)
        {
            GameManager.Data.HallDeckSet();
            SceneChanger.SceneChange("ActSelectScene");
        }
        else
        {
            GameManager.Data.GameData.FallenUnits.AddRange(GameManager.Data.GameData.DeckUnits);
            SceneChanger.SceneChange("CutScene");
        }

        GameManager.SaveManager.DeleteSaveData();
        GameManager.Data.Init();
    }

    public void DiscordButton()
    {
        Application.OpenURL("https://discord.com/invite/DhN6RRYxy5");
    }

    public void XButton()
    {
        Application.OpenURL("https://x.com/Revelatio_");
    }

    public void ExitButton()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
        Application.Quit();
    }

    public void ButtonHover()
    {
        GameManager.Sound.Play("UI/UISFX/UIHoverSFX");
    }
}