### 第二次作业 牧师与魔鬼游戏 作业文档



**使用介绍**：将github的中Assets文档直接代替unity中的文件夹，之后新建一个gameobject，将Assets中的Script文件夹中的FirstController脚本挂载到该游戏对象中。



**项目内容介绍**：这里一共有三个文件夹，material和resource和script，第一个文件夹存的是图片，第二个是预制的角色和物体等内容，第三个就是脚本，将前面的素材加以连接和加上逻辑形成游戏。



**作业内容介绍**：这次的作业内容是实现牧师与魔鬼的游戏，游戏规则如下：

Priests and Devils is a puzzle game in which you will help the Priests and Devils to cross the river within the time limit. There are 3 priests and 3 devils at one side of the river. They all want to get to the other side of this river, but there is only one boat and this boat can only carry two persons each time. And there must be one person steering the boat from one side to the other side. In the flash game, you can click on them to move them and click the go button to move the boat to the other direction. If the priests are out numbered by the devils on either side of the river, they get killed and the game is over. You can try it in many > ways. Keep all priests alive! Good luck!



这里使用了老师课堂上说的MVC模式，使用的拍戏中的导演的场景控制。

首先有一个导演，和拍戏一样，一个游戏也只有一个导演，所以使用的单例模式来设计该类。

```
public class SSDirector : System.Object {

private static SSDirector _instance;

public ISceneController currentScenceController { get; set; }
public bool running { get; set; }

public static SSDirector getInstance()
{
    if (_instance == null)
    {
    	_instance = new SSDirector();
    }
	return _instance;
}
}
```



之后就是场务了，场务就是负责一个场景的类，由于场务有多个，但是为了封装，所以用了接口封装起来，这样导演调用方便一些，下面也列出了另一个接口——游戏玩家的输入响应接口。

```
public interface ISceneController
{
	void LoadResources();  //load resources
}

public interface UserAction
{
    void MoveBoat();//move the boat
    void ObjectIsClicked(GameObjects characterCtrl);
	void Restart();//restart game
}
```



这个游戏的场景比较简单，所以只使用了一个场务FirstController，该类实例化了上面的两个接口，使得整个游戏运转起来。



```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;

public class FirstController : MonoBehaviour, ISceneController, UserAction
{
	InteracteGUI UserGUI;
	public CoastController fromCoast;
	public CoastController toCoast;
	public BoatController boat;
	private GameObjects[] GameObjects;

	void Awake()
	{
		SSDirector director = SSDirector.getInstance();
		director.currentScenceController = this;
		UserGUI = gameObject.AddComponent<InteracteGUI>() as InteracteGUI;
		GameObjects = new GameObjects[6];
		LoadResources();
	}

	public void LoadResources()
	{
		fromCoast = new CoastController("from");
		toCoast = new CoastController("to");
		boat = new BoatController();
		GameObject water = Instantiate(Resources.Load("Perfabs/Water", typeof(GameObject)), new Vector3(0, 0.5F, 0), Quaternion.identity, null) as GameObject;
		water.name = "water";
		for (int i = 0; i < 3; i++)
		{
			GameObjects s = new GameObjects("priest");
			s.setName("priest" + i);
			s.setPosition(fromCoast.getEmptyPosition());
			s.getOnCoast(fromCoast);
			fromCoast.getOnCoast(s);
			GameObjects[i] = s;
		}

		for (int i = 0; i < 3; i++)
		{
			GameObjects s = new GameObjects("devil");
			s.setName("devil" + i);
			s.setPosition(fromCoast.getEmptyPosition());
			s.getOnCoast(fromCoast);
			fromCoast.getOnCoast(s);
			GameObjects[i + 3] = s;
		}
	}

	public void ObjectIsClicked(GameObjects Objects)
	{
		if (Objects.isOnBoat())
		{
			CoastController whichCoast;
			if (boat.get_State() == -1)
			{ // to->-1; from->1
				whichCoast = toCoast;
			}
			else
			{
				whichCoast = fromCoast;
			}

			boat.GetOffBoat(Objects.getName());
			Objects.moveToPosition(whichCoast.getEmptyPosition());
			Objects.getOnCoast(whichCoast);
			whichCoast.getOnCoast(Objects);

		}
		else
		{                                   
			CoastController whichCoast = Objects.getCoastController(); // obejects on coast

			if (boat.getEmptyIndex() == -1)
			{      
				return;
			}
			if (whichCoast.get_State() != boat.get_State())   // boat is not on the side of character
				return;

			whichCoast.getOffCoast(Objects.getName());
			Objects.moveToPosition(boat.getEmptyPosition());
			Objects.getOnBoat(boat);
			boat.GetOnBoat(Objects);
		}
		UserGUI.SetState = Check();
	}

	public void MoveBoat()
	{
		if (boat.isEmpty()) return;
		boat.Move();
		UserGUI.SetState = Check();
	}

	int Check()
	{   // 0->not finish, 1->lose, 2->win
		int from_priest = 0;
		int from_devil = 0;
		int to_priest = 0;
		int to_devil = 0;

		int[] fromCount = fromCoast.GetobjectsNumber();
		from_priest += fromCount[0];
		from_devil += fromCount[1];

		int[] toCount = toCoast.GetobjectsNumber();
		to_priest += toCount[0];
		to_devil += toCount[1];

		if (to_priest + to_devil == 6)      // win
			return 2;

		int[] boatCount = boat.GetobjectsNumber();
		if (boat.get_State() == -1)
		{   // boat at toCoast
			to_priest += boatCount[0];
			to_devil += boatCount[1];
		}
		else
		{   // boat at fromCoast
			from_priest += boatCount[0];
			from_devil += boatCount[1];
		}
		if (from_priest < from_devil && from_priest > 0)
		{       // lose
			return 1;
		}
		if (to_priest < to_devil && to_priest > 0)
		{
			return 1;
		}
		return 0;           // not finish
	}

	public void Restart()
	{
		boat.reset();
		fromCoast.reset();
		toCoast.reset();
		for (int i = 0; i < GameObjects.Length; i++)
		{
			GameObjects[i].reset();
		}
	}
}
```



