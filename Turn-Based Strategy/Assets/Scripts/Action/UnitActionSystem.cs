using System;
using UnityEngine;

public class UnitActionSystem : MonoBehaviour
{
    [SerializeField] LayerMask unitLayerMask;
    
    public event EventHandler OnSelectedUnitChanged;
    public static UnitActionSystem Instance { get; private set; }

    [SerializeField] Unit selectedUnit;
    bool isBusy;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("There's more than one UnitActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if(isBusy) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (TryUnitSelection()) return;

            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (selectedUnit.GetMoveAction().IsValidGridPosition(mouseGridPosition))
            {
                Debug.Log($"{this} requests movement");
                selectedUnit.GetMoveAction().Move(mouseGridPosition, ClearBusy);
                SetBusy();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            selectedUnit.GetSpinAction().Spin(ClearBusy);
            SetBusy();
        }
    }

    void SetBusy() => isBusy = true;
    void ClearBusy() => isBusy = false;

    private bool TryUnitSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, unitLayerMask))
        {
            if(hit.transform.TryGetComponent(out Unit unit))
            {
                SetSelectedUnit(unit);
                return true;
            }
        }
        return false;
    }

    void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;

        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }
    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

}

