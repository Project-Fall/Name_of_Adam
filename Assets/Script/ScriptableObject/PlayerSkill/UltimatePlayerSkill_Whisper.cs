using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "UltimatePlayerSkill_Whisper", menuName = "Scriptable Object/UltimatePlayerSkill_Whisper")]

public class UltimatePlayerSkill_Whisper : PlayerSkill
{
    private string playerSkillName = "U-Whisper";
    private int manaCost = 20;
    private int darkEssence = 1;
    private string description = "20 ������ 1 ���� ������ �����ϰ� ���ϴ� �� ���ֿ��� Ÿ���� 1�� �ο��մϴ�.";

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
                //����Ʈ�� ���⿡ �߰�
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