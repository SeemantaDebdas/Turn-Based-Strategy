using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    [SerializeField] LayerMask unitLayerMask;
    
    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;  
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;


    public static UnitActionSystem Instance { get; private set; }

    [SerializeField] Unit selectedUnit;
    BaseAction selectedAction;
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

    private void Start()
    {
        SetSelectedUnit(selectedUnit);
    }

    private void Update()
    {
        if(isBusy) return;

        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (TryUnitSelection()) return;

        HandleSelectedAction();
    }


    void SetBusy()
    {
        isBusy = true;
        OnBusyChanged?.Invoke(this, true);
    }
    void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(this, false);
    }

    private bool TryUnitSelection()
    {
        if (!Input.GetMouseButtonDown(0)) return false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, unitLayerMask))
        {
            if (hit.transform.TryGetComponent(out Unit unit))
            {
                if (unit == selectedUnit) return false;

                SetSelectedUnit(unit);
                return true;
            }
        }
        return false;
    }
    private void HandleSelectedAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            if (!selectedAction.IsValidGridPosition(mouseGridPosition)) return;
            if (!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction)) return;

            SetBusy();
            selectedAction.TakeAction(mouseGridPosition, ClearBusy);
            OnActionStarted?.Invoke(this, EventArgs.Empty);
        }
    }

    void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        SetSelectedAction(unit.GetMoveAction());
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction action)
    {
        selectedAction = action;
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit() => selectedUnit;

    public BaseAction GetSelectedAction() => selectedAction;
}

