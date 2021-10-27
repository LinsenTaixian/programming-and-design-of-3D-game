using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskFactory : MonoBehaviour
{
    public GameObject disk;                 //飞碟预制体
    private List<DiskData> used = new List<DiskData>();   //正在被使用的飞碟列表
    private List<DiskData> free = new List<DiskData>();   //空闲的飞碟列表

    public GameObject GetDisk(int round)
    {
        disk = null;
        if (free.Count > 0)
        {
            disk = free[0].gameObject;
            free.Remove(free[0]);
        }
        else
        {
            disk = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/disk"), Vector3.zero, Quaternion.identity);
            disk.AddComponent<DiskData>(); 
        }
        // round 1: 全是黄色disk，最慢 
        // round 2: 40%可能出现黄色，60%可能出现红色（更快）
        // round 3: 20%可能出现黄色，30%可能出现红色，50%可能出现黑色（更快 or 同时出现？）
        int start = 0;
        int selectedColor = Random.Range(start, round * 500);

        if (selectedColor >= 500)
        {
            round = 2;
        }
        else if (selectedColor >= 200)
        {
            round = 1;
        }
        else
        {
            round = 0;
        }


        switch (round)
        {

            case 0:
                {
                    disk.GetComponent<DiskData>().color = Color.yellow;
                    disk.GetComponent<DiskData>().speed = Random.Range(10f, 12f);
                    float startX = Random.Range(-1f, 1f) < 0 ? -1 : 1;
                    float startY = Random.Range(0.5f, 0.8f);
                    disk.GetComponent<DiskData>().direction = new Vector3(startX, startY, 0);
                    disk.GetComponent<DiskData>().score = 1;
                    disk.GetComponent<Renderer>().material.color = Color.yellow;
                    break;
                }
            case 1:
                {
                    disk.GetComponent<DiskData>().color = Color.red;
                    disk.GetComponent<DiskData>().speed = Random.Range(15f, 18f);
                    float startX = Random.Range(-1f, 1f) < 0 ? -1 : 1;
                    float startY = Random.Range(0.3f, 0.5f);
                    disk.GetComponent<DiskData>().direction = new Vector3(startX, startY, 0);
                    disk.GetComponent<DiskData>().score = 2;
                    disk.GetComponent<Renderer>().material.color = Color.red;
                    break;
                }
            case 2:
                {
                    disk.GetComponent<DiskData>().color = Color.blue;
                    disk.GetComponent<DiskData>().speed = Random.Range(10f, 15f);
                    float startX = Random.Range(-1f, 1f) < 0 ? -1 : 1;
                    float startY = Random.Range(0.5f, 0.6f);
                    disk.GetComponent<DiskData>().direction = new Vector3(startX, startY, 0);
                    disk.GetComponent<DiskData>().score = 3;
                    disk.GetComponent<Renderer>().material.color = Color.blue;
                    break;
                }
        }

        used.Add(disk.GetComponent<DiskData>());
        return disk;
    }

    //回收飞碟
    public void FreeDisk(GameObject disk)
    {
        for (int i = 0; i < used.Count; i++)
        {
            if (disk.GetInstanceID() == used[i].gameObject.GetInstanceID())
            {
                used[i].gameObject.SetActive(false);
                free.Add(used[i]);
                used.Remove(used[i]);
                break;
            }
        }
    }
}
