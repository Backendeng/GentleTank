﻿
using System.Collections;
using UnityEngine;

public class MedicalKitProp : PropBase
{
    public float healAmount = 150f;
    public new Collider collider;
    public MeshRenderer meshRenderer;
    public Effect healEffect;

    protected HealthManager targetHealth;

    protected void OnEnable()
    {
        collider.enabled = true;
        meshRenderer.gameObject.SetActive(true);
        healEffect.gameObject.SetActive(false);
    }

    protected override bool OnPlayerTouch(PlayerManager player)
    {
        targetHealth = player.GetComponent<HealthManager>();
        if (targetHealth == null)
            return false;
        targetHealth.SetHealthAmount(healAmount);
        StartCoroutine(InactiveAndShowEffect());
        return true;
    }

    private IEnumerator InactiveAndShowEffect()
    {
        if (healEffect != null)
        {
            collider.enabled = false;
            meshRenderer.gameObject.SetActive(false);
            healEffect.gameObject.SetActive(true);
            while (healEffect.isActiveAndEnabled)
                yield return null;
        }
        gameObject.SetActive(false);
    }

}
