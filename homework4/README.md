#### 第四次作业 打飞碟游戏 作业文档

作业说明：下载文档之后直接在unity中打开即可。

游戏规则介绍：

共有三个回合，难度递增，每个回合20秒.
回合一：黄色飞碟，一个一分，十分进入下一回合
回合二：加入红色飞碟，一个两分，二十分进入下一个回合
回合三，加入蓝色飞碟，一个一分，三个一起出现。

遗漏飞碟：扣两分

游戏由一下部分组成：负责与用户交互的用户界面（UserGUI）、负责游戏对象的动作的运动管理师（ActionManager）、负责管理飞碟的生产与回收的飞碟厂（DiskFactory）以及统筹这几个模块的中心管理模块也是这个唯一场景的管理者——Controller模块。代码如下：



GUI模块中实现与用户的交互

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserGUI : MonoBehaviour
{
    private IUserAction action;

    GUIStyle style;
    GUIStyle buttonStyle;
    GUIStyle resultStyle;

    void Start()
    {
        action = SSDirector.GetInstance().CurrentScenceController as IUserAction;
        style = new GUIStyle();
        style.fontSize = 25;
        style.normal.textColor = Color.blue;

        buttonStyle = new GUIStyle("button");
        buttonStyle.fontSize = 30;

        resultStyle = new GUIStyle();
        resultStyle.fontSize = 50;
        resultStyle.alignment = TextAnchor.MiddleCenter;
        resultStyle.normal.textColor = Color.red;
    }

    void OnGUI()
    {
        if (Input.GetButtonDown("Fire1") && action.GetGameState() == GameState.RUNNING)
        {

            Vector3 pos = Input.mousePosition;
            action.Hit(pos);

        }
        if (action.GetGameState() != GameState.START)
        {
            GUI.Label(new Rect(300, 0, 80, 20), "Round " + (action.GetRound() + 1).ToString(), style);
            GUI.Label(new Rect(300, 20, 80, 20), "Score: " + action.GetScore().ToString(), style);
        }

        if (action.GetGameState() == GameState.START && GUI.Button(new Rect(320, 280, 130, 55), "Start", buttonStyle))
        {
            action.SetGameState(GameState.RUNNING);
        }
        else if (action.GetGameState() == GameState.OVER)
        {
            if (GUI.Button(new Rect(320, 280, 130, 55), "Restart", buttonStyle))
                action.Restart();

            GUI.Label(new Rect(285, 130, 200, 50), "Your score is " + action.GetScore().ToString() + "!", resultStyle);
        }
    }
}
```

ActionManager  动作管理负责飞碟的飞行

```
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
```

DiskFactory  飞碟工厂主要辅助飞碟的生产和回收，即生产一个飞碟并投入使用时会加入到used队列，当被使用完了进行回收就会加入到free队列，以供下次继续使用而不比重新生产新的飞碟

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskFactory : MonoBehaviour
{
    public GameObject disk;                 //飞碟预制体
    private List<DiskData> used = new List<DiskData>();   //正在被使用的飞碟列表
    private List<DiskData> free = new List<DiskData>();   //空闲的飞碟列表

    public GameObject GetDisk(int round)
    {
        disk = null;
        if (free.Count > 0)
        {
            disk = free[0].gameObject;
            free.Remove(free[0]);
        }
        else
        {
            disk = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/disk"), Vector3.zero, Quaternion.identity);
            disk.AddComponent<DiskData>(); 
        }
        
        int start = 0;
        int selectedColor = Random.Range(start, round * 500);

        if (selectedColor >= 500)
        {
            round = 2;
        }
        else if (selectedColor >= 200)
        {
            round = 1;
        }
        else
        {
            round = 0;
        }


        switch (round)
        {

            case 0:
                {
                    disk.GetComponent<DiskData>().color = Color.yellow;
                    disk.GetComponent<DiskData>().speed = Random.Range(10f, 12f);
                    float startX = Random.Range(-1f, 1f) < 0 ? -1 : 1;
                    float startY = Random.Range(0.5f, 0.8f);
                    disk.GetComponent<DiskData>().direction = new Vector3(startX, startY, 0);
                    disk.GetComponent<DiskData>().score = 1;
                    disk.GetComponent<Renderer>().material.color = Color.yellow;
                    break;
                }
            case 1:
                {
                    disk.GetComponent<DiskData>().color = Color.red;
                    disk.GetComponent<DiskData>().speed = Random.Range(15f, 18f);
                    float startX = Random.Range(-1f, 1f) < 0 ? -1 : 1;
                    float startY = Random.Range(0.3f, 0.5f);
                    disk.GetComponent<DiskData>().direction = new Vector3(startX, startY, 0);
                    disk.GetComponent<DiskData>().score = 2;
                    disk.GetComponent<Renderer>().material.color = Color.red;
                    break;
                }
            case 2:
                {
                    disk.GetComponent<DiskData>().color = Color.blue;
                    disk.GetComponent<DiskData>().speed = Random.Range(10f, 15f);
                    float startX = Random.Range(-1f, 1f) < 0 ? -1 : 1;
                    float startY = Random.Range(0.5f, 0.6f);
                    disk.GetComponent<DiskData>().direction = new Vector3(startX, startY, 0);
                    disk.GetComponent<DiskData>().score = 3;
                    disk.GetComponent<Renderer>().material.color = Color.blue;
                    break;
                }
        }

        used.Add(disk.GetComponent<DiskData>());
        return disk;
    }

    //回收飞碟
    public void FreeDisk(GameObject disk)
    {
        for (int i = 0; i < used.Count; i++)
        {
            if (disk.GetInstanceID() == used[i].gameObject.GetInstanceID())
            {
                used[i].gameObject.SetActive(false);
                free.Add(used[i]);
                used.Remove(used[i]);
                break;
            }
        }
    }
}

```

