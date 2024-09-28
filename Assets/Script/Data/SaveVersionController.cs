using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ������ ���� ���̺� �������� ���Ἲ�� üũ�ϴ� Ŭ����

public class SaveVersionController
{
    // �� Unity Version�� ���� History�� �ݵ�� ����ؾ� �մϴ�!
    // ���� ���� ��Ī�� �� �� �ݵ�� ������ üũ�ϰ�, ������Ʈ�� �ʿ��� ��쿡�� ���̱׷��̼��� �����ؾ� �մϴ�.

    private List<string> versionHistory = new List<string>
    {
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

        // ������ ������(Linearly)���� ������Ʈ �Ǿ�� �մϴ�.
        while (userVersion.Equals(Application.version) == false)
        {
            switch (userVersion)
            {
                // ���� ������Ʈ���� ���� �ʿ� �� �߰�
                case "1.0.0-release":
                    GameManager.SaveManager.DeleteSaveData();

                    // ���� ���� ����Ű(PrivateKey) �߰�
                    foreach (HallUnit unit in GameManager.OutGameData.FindHallUnitList())
                    {
                        if (string.IsNullOrEmpty(unit.PrivateKey))
                            unit.PrivateKey = GameManager.CreatePrivateKey();
                    }
                    break;

                case "1.0.1-release":

                    // ȣ�罺 �̸� ����
                    foreach (HallUnit unit in GameManager.OutGameData.FindHallUnitList())
                        if (unit.UnitName == "ȣ�罺")
                            unit.UnitName = "������";

                    SaveData saveData = GameManager.SaveManager.GetSaveData();
                    foreach (SaveUnit unit in saveData.DeckUnitData)
                        if (unit.UnitDataID == "ȣ�罺")
                            unit.UnitDataID = "������";

                    foreach (SaveUnit unit in saveData.FallenUnitsData)
                        if (unit.UnitDataID == "ȣ�罺")
                            unit.UnitDataID = "������";

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

                    HallUnit darkKnight = hallUnits.Find(x => x.PrivateKey == "����_Origin");
                    darkKnight.IsMainDeck = true;
                    darkKnight.ID = 0;

                    HallUnit prisoner = hallUnits.Find(x => x.PrivateKey == "�˼�_Origin");
                    prisoner.IsMainDeck = true;
                    prisoner.ID = 1;

                    HallUnit gravekeeper = hallUnits.Find(x => x.PrivateKey == "������_Origin");
                    gravekeeper.IsMainDeck = true;
                    gravekeeper.ID = 2;

                    break;

                case "1.0.2v":
                    // ���ο� ��ô�� ��� �߰�
                    List<ProgressSave> progressSaves = GameManager.OutGameData.Data.ProgressSaves;
                    foreach (var progressItem in GameManager.OutGameData.ProgressItems)
                    {
                        if (progressSaves.Find(x => x.ID == progressItem.Value.ID) == null)
                            progressSaves.Add(new ProgressSave(progressItem.Value.ID, false));
                    }

                    // ��ô�� ���� ��� �ʱ�ȭ �� ȯ��
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
                    Debug.Log($"��ô�� �ʱ�ȭ �� ���� '{sum}' ȹ��");
                    GameManager.OutGameData.Data.ProgressCoin += sum;
                    GameManager.OutGameData.SetProgressInit();

                    break;
                case "1.0.4v":
                    // ���� ������ ��Ī ����
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

                    GameManager.OutGameData.InitCutSceneData();

                    break;
            }

            userVersionIndex++;

            if (userVersionIndex >= versionHistory.Count)
            {
                Debug.LogError($"���� �����丮�� ����ϼ���! ���ο� ������ SaveVersionController.cs ��ܿ� ����ϼ���.");
                Debug.LogError($"���� ����: {versionHistory[versionHistory.Count - 1]}");
                return;
            }

            GameManager.OutGameData.Data.Version = versionHistory[userVersionIndex];
            userVersion = GameManager.OutGameData.Data.Version;
        }

        GameManager.OutGameData.SaveData();
    }
}
