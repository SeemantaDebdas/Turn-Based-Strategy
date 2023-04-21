using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GrenadeAction : BaseAction
{

    [SerializeField] GameObject grenadePrefab = null;
    [SerializeField] int maxThrowRange = 7;

    public override string GetActionName()
    {
        return "Grenade";
    }

    private void Update()
    {
        if (!isActive) return;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxThrowRange; x <= maxThrowRange; x++)
        {
            for (int z = -maxThrowRange; z <= maxThrowRange; z++)
            {
                GridPosition offsetGridPosition = new(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    continue;

                float taxiCabDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (taxiCabDistance > maxThrowRange)
                    continue;

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Transform grenadeSpawn = Instantiate(grenadePrefab, unit.GetWorldPosition(), Quaternion.identity).transform;
        Grenade grenade = grenadeSpawn.GetComponent<Grenade>();

        grenade.Setup(gridPosition, OnGrenadeBehaviourComplete); 
        ActionStart(onActionComplete);
    }

    void OnGrenadeBehaviourComplete()
    {
        ActionComplete();
    }
}
