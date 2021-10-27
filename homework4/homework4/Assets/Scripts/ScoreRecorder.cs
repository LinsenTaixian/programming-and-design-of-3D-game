using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreRecorder : MonoBehaviour
{
    public int score;                   //分数

    void Start()
    {
        score = 0;
    }

    //记录分数
    public void Record(GameObject disk)
    {
        score = disk.GetComponent<DiskData>().score + score;
    }

    public void Miss()
    {
        if (score >= 2)
            score -= 2;
        else
            score = 0;
    }

    //重置分数
    public void Reset()
    {
        score = 0;
    }
}
