using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionManager
{
    void StartThrow(GameObject disk);
}

public class PhysicActionManager : MonoBehaviour, IActionManager, ISSActionCallback
{
    public FirstController sceneController;
    public int DiskNumber = 0;


    private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
    //保存所以已经注册的动作
    private List<SSAction> waitingAdd = new List<SSAction>();
    //动作的等待队列，在这个对象保存的动作会稍后注册到动作管理器里
    private List<int> waitingDelete = new List<int>();
    //动作的删除队列，在这个对象保存的动作会稍后删除
    private bool run;

    public void Start()
    {
        sceneController = (FirstController)SSDirector.GetInstance().CurrentScenceController;
        sceneController.actionManager = this;
        run = true;

    }

    public void FixedUpdate()
    {
        if (run)
        {
            foreach (SSAction action in waitingAdd)
            {
                actions[action.GetInstanceID()] = action;
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
                    action.FixedUpdate();
                }
            }

            //把删除队列里所有的动作删除
            foreach (int key in waitingDelete)
            {
                SSAction action = actions[key];
                actions.Remove(key);
                DestroyObject(action);
            }
            waitingDelete.Clear();
        }
    }

    //初始化一个动作
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
        FlyActionFactory factory = Singleton<FlyActionFactory>.Instance;
        RunAction(disk, factory.GetSSAction(), (ISSActionCallback)this);
    }

}