之后就是移动的两个游戏角色，牧师和魔鬼了，这两个有着很多相同的地方，所以使用一个类来实现。具体不同就在创建函数中创建不一样的角色。



```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjects
{
	readonly GameObject Instance;
	readonly Move Move;
	readonly ClickGUI clickGUI;
	readonly int characterType; // 0->priest, 1->devil

	bool _isOnBoat = false;
	CoastController coastController;


	public GameObjects(string Type)
	{

		if (Type == "priest")
		{
			Instance = Object.Instantiate(Resources.Load("Perfabs/Priest", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
			characterType = 0;
		}
		else
		{
			Instance = Object.Instantiate(Resources.Load("Perfabs/Devil", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
			characterType = 1;
		}
		Move = Instance.AddComponent(typeof(Move)) as Move;

		clickGUI = Instance.AddComponent(typeof(ClickGUI)) as ClickGUI;
		clickGUI.setController(this);
	}

	public void setName(string name)
	{
		Instance.name = name;
	}

	public void setPosition(Vector3 pos)
	{
		Instance.transform.position = pos;
	}

	public void moveToPosition(Vector3 destination)
	{
		Move.SetDestination(destination);
	}

	public int getType()
	{   // 0->priest, 1->devil
		return characterType;
	}

	public string getName()
	{
		return Instance.name;
	}

	public void getOnBoat(BoatController boatCtrl)
	{
		coastController = null;
		Instance.transform.parent = boatCtrl.getGameobj().transform;
		_isOnBoat = true;
	}

	public void getOnCoast(CoastController coastCtrl)
	{
		coastController = coastCtrl;
		Instance.transform.parent = null;
		_isOnBoat = false;
	}

	public bool isOnBoat()
	{
		return _isOnBoat;
	}

	public CoastController getCoastController()
	{
		return coastController;
	}

	public void reset()
	{
		Move.Reset();
		coastController = (SSDirector.getInstance().currentScenceController as FirstController).fromCoast;
		getOnCoast(coastController);
		setPosition(coastController.getEmptyPosition());
		coastController.getOnCoast(this);
	}
}
```



这里还有一个很重要的问题就是角色的移动，这里单独写一个脚本来实现这个功能

