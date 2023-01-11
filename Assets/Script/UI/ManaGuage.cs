using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaGuage : MonoBehaviour
{
    [SerializeField] Image ManaGauge;
    [SerializeField] Text ManaText;

    const int MaxManaCost = 10;
    [SerializeField] int ManaCost = 0;

    private void Start()
    {
        ManaCost = 0;
        AddMana(4);
    }

    public void AddMana(int value)
    {
        if (10 <= ManaCost + value)
            ManaCost = 10;
        else
            ManaCost += value;

        ManaGauge.fillAmount = (float)ManaCost / (float)MaxManaCost;
        ManaText.text = ManaCost.ToString();
    }
}
