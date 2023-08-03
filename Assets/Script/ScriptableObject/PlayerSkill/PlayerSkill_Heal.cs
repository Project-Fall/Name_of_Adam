using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayerSkillHeal", menuName = "Scriptable Object/PlayerSkillHeal")]

public class PlayerSkill_Heal : PlayerSkill
{
    private string playerSkillName = "Heal";
    private int manaCost = 20;
    private int darkEssence = 0;
    private string description = "20 ������ �����ϰ� ���ϴ� ������ ü���� 20 ȸ���մϴ�.";

    public override int GetDarkEssenceCost() => darkEssence;
    public override int GetManaCost() => manaCost;
    public override string GetName() => playerSkillName;
    public override string GetDescription() => description;

    public override void Use(Vector2 coord)
    {
        //GameManager.Sound.Play("UI/PlayerSkillSFX/Fall");
        //����Ʈ�� ���⿡ �߰�
        BattleManager.Field.GetUnit(coord).ChangeHP(20);
    }
    public override void CancelSelect()
    {
        BattleManager.PlayerSkillController.UnitTargetPlayerSkillReady(FieldColorType.none);
    }

    public override void OnSelect()
    {
        BattleManager.PlayerSkillController.UnitTargetPlayerSkillReady(FieldColorType.PlayerSkill);
    }
}