using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "UltimatePlayerSkill_Whisper", menuName = "Scriptable Object/UltimatePlayerSkill_Whisper")]

public class UltimatePlayerSkill_Whisper : PlayerSkill
{
    private string playerSkillName = "U-Whisper";
    private int manaCost = 20;
    private int darkEssence = 1;
    private string description = "20 마나와 1 검은 정수를 지불하고 원하는 적 유닛에게 타락도 1을 부여합니다.";

    public override int GetDarkEssenceCost() => darkEssence;
    public override int GetManaCost() => manaCost;
    public override string GetName() => playerSkillName;
    public override string GetDescription() => description;

    public override void Use(Vector2 coord)
    {
        foreach (BattleUnit unit in BattleManager.Data.BattleUnitList)
        {
            if (unit.Team == Team.Enemy)
            {
                GameManager.Sound.Play("UI/PlayerSkillSFX/Fall");
                //이팩트를 여기에 추가
                unit.ChangeFall(1);
            }
        }
    }

    public override void CancelSelect()
    {
        BattleManager.PlayerSkillController.EnemyTargetPlayerSkillReady(FieldColorType.none);
    }

    public override void OnSelect()
    {
        BattleManager.PlayerSkillController.EnemyTargetPlayerSkillReady(FieldColorType.UltimatePlayerSkill);
    }
}