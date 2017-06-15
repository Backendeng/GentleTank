using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int numRoundsToWin = 5;                  // 赢得游戏需要赢的回合数
    public float startDelay = 3f;                   // 开始延时时间
    public float endDelay = 3f;                     // 结束延时时间
    public CameraControl cameraControl;             // 相机控制脚本
    public MinimapCameraController minimapCamera;   // 跟踪相机，用于小地图
    public Text messageText;                        // UI文本（玩家获胜等）
    public PointList spawnPointList;                // 坦克出生点
    public PointList wayPointList;                  // AI的巡逻点列表
    public TankArray tankArray;                     // 坦克管理器数组

    private int roundNumber;                        // 当前回合数
    private WaitForSeconds startWait;               // 开始回合延时
    private WaitForSeconds endWait;                 // 结束回合延时
    private TankManager roundWinner;                // 当前回合获胜玩家
    private TankManager gameWinner;                 // 最终获胜玩家

    private void Awake()
    {
        startWait = new WaitForSeconds(startDelay);
        endWait = new WaitForSeconds(endDelay);
    }

    private void Start()
    {
        spawnPointList.EnableAllPoints();
        SpawnAllTanks();
        SetCameraTargets();

        minimapCamera.SetTarget(tankArray[0].Instance);     //设置小地图跟随目标为第一个玩家

        // 开始游戏循环（检测获胜者，重新回合，结束游戏等）
        StartCoroutine(GameLoop());
    }

    // 产生所有坦克，包括玩家和AI
    private void SpawnAllTanks()
    {
        for (int i = 0; i < tankArray.Length; i++)
        {
            //获取有效随机出生点，且每个坦克位置不一样
            Point spawnPoint = spawnPointList.GetRandomPoint(false);
            if (spawnPoint == null)
                continue;

            tankArray[i].InitTank(Instantiate(tankArray[i].tankPerfab, spawnPoint.position, Quaternion.Euler(spawnPoint.rotation)) as GameObject, i + 1, 0, spawnPoint, wayPointList);
            tankArray[i].SetupTank();
        }
    }

    // 给相机添加所有坦克
    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[tankArray.Length];

        for (int i = 0; i < targets.Length; i++)
            targets[i] = tankArray[i].Instance.transform;

        cameraControl.targets = targets;
    }

    // 游戏的协程
    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());           //回合开始，有一段延时

        yield return StartCoroutine(RoundPlaying());            //回合中，多于一个坦克存活时一直在这里死循环

        yield return StartCoroutine(RoundEnding());             //回合结束

        // 如果结束了游戏，重新加载场景，否则进行下一回合
        if (gameWinner != null)
            SceneManager.LoadScene(0);
        else
            StartCoroutine(GameLoop());
    }

    // 回合开始
    private IEnumerator RoundStarting()
    {
        ResetAllTanks();                                // 重置所有坦克
        DisableTankControl();                           // 并且锁定他们的控制权

        cameraControl.SetStartPositionAndSize();        // 重置相机

        ++roundNumber;                                  // 回合数增加                
        messageText.text = "ROUND " + roundNumber;

        yield return startWait;                         // 延时一段时间再开始
    }

    // 回合中
    private IEnumerator RoundPlaying()
    {
        EnableTankControl();                            // 解锁玩家控制权

        messageText.text = string.Empty;                // 清空显示信息

        while (!OneTankLeft())                          // 只剩一个坦克才结束该协程
            yield return null;
    }

    // 回合结束
    private IEnumerator RoundEnding()
    {
        DisableTankControl();                           // 锁定玩家控制权

        roundWinner = GetRoundWinner();                 // 获取回合胜利的玩家

        if (roundWinner != null)                        // 不为空就给胜出玩家加获胜次数
            roundWinner.Win();

        gameWinner = GetGameWinner();                   // 获取最终获胜玩家

        string message = EndMessage();                  // 获取结束信息并显示之
        messageText.text = message;

        yield return endWait;
    }

    // 返回是否小于等于一个坦克存活（0个说明是同归了）
    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < tankArray.Length; i++)
            if (tankArray[i].Instance.activeSelf)
                numTanksLeft++;

        return numTanksLeft <= 1;
    }

    // 获取获胜的玩家，为空就是平局
    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < tankArray.Length; i++)
            if (tankArray[i].Instance.activeSelf)
                return tankArray[i];

        return null;
    }

    // 获取最终胜利的玩家
    private TankManager GetGameWinner()
    {
        for (int i = 0; i < tankArray.Length; i++)
            if (tankArray[i].WinTimes == numRoundsToWin)
                return tankArray[i];

        return null;
    }

    // 获取回合或总的游戏结束信息
    private string EndMessage()
    {
        string message = "DRAW!";                       // 默认平局

        // 添加获胜玩家的带颜色的玩家名字字符串
        if (roundWinner != null)
            message = roundWinner.PlayerName + " WINS THE ROUND!";

        message += "\n\n";

        // 添加所有玩家获胜次数
        for (int i = 0; i < tankArray.Length; i++)
            message += tankArray[i].PlayerName + " : " + tankArray[i].WinTimes + " WINS\n";

        // 添加最后获胜玩家
        if (gameWinner != null)
            message = gameWinner.PlayerName + " WINS THE GAME!";

        return message;
    }

    // 重置所有坦克
    private void ResetAllTanks()
    {
        spawnPointList.EnableAllPoints();                     // 初始化出生点
        for (int i = 0; i < tankArray.Length; i++)
        {
            //获取有效随机出生点，且每个坦克位置不一样
            Point spawnPoint = spawnPointList.GetRandomPoint(false, true);
            if (spawnPoint == null)
                continue;
            tankArray[i].Reset(spawnPoint);
        }
    }

    // 锁定所有玩家控制权
    private void EnableTankControl()
    {
        for (int i = 0; i < tankArray.Length; i++)
            tankArray[i].SetControlEnable(true);
    }

    // 解锁所有玩家控制权
    private void DisableTankControl()
    {
        for (int i = 0; i < tankArray.Length; i++)
            tankArray[i].SetControlEnable(false);
    }

    // 画出所有点
    private void OnDrawGizmos()
    {
        spawnPointList.DebugDrawPoint();
        wayPointList.DebugDrawPoint();
    }
}
