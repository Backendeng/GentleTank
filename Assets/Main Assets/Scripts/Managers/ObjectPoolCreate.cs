﻿using UnityEngine;

//对象池的集中初始化创建
public class ObjectPoolCreate : MonoBehaviour 
{
    public ObjectPool[] objectPools;

    private void Awake()
    {
        for (int i = 0; i < objectPools.Length; i++)
            if (objectPools[i] != null)
                objectPools[i].CreateObjectPool();
    }
}