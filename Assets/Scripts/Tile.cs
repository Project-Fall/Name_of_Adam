using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] int x;
    [SerializeField] int y;
    [SerializeField] Unit _unit = null;
    [SerializeField] bool _deployable = true;

    public void ChangeColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    public void OnMouseOver()
    {
        if(_deployable)
            ChangeColor(Color.blue);
        InputManager.HoverTarget = this;
    }

    public void OnMouseExit()
    {
        ChangeColor(Color.white);
        InputManager.HoverTarget = null;
    }

    public void DeployUnit(Unit unit)
    {
        if (!_deployable)
            return;

        unit.transform.parent = transform;
        float z = unit.transform.localPosition.z;
        unit.transform.localPosition = new Vector3(0, 0, z);
        _deployable = false;
    }
}
