using System;
using UnityEngine;

public class UIOverView : UIViewBase
{
    public override void Initialize()
    {
        base.Initialize();
        viewType = EViewType.Over;
    }
    private void Awake()
    {
       
    }
}

