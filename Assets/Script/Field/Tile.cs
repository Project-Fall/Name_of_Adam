using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    SpriteRenderer SR;

    Character chara;
    #region isOnTile
    bool _isOnTile;
    public bool isOnTile => _isOnTile;
    #endregion

    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        SR.color = Color.gray;

        _isOnTile = false;
        chara = null;
    }

    public void EnterTile(Character ch)
    {
        _isOnTile = true;
        chara = ch;
    }

    public void ExitTile()
    {
        _isOnTile = false;
        chara = null;
    }

    #region OnAttack
    public void OnAttack(Character ch)
    {
        StartCoroutine(CoOnAttack(ch));
    }
    IEnumerator CoOnAttack(Character AttackChar)
    {
        SR.color = Color.red;

        if (chara != null)
        {
            if (AttackChar.characterSO.team != chara.characterSO.team)
            {
                chara.GetDamage(AttackChar.characterSO.stat.ATK);
            }
        }

        yield return new WaitForSeconds(0.5f);

        SR.color = Color.gray;

    }
    #endregion

    #region OnHeal
    public void OnHeal(Character ch)
    {
        StartCoroutine(CoOnHeal(ch));
    }
    IEnumerator CoOnHeal(Character AttackChar)
    {
        SR.color = Color.white;

        if (chara != null)
        {
            if (AttackChar.characterSO.team == chara.characterSO.team)
            {
                chara.GetDamage(-AttackChar.characterSO.stat.ATK);
            }
        }

        yield return new WaitForSeconds(0.5f);

        SR.color = Color.gray;

    }
    #endregion
}
