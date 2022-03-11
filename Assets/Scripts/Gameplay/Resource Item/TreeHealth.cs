using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using DG.Tweening;

public class TreeHealth : MonoBehaviour
{
    public CollectableType collectableType;
    public FloatVariable maxHealth;
    public float currentHealth;

    public IntVariable resourceAmount;

    [Header("Tree")]
    public GameObject _mesh;

    void Start()
    {
        Init();

        RegenerateStart();
    }

    public void Init()
    {
        currentHealth = maxHealth.Value;
    }

    public UnityEvent OnDamage;
    public UnityEvent OnDeath;
    public UnityEvent OnRespawn;

    // okay
    public void GetDamage(float _damageAmount)
    {
        OnDamage.Invoke();

        currentHealth -= _damageAmount;

        if (currentHealth <= 0)
            Dead();
    }

    public void Dead()
    {
        // not okay
        OnDeath.Invoke();

        GatherResource(resourceAmount.Value);

        _mesh.SetActive(false);
    }

    // okay
    public void GatherResource(int _amount)
    {
        ArcadeManager.Instance.IncreaseResource(_amount, collectableType);
    }

    // Regenerate

    // okay
    public void Respawn()
    {
        currentHealth = maxHealth.Value;

        _mesh.SetActive(true);
    }

    [Header("Regenerate")]
    public FloatVariable regenerateTime;

    public UnityEvent RegenerateSuccessful;

    private WaitForSeconds _waitForSeconds;

    private void RegenerateStart()
    {
        _waitForSeconds = new WaitForSeconds(regenerateTime.Value);
    }

    public void StartRegeneration()
    {
        Invoke("WaitInvoke", regenerateTime.Value);
    }

    void WaitInvoke()
    {
        RegenerateSuccessful.Invoke();
    }
}
