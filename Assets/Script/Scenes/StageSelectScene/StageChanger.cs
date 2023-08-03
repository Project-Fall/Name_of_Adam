using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StageDataContainer
{
    public List<StageSpawnData> StageData;

    public StageSpawnData GetStageData(string faction, int level, int id) => StageData.Find(x => x.FactionName == faction && x.ID == id && x.Level == level);
}
[Serializable]
public class StageSpawnData
{
    public string FactionName;
    public int Level;
    public int ID;
    public List<StageUnitData> Units;
}
[Serializable]
public class StageUnitData
{
    public string Name;
    public Vector2 Location;
}



public class StageChanger
{
    // 다음 스테이지 이동용
    public void SetNextStage(Stage stage)
    {
        GameManager.Data.CurStageData = stage;

        if (stage.GetStageType() == StageType.Battle)
        {
            SetBattleScene(stage);
        }
        else if (stage.GetStageType() == StageType.Store)
        {
            if (stage.Name == StageName.StigmaStore)
            {
                SceneChanger.SceneChange("StigmaScene");

            }
            else if (stage.Name == StageName.UpgradeStore)
            {
                SceneChanger.SceneChange("UpgradeScene");
            }
        }
    }

    private void SetBattleScene(Stage stage)
    {
        SetSpawnUnit(stage.BattleStageData.faction, stage.BattleStageData.level, stage.BattleStageData.id);

        SceneChanger.SceneChange("BattleScene");
    }

    private void SetSpawnUnit(Faction faction, int level, int id) // 다음에 받을 팩션, 레벨, 아이디 넣기
    {
        GameManager.Data.CurrentStageData = GameManager.Data.StageDatas.GetStageData(faction.ToString(), level, id);
    }
}