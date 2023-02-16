using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] Unit unit;
    [SerializeField] TextMeshProUGUI actionPointsUI;

    [Header("Health")]
    [SerializeField] HealthSystem healthSystem;
    [SerializeField] Image healthBar;
    [SerializeField] Color healthGoodColor;
    [SerializeField] Color healthBadColor;

    private void Start()
    {
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;

        UpdateActionPointsUI();
        UpdateHealthBar();
    }

    private void OnDestroy()
    {
        Unit.OnAnyActionPointsChanged -= Unit_OnAnyActionPointsChanged;
        healthSystem.OnDamaged -= HealthSystem_OnDamaged;
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsUI();
    }

    void UpdateActionPointsUI()
    {
        actionPointsUI.text = unit.GetActionPoints().ToString();
    }

    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        float normalizedHealthPoints = healthSystem.GetNormalizedHealthPoints();
        healthBar.fillAmount = normalizedHealthPoints;
        healthBar.color = normalizedHealthPoints > 0.3 ? healthGoodColor : healthBadColor;
    }
}
