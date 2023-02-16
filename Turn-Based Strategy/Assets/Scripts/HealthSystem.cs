using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] int healthPoints = 100;
    int currentHealthPoints;

    public event EventHandler OnDie;
    public event EventHandler OnDamaged;

    private void Awake()
    {
        currentHealthPoints = healthPoints;
    }

    public void Damage(int damagePoints)
    {
        currentHealthPoints -= damagePoints;
        OnDamaged?.Invoke(this, EventArgs.Empty);

        if (currentHealthPoints <= 0)
        {
            OnDie?.Invoke(this, EventArgs.Empty);
        }
    }

    public float GetNormalizedHealthPoints()
    {
        return (float)currentHealthPoints / healthPoints;
    }
}
