using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHPEffect : MonoBehaviour
{
    private readonly string HP_DECREASE_ANIMNAME = "DecreasePlayerHP";

    private Animation anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
    }

    public void StartDecreaseHPEffect()
    {
        anim.Play(HP_DECREASE_ANIMNAME);
    }
}
