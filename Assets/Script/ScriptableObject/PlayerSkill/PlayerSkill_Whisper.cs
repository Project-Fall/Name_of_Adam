using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayerSkillWhisper", menuName = "Scriptable Object/PlayerSkillWhisper")]

public class PlayerSkill_Whisper : PlayerSkill
{
    private string playerSkillName = "Whisper";
    private int manaCost = 20;
    private int darkEssence = 1;
    private string description = "20 ������ 1 ���� ������ �����ϰ� ���ϴ� �� ���ֿ��� Ÿ���� 1�� �ο��մϴ�.";

    public override int GetDarkEssenceCost() => darkEssence;
    public override int GetManaCost() => manaCost;
    public override string GetName() => playerSkillName;
    public override string GetDescription() => description;

    public override void Use(Vector2 coord)
    {
        GameManager.Sound.Play("UI/PlayerSkillSFX/Fall");
        //����Ʈ�� ���⿡ �߰�
        BattleManager.Field.GetUnit(coord).ChangeFall(1);
    }

    public override void CancelSelect()
    {
        BattleManager.PlayerSkillController.EnemyTargetPlayerSkillReady(FieldColorType.none);
    }

    public override void OnSelect()
    {
        BattleManager.PlayerSkillController.EnemyTargetPlayerSkillReady(FieldColorType.PlayerSkill);
    }
}