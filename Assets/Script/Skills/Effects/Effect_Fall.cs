using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect_Fall", menuName = "Scriptable Object/Effect_Fall")]
public class Effect_Fall : Effect
{
    private int _fallValue = 1;

    public override void Use(BattleUnit caster, BattleUnit receiver)
    {
        receiver.ChangeFall(_fallValue);
    }
}