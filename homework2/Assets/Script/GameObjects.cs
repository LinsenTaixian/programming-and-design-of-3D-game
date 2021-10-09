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