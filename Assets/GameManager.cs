using System;
using UnityEngine;
public enum EGameStatus
{
    None,
    Menu,
    Game,
    Over
}

public class GameManager :IDisposable
{
    #region static field & function
    public static void CreateInstance()
    {
        Instance = new GameManager();
    }
    public static void DestroyInstance()
    {
        if (Instance != null) Instance.Dispose();
        Instance = null;
    }
    public static GameManager Instance { get; set; }
    #endregion

    EGameStatus _gameState;
    EntityCtrl _entityCtrl;
    float _lastEnemyCreateTime = -1;   
   
    public GameManager()
    {
        _gameState = EGameStatus.None;
        _entityCtrl = new EntityCtrl();
    }
    
    public void GotoState(EGameStatus state)
    {
        if (_gameState == state) return;
      
        DoChangeState(_gameState,state);
    }

    private void DoChangeState(EGameStatus current, EGameStatus next)
    {
        DoStateOut(current);
        DoStateIn(next);
        _gameState = next;
    }

    private void DoStateOut(EGameStatus outState)
    {
        switch (outState)
        {
            case EGameStatus.Game:
                _entityCtrl.Dispose();
                break;
        }
    }

    private void DoStateIn(EGameStatus inState)
    {
        switch (inState)
        {
            case EGameStatus.Menu:
                UIManager.Instance.OpenView(EViewType.Menu);
                break;
            case EGameStatus.Game:
                UIManager.Instance.OpenView(EViewType.Game);
                _entityCtrl.Start();
                _entityCtrl.CreatePlayer();
                _entityCtrl.CreateEnemy(2000);
                break;
            case EGameStatus.Over:
                UIManager.Instance.OpenView(EViewType.Over);
                break;
        }
    }

    public void FixedUpdate(float deltaTime)
    {
        if (_gameState != EGameStatus.Game) return;

        //LoopSpawnEnemy();
    }

    private void LoopSpawnEnemy()
    {
        float interval = 1f / GameSetting.Instance.enemyCreateRate;
        if (Time.realtimeSinceStartup - _lastEnemyCreateTime > interval)
        {
            _entityCtrl.CreateEnemy(1);
            _lastEnemyCreateTime = Time.realtimeSinceStartup;
        }
        
    }

    public void Dispose()
    {
        
    }
}