FirstController:场记，实现场景的调度

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstController : MonoBehaviour, ISceneController, IUserAction
{
    readonly float roundTime = 20;
    private float speed = 1.5f;                                                //发射一个飞碟的时间间隔
    readonly int[] passScore = { 10, 20 };

    private List<GameObject> disks = new List<GameObject>();          //飞碟队列
    private int currentRound = 0;                                                   //回合
    private float time = 0f;                                                 //记录时间间隔
    private float currrentTime = 0f;
    private GameState gameState = GameState.START;

    public UserGUI userGUI;
    public ScoreRecorder scoreRecorder;      //计分   
    public DiskFactory diskFactory;          //生成和回收飞碟
    public IActionManager actionManager;   //动作管理


    // Start is called before the first frame update
    void Start()
    {
        SSDirector director = SSDirector.GetInstance();
        director.CurrentScenceController = this;
        diskFactory = Singleton<DiskFactory>.Instance;
        scoreRecorder = Singleton<ScoreRecorder>.Instance;
        actionManager = gameObject.AddComponent<FlyActionManager>() as FlyActionManager;
        userGUI = gameObject.AddComponent<UserGUI>() as UserGUI;

        gameState = GameState.START;
        time = 0f;
        currentRound = 0;
        currrentTime = 0;

        LoadResources();
    }

    void Update()
    {
        if (gameState == GameState.RUNNING)
        {
            for (int i = 0; i < disks.Count; i++)
            {
                //飞碟飞出摄像机视野也没被打中
                if ((disks[i].transform.position.y <= -1) && disks[i].gameObject.activeSelf == true)
                {
                    scoreRecorder.Miss();
                    diskFactory.FreeDisk(disks[i]);
                    disks.Remove(disks[i]);
                }
            }
            if (time > speed)
            {
                time = 0;
                SendDisk();
            }
            else
            {
                time += Time.deltaTime;
            }

            if (currrentTime > roundTime)
            {
                currrentTime = 0;
                if (currentRound < 2 && GetScore() >= passScore[currentRound])
                {
                    currentRound++;
                    time = 0f;
                }
                else
                {
                    GameOver();
                }
            }
            else
            {
                currrentTime += Time.deltaTime;
            }
        }
    }

    private void GameOver()
    {
        gameState = GameState.OVER;
        currrentTime = 40;
    }

    public void LoadResources()
    {
        
    }

    public void Hit(Vector3 pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);
        bool isHit = false;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];


            if (hit.collider.gameObject.GetComponent<DiskData>() != null)       //射线打中某物体
            {
                scoreRecorder.Record(hit.collider.gameObject);      //记分员记录分数
                hit.collider.gameObject.transform.position = new Vector3(0, -9, 0);
                diskFactory.FreeDisk(hit.collider.gameObject);
            }
        }
    }

    //发送飞碟
    private void SendDisk()
    {
        GameObject disk = diskFactory.GetDisk(currentRound);
        disks.Add(disk);
        disk.SetActive(true);

        Vector3 position = new Vector3(0, 0, 0);
        float y = Random.Range(1f, 4f);
        position = new Vector3(-disk.GetComponent<DiskData>().direction.x * 12, y, 0);
        disk.transform.position = position;
        actionManager.StartThrow(disk);

        if (disk.GetComponent<DiskData>().color == Color.blue)
        {
            GameObject disk1 = Instantiate(disk);
            GameObject disk2 = Instantiate(disk);

            disks.Add(disk1);
            disk1.SetActive(true);
            disk1.transform.position = position;
            disk1.GetComponent<DiskData>().direction.y = Random.Range(0.6f, 0.7f);
            actionManager.StartThrow(disk1);

            disks.Add(disk2);
            disk2.SetActive(true);
            disk2.transform.position = position;
            disk2.GetComponent<DiskData>().direction.y = Random.Range(0.4f, 0.5f);
            actionManager.StartThrow(disk2);
        }
    }

    public void Restart()
    {
        time = 0f;
        currentRound = 0;
        currrentTime = 0;
        scoreRecorder.Reset();
        gameState = GameState.RUNNING;
    }

    public int GetScore()
    {
        return scoreRecorder.score;
    }

    public void SetGameState(GameState state)
    {
        gameState = state;
    }

    public GameState GetGameState()
    {
        return gameState;
    }

    public float GetTime()
    {
        return currrentTime;
    }

    public int GetRound()
    {
        return currentRound;
    }
}
```

最后还有一个ScoreRecorder，负责记分

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreRecorder : MonoBehaviour
{
    public int score;                   //分数

    void Start()
    {
        score = 0;
    }

    //记录分数
    public void Record(GameObject disk)
    {
        score = disk.GetComponent<DiskData>().score + score;
    }

    public void Miss()
    {
        if (score >= 2)
            score -= 2;
        else
            score = 0;
    }

    //重置分数
    public void Reset()
    {
        score = 0;
    }
}
```



游戏演示在文件中。