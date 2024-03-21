using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Option : UI_Popup
{
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private TMP_Dropdown _languageDropdown;
    [SerializeField] private Toggle _windowToggle;

    [SerializeField] private ClickableSlider _masterSlider;
    [SerializeField] private ClickableSlider _bgmSlider;
    [SerializeField] private ClickableSlider _seSlider;

    [SerializeField] private GameObject _credit;

    private List<Resolution> _resolutions;
    private int _currentLanguage;
    private int _currentResolution;
    private bool _isWindowed = false;
    private float _masterPower;
    private float _bgmPower;
    private float _sePower;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Sound.Play("UI/ButtonSFX/BackButtonClickSFX");
            QuitOption();
        }
    }


    private void OnEnable()
    {
        InitUI();
    }

    private void InitUI()
    {
        // 저장된 데이터 불러오기
        _resolutions = GameManager.OutGameData.GetAllResolution();
        _currentLanguage = GameManager.OutGameData.GetLanguage();
        _currentResolution = GameManager.OutGameData.GetResolutionIndex();
        _isWindowed = GameManager.OutGameData.IsWindowed();

        _masterPower = GameManager.OutGameData.GetMasterSoundPower();
        _bgmPower = GameManager.OutGameData.GetBGMSoundPower();
        _sePower = GameManager.OutGameData.GetSESoundPower();

        // UI 세팅
        _languageDropdown.onValueChanged.AddListener(GameManager.Locale.LanguageChanged);
        _resolutionDropdown.onValueChanged.AddListener(ResolutionDropdownChanged);
        _resolutionDropdown.options.Clear();

        foreach (var resolution in _resolutions)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
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

        _masterSlider.Slider.value = _masterPower;
        _bgmSlider.Slider.value = _bgmPower;
        _seSlider.Slider.value = _sePower;
    }

    public void LanguageDropdownChanged()
    {
        GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
    }

    private void ResolutionDropdownChanged(int idx)
    {
        GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
        GameManager.OutGameData.SetResolution(idx);
    }

    private void WindowToggleChanged(bool isOn)
    {
        GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
        _isWindowed = isOn;
        GameManager.OutGameData.SetWindow(_isWindowed);
    }

    private void MasterSliderChanged(float power)
    {
        _masterPower = power;
        GameManager.OutGameData.SetMasterSoundPower(power);
        GameManager.Sound.SetSoundVolume(Sounds.BGM);
        GameManager.Sound.SetSoundVolume(Sounds.Effect);
    }

    private void BGMSliderChanged(float power)
    {
        _bgmPower = power;
        GameManager.OutGameData.SetBGMSoundPower(power);
        GameManager.Sound.SetSoundVolume(Sounds.BGM);
    }

    private void SESliderChanged(float power)
    {
        _sePower = power;
        GameManager.OutGameData.SetSESoundPower(power);
        GameManager.Sound.SetSoundVolume(Sounds.Effect);
    }

    public void ReSetOption()
    {
        GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
        GameManager.OutGameData.ReSetOption();
        InitUI();
    }

    public void CreditOption()
    {
        GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
        _credit.SetActive(true);
    }

    public void QuitOption()
    {
        GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
        GameManager.UI.ClosePopup(this);
        if (GameManager.UI.ESCOPopups.Count != 0)
            GameManager.UI.ESCOPopups.Pop();
        GameManager.OutGameData.SaveData();
    }
}
