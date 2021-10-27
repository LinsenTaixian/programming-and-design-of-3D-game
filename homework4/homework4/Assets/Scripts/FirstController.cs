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
        //不需要加载，飞碟由diskFactory生产了
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
