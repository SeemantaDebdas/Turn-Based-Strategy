using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    float totalSpinAmount = 0;
    string actionName = "Spin";

    void Update()
    {
        if (!isActive) return;

        if(totalSpinAmount > 360)
        {
            isActive = false;
            onActionComplete();
        }
        
        if (isActive)
        {
            float spinAmount = 360 * Time.deltaTime;
            transform.eulerAngles += Vector3.up * spinAmount;
            totalSpinAmount += spinAmount;
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
        totalSpinAmount = 0;
        isActive = true;
    }

    public override string GetActionName() => actionName;

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return new List<GridPosition> { unitGridPosition };
    }

    public override int GetActionPointsCost()
    {
        return 2;
    }
}
