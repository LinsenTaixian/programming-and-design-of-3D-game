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

    private FirstSceneActionManager FSActionManager;

    void Start()
    {
        FSActionManager = GetComponent<FirstSceneActionManager>();
    }

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
        if (FSActionManager.Complete == SSActionEventType.Started) return;
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
            FSActionManager.GameObjectsMove(Objects,whichCoast.getEmptyPosition());
            Objects.getOnCoast(whichCoast);
            whichCoast.getOnCoast(Objects);

        }
        else
        {
            Debug.Log("On Coast!");
            CoastController whichCoast = Objects.getCoastController(); 

            if (boat.getEmptyIndex() == -1)
            {      
                return;
            }

            if (whichCoast.get_State() != boat.get_State())  
                return;

            whichCoast.getOffCoast(Objects.getName());
            FSActionManager.GameObjectsMove(Objects, boat.getEmptyPosition());
            Objects.getOnBoat(boat);
            boat.GetOnBoat(Objects);
        }
        UserGUI.SetState = Check();
    }

    public void MoveBoat()
    {
        if (FSActionManager.Complete == SSActionEventType.Started || boat.isEmpty()) return;
        FSActionManager.BoatMove(boat);
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
        {   // toCoast
            to_priest += boatCount[0];
            to_devil += boatCount[1];
        }
        else
        {   // fromCoast
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
        return 0;       
    }

    public void Restart()
    {
        fromCoast.reset();
        toCoast.reset();
        foreach (GameObjects gameobject in GameObjects)
        {
            gameobject.reset();
        }
        boat.reset();
    }
}