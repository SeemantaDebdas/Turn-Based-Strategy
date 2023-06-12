using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShootAction : BaseAction
{
    enum ShootActionState
    {
        Aiming,
        Shooting,
        CoolOff
    }

    [SerializeField] int maxShootRange = 7;
    [SerializeField] float rotateSpeed = 360f;
    [SerializeField] int damagePoints = 40;
    [SerializeField] LayerMask obstaclesLayerMask;

    public static event EventHandler<Unit> OnAnyShoot;
    public event EventHandler<Unit> OnShoot;

    string actionName = "Shoot";

    ShootActionState state;
    float stateTimer;
    const float AIMING_TIME = 1f;
    const float SHOOTING_TIME = 0.1f;
    const float COOLOFF_TIME = 0.5f;
    bool canShoot = true;

    Unit targetUnit;

    void Update()
    {
        if (!isActive) return;

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case ShootActionState.Aiming:
                Vector3 direction = (targetUnit.GetWorldPosition() - transform.position).normalized;
                RotateTowardsDirection(direction);

                break;
            case ShootActionState.Shooting:
                if (canShoot)
                {
                    Shoot();
                    canShoot = false;
                }
                break;
            case ShootActionState.CoolOff:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    void Shoot()
    {
        OnShoot?.Invoke(this, targetUnit);
        OnAnyShoot?.Invoke(this, targetUnit);

        targetUnit.Damage(damagePoints);
    }

    private void RotateTowardsDirection(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    void NextState()
    {
        switch (state)
        {
            case ShootActionState.Aiming:
                state = ShootActionState.Shooting;
                stateTimer = SHOOTING_TIME;
                break;
            case ShootActionState.Shooting:
                state = ShootActionState.CoolOff;
                stateTimer = COOLOFF_TIME;
                break;
            case ShootActionState.CoolOff:
                ActionComplete();
                break;
        }
    }

    public override string GetActionName()
    {
        return actionName;
    }
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }

    public  List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new();

        for (int x = -maxShootRange; x <= maxShootRange; x++)
        {
            for (int z = -maxShootRange; z <= maxShootRange; z++)
            {
                GridPosition offsetGridPosition = new(x, z, 0);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    continue;

                if (!LevelGrid.Instance.HasUnitOnGridPosition(testGridPosition))
                    continue;

                Unit targetUnit =  LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy())
                    continue;

                float taxiCabDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (taxiCabDistance > maxShootRange)
                    continue;

                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                float unitShoulderHeight = 1.7f;
                Vector3 shootDirection = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;

                if(Physics.Raycast(unitWorldPosition + Vector3.up * unitShoulderHeight, shootDirection,
                                   Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                                   obstaclesLayerMask))
                { continue;}

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = ShootActionState.Aiming;
        stateTimer = AIMING_TIME;

        canShoot = true;
        ActionStart(onActionComplete);
    }

    public Unit GetTargetUnit() => targetUnit;

    public int GetShootRange() => maxShootRange;

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100),
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }
}
