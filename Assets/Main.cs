using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
public class Main : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(StepStart());
    }

    IEnumerator StepStart()
    {
        yield return 0;
        GameManager.CreateInstance();
        yield return 0;
        GameManager.Instance.GotoState(EGameStatus.Menu);
    }
  

    private void Update()
    {
       
    }

    private void FixedUpdate()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.FixedUpdate(Time.deltaTime);
    }

    private void OnDestroy()
    {
        GameManager.DestroyInstance();
    }

}
