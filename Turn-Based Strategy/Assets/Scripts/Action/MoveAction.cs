using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    [SerializeField] int maxMoveDistance = 1;
    [SerializeField] float stoppingDistance = 0.1f;
    [SerializeField] float movementSpeed = 2.87f;
    [SerializeField] float rotateSpeed = 50f;

    Vector3 targetPosition = Vector3.zero;
    string actionName = "Move";

    #region Animation Variables

    const string animAimIdle = "Aim Idle";
    const string animRun = "Run";

    int AimIdleHash = Animator.StringToHash(animAimIdle);
    int RunHash = Animator.StringToHash(animRun);

    float fixedTimeDuration = 0.1f;

    #endregion

    protected override void Awake()
    {
        base.Awake();
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (!isActive) return;

        Vector3 direction = (targetPosition - transform.position).normalized;
        RotateTowardsDirection(direction);

        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            transform.position += movementSpeed * Time.deltaTime * direction;
            PlayAnimation(RunHash, animRun);
        }
        else
        {
            PlayAnimation(AimIdleHash, animAimIdle);
            isActive = false;
            onActionComplete();
        }
    }
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        isActive = true;
        this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        this.onActionComplete = onActionComplete;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for(int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (testGridPosition == unitGridPosition)
                    continue;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    continue;

                if (LevelGrid.Instance.HasUnitOnGridPosition(testGridPosition))
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
    void PlayAnimation(int animHash, string animString)
    {
        AnimatorClipInfo[] clips = anim.GetCurrentAnimatorClipInfo(0);
        if (anim.IsInTransition(0) || clips[0].clip.name == animString) return;

        anim.CrossFadeInFixedTime(animHash, fixedTimeDuration);
    }

    public override string GetActionName() => actionName;
}
