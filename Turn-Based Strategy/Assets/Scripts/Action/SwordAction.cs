using Cinemachine.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{

    [SerializeField] int maxSwordRange = 1;
    [SerializeField] int damageAmount = 100;
    [SerializeField] float rotateSpeed = 360f;

    public static event Action OnSwordHit;
    
    public event Action OnSwordActionStarted;
    public event Action OnSwordActionCompleted;


    enum SwordActionState
    {
        SwingingSwordBeforeHit,
        SwingingSwordAfterHit
    }
    SwordActionState state;

    Unit targetUnit = null;

    float stateTimer = 0;
    const float BEFORE_HIT_TIME = 0.7f;
    const float AFTER_HIT_TIME = 0.5f;

    public override string GetActionName()
    {
        return "Sword";
    }

    private void Update()
    {
        if (!isActive) return;

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case SwordActionState.SwingingSwordBeforeHit:

                Vector3 direction = (targetUnit.GetWorldPosition() - transform.position).normalized;
                RotateTowardsDirection(direction);

                break;
            case SwordActionState.SwingingSwordAfterHit:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    void NextState()
    {
        switch (state)
        {
            case SwordActionState.SwingingSwordBeforeHit:
                state = SwordActionState.SwingingSwordAfterHit;
                stateTimer = AFTER_HIT_TIME;
                targetUnit.Damage(damageAmount);
                OnSwordHit?.Invoke();
                break;
            case SwordActionState.SwingingSwordAfterHit:
                ActionComplete();
                OnSwordActionCompleted?.Invoke();
                break;
        }
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 200,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxSwordRange; x <= maxSwordRange; x++)
        {
            for (int z = -maxSwordRange; z <= maxSwordRange; z++)
            {
                GridPosition offsetGridPosition = new(x, z, 0);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    continue;

                if (!LevelGrid.Instance.HasUnitOnGridPosition(testGridPosition))
                    continue;

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy())
                    continue;

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    private void RotateTowardsDirection(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = SwordActionState.SwingingSwordBeforeHit;
        stateTimer = BEFORE_HIT_TIME;

        OnSwordActionStarted?.Invoke();

        ActionStart(onActionComplete);
    }

    public int GetSwordRange()
    {
        return maxSwordRange;
    }
}
