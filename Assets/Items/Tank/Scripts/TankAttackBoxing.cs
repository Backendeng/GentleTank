﻿using Item.Ammo;
using System.Collections;
using UnityEngine;

namespace Item.Tank
{
    public class TankAttackBoxing : TankAttack
    {
        public SpringBoxingAmmo springBoxing;   // 弹簧拳组件
        public SpringManager springManager;     // 弹簧控制
        public Collider boxingCollider;         // 拳套
        public AnimationCurve launchDistance = AnimationCurve.Linear(0, 0, 0.4f, 1);    // 发射距离比例
        public AnimationCurve backDistance = AnimationCurve.Linear(0, 1, 0.4f, 0);      // 回来距离比例
        public float launchTotalTime = 0.8f;    // 总共发射来回时间

        private float launchElapsed;            // 发射后经过的时间

        /// <summary>
        /// 攻击实际效果
        /// </summary>
        protected override void OnAttack(params object[] values)
        {
            if (values == null || values.Length == 0)
                Launch(forceSlider.value, damage, coolDownTime);
            else if (values.Length == 3)
                Launch((float)values[0], (float)values[1], (float)values[2]);
        }

        /// <summary>
        /// 发射炮弹，自定义参数变量
        /// </summary>
        /// <param name="launchForce">发射力度</param>
        /// <param name="fireDamage">伤害值</param>
        /// <param name="coolDownTime">发射后冷却时间</param>
        private void Launch(float launchForce, float fireDamage, float coolDownTime)
        {
            StartCoroutine(LaunchBoxingGlove());

            cdTimer.Reset(coolDownTime);
        }

        /// <summary>
        /// 发射弹簧拳协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator LaunchBoxingGlove()
        {
            launchElapsed = 0f;
            springManager.maxDistance = forceSlider.value / 10f;
            boxingCollider.enabled = true;

            while (launchElapsed < launchTotalTime)
            {
                launchElapsed += Time.deltaTime;

                if (launchElapsed < launchTotalTime / 2f)
                    springManager.fillAmount = launchDistance.Evaluate(launchElapsed);
                else
                {
                    springManager.fillAmount = backDistance.Evaluate(launchElapsed - (launchTotalTime / 2f));
                    boxingCollider.enabled = false;
                }
                yield return null;
            }

            springManager.fillAmount = 0f;
            boxingCollider.enabled = false;
        }
    }
}