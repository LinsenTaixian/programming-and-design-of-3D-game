using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;

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