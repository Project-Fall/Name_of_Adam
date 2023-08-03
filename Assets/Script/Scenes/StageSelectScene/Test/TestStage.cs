using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestStage : MonoBehaviour
{
    [SerializeField] public int ID;
    [Space (10f)]
    [SerializeField] Animation Anim;
    [Space(10f)]
    [SerializeField] public List<TestStage> NextStage;
    [SerializeField] private StageType _type;
    public StageType Type => _type;
    [SerializeField] private StageName _stage;
    public StageName Stage => _stage;

    private Coroutine coro;
    private float ZoomSpeed = 0.05f;

    private void Awake()
    {
        coro = null;
        Anim.Stop();
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


    public void OnMouseUp() => TestStageManager.Instance.StageMove(this);

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
}