using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Steamworks;

public class UI_Option : UI_Popup
{
    [SerializeField] private TextMeshProUGUI _version;

    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private TMP_Dropdown _languageDropdown;
    [SerializeField] private Toggle _windowToggle;

    [SerializeField] private ClickableSlider _masterSlider;
    [SerializeField] private ClickableSlider _bgmSlider;
    [SerializeField] private ClickableSlider _seSlider;

    [SerializeField] private ClickableSlider _gameSpeedSlider;

    [SerializeField] private GameObject _credit;

    [SerializeField] private GameObject _gameResetButton;

    private List<Resolution> _resolutions;
    private int _currentLanguage;
    private int _currentResolution;
    private bool _isWindowed = false;
    private float _masterPower;
    private float _bgmPower;
    private float _sePower;
    private float _gameSpeed;

    private bool _isInitialized = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Sound.Play("UI/UISFX/UICloseSFX");
            QuitOption();
        }
    }


    private void OnEnable()
    {
        InitUI();
    }

    private void InitUI()
    {
        _version.text = GameManager.OutGameData.Data.Version;

        // 저장된 데이터 불러오기
        _resolutions = GameManager.OutGameData.GetAllResolution();
        _currentLanguage = GameManager.OutGameData.Data.Language;
        _currentResolution = GameManager.OutGameData.Data.Resolution;
        _isWindowed = GameManager.OutGameData.Data.IsWindowed;

        _masterPower = GameManager.OutGameData.Data.MasterSoundPower;
        _bgmPower = GameManager.OutGameData.Data.BGMSoundPower;
        _sePower = GameManager.OutGameData.Data.SESoundPower;

        _gameSpeed = GameManager.OutGameData.Data.BattleSpeed;

        // UI 세팅
        _languageDropdown.onValueChanged.AddListener(GameManager.Locale.LanguageChanged);
        _resolutionDropdown.onValueChanged.AddListener(ResolutionDropdownChanged);
        _resolutionDropdown.options.Clear();

        foreach (var resolution in _resolutions)
        {
            TMP_Dropdown.OptionData option = new();
            option.text = $"{resolution.width} x {resolution.height}";
            _resolutionDropdown.options.Add(option);
        }

        _languageDropdown.value = _currentLanguage;
        _resolutionDropdown.RefreshShownValue();
        _resolutionDropdown.value = _currentResolution;

        _windowToggle.onValueChanged.AddListener(WindowToggleChanged);
        _windowToggle.isOn = _isWindowed;

        _masterSlider.Slider.onValueChanged.AddListener(MasterSliderChanged);
        _bgmSlider.Slider.onValueChanged.AddListener(BGMSliderChanged);
        _seSlider.Slider.onValueChanged.AddListener(SESliderChanged);

        _gameSpeedSlider.Slider.onValueChanged.AddListener(GameSpeedSliderChanged);

        _masterSlider.Slider.value = _masterPower;
        _bgmSlider.Slider.value = _bgmPower;
        _seSlider.Slider.value = _sePower;

        _gameSpeedSlider.Slider.value = _gameSpeed;

        if (SceneManager.GetActiveScene().name != "MainScene")
        {
            _gameResetButton.SetActive(false);
        }


        _isInitialized = true;
    }

    public void LanguageDropdownChanged()
    {
        if (_isInitialized)
            GameManager.Sound.Play("UI/UISFX/UIUnimportantButtonSFX");
    }

    private void ResolutionDropdownChanged(int idx)
    {
        if (_isInitialized)
            GameManager.Sound.Play("UI/UISFX/UIUnimportantButtonSFX");
        GameManager.OutGameData.Data.Resolution = idx;
        GameManager.OutGameData.SetResolution();
    }

    private void WindowToggleChanged(bool isOn)
    {
        if (_isInitialized)
            GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
        _isWindowed = isOn;
        GameManager.OutGameData.Data.IsWindowed = _isWindowed;
        GameManager.OutGameData.SetResolution();
    }

    private void MasterSliderChanged(float power)
    {
        _masterPower = power;
        GameManager.OutGameData.Data.MasterSoundPower = power;
        GameManager.Sound.SetSoundVolume(Sounds.BGM);
        GameManager.Sound.SetSoundVolume(Sounds.Effect);
    }

    private void BGMSliderChanged(float power)
    {
        _bgmPower = power;
        GameManager.OutGameData.Data.BGMSoundPower = power;
        GameManager.Sound.SetSoundVolume(Sounds.BGM);
    }

    private void SESliderChanged(float power)
    {
        _sePower = power;
        GameManager.OutGameData.Data.SESoundPower = power;
        GameManager.Sound.SetSoundVolume(Sounds.Effect);
    }

    private void GameSpeedSliderChanged(float speed)
    {
        _gameSpeed = speed;
        GameManager.OutGameData.Data.BattleSpeed = speed;
    }

    public void ReSetOption()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
        GameManager.OutGameData.ResetOption();
        InitUI();
    }

    public void CreditOption()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
        _credit.SetActive(true);
    }

    public void QuitOption()
    {
        GameManager.Sound.Play("UI/UISFX/UIImportantButtonSFX");
        GameManager.UI.ClosePopup(this);
        if (GameManager.UI.ESCOPopups.Count != 0)
            GameManager.UI.ESCOPopups.Pop();
        GameManager.OutGameData.SaveData();
    }

    public void GameReset()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
        GameManager.UI.ShowPopup<UI_SystemSelect>().Init("GameResetInfo", () => {
            GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
            GameManager.VisualEffect.ClearAllEffect();
            GameManager.UI.CloseAllOption();
            SceneChanger.SceneChange("MainScene");
            GameManager.Data.MainDeckLayoutSet();
            GameManager.SaveManager.DeleteSaveData();
            GameManager.OutGameData.DeleteAllData();
        });
    }
}
