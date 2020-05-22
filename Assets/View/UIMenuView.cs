using System;
using UnityEngine;
using UnityEngine.UI;
public class UIMenuView :UIViewBase
{
    public Button startBtn;
    public Button quiteBtn;
    public Button helpBtn;
    public override void Initialize()
    {
        base.Initialize();
        viewType = EViewType.Menu;
    }
    private void Awake()
    {
       
    }

    private void Start()
    {
        if(startBtn) startBtn.onClick.AddListener(OnClickStart);
        if (quiteBtn) quiteBtn.onClick.AddListener(OnClickQuite);
        if (helpBtn) helpBtn.onClick.AddListener(OnClickHelp);
    }

    void OnClickStart()
    {
        GameManager.Instance.GotoState(EGameStatus.Game);
    }
    void OnClickQuite()
    {

    }
    void OnClickHelp()
    {

    }
}

