using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ActSelectSceneController : MonoBehaviour
{
    [SerializeField] private GameObject _actSelector;

    [SerializeField] private GameObject _actInfo;

    [SerializeField] private Image _actInfoActImage;
    [SerializeField] private TextMeshProUGUI _actInfoActText;

    [SerializeField] private TextMeshProUGUI _actInfoText;
    [SerializeField] private List<TextMeshProUGUI> _actInfoChapterText;
    [SerializeField] private List<Image> _actInfoChapterImage;

    [SerializeField] private Toggle _isReplayCutScene;
    [SerializeField] private Toggle _isReplayDialog;

    [SerializeField] private List<GameObject> _actLock;

    private bool _isInfoOn = false;

    private void Start()
    {
        GameManager.Sound.Clear();
        GameManager.Sound.SceneBGMPlay("DifficultySelectScene");
        _isReplayCutScene.isOn = GameManager.OutGameData.Data.IsReplayCutScene;
        _isReplayDialog.isOn = GameManager.OutGameData.Data.IsReplayDialog;
        ActLockCheck();
    }

    public void ActSelect(int act)
    {
        GameManager.Data.GameData.CurrentAct = act;
        SceneChanger.SceneChange("DifficultySelectScene");
    }

    public void ActInfoSelect(int act)
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");

        _actSelector.SetActive(false);
        _actInfo.SetActive(true);

        Dictionary<int, string> imageDic = new()
        {
            { 11, "EliteBattle_투발카인" }, { 12, "EliteBattle_라헬&레아" }, { 13, "BossBattle_바누엘" },
            { 21, "EliteBattle_엘리우스" }, { 22, "EliteBattle_압바임" }, { 23, "BossBattle_구원자" },
            { 31, "EliteBattle_리비엘" }, { 32, "EliteBattle_아라벨라" }, { 33, "BossBattle_욘" },
        };

        string key;
        if (act == 99)
            key = "Endless";
        else
            key = "Act" + act.ToString();

        _actInfoActImage.sprite = GameManager.Resource.Load<Sprite>($"Arts/UI/ActSelect/{key}");
        _actInfoActText.text = GameManager.Locale.GetLocalizedActSelect($"{key}Title");

        _actInfoText.text = GameManager.Locale.GetLocalizedActSelect($"ActInfo_{key}");

        for (int i = 0; i < 3; i++)
        {
            if (key == "Endless")
            {
                _actInfoChapterText[i].gameObject.SetActive(false);
                _actInfoChapterImage[i].gameObject.SetActive(false);
            }
            else
            {
                _actInfoChapterText[i].gameObject.SetActive(true);
                _actInfoChapterImage[i].gameObject.SetActive(true);
                _actInfoChapterText[i].text = GameManager.Locale.GetLocalizedActSelect($"ActInfoChapter_{key}_Chapter{i + 1}");
                _actInfoChapterImage[i].sprite = GameManager.Resource.Load<Sprite>($"Arts/StageSelect/Node/{imageDic[act * 10 + (i + 1)]}");
            }
        }

        _isInfoOn = true;
    }

    public void ActLockCheck()
    {
        _actLock[0].SetActive(!GameManager.OutGameData.Data.PhanuelClear);
        _actLock[1].SetActive(!GameManager.OutGameData.Data.SaviorClear);
        _actLock[2].SetActive(!GameManager.OutGameData.Data.YohrnClear);
    }

    public void CutSceneToggle(bool isOn)
    {
        GameManager.OutGameData.Data.IsReplayCutScene = isOn;
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
    }

    public void ConversationToggle(bool isOn)
    {
        GameManager.OutGameData.Data.IsReplayDialog = isOn;
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
    }

    public void Quit()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");

        if (_isInfoOn)
        {
            _actSelector.SetActive(true);
            _actInfo.SetActive(false);
            _isInfoOn = false;
        }
        else
            SceneChanger.SceneChange("MainScene");
    }
}
