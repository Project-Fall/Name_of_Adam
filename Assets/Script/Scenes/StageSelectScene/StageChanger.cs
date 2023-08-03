using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageChanger
{
    // 다음 스테이지 이동용
    public void SetNextStage(int _id)
    {
        TestStage stage = GameManager.Data.Map.StageList.Find(x => x.ID == _id);
        GameManager.Data.Map.CurrentTileID = _id;

        if (stage.Type == StageType.Battle)
        {
            SceneChanger.SceneChange("BattleScene");
        }
        else if (stage.Type == StageType.Store)
        {
            if (stage.Stage == StageName.StigmaStore)
            {
                SceneChanger.SceneChange("StigmaScene");

            }
            else if (stage.Stage == StageName.UpgradeStore)
            {
                SceneChanger.SceneChange("UpgradeScene");
            }
        }
    }
}