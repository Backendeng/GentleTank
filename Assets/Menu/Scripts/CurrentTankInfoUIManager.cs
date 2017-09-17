﻿using TMPro;
using UnityEngine;

public class CurrentTankInfoUIManager : MonoBehaviour 
{
    static public CurrentTankInfoUIManager Instance { get; private set; }

    public TextMeshProUGUI weightText;
    public string weightPrefix = "重量：";
    public Color positiveColor = Color.green;
    public Color negativeColor = Color.red;

    private float temValue;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AllCustomTankPreviewManager.Instance.allTankSetupHandle += ShowInfoOnAllTankSetup;
    }

    /// <summary>
    /// 在所有坦克配置完后更新当前坦克重量
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ShowInfoOnAllTankSetup(object sender, System.EventArgs e)
    {
        UpdateCurrentTankInfo();
    }

    /// <summary>
    /// 更新当前坦克组合信息
    /// </summary>
    public void UpdateCurrentTankInfo()
    {
        if (AllCustomTankManager.Instance.CurrentTankAssemble == null)
            return;
        UpdateCurrentTankWeight();
    }

    /// <summary>
    /// 更新当前坦克组合的总重量
    /// </summary>
    private void UpdateCurrentTankWeight()
    {
        if (AllCustomTankManager.Instance.CurrentTankAssemble == null)
            return;
        weightText.text = weightPrefix + AllCustomTankManager.Instance.CurrentTankAssemble.GetTotalWeight();
    }

    /// <summary>
    /// 更新临时坦克信息
    /// </summary>
    public void UpdateTemporaryTankInfo()
    {
        if (AllCustomTankManager.Instance.TemporaryAssemble == null)
            return;
        UpdateTemporaryTankWeight();
    }

    /// <summary>
    /// 更新临时坦克组合总重量
    /// </summary>
    private void UpdateTemporaryTankWeight()
    {
        if (AllCustomTankManager.Instance.TemporaryAssemble == null)
            return;
        temValue = AllCustomTankManager.Instance.GetTemAndCurrentWeightDifference();
        weightText.text = string.Format("{0}{1} ({2})",weightPrefix, AllCustomTankManager.Instance.TemporaryAssemble.GetTotalWeight(),ColorTool.GetColorString(temValue > 0f ? negativeColor:positiveColor,temValue.ToString()));
    }
}
