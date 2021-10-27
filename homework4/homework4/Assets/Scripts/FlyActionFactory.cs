using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyActionFactory : MonoBehaviour
{
    private Dictionary<int, SSAction> used = new Dictionary<int, SSAction>();
    private List<SSAction> free = new List<SSAction>();
    private List<int> wait = new List<int>();

    public UFOFlyAction Fly;

    void Start()
    {
        Fly = UFOFlyAction.GetCCFlyAction();
    }

    //每此刷新对判定为destroy的动作进行释放
    private void Update()
    {
        foreach (var action in used.Values)
        {
            if (action.destroy)
            {
                wait.Add(action.GetInstanceID());
            }
        }

        foreach (int ID in wait)
        {
            FreeSSAction(used[ID]);
        }
        wait.Clear();
    }
    //获得一个新的动作，先从free队列中找，空则生成一个
    public SSAction GetSSAction()
    {
        SSAction action = null;
        if (free.Count > 0)
        {
            action = free[0];
            free.Remove(free[0]);
            //Debug.Log(free.Count);
        }
        else
        {
            action = ScriptableObject.Instantiate<UFOFlyAction>(Fly);

        }

        used.Add(action.GetInstanceID(), action);
        return action;
    }
    //释放某个动作，加入到free队列
    public void FreeSSAction(SSAction action)
    {
        SSAction tmp = null;
        int key = action.GetInstanceID();
        //检查是否含有某动作
        if (used.ContainsKey(key))
        {
            tmp = used[key];
        }
        //有则释放
        if (tmp != null)
        {
            tmp.Reset();
            free.Add(tmp);
            used.Remove(key);
        }
    }
    //清除所有在使用的动作
    public void clear()
    {
        foreach (var action in used.Values)
        {
            action.enable = false;
            action.destroy = true;

        }
    }
}