```
public class Move : MonoBehaviour
{
	readonly float Speed = 20;
	Vector3 Target;
	Vector3 Middle;
	int Move_State = 0;  
	// 0-no move, 1->object move , 2->boat move
	bool To_Middle = true;

	void Update()
	{
		if (Move_State == 1)
		{
			if (To_Middle)
			{
				transform.position = Vector3.MoveTowards(transform.position, Middle, Speed * Time.deltaTime);
				if (transform.position == Middle) To_Middle = false;
			}
			else
			{
				transform.position = Vector3.MoveTowards(transform.position, Target, Speed * Time.deltaTime);
				if (transform.position == Target) Move_State = 0;
			}
		}
		else if (Move_State == 2)
		{
			transform.position = Vector3.MoveTowards(transform.position, Target, Speed * Time.deltaTime);
			if (transform.position == Target)
			{
				To_Middle = true;
				Move_State = 0;
			}
		}
	}

	public void SetDestination(Vector3 Position)
	{
		if (Move_State != 0) return;
		Target = Middle = Position;
		To_Middle = true;
		if (transform.position.y == Target.y)
		{
			Move_State = 2;
		}
		else
		{
			Move_State = 1;
			if (transform.position.y < Target.y)
			{
				Middle.x = transform.position.x;
			}
			else if (transform.position.y > Target.y)
			{
				Middle.y = transform.position.y;
			}
		}
	}

	public void Reset()
	{
		Move_State = 0;
		To_Middle = true;
	}
}
```



之后还有两个脚本CoastController和BoatController，这两个脚本分别是岸边和船，

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController
{
	readonly GameObject boat;
	readonly Move Moving;
	readonly Vector3 fromPosition = new Vector3(5, 1, 0);
	readonly Vector3 toPosition = new Vector3(-5, 1, 0);
	readonly Vector3[] from_positions;
	readonly Vector3[] to_positions;

	int State; 
	// to->-1; from->1

	GameObjects[] passenger = new GameObjects[2];

	public BoatController()
	{
		State = 1;

		from_positions = new Vector3[] { new Vector3(4.5F, 1.5F, 0), new Vector3(5.5F, 1.5F, 0) };
		to_positions = new Vector3[] { new Vector3(-5.5F, 1.5F, 0), new Vector3(-4.5F, 1.5F, 0) };

		boat = Object.Instantiate(Resources.Load("Perfabs/Boat", typeof(GameObject)), fromPosition, Quaternion.identity, null) as GameObject;
		boat.name = "boat";

		Moving = boat.AddComponent(typeof(Move)) as Move;
		boat.AddComponent(typeof(ClickGUI));
	}

	public int getEmptyIndex()
	{
		for (int i = 0; i < passenger.Length; i++)
		{
			if (passenger[i] == null)
			{
				return i;
			}
		}
		return -1;
	}

	public bool isEmpty()
	{
		for (int i = 0; i < passenger.Length; i++)
		{
			if (passenger[i] != null)
			{
				return false;
			}
		}
		return true;
	}

	public Vector3 getEmptyPosition()
	{
		Vector3 pos;
		int emptyIndex = getEmptyIndex();
		if (State == -1)
		{
			pos = to_positions[emptyIndex];
		}
		else
		{
			pos = from_positions[emptyIndex];
		}
		return pos;
	}

	public void GetOnBoat(GameObjects ObjectControl)
	{
		int index = getEmptyIndex();
		passenger[index] = ObjectControl;
	}

	public GameObjects GetOffBoat(string passenger_name)
	{
		for (int i = 0; i < passenger.Length; i++)
		{
			if (passenger[i] != null && passenger[i].getName() == passenger_name)
			{
				GameObjects charactorCtrl = passenger[i];
				passenger[i] = null;
				return charactorCtrl;
			}
		}
		return null;
	}

	public GameObject getGameobj()
	{
		return boat;
	}

	public int get_State()
	{ 
		// to->-1; from->1
		return State;
	}

	public int[] GetobjectsNumber()
	{
		int[] count = { 0, 0 };
		for (int i = 0; i < passenger.Length; i++)
		{
			if (passenger[i] == null)
				continue;
			if (passenger[i].getType() == 0)
			{
				count[0]++;
			}
			else
			{
				count[1]++;
			}
		}
		return count;
	}

	public void Move()
	{
		if (State == -1)
		{
			Moving.SetDestination(fromPosition);
			State = 1;
		}
		else
		{
			Moving.SetDestination(toPosition);
			State = -1;
		}
	}

	public void reset()
	{
		Moving.Reset();
		if (State == -1)
		{
			Move();
		}
		passenger = new GameObjects[2];
	}
}
```



````
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoastController
{
	readonly GameObject coast;
	readonly Vector3 from_pos = new Vector3(9, 1, 0);
	readonly Vector3 to_pos = new Vector3(-9, 1, 0);
	readonly Vector3[] positions;
	readonly int State;    // to->-1, from->1

	GameObjects[] passengerPlaner;

	public CoastController(string _State)
	{
		positions = new Vector3[] {new Vector3(6.5F,2.25F,0), new Vector3(7.5F,2.25F,0), new Vector3(8.5F,2.25F,0),
			new Vector3(9.5F,2.25F,0), new Vector3(10.5F,2.25F,0), new Vector3(11.5F,2.25F,0)};

		passengerPlaner = new GameObjects[6];

		if (_State == "from")
		{
			coast = Object.Instantiate(Resources.Load("Perfabs/Ston", typeof(GameObject)), from_pos, Quaternion.identity, null) as GameObject;
			coast.name = "from";
			State = 1;
		}
		else
		{
			coast = Object.Instantiate(Resources.Load("Perfabs/Ston", typeof(GameObject)), to_pos, Quaternion.identity, null) as GameObject;
			coast.name = "to";
			State = -1;
		}
	}

	public int getEmptyIndex()
	{
		for (int i = 0; i < passengerPlaner.Length; i++)
		{
			if (passengerPlaner[i] == null)
			{
				return i;
			}
		}
		return -1;
	}

	public Vector3 getEmptyPosition()
	{
		Vector3 pos = positions[getEmptyIndex()];
		pos.x *= State;
		return pos;
	}

	public void getOnCoast(GameObjects ObjectControl)
	{
		int index = getEmptyIndex();
		passengerPlaner[index] = ObjectControl;
	}

	public GameObjects getOffCoast(string passenger_name)
	{   // 0->priest, 1->devil
		for (int i = 0; i < passengerPlaner.Length; i++)
		{
			if (passengerPlaner[i] != null && passengerPlaner[i].getName() == passenger_name)
			{
				GameObjects charactorCtrl = passengerPlaner[i];
				passengerPlaner[i] = null;
				return charactorCtrl;
			}
		}
		Debug.Log("cant find passenger on coast: " + passenger_name);
		return null;
	}

	public int get_State()
	{
		return State;
	}

	public int[] GetobjectsNumber()
	{
		int[] count = { 0, 0 };
		for (int i = 0; i < passengerPlaner.Length; i++)
		{
			if (passengerPlaner[i] == null)
				continue;
			if (passengerPlaner[i].getType() == 0)
			{   // 0->priest, 1->devil
				count[0]++;
			}
			else
			{
				count[1]++;
			}
		}
		return count;
	}

	public void reset()
	{
		passengerPlaner = new GameObjects[6];
	}
}
````



最后就是游戏和玩家的交互部分。

```

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;

