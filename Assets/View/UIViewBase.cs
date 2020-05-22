using System;
using UnityEngine;

public enum EViewType
{
    Menu,
    Game,
    Over,
}
public class UIViewBase : MonoBehaviour
{
    [HideInInspector]
    public EViewType viewType;
    public GameObject rootGameobject {
        get {
            return this.gameObject;
        }
    }
    public Transform rootTransform {
        get {
            return this.transform;
        }
    }
    public void Open()
    {
        this.gameObject.SetActive(true);
        OnViewOpen();
    }
    public void Close()
    {
        OnViewClose();
        this.gameObject.SetActive(false);
    }
    public virtual void Initialize()
    {

    }
    protected virtual void OnViewOpen()
    {

    }
    protected virtual void OnViewClose()
    {

    }
}

