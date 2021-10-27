using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSDirector : System.Object
{
    //singlton instance
    private static SSDirector _instance;

    public ISceneController CurrentScenceController { get; set; }

    //get instance
    public static SSDirector GetInstance()
    {
        if (_instance == null)
        {
            _instance = new SSDirector();
        }
        return _instance;
    }
}