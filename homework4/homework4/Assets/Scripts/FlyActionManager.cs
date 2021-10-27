using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SSActionEventType : int { Started, Competeted }

public interface ISSActionCallback
{
    void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0, string strParam = null, Object objectParam = null);
}

public class FlyActionManager : MonoBehaviour, ISSActionCallback, IActionManager
{
    public int DiskNumber = 0;
    public FirstController sceneController;

    private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();        //保存所以已经注册的动作
    private List<SSAction> waitingAdd = new List<SSAction>();                           //动作的等待队列，在这个对象保存的动作会稍后注册到动作管理器里
    private List<int> waitingDelete = new List<int>();
    private bool run;

    protected void Start()
    {
        sceneController = (FirstController)SSDirector.GetInstance().CurrentScenceController;
        sceneController.actionManager = this;
        run = true;
    }

    protected void Update()
    {
        if (run)
        {
            foreach (SSAction ac in waitingAdd)
            {
                actions[ac.GetInstanceID()] = ac;
            }
            waitingAdd.Clear();

            foreach (KeyValuePair<int, SSAction> kv in actions)
            {
                SSAction action = kv.Value;
                if (action.destroy)
                {
                    waitingDelete.Add(action.GetInstanceID());
                }
                else if (action.enable)
                {
                    action.Update();
                }
            }

            foreach (int key in waitingDelete)
            {
                SSAction action = actions[key];
                actions.Remove(key);
                DestroyObject(action);
            }
            waitingDelete.Clear();
        }
    }

    public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager)
    {
        action.gameobject = gameobject;
        action.transform = gameobject.transform;
        action.callback = manager;
        waitingAdd.Add(action);
        action.Start();
    }

    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted, int intParam = 0, string strParam = null, Object objectParam = null)
    {
        if (source is UFOFlyAction)
        {
            DiskNumber--;
            source.gameobject.SetActive(false);
        }
    }

    public void StartThrow(GameObject disk)
    {
        FlyActionFactory cf = Singleton<FlyActionFactory>.Instance;
        RunAction(disk, cf.GetSSAction(), (ISSActionCallback)this);
    }
}