using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayerSkillDamage", menuName = "Scriptable Object/PlayerSkillDamage")]

public class PlayerSkill_Damage : PlayerSkill
{
    private string playerSkillName = "Damage";
    private int manaCost = 20;
    private int darkEssence = 0;
    private string description = "20 마나를 지불하고 원하는 적 유닛에게 20 대미지를 줍니다.";


    public override int GetDarkEssenceCost() => darkEssence;
    public override int GetManaCost() => manaCost;
    public override string GetName() => playerSkillName;
    public override string GetDescription() => description;

    public override void Use(Vector2 coord)
    {
        //GameManager.Sound.Play("UI/PlayerSkillSFX/Fall");
        //이팩트를 여기에 추가
        BattleManager.Field.GetUnit(coord).GetAttack(-20, null);
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