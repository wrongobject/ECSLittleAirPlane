using System;
using UnityEngine;

public class UIGameView : UIViewBase
{
    public override void Initialize()
    {
        base.Initialize();
        viewType = EViewType.Game;
    }
    private void Awake()
    {        
    }
}

