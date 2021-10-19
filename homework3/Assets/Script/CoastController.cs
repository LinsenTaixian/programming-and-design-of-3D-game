using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoastController
{
    readonly GameObject coast;
    readonly Vector3 from_pos = new Vector3(9, 1, 0);
    readonly Vector3 to_pos = new Vector3(-9, 1, 0);
    readonly Vector3[] positions;
    readonly int State;   

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