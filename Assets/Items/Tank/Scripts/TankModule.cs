﻿using System.Collections.Generic;
using UnityEngine;

public enum TankModuleType
{
    None, Head, Body, WheelLeft, WheelRight, Other, Skin
}

[System.Serializable]
public class TankModuleConnectAnchor
{
    public TankModuleType type;
    public Vector3 anchor;
}

[CreateAssetMenu(menuName = "TankModule/Default")]
public class TankModule : ScriptableObject
{
    public GameObject prefab;
    public Sprite preview;
    public TankModuleType type;
    public Vector3 center;
    public Vector3 forward;
    public Vector3 back;
    public Vector3 left;
    public Vector3 right;
    public Vector3 up;
    public Vector3 down;

    public List<TankModuleConnectAnchor> connectAnchor = new List<TankModuleConnectAnchor>();

    public TankModuleConnectAnchor this[TankModuleType type]
    {
        get
        {
            for (int i = 0; i < connectAnchor.Count; i++)
                if (connectAnchor[i].type == type)
                    return connectAnchor[i];
            return null;
        }
    }

    /// <summary>
    /// 连接头到身体
    /// </summary>
    /// <param name="head">头部件</param>
    /// <param name="headObj">头部件对象</param>
    /// <param name="body">身体部件</param>
    /// <param name="bodyObj">身体部件对象</param>
    static public void ConnectHeadToBody(TankModuleHead head,GameObject headObj,TankModuleBody body,GameObject bodyObj)
    {
        headObj.transform.position = bodyObj.transform.position + body.up - head.down;
    }

    /// <summary>
    /// 连接左轮胎到身体
    /// </summary>
    /// <param name="leftWheel">左轮胎</param>
    /// <param name="leftWheelObj">左轮胎对象</param>
    /// <param name="body">身体</param>
    /// <param name="bodyObj">身体对象</param>
    static public void ConnectLeftWheelToBody(TankModuleWheel leftWheel, GameObject leftWheelObj, TankModuleBody body, GameObject bodyObj)
    {
        leftWheelObj.transform.position = bodyObj.transform.position + body.leftWheelTop - leftWheel.up;
    }

    /// <summary>
    /// 连接右轮胎到身体
    /// </summary>
    /// <param name="rightWheel">右轮胎</param>
    /// <param name="rightObj">右轮胎对象</param>
    /// <param name="body">身体</param>
    /// <param name="bodyObj">身体对象</param>
    static public void ConnectRightWheelToBody(TankModuleWheel rightWheel, GameObject rightObj, TankModuleBody body, GameObject bodyObj)
    {
        rightObj.transform.position = bodyObj.transform.position + body.rightWheelTop - rightWheel.up;
    }
}
