using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StageType
{
    Store,
    Event,
    Battle
}

public class Stage : MonoBehaviour
{
    [SerializeField] public int ID;
    [Space (10f)]
    [SerializeField] Animation Anim;
    [Space(10f)]
    [SerializeField] public List<Stage> NextStage;
    [Space(10f)]
    [Header("StageInfo")]
    [SerializeField] private StageType _type;
    public StageType Type => _type;
    [SerializeField] private StageName _name;
    public StageName Name => _name;

    [SerializeField] private int _battleStageLevel;
    public int BattleStageLevel => _battleStageLevel;
    [SerializeField] private int _battleStageID;
    public int BattleStageID => _battleStageID;

    private Coroutine coro;
    private float ZoomSpeed = 0.05f;

    private void Awake()
    {
        StageManager.Instance.InputStageList(this);

        coro = null;
        Anim.Stop();
    }

    public void OnMouseUp() => StageManager.Instance.StageMove(ID);

    public void OnMouseEnter()
    {
        if (coro != null)
            StopCoroutine(coro);
        coro = StartCoroutine(SizeUp());
    }

    public void OnMouseExit()
    {
        if (coro != null)
            StopCoroutine(coro);
        coro = StartCoroutine(SizeDown());
    }

    public void SetBattleStage(int a, int b)
    {
        _battleStageLevel = a;
        _battleStageID = b;
    }

    public void StartBlink()
    {
        Anim.Play();
    }

    IEnumerator SizeUp()
    {
        while (transform.localScale.x < 1.5f)
        {
            transform.localScale += new Vector3(ZoomSpeed, ZoomSpeed);
            yield return null;
        }

        transform.localScale = new Vector3(1.5f, 1.5f, 1);
    }

    IEnumerator SizeDown()
    {
        while (transform.localScale.x > 1)
        {
            transform.localScale -= new Vector3(ZoomSpeed, ZoomSpeed);
            yield return null;
        }

        transform.localScale = new Vector3(1, 1, 1);
    }
}