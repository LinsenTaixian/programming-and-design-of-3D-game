
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{

	readonly float Speed = 15;

	Vector3 Target;
	Vector3 Middle;
	int Move_State = 0;  // 0-no move, 1->object move , 2->boat move
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
