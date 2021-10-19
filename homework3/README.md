#### 第三次作业 牧师与魔鬼游戏动作分离版 作业文档



作业使用说明：在unity中将Assets目录替换原来的目录，然后创建一个3D空对象，将Assets目录中的Script目录下的FirstController和FirstControllerManager脚本挂在在该对象上，随后运行。



作业目的：实现动作管理器的设计。



设计图如下：

![设计图](.\ch04-oo-design.png)



设计思路如下：

- 通过门面模式（控制器模式）输出组合好的几个动作，共原来程序调用。
  - 好处，动作如何组合变成动作模块内部的事务
  - 这个门面就是 CCActionManager
- 通过组合模式实现动作组合，按组合模式设计方法
  - 必须有一个抽象事物表示该类事物的共性，例如 SSAction，表示动作，不管是基本动作或是组合后动作
  - 基本动作，用户设计的基本动作类。 例如：CCMoveToAction
  - 组合动作，由（基本或组合）动作组合的类。例如：CCSequenceAction
- 接口回调（函数回调）实现管理者与被管理者解耦
  - 如组合对象实现一个事件抽象接口（ISSCallback），作为监听器（listener）监听子动作的事件
  - 被组合对象使用监听器传递消息给管理者。至于管理者如何处理就是实现这个监听器的人说了算了
  - 例如：每个学生做完作业通过邮箱发消息给学委，学委是谁，如何处理，学生就不用操心了
- 通过模板方法，让使用者减少对动作管理过程细节的要求
  - SSActionManager 作为 CCActionManager 基类



##### 关键代码解读

1. 动作基类

   ```
   public class SSAction : ScriptableObject {
   
       public bool enable = true;
       public bool destroy = false;
   
       public GameObject gameObject;
       public Transform transform;
       public SSActionCallback CallBack;
   
       public virtual void Start()
       {
           throw new System.NotImplementedException();
       }
   
       public virtual void Update()
       {
           throw new System.NotImplementedException();
       }
   }
   ```

   设计要点：

   - [ScriptableObject](https://docs.unity3d.com/ScriptReference/ScriptableObject.html) 是不需要绑定 GameObject 对象的可编程基类。这些对象受 Unity 引擎场景管理
   - `protected` 防止用户自己 new 抽象的对象
   - 使用 `virtual` 申明虚方法，通过重写实现多态。这样继承者就明确使用 Start 和 Update 编程游戏对象行为
   - 利用接口（`SSACtionCallback`）实现消息通知，避免与动作管理者直接依赖

2. **简单动作实现**

   实现具体动作，将一个物体移动到目标位置，并通知任务完成：

   ```
   public class CCMoveToAction : SSAction
   {
       public Vector3 target;
       public float speed;
   
       private CCMoveToAction() { }
       public static CCMoveToAction getAction(Vector3 target, float speed)
       {
           CCMoveToAction action = ScriptableObject.CreateInstance<CCMoveToAction>();
           action.target = target;
           action.speed = speed;
           return action;
       }
   
       public override void Update()
       {
           this.transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
           if (transform.position == target)
           {
               destroy = true;
               CallBack.SSActionCallback(this);
           }
       }
   
       public override void Start({}
   }
   ```

   设计要点：

   - 让 Unity 创建动作类，确保内存正确回收。
   - 多态。
   - 似曾相识的运动代码。动作完成，并发出事件通知，期望管理程序自动回收运行对象。

   

3. **顺序动作组合类实现**

   实现一个动作组合序列，顺序播放动作：

   ```
   public class CCSequenceAction : SSAction, SSActionCallback
   {
       public List<SSAction> sequence;
       public int repeat = 1; 
       public int currentActionIndex = 0;
   
       public static CCSequenceAction getAction(int repeat, int currentActionIndex, List<SSAction> sequence)
       {
           CCSequenceAction action = ScriptableObject.CreateInstance<CCSequenceAction>();
           action.sequence = sequence;
           action.repeat = repeat;
           action.currentActionIndex = currentActionIndex;
           return action;
       }
   
       public override void Update()
       {
           if (sequence.Count == 0) return;
           if (currentActionIndex < sequence.Count)
           {
               sequence[currentActionIndex].Update();
           }
       }
   
       public void SSActionCallback(SSAction source)
       {
           source.destroy = false;
           this.currentActionIndex++;
           if (this.currentActionIndex >= sequence.Count)
           {
               this.currentActionIndex = 0;
               if (repeat > 0) repeat--;
               if (repeat == 0)
               {
                   this.destroy = true;
                   this.CallBack.SSActionCallback(this);
               }
           }
       }
   
       public override void Start()
       {
           foreach (SSAction action in sequence)
           {
               action.gameObject = this.gameObject;
               action.transform = this.transform;
               action.CallBack = this;
               action.Start();
           }
       }
   
       void OnDestroy()
       {
           foreach (SSAction action in sequence)
           {
               DestroyObject(action);
           }
       }
   }
   ```

   设计要点：

   - 让动作组合继承抽象动作，能够被进一步组合；实现回调接受，能接收被组合动作的事件
   - 创建一个动作顺序执行序列，-1 表示无限循环，start 开始动作。
   - `Update`方法执行执行当前动作
   - `SSActionEvent` 收到当前动作执行完成，推下一个动作，如果完成一次循环，减次数。如完成，通知该动作的管理者
   - `Start` 执行动作前，为每个动作注入当前动作游戏对象，并将自己作为动作事件的接收者
   - `OnDestory` 如果自己被注销，应该释放自己管理的动作。

   

4. **动作管理基类 – SSActionManager**

   这是动作对象管理器的基类，实现了所有动作的基本管理

   ```
   public class SSActionManager : MonoBehaviour
   {
       private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
       private List<SSAction> waitingToAdd = new List<SSAction>();
       private List<int> watingToDelete = new List<int>();
   
       protected void Update()
       {
           foreach (SSAction ac in waitingToAdd)
           {
               actions[ac.GetInstanceID()] = ac;
           }
           waitingToAdd.Clear();
   
           foreach (KeyValuePair<int, SSAction> kv in actions)
           {
               SSAction ac = kv.Value;
               if (ac.destroy)
               {
                   watingToDelete.Add(ac.GetInstanceID());
               }
               else if (ac.enable)
               {
                   ac.Update();
               }
           }
   
           foreach (int key in watingToDelete)
           {
               SSAction ac = actions[key];
               actions.Remove(key);
               DestroyObject(ac);
           }
           watingToDelete.Clear();
       }
   
       public void addAction(GameObject gameObject, SSAction action, SSActionCallback ICallBack)
       {
           action.gameObject = gameObject;
           action.transform = gameObject.transform;
           action.CallBack = ICallBack;
           waitingToAdd.Add(action);
           action.Start();
       }
   }
   ```

   设计要点：

   - 创建 MonoBehaiviour 管理一个动作集合，动作做完自动回收动作。
   - update 演示了由添加、删除等复杂集合对象的使用。
   - addAction方法把游戏对象与动作绑定，并绑定该动作事件的消息接收者(AddAction)
   - 执行该动作的 Start 方法

   

5. #### FirstSceneActionManager 实战动作管理

   实战动作管理器:与场景控制器配合,从而实现对场景的控制

   ```
   public class FirstSceneActionManager : SSActionManager, SSActionCallback
   {
       public SSActionEventType Complete = SSActionEventType.Completed;
   
       public void BoatMove(BoatController Boat)
       {
           Complete = SSActionEventType.Started;
           CCMoveToAction action = CCMoveToAction.getAction(Boat.GetDestination(), Boat.GetMoveSpeed());
           addAction(Boat.GetGameObject(), action, this);
           Boat.ChangeState();
       }
   
       public void GameObjectsMove(GameObjects GameObject, Vector3 Destination)
       {
           Complete = SSActionEventType.Started;
           Vector3 CurrentPos = GameObject.GetPosition();
           Vector3 MiddlePos = CurrentPos;
           if (Destination.y > CurrentPos.y)
           {
               MiddlePos.y = Destination.y;
           }
           else
           {
               MiddlePos.x = Destination.x;
           }
           SSAction action1 = CCMoveToAction.getAction(MiddlePos, GameObject.GetMoveSpeed());
           SSAction action2 = CCMoveToAction.getAction(Destination, GameObject.GetMoveSpeed());
           SSAction seqAction = CCSequenceAction.getAction(1, 0, new List<SSAction> { action1, action2 });
           this.addAction(GameObject.GetGameobject(), seqAction, this);
       }
   
       public void SSActionCallback(SSAction source)
       {
           Complete = SSActionEventType.Completed;
       }
   }
   ```

   修改场景控制器,实现动作分离:

   ```
   void Start(){
   	FSActionManager = GetComponent<FirstSceneActionManager>();
   }
   ```

   moveboat

   ```
       public void MoveBoat()
       {
           if (FSActionManager.Complete == SSActionEventType.Started || boat.isEmpty()) return;
           FSActionManager.BoatMove(boat);//满足条件,动作管理器去实现动作
           UserGUI.SetState = Check();
       }
   ```



程序运行视频在文件中相同目录下。