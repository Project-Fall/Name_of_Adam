using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public enum SteamAchievementType
{
    None = 0,
    KILL_TUBALCAIN,
    KILL_RAHELLEA,
    KILL_PHANUEL,
    KILL_ELIEUS,
    KILL_YANA,
    KILL_APPAIM,
    KILL_THESAVIOR,
    KILL_LIBIEL,
    KILL_ARABELLA,
    KILL_YOHRN,

    CORRUPT_TUBALCAIN,
    CORRUPT_RAHELLEA,
    CORRUPT_PHANUEL,
    CORRUPT_ELIEUS,
    CORRUPT_YANA,
    CORRUPT_APPAIM,
    CORRUPT_THESAVIOR,
    CORRUPT_LIBIEL,
    CORRUPT_ARABELLA,
    CORRUPT_YOHRN,

    CORRUPT_NPC_UPGRADE,
    CORRUPT_NPC_STIGMATA,
    CORRUPT_NPC_HARLOT,

    UNLOCK_INCARNA_1,
    UNLOCK_INCARNA_2,
    UNLOCK_INCARNA_3,
}

public struct SteamAchievementData
{
    public string achievementAPIKey;
    public string statAPIKey;
    public int curCount;
    public int maxCount;

    public SteamAchievementData(string achievementAPIKey, string statAPIKey, int curCount, int maxCount)
    {
        this.achievementAPIKey = achievementAPIKey;
        this.statAPIKey = statAPIKey;
        this.curCount = curCount;
        this.maxCount = maxCount;
    }
}

public class SteamClientManager : MonoBehaviour
{
    Dictionary<SteamAchievementType, SteamAchievementData> achievementDatas;

    public void Init()
    {
        achievementDatas = new Dictionary<SteamAchievementType, SteamAchievementData>()
        {
            { SteamAchievementType.None, new SteamAchievementData() },
            { SteamAchievementType.KILL_TUBALCAIN, new SteamAchievementData("KILL_TUBALCAIN", "STAT_KILL_TUBALCAIN", 0, 1) },
            { SteamAchievementType.KILL_RAHELLEA, new SteamAchievementData("KILL_RAHELLEA", "STAT_KILL_RAHELLEA", 0, 1) },
            { SteamAchievementType.KILL_PHANUEL, new SteamAchievementData("KILL_PHANUEL", "STAT_KILL_PHANUEL", 0, 1) },
            { SteamAchievementType.KILL_ELIEUS, new SteamAchievementData("KILL_ELIEUS", "STAT_KILL_ELIEUS", 0, 1) },
            { SteamAchievementType.KILL_YANA, new SteamAchievementData("KILL_YANA", "STAT_KILL_YANA", 0, 1) },
            { SteamAchievementType.KILL_APPAIM, new SteamAchievementData("KILL_APPAIM", "STAT_KILL_APPAIM", 0, 1) },
            { SteamAchievementType.KILL_THESAVIOR, new SteamAchievementData("KILL_THESAVIOR", "STAT_KILL_THESAVIOR", 0, 1) },
            { SteamAchievementType.KILL_LIBIEL, new SteamAchievementData("KILL_LIBIEL", "STAT_KILL_LIBIEL", 0, 1) },
            { SteamAchievementType.KILL_ARABELLA, new SteamAchievementData("KILL_ARABELLA", "STAT_KILL_ARABELLA", 0, 1) },
            { SteamAchievementType.KILL_YOHRN, new SteamAchievementData("KILL_YOHRN", "STAT_KILL_YOHRN", 0, 1) },

            { SteamAchievementType.CORRUPT_TUBALCAIN, new SteamAchievementData("CORRUPT_TUBALCAIN", "STAT_CORRUPT_TUBALCAIN", 0, 1) },
            { SteamAchievementType.CORRUPT_RAHELLEA, new SteamAchievementData("CORRUPT_RAHELLEA", "STAT_CORRUPT_RAHELLEA", 0, 1) },
            { SteamAchievementType.CORRUPT_PHANUEL, new SteamAchievementData("CORRUPT_PHANUEL", "STAT_CORRUPT_PHANUEL", 0, 1) },
            { SteamAchievementType.CORRUPT_ELIEUS, new SteamAchievementData("CORRUPT_ELIEUS", "STAT_CORRUPT_ELIEUS", 0, 1) },
            { SteamAchievementType.CORRUPT_YANA, new SteamAchievementData("CORRUPT_YANA", "STAT_CORRUPT_YANA", 0, 1) },
            { SteamAchievementType.CORRUPT_APPAIM, new SteamAchievementData("CORRUPT_APPAIM", "STAT_CORRUPT_APPAIM", 0, 1) },
            { SteamAchievementType.CORRUPT_THESAVIOR, new SteamAchievementData("CORRUPT_THESAVIOR", "STAT_CORRUPT_THESAVIOR", 0, 1) },
            { SteamAchievementType.CORRUPT_LIBIEL, new SteamAchievementData("CORRUPT_LIBIEL", "STAT_CORRUPT_LIBIEL", 0, 1) },
            { SteamAchievementType.CORRUPT_ARABELLA, new SteamAchievementData("CORRUPT_ARABELLA", "STAT_CORRUPT_ARABELLA", 0, 1) },
            { SteamAchievementType.CORRUPT_YOHRN, new SteamAchievementData("CORRUPT_YOHRN", "STAT_CORRUPT_YOHRN", 0, 1) },

            { SteamAchievementType.CORRUPT_NPC_UPGRADE, new SteamAchievementData("CORRUPT_NPC_UPGRADE", "STAT_CORRUPT_NPC_UPGRADE", 0, 1) },
            { SteamAchievementType.CORRUPT_NPC_STIGMATA, new SteamAchievementData("CORRUPT_NPC_STIGMATA", "STAT_CORRUPT_NPC_STIGMATA", 0, 1) },
            { SteamAchievementType.CORRUPT_NPC_HARLOT, new SteamAchievementData("CORRUPT_NPC_HARLOT", "STAT_CORRUPT_NPC_HARLOT", 0, 1) },

            { SteamAchievementType.UNLOCK_INCARNA_1, new SteamAchievementData("UNLOCK_INCARNA_1", "STAT_UNLOCK_INCARNA_1", 0, 1) },
            { SteamAchievementType.UNLOCK_INCARNA_2, new SteamAchievementData("UNLOCK_INCARNA_2", "STAT_UNLOCK_INCARNA_2", 0, 1) },
            { SteamAchievementType.UNLOCK_INCARNA_3, new SteamAchievementData("UNLOCK_INCARNA_3", "STAT_UNLOCK_INCARNA_3", 0, 1) },
        };
    }