public class InteracteGUI : MonoBehaviour {
	UserAction UserAcotionController;
	public int SetState { get { return GameState; } set { GameState = value; } }
	static int GameState = 0;

	//  Initialization
	void Start () {
		UserAcotionController = SSDirector.getInstance().currentScenceController as UserAction;
	}

	private void OnGUI()
	{
		if (GameState == 1)
		{
			GUI.Label(new Rect(Screen.width / 2 -30, Screen.height / 2 - 30, 100, 50), "Gameover!");
			if (GUI.Button(new Rect(Screen.width / 2 - 70, Screen.height / 2+110, 140, 30), "Restart"))
			{
				GameState = 0;
				UserAcotionController.Restart();
			}
		}
		else if (GameState == 2)
		{
			GUI.Label(new Rect(Screen.width / 2 - 30, Screen.height / 2 - 30 , 100, 50), "You Win!");
			if (GUI.Button(new Rect(Screen.width / 2 - 70, Screen.height / 2+110, 140, 30), "Restart"))
			{
				GameState = 0;
				UserAcotionController.Restart();
			}
		}
	}
}

public class ClickGUI : MonoBehaviour{
	UserAction UserAcotionController;
	GameObjects GameObjectsInScene;

	public void setController(GameObjects characterCtrl)
	{
		GameObjectsInScene = characterCtrl;
	}

	void Start()
	{
		UserAcotionController = SSDirector.getInstance().currentScenceController as UserAction;
	}

	void OnMouseDown()
	{
		if (gameObject.name == "boat")
		{
			UserAcotionController.MoveBoat();
		}
		else
		{
			UserAcotionController.ObjectIsClicked(GameObjectsInScene);
		}
	}
}
```



游戏主要逻辑如上。