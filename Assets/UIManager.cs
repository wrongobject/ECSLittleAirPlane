using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
   
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("you shall not use uimanger when that are not inited");
            }
            return _instance;
        }
    }

    private Dictionary<int, UIViewBase> viewDict = new Dictionary<int, UIViewBase>();
    private Queue<UIViewBase> openedView = new Queue<UIViewBase>();
    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        UIViewBase[] views = GetComponentsInChildren<UIViewBase>(true);
        foreach (var item in views)
        {
            item.Initialize();
            viewDict.Add((int)item.viewType,item);
            item.Close();
        }
    }

    public void OpenView(EViewType viewType)
    {
        while (openedView.Count > 0)
        {
            UIViewBase view = openedView.Dequeue();
            view.Close();
        }
        UIViewBase target = viewDict[(int)viewType];
        target.Open();
        openedView.Enqueue(target);
    }
}

