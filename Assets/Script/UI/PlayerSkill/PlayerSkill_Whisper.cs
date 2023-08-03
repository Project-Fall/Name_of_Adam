using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill_Whisper : PlayerSkill
{
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