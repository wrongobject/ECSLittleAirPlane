using System;
using UnityEngine;

public class GameSetting : MonoBehaviour
{
    public Material enemyMat;
    public Material playerMat;
    public Mesh entityMesh;
    public int enemySpawnRange = 10;
    public float enemySpeedRange = 1;
    public float maxAccDirStep = 3;
    public float playerAccParam = .01f;
    public float negtivePlayerAcc = -2;
    public float playerMaxSpeed = 10;
    public int enemyCreateRate = 1;
    public float enemyRadius = 0.2f;
    public float playerRadius = 0.5f;

    static GameSetting _instance;

    public static GameSetting Instance {
        get {
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
}