    public void IncreaseAchievement(SteamAchievementType type)
    {
        if (!SteamManager.Initialized)
        {
            GameManager.Instance.SetSystemInfoText($"Steam is not connected.");
            return;
        }

        achievementDatas.TryGetValue(type, out SteamAchievementData data);

        if (string.IsNullOrEmpty(data.achievementAPIKey))
        {
            GameManager.Instance.SetSystemInfoText("No steam achievement is found.");
            return;
        }

        bool isSuccess = SteamUserStats.GetAchievement(data.achievementAPIKey, out bool achievementCompleted);
        if (isSuccess == false)
        {
            GameManager.Instance.SetSystemInfoText($"Failed to get steam achievement : {data.achievementAPIKey}");
            return;
        }

        if (achievementCompleted)
        {
            GameManager.Instance.SetSystemInfoText($"The steam acheivement is done : {data.achievementAPIKey}");
            return; // �̹� �Ϸ�� ����
        }

        SteamUserStats.GetStat(data.statAPIKey, out data.curCount);
        data.curCount++;
        SteamUserStats.SetStat(data.statAPIKey, data.curCount);
        SteamUserStats.StoreStats();

        GameManager.Instance.SetSystemInfoText($"���� ���� ���� : {data.achievementAPIKey}/{data.curCount}/{data.maxCount}");

        if (data.curCount >= data.maxCount) 
        {
            GameManager.Instance.SetSystemInfoText($"���� ���� �޼�! : {data.achievementAPIKey}/{data.curCount}/{data.maxCount}/{achievementCompleted}");
            SteamUserStats.SetAchievement(data.achievementAPIKey);
            SteamUserStats.StoreStats();
        }
    }

    public bool IsDoneIncarnaUnlock01()
    {
        if (GameManager.OutGameData.IsUnlockedItem(52) && GameManager.OutGameData.IsUnlockedItem(53) &&
            GameManager.OutGameData.IsUnlockedItem(54))
            return true;
        return false;
    }

    public bool IsDoneIncarnaUnlock02()
    {
        if (GameManager.OutGameData.IsUnlockedItem(71) && GameManager.OutGameData.IsUnlockedItem(72) &&
            GameManager.OutGameData.IsUnlockedItem(73) && GameManager.OutGameData.IsUnlockedItem(74))
            return true;
        return false;
    }

    public bool IsDoneIncarnaUnlock03()
    {
        if (GameManager.OutGameData.IsUnlockedItem(61) && GameManager.OutGameData.IsUnlockedItem(62) &&
            GameManager.OutGameData.IsUnlockedItem(63) && GameManager.OutGameData.IsUnlockedItem(64))
            return true;
        return false;
    }

    public int GetCurrentGameLanguage()
    {
        string language = SteamApps.GetCurrentGameLanguage();
        if (language == "koreana" || language == "ko")
            return 1;
        else
            return 0;
    }
}
