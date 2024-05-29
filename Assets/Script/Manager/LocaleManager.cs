using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;


public class LocaleManager : MonoBehaviour
{
    Locale currentLocale;
    bool isChangingLanguage;

    int currentLocaleIndex;
    /// <summary> 0 = EN, 1 = KR, 2 = JA </summary>
    public int CurrentLocaleIndex => currentLocaleIndex;

    public void Init()
    {
        isChangingLanguage = false;

        LanguageChanged(GameManager.OutGameData.GetLanguage());
    }

    public string GetLocalizedPlayerSkillInfo(int skillIdx)
    {
        int playerID = (skillIdx - 1) / 3 + 1;
        int skillID = (skillIdx - 1) % 3 + 1;
        string info = $"Player_{playerID}_{skillID}";

        switch (skillIdx)
        {
            case 2: if (GameManager.OutGameData.IsUnlockedItem(54)) info += "_Up"; break;
            case 4: if (GameManager.OutGameData.IsUnlockedItem(62)) info += "_Up"; break;
            case 6: if (GameManager.OutGameData.IsUnlockedItem(64)) info += "_Up"; break;
            case 8: if (GameManager.OutGameData.IsUnlockedItem(72)) info += "_Up"; break;
        }

        return GetLocalizedPlayerSkillInfo(info);
    }

    public string GetLocalizedBattleScene(string battleSceneStr) => GetLocalizedString("BattleSceneTable", battleSceneStr);

    public string GetLocalizedProgress(string progressInfo) => GetLocalizedString("ProgressSceneTable", progressInfo);

    public string GetLocalizedUpgrade(string upgradeInfo) => GetLocalizedString("UpgradeTable", upgradeInfo);

    public string GetLocalizedEventScene(string eventSceneStr) => GetLocalizedString("EventSceneTable", eventSceneStr);

    public string GetLocalizedPlayerSkillInfo(string playerSkillInfo) => GetLocalizedString("PlayerSkillInfoTable", playerSkillInfo);

    public string GetLocalizedPlayerSkillName(string playerSkillName) => GetLocalizedString("PlayerSkillNameTable", playerSkillName);

    public string GetLocalizedScriptName(string scriptName) => GetLocalizedString("ScriptNameTable", scriptName);

    public string GetLocalizedScriptInfo(string scriptInfo) => GetLocalizedString("ScriptInfoTable", scriptInfo);

    public string GetLocalizedBuffName(string buffName) => GetLocalizedString("BuffNameTable", buffName);

    public string GetLocalizedBuffInfo(string buffInfo) => GetLocalizedString("BuffInfoTable", buffInfo);

    public string GetLocalizedStigmaName(string stigmaName) => GetLocalizedString("StigmaNameTable", stigmaName);

    public string GetLocalizedStigmaInfo(string stigmaInfo) => GetLocalizedString("StigmaInfoTable", stigmaInfo);

    public string GetLocalizedUnitName(string unitName) => GetLocalizedString("UnitTable", unitName);

    public string GetLocalizedUI(string uiStr) => GetLocalizedString("UITable", uiStr);

    public string GetLocalizedSystem(string system) => GetLocalizedString("SystemTable", system);

    private string GetLocalizedString(string tableName, string key)
    {
        string str = LocalizationSettings.StringDatabase.GetLocalizedString(tableName, key, currentLocale);
        if (str.Contains("No translation"))
        {
            Debug.Log($"'{key}' Localization is faied.");
            return key;
        }

        return str;
    }

    public void LanguageChanged(int localeIndex)
    {
        if (isChangingLanguage)
            return;

        StartCoroutine(ChangeLanuage(localeIndex));
    }

    IEnumerator ChangeLanuage(int localeIndex)
    {
        isChangingLanguage = true;

        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeIndex];
        currentLocale = LocalizationSettings.SelectedLocale;
        currentLocaleIndex = localeIndex;
        GameManager.OutGameData.SetLanguage(localeIndex);

        Debug.Log(LocalizationSettings.SelectedLocale.LocaleName);

        isChangingLanguage = false;
    }
}
