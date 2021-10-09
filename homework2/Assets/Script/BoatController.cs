﻿using System.Collections;
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