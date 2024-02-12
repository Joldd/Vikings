using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Messenger : Selectable
{
    public Viking vikingSelected;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        Debug.Log(vikingSelected);
    }
}
