using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class game : MonoBehaviour
{
    private bool turn;
    // 1 for begin
    // 0 for stop
    private bool singlemode;
    // 0 for block
    // 1 for user
    // 2 for computer
    private bool state;

    private int [,] map = new int [3, 3];
    // 0 for no one
    // 1 for user or user1
    // 2 for computer or user2

    int Winner;

    // Start is called before the first frame update
    void Start()
    {
        singlemode = true;
        reset_fun();
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }
    void OnGUI(){
        if (GUI.Button(new Rect(200, 20, 100, 50), "Single mode")){
            singlemode = true;
            reset_fun();
        }
        if (GUI.Button(new Rect(500, 20, 100, 50), "Double mode")){
            singlemode = false;
            reset_fun();
        }
        if (GUI.Button(new Rect(200, 90, 50, 30), "Reset")){
            reset_fun();
        }
        for(int i = 0;i < 3;i++){
            for(int j = 0;j < 3;j++){
                drawgame(i,j);
            }
        }
        if(Winner != 0){
            if(singlemode && Winner == 1) GUI.Button(new Rect(350, 20, 100, 50), "You win");
            else if(singlemode && Winner == 2) GUI.Button(new Rect(350, 20, 100, 50), "You loss");
            else if(!singlemode && Winner == 1) GUI.Button(new Rect(350, 20, 100, 50), "Player1 win");
            else GUI.Button(new Rect(350, 20, 100, 50), "Player2 win");
        }
    }

    void drawgame(int i, int j){
        if(map[i,j] == 0){
            if(GUI.Button(new Rect(320 + i*50,150 + j*50,50,50), "")){
                if(state)
                    if(step(i,j,turn)) turn = !turn;
            }
        }
        else if(map[i,j] == 1){
            GUI.Button(new Rect(320 + i*50,150 + j*50,50,50), "X");
        }
        else{
            GUI.Button(new Rect(320 + i*50,150 + j*50,50,50), "O");
        }
        if(check() != 0){
            state = false;
            Winner = check();
        }
        if(state && singlemode && turn){
            int [] randposite = rand();
            if(randposite[0] != -1) {
                step(randposite[0], randposite[1], turn);
                turn = !turn;
            }
            else state = false;
            if(check() != 0){
                state = false;
                Winner = check();
            }
        }
    }

    void reset_fun(){
        state = true;
        turn = false;
        Winner = 0;
        for(int i = 0;i < 3;i++){
            for(int j = 0;j < 3;j++){
                map[i,j] = 0;
            }
        }
    }
    bool step(int i , int j, bool iscomputeturn = false){
        if(map[i, j] == 0){
            if(iscomputeturn) map[i,j] = 2;
            else map[i,j] = 1;
            return true;
        }
        else return false;
    }
    // 0 for no winner
    // 1 for user
    // 2 for computer
    int check(){
        for(int k = 1;k < 3;k++){
            // row check
            for(int i = 0;i < 3;i++){
                if(map[i,0] == k && map[i,1] == k && map[i,2] == k) return k;
            }
            // column check
            for(int i = 0;i < 3;i++){
                if(map[0,i] == k && map[1,i] == k && map[2,i] == k) return k;
            }
            // cross line check
            if(map[0,0] == k && map[1,1] == k && map[2,2] == k) return k;
            if(map[0,2] == k && map[1,1] == k && map[2,0] == k) return k;
        }
        return 0;
    }
    int [] rand(){
        int x,y;
        List<int> row = new List<int>();
        List<int> col = new List<int>();
        int count = 0;
        for (int i = 0; i < 3; ++i) {
            for (int j = 0; j < 3; ++j) {
                if (map[i,j] == 0) {
                    row.Add(i);
                    col.Add(j);
                    count++;
                }
            }
        }
        if (count != 0) {
            System.Random ran = new System.Random();
            int index = ran.Next(0, count);
            x = row[index];
            y = col[index];

        } else {
            x = y = -1;
        }
        int [] ans = {x,y};
        return ans;
    }
}
