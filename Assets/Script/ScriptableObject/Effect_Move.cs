using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Dir
{
    Left,
    Right,
    Up,
    Down
}

[CreateAssetMenu(fileName = "Effect_Move", menuName = "Scriptable Object/Effect_Move", order = 2)]
public class Effect_Move : EffectSO
{
    [SerializeField] List<Dir> MoveDir;
    [SerializeField] Dictionary<string, Vector2> dic = new Dictionary<string, Vector2>();

    // 이동 실행
    public override void Effect(Character caster)
    {
        for (int i = 0; i < MoveDir.Count; i++)
        {
            switch (MoveDir[i])
            {
                case Dir.Left:
                    caster.MoveLotate(-1, 0);
                    break;
                case Dir.Right:
                    caster.MoveLotate(1, 0);
                    break;
                case Dir.Up:
                    caster.MoveLotate(0, 1);
                    break;
                case Dir.Down:
                    caster.MoveLotate(0, -1);
                    break;
            }
        }
    }
}
