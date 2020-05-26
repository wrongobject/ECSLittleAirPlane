using System;
using UnityEngine;

public class GameSetting : MonoBehaviour
{
    public Material enemyMat;
    public Material playerMat;
    public Mesh entityMesh;

    public int enemySpawnRange = 10;
    [Range(0.01f, 1)]
    public float enemySpeedRange = 0.1f;
    public float maxAccDirStep = 3;
    public float playerAccParam = 1;
    public float negtivePlayerAcc = -2;
    [Range(1, 5)]
    public float playerMaxSpeed = 3;
    [Range(1, 20)]
    public int enemyCreateRate = 1;
    [Range(0, 1)]
    public float enemyRadius = 0.2f;
    [Range(0, 1)]
    public float playerRadius = 0.5f;

    static GameSetting _instance;

    public static GameSetting Instance
    {
        get
        {
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
}

