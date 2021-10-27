using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserGUI : MonoBehaviour
{
    private IUserAction action;

    GUIStyle style;
    GUIStyle buttonStyle;
    GUIStyle resultStyle;

    void Start()
    {
        action = SSDirector.GetInstance().CurrentScenceController as IUserAction;
        style = new GUIStyle();
        style.fontSize = 25;
        style.normal.textColor = Color.blue;

        buttonStyle = new GUIStyle("button");
        buttonStyle.fontSize = 30;

        resultStyle = new GUIStyle();
        resultStyle.fontSize = 50;
        resultStyle.alignment = TextAnchor.MiddleCenter;
        resultStyle.normal.textColor = Color.red;
    }

    void OnGUI()
    {
        if (Input.GetButtonDown("Fire1") && action.GetGameState() == GameState.RUNNING)
        {

            Vector3 pos = Input.mousePosition;
            action.Hit(pos);

        }
        if (action.GetGameState() != GameState.START)
        {
            GUI.Label(new Rect(300, 0, 80, 20), "Round " + (action.GetRound() + 1).ToString(), style);
            GUI.Label(new Rect(300, 20, 80, 20), "Score: " + action.GetScore().ToString(), style);
        }

        if (action.GetGameState() == GameState.START && GUI.Button(new Rect(320, 280, 130, 55), "Start", buttonStyle))
        {
            action.SetGameState(GameState.RUNNING);
        }
        else if (action.GetGameState() == GameState.OVER)
        {
            if (GUI.Button(new Rect(320, 280, 130, 55), "Restart", buttonStyle))
                action.Restart();

            GUI.Label(new Rect(285, 130, 200, 50), "Your score is " + action.GetScore().ToString() + "!", resultStyle);
        }
    }
}
