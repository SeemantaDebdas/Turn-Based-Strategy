using System;
using UnityEngine;

public class Unit : MonoBehaviour
{ 
    const int ACTION_POINTS_MAX = 2;

    public static event EventHandler OnAnyActionPointsChanged;

    GridPosition gridPosition;

    BaseAction[] baseActionArray;
    MoveAction moveAction;
    SpinAction spinAction;
    int actionPoints = ACTION_POINTS_MAX;

    private void Awake()
    {
        moveAction = GetComponent<MoveAction>();
        spinAction = GetComponent<SpinAction>();
        baseActionArray = GetComponents<BaseAction>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if(gridPosition != newGridPosition)
        {
            //Unit Changed Grid Position
            LevelGrid.Instance.UnitMovedGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }
    }

    public MoveAction GetMoveAction() => moveAction;

    public SpinAction GetSpinAction() => spinAction;

    public GridPosition GetGridPosition() => gridPosition;

    public BaseAction[] GetBaseActionArray() => baseActionArray;

    public int GetActionPoints() => actionPoints;  

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        return false;
    }

    bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        return actionPoints >= baseAction.GetActionPointsCost();
    }

    void SpendActionPoints(int amount)
    {
        actionPoints -= amount;
    }

    private void TurnSystem_OnTurnChanged(object sender, int e)
    {
        actionPoints = ACTION_POINTS_MAX;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }
}
