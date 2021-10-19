﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interfaces
{
    public interface ISceneController
    {
		void LoadResources();//load resources
    }

    public interface UserAction
    {
		void MoveBoat();//move the boat
        void ObjectIsClicked(GameObjects characterCtrl);
		void Restart();//restart game
    }

    public enum SSActionEventType : int { Started, Completed }

    public interface SSActionCallback
    {
        void SSActionCallback(SSAction source);
    }
}