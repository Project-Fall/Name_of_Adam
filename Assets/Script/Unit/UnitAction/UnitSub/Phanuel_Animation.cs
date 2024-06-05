using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phanuel_Animation : MonoBehaviour
{
    [SerializeField] public RuntimeAnimatorController AnimatorController;
    [SerializeField] public RuntimeAnimatorController CorruptionAnimatorController;

    [SerializeField] private Animator _animator;

    public void SetBool(string varName, bool boolean)
    {
        _animator.SetBool(varName, boolean);
    }

    public void SetAnimator(Team team)
    {
        transform.position = new(0f, 1.8f, 0f);
        transform.localScale = new(2f, 2f, 1f); // ������ ũ��

        if (team == Team.Player)
        {
            _animator.runtimeAnimatorController = CorruptionAnimatorController;
        }
        else
        {
            _animator.runtimeAnimatorController = AnimatorController;
        }
    }

    public void ChangeAnimator(Team team)
    {
        SetAnimator(team);
        _animator.Play("Unit_Idle");
    }
}
