using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayerSkillBounce", menuName = "Scriptable Object/PlayerSkillBounce")]
public class PlayerSkill_Bounce : PlayerSkill
{
    private string playerSkillName = "Bounce";
    private int manaCost = 20;
    private int darkEssence = 0;
    private string description = "20 마나를 지불하고 원하는 아군 유닛을 손으로 가져옵니다.";

    public override int GetDarkEssenceCost() => darkEssence;
    public override int GetManaCost() => manaCost;
    public override string GetName() => playerSkillName;
    public override string GetDescription() => description;

    public override void Use(Vector2 coord)
    {
        //GameManager.Sound.Play("UI/PlayerSkillSFX/Fall");
        //이팩트를 여기에 추가

        BattleUnit unit = BattleManager.Field.GetUnit(coord);

        BattleManager.Data.BattleUnitRemove(unit);
        BattleManager.Data.BattleOrderRemove(unit);
        BattleManager.Data.AddDeckUnit(unit.DeckUnit);
        BattleManager.BattleUI.FillHand();
        BattleManager.Field.FieldCloseInfo(BattleManager.Field.TileDict[coord]);
        Destroy(unit.gameObject);
    }

    public override void CancelSelect()
    {
        BattleManager.PlayerSkillController.FriendlyTargetPlayerSkillReady(FieldColorType.none);
    }

    public override void OnSelect()
    {
        BattleManager.PlayerSkillController.FriendlyTargetPlayerSkillReady(FieldColorType.PlayerSkill);
    }
}