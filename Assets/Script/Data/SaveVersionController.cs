using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 게임 버전에 따라 세이브 데이터의 무결성을 체크하는 클래스

public class SaveVersionController
{
    // ※ Unity Version에 따라 History를 반드시 기억해야 합니다!
    // 따라서 게임 런칭을 할 때 반드시 버전을 체크하고, 업데이트가 필요한 경우에만 마이그레이션을 진행해야 합니다.

    private List<string> versionHistory = new List<string>
    {
        "1.0.0-demo",
        "1.0.1-demo",
        "1.0.3-demo",
        "1.1.0-demo",
        "1.0.0-release",
        "1.0.1-release",
        "1.0.2-release",
        "1.0.0v",
        "1.0.1v",
        "1.0.2v",
        "1.0.3v",
        "1.0.4v",
        "1.1.0v"
    };

    public bool IsValildVersion()
        => GameManager.OutGameData.Data.Version.Equals(Application.version);

    public bool CheckNeedMigration()
    {
        if (IsValildVersion() == true)
        {
            Debug.Log("The OutGameData is up to data.");
            return false;
        }

        Debug.Log($"Data Version is not matched! Save Version : {GameManager.OutGameData.Data.Version} / Build Version {Application.version}");
        return true;
    }

    public void MigrateData()
    {
        string userVersion = GameManager.OutGameData.Data.Version;
        int userVersionIndex = versionHistory.IndexOf(userVersion);

        if (userVersionIndex == -1)
        {
            Debug.LogError("Invalid Version! Check [ProjectSetting] and [SaveVersionController]!");
            return;
        }

        if ("1.1.0-demo" != userVersion)
        {
            GameManager.SaveManager.DeleteSaveData();
            GameManager.OutGameData.DeleteAllData();
            GameManager.OutGameData.Init();
        }

        // 버전은 순차적(Linearly)으로 업데이트 되어야 합니다.
        while (userVersion.Equals(Application.version) == false)
        {
            switch (userVersion)
            {
                case "1.0.0-demo":
                    GameManager.SaveManager.DeleteSaveData();
                    GameManager.OutGameData.DeleteAllData();
                    GameManager.OutGameData.CreateData();
                    break;
                case "1.0.1-demo":
                    GameManager.SaveManager.DeleteSaveData();
                    GameManager.OutGameData.DeleteAllData();
                    GameManager.OutGameData.CreateData();
                    break;
                case "1.0.3-demo":
                    GameManager.SaveManager.DeleteSaveData();
                    GameManager.OutGameData.DeleteAllData();
                    GameManager.OutGameData.CreateData();
                    break;

                // 이후 업데이트에서 구현 필요 시 추가
                case "1.0.0-release":
                    GameManager.SaveManager.DeleteSaveData();

                    // 전당 유닛 고유키(PrivateKey) 추가
                    foreach (HallUnit unit in GameManager.OutGameData.FindHallUnitList())
                    {
                        if (string.IsNullOrEmpty(unit.PrivateKey))
                            unit.PrivateKey = GameManager.CreatePrivateKey();
                    }
                    break;

                case "1.0.1-release":

                    // 호루스 이름 변경
                    foreach (HallUnit unit in GameManager.OutGameData.FindHallUnitList())
                        if (unit.UnitName == "호루스")
                            unit.UnitName = "구원자";

                    SaveData saveData = GameManager.SaveManager.GetSaveData();
                    foreach (SaveUnit unit in saveData.DeckUnitData)
                        if (unit.UnitDataID == "호루스")
                            unit.UnitDataID = "구원자";

                    foreach (SaveUnit unit in saveData.FallenUnitsData)
                        if (unit.UnitDataID == "호루스")
                            unit.UnitDataID = "구원자";

                    GameManager.SaveManager.SaveData(saveData);

                    break;

                case "1.0.2-release":
                    GameManager.SaveManager.DeleteSaveData();
                    GameManager.OutGameData.DeleteAllData();
                    GameManager.OutGameData.CreateData();
                    break;

                case "1.0.0v":
                case "1.0.1v":
                    GameManager.SaveManager.DeleteSaveData();

                    List<HallUnit> hallUnits = GameManager.OutGameData.FindHallUnitList();
                    for (int i = 0; i < hallUnits.Count; i++)
                    {
                        HallUnit unit = hallUnits[i];
                        unit.IsMainDeck = false;
                        unit.ID = 4 + i;
                    }

                    HallUnit darkKnight = hallUnits.Find(x => x.PrivateKey == "흑기사_Origin");
                    darkKnight.IsMainDeck = true;
                    darkKnight.ID = 0;

                    HallUnit prisoner = hallUnits.Find(x => x.PrivateKey == "죄수_Origin");
                    prisoner.IsMainDeck = true;
                    prisoner.ID = 1;

                    HallUnit gravekeeper = hallUnits.Find(x => x.PrivateKey == "묘지기_Origin");
                    gravekeeper.IsMainDeck = true;
                    gravekeeper.ID = 2;

                    break;

                case "1.0.2v":
                    // 새로운 진척도 기능 추가
                    List<ProgressSave> progressSaves = GameManager.OutGameData.Data.ProgressSaves;
                    foreach (var progressItem in GameManager.OutGameData.ProgressItems)
                    {
                        if (progressSaves.Find(x => x.ID == progressItem.Value.ID) == null)
                            progressSaves.Add(new ProgressSave(progressItem.Value.ID, false));
                    }

                    // 진척도 상점 기능 초기화 및 환원
                    int sum = 0;
                    foreach (var progressSave in progressSaves)
                    {
                        if (progressSave.isUnLock)
                        {
                            progressSave.isUnLock = false;
                            var progressItem = GameManager.OutGameData.GetProgressItem(progressSave.ID);
                            sum += progressItem.Cost;
                        }
                    }
                    Debug.Log($"진척도 초기화 및 코인 '{sum}' 획득");
                    GameManager.OutGameData.Data.ProgressCoin += sum;
                    GameManager.OutGameData.SetProgressInit();

                    break;
                case "1.0.4v":
                    // 기존 데이터 명칭 변경
                    GameManager.SaveManager.DeleteSaveData();
                    GameManager.OutGameData.ResetOption();
                    if (GameManager.OutGameData.Data.HorusClear)
                        GameManager.OutGameData.Data.SaviorClear = true;

                    NPCQuest npcQuest = GameManager.OutGameData.Data.NpcQuest;
                    GameManager.OutGameData.Data.BaptismCorruptValue = npcQuest.UpgradeQuest;
                    GameManager.OutGameData.Data.StigmataCorruptValue = npcQuest.StigmaQuest;
                    GameManager.OutGameData.Data.SacrificeCorruptValue = npcQuest.DarkshopQuest;

                    GameManager.OutGameData.Data.IsVisitBaptism = npcQuest.UpgradeQuest > 0;
                    GameManager.OutGameData.Data.IsVisitStigmata = npcQuest.StigmaQuest > 0;
                    GameManager.OutGameData.Data.IsVisitSacrifice = npcQuest.DarkshopQuest > 0;

                    foreach (var hallUnit in GameManager.OutGameData.FindHallUnitList())
                    {
                        List<UpgradeData> updateUpgrades = new();

                        foreach (UpgradeData upgrade in hallUnit.Upgrades)
                        {
                            GameManager.Data.Init();
                            switch (upgrade.ID)
                            {
                                case "UP101" :
                                    if (upgrade.ATK == "4")
                                    {
                                        upgrade.ID = "UP105";
                                    }
                                break;
                                case "UP102" :
                                    if (upgrade.HP == "10" || upgrade.HP == "11")
                                    {
                                        upgrade.ID = "UP106";
                                    }
                                    break;
                                case "UP103" :
                                    if (upgrade.SPD == "30")
                                    {
                                        upgrade.SPD = "40";
                                    }
                                    else if (upgrade.SPD == "50")
                                    {
                                        upgrade.ID = "UP107";
                                    }
                                    break;
                                case "UP211":
                                    upgrade.ATK = "3";
                                    break;

                                case "UP301":
                                    upgrade.ATK = "6";
                                    upgrade.HP = "6";
                                    break;
                                case "UP302":
                                    upgrade.ATK = "7";
                                    break;
                                case "UP303":
                                    upgrade.ATK = "8";
                                    upgrade.HP = "-6";
                                    break;
                            }

                            updateUpgrades.Add(GameManager.Data.UpgradeController.UpgradeUpdateCheck(upgrade));
                        }

                        hallUnit.Upgrades = updateUpgrades;
                    }

                    break;
            }

            userVersionIndex++;

            if (userVersionIndex >= versionHistory.Count)
            {
                Debug.LogError($"버전 히스토리를 등록하세요! 새로운 버전을 SaveVersionController.cs 상단에 기록하세요.");
                Debug.LogError($"현재 버전: {versionHistory[versionHistory.Count - 1]}");
                return;
            }

            GameManager.OutGameData.Data.Version = versionHistory[userVersionIndex];
            userVersion = GameManager.OutGameData.Data.Version;
        }

        GameManager.OutGameData.SaveData();
    }
}
