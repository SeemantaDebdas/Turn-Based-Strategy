using System;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    Unit unit;
    MeshRenderer meshRenderer;

    private void Awake()
    {
        unit = GetComponentInParent<Unit>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UpdateVisual();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs empty)
    {
        UpdateVisual();
    }
    
    void UpdateVisual()
    {
        Unit unit = UnitActionSystem.Instance.GetSelectedUnit();
        meshRenderer.enabled = (this.unit == unit);
    }
}
