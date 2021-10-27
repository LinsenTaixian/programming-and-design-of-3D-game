using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { START, RUNNING, OVER, PAUSE }

public interface IUserAction
{
    void Restart();
    // void Pause();
    // void Run();
    void Hit(Vector3 pos);
    int GetScore();
    float GetTime();
    int GetRound();
    void SetGameState(GameState state);
    GameState GetGameState();
}