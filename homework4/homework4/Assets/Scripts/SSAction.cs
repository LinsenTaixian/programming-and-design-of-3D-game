using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAction : ScriptableObject            //动作
{

    public bool enable = true;                      //是否正在进行此动作
    public bool destroy = false;                    //是否需要被销毁

    public GameObject gameobject;                   //动作对象
    public Transform transform;                     //动作对象的transform
    public ISSActionCallback callback;              //回调函数

    protected SSAction() { }                        //保证SSAction不会被new

    public virtual void Start()                    //子类可以使用这两个函数
    {
        throw new System.NotImplementedException();
    }

    public virtual void Update()
    {
        throw new System.NotImplementedException();
    }

    public virtual void FixedUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void Reset()
    {
        enable = false;
        destroy = false;
        gameobject = null;
        transform = null;
        callback = null;
    }
}