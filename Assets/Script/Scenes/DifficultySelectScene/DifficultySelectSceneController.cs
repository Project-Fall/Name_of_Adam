using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DifficultySelectSceneController : MonoBehaviour
{
    [SerializeField] private GameObject _incarnaSelectUI;
    [SerializeField] private GameObject _hallSelectUI;
    [SerializeField] private GameObject _confirmButtonUI;
    [SerializeField] private GameObject _incarnaListUI;

    [SerializeField] private List<GameObject> _incarnaBlocker;
    [SerializeField] private List<GameObject> _incarnaInfo;

    [SerializeField] private List<TextMeshProUGUI> _incarnaPlayerSkillInfo;

    [SerializeField] private UI_HallCard[] _hallCards;

    private enum CurrentUI  
    {
        IncarnationSelect,
        IncarnationInfo,
        HallSelect,
    }

    private CurrentUI _currentUI = CurrentUI.IncarnationSelect;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        GameManager.Sound.Play("UI/UISFX/UIImportantButtonSFX");

        _incarnaSelectUI.SetActive(true);
        _hallSelectUI.SetActive(false);
        _confirmButtonUI.SetActive(false);

        _incarnaBlocker[0].SetActive(!GameManager.OutGameData.IsUnlockedItem(71));
        _incarnaBlocker[1].SetActive(!GameManager.OutGameData.IsUnlockedItem(61));

        GameManager.OutGameData.DataIntegrityCheck();

        foreach (UI_HallCard card in _hallCards)
        {
            card.Init();
        }

        _currentUI = CurrentUI.IncarnationSelect;
    }

    public void Confirm()
    {
        GameManager.Sound.Play("UI/UISFX/UIImportantButtonSFX");

        GameManager.Data.MainDeckSet();
        GameManager.Data.GameData.FallenUnits.Clear();
        GameManager.Data.GameData.FallenUnits.AddRange(GameManager.Data.GameDataMain.DeckUnits);

        if (GameManager.OutGameData.IsUnlockedItem(6))
        {
            GameManager.Data.GameDataMain.DarkEssence = 10;
        }
        else if (GameManager.OutGameData.IsUnlockedItem(3))
        {
            GameManager.Data.GameDataMain.DarkEssence = 7;
        }


        if (GameManager.OutGameData.CutScenePlayCheck(CutSceneType.Tubalcain_Enter)
            && GameManager.Data.GameData.CurrentAct == 1)
        {
            GameManager.OutGameData.SetCutSceneData(CutSceneType.Tubalcain_Enter, true);
            SceneChanger.SceneChangeToCutScene(CutSceneType.Tubalcain_Enter);
        }
        else if (GameManager.OutGameData.CutScenePlayCheck(CutSceneType.Elieus_Enter)
            && GameManager.Data.GameData.CurrentAct == 2)
        {
            GameManager.OutGameData.SetCutSceneData(CutSceneType.Elieus_Enter, true);
            SceneChanger.SceneChangeToCutScene(CutSceneType.Elieus_Enter);
        }
        else if (GameManager.OutGameData.CutScenePlayCheck(CutSceneType.Libiel_Enter)
            && GameManager.Data.GameData.CurrentAct == 3)
        {
            GameManager.OutGameData.SetCutSceneData(CutSceneType.Libiel_Enter, true);
            SceneChanger.SceneChangeToCutScene(CutSceneType.Libiel_Enter);
        }
        else
        {
            SceneChanger.SceneChange("StageSelectScene");
        }
    }

    public void OnIncarnaClick(int incarnaIndex)
    {
        if ((incarnaIndex == 1 && _incarnaBlocker[0].activeSelf) || 
            (incarnaIndex == 2 && _incarnaBlocker[1].activeSelf))
        {
            GameManager.Sound.Play("UI/UISFX/UIFailSFX");
            return;
        }

        GameManager.Sound.Play("UI/UISFX/UIImportantButtonSFX");

        GameManager.Data.GameDataMain.Incarna = GameManager.Resource.Load<Incarna>($"ScriptableObject/Incarna/{_incarnaInfo[incarnaIndex].name}");

        _incarnaListUI.SetActive(false);
        _incarnaSelectUI.SetActive(false);
        _hallSelectUI.SetActive(true);
        _confirmButtonUI.SetActive(true);

        _currentUI = CurrentUI.HallSelect;
    }

    public void OnIncarnaInfoClick(int incarnaIndex)
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");

        _incarnaListUI.SetActive(false);
        _incarnaInfo[incarnaIndex].SetActive(true);

        for (int i = 0; i < 3; i++)
        {
            _incarnaPlayerSkillInfo[3 * incarnaIndex + i].SetText(GameManager.Locale.GetLocalizedPlayerSkillInfo(3 * incarnaIndex + i + 1));
        }

        _currentUI = CurrentUI.IncarnationInfo;
    }

    public void Quit()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");

        if (_currentUI == CurrentUI.IncarnationSelect)
        {
            SceneChanger.SceneChange("ActSelectScene");
        }
        else if (_currentUI == CurrentUI.IncarnationInfo)
        {
            _incarnaInfo.ForEach(obj => { obj.SetActive(false); });
            _confirmButtonUI.SetActive(false);
            _incarnaListUI.SetActive(true);

            _currentUI = CurrentUI.IncarnationSelect;
        }
        else if (_currentUI == CurrentUI.HallSelect)
        {
            _hallSelectUI.SetActive(false);
            _incarnaSelectUI.SetActive(true);
            _incarnaInfo.ForEach(obj => { obj.SetActive(false); });
            _confirmButtonUI.SetActive(false);
            _incarnaListUI.SetActive(true);

            _currentUI = CurrentUI.IncarnationSelect;
        }
    }
}
