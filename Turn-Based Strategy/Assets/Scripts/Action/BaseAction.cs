using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    protected Animator anim;
    protected Unit unit;
    protected bool isActive;
    protected Action onActionComplete;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        unit = GetComponent<Unit>();
    }

    public abstract string GetActionName();

    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);
    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositions = GetValidActionGridPositionList();

        Debug.Log(validGridPositions.Contains(gridPosition));
        return validGridPositions.Contains(gridPosition);
    }
    public abstract List<GridPosition> GetValidActionGridPositionList();

    public virtual int GetActionPointsCost() => 1;   
}
