﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShellRainSkill : Skill
{
    public ObjectPool shellPool;            //炮弹池
    [Range(0, 10f)]
    public float skillDelay = 1.5f;         //技能释放延迟
    [Range(1, 100)]
    public int skillLevel = 1;              //技能等级
    [Range(1, 100f)]
    public float attackRadius = 5f;         //技能攻击范围半径
    [Range(0, 1f)]
    public float attackRate = 0.5f;         //技能每次释放频率
    [Range(0, 100f)]
    public float attackDamage = 30f;        //每一粒炮弹最大伤害
    public Image warnningAreaImage;         //警告区域

    private int blinkTimes = 3;             //警告区域闪烁次数
    private float elapsedTime = 0f;         //计时器

    /// <summary>
    /// 技能效果
    /// </summary>
    public override IEnumerator SkillEffect()
    {
        elapsedTime = 0f;
        Vector3 position = aim.HitPosition;
        yield return ShowWarnningArea(position);                         // 显示警告区域，在一段时间后再发起攻击
        for (int i = 0; i < skillLevel; i++)                // 根据技能等级改变攻击波数
            yield return CreateShell(position, Random.insideUnitCircle * attackRadius);
        yield return HideWarnningArea(1f);                  // 一段时间后隐藏警告区域
    }

    /// <summary>
    /// 每隔一段时间创建炮弹
    /// </summary>
    /// <param name="randomCircle">创建的XZ坐标</param>
    /// <returns></returns>
    private IEnumerator CreateShell(Vector3 inputPosition, Vector2 randomCircle)
    {
        //创建炮弹 从上而下
        GameObject shell = shellPool.GetNextObjectActive();
        shell.transform.position = new Vector3(inputPosition.x + randomCircle.x, 20f, inputPosition.z + randomCircle.y);
        shell.transform.rotation = Quaternion.Euler(new Vector3(180f, 0, 0));
        shell.GetComponent<Rigidbody>().velocity = new Vector3(0, -20f, 0);
        shell.GetComponent<Shell>().maxDamage = attackDamage;
        yield return new WaitForSeconds(Random.Range(0, attackRate));
    }

    /// <summary>
    /// 显示警告区域，大小随攻击范围改变
    /// </summary>
    /// <param name="position">区域位置</param>
    private IEnumerator ShowWarnningArea(Vector3 position)
    {
        warnningAreaImage.gameObject.SetActive(true);
        warnningAreaImage.transform.position = new Vector3(position.x, position.y + 0.5f, position.z);
        warnningAreaImage.rectTransform.sizeDelta = new Vector2(attackRadius * 2, attackRadius * 2);

        float onceBlinkTime = skillDelay / blinkTimes;

        for (int i = 0; i < blinkTimes - 1; i++)            //前面闪烁,Alpha是从0->1->0
            while (elapsedTime < (i+1)*onceBlinkTime)
                yield return OnceBlink(elapsedTime  % onceBlinkTime * 2);

        while (elapsedTime < blinkTimes * onceBlinkTime)   //最后一次闪烁,Alpha是从0->1
            yield return OnceBlink(elapsedTime % onceBlinkTime);
    }

    private IEnumerator OnceBlink(float currentTime)
    {
        elapsedTime += Time.deltaTime;
        Debug.Log(currentTime);
        warnningAreaImage.color = new Color(1f, 0f, 0f, Mathf.PingPong(currentTime, 1));
        yield return null;
    }


    /// <summary>
    /// 延时一段时间后，隐藏警告区域
    /// </summary>
    /// <param name="delayTime">延时时间</param>
    /// <returns></returns>
    private IEnumerator HideWarnningArea(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        warnningAreaImage.gameObject.SetActive(false);
    }

}
