using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    [SerializeField] int maxMoveDistance = 1;
    [SerializeField] float stoppingDistance = 0.1f;
    [SerializeField] float movementSpeed = 2.87f;
    [SerializeField] float rotateSpeed = 50f;

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;
    public event Action<GridPosition, GridPosition> OnStartChangingFloors;

    List<Vector3> targetPositionList;
    int currentPositionIdx = 0;
    string actionName = "Move";

    bool isChangingFloors = false;
    float differentFloorTeleportTimer, differentFloorTeleportTimerMax = 0.5f;
    

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (!isActive) return;

        Vector3 targetPosition = targetPositionList[currentPositionIdx];


        if (isChangingFloors)
        {
            //stop and teleport
            Vector3 targetPositionOnSameFloor = targetPosition;
            targetPositionOnSameFloor.y = transform.position.y; 

            Vector3 direction = targetPositionOnSameFloor - transform.position;

            RotateTowardsDirection(direction);
            differentFloorTeleportTimer -= Time.deltaTime;

            if(differentFloorTeleportTimer < 0)
            {
                isChangingFloors = false;
                transform.position = targetPosition;
            }
        }
        else
        {
            //regular move

            Vector3 direction = (targetPosition - transform.position).normalized;
            RotateTowardsDirection(direction);
            transform.position += movementSpeed * Time.deltaTime * direction;
        }

        if (Vector3.Distance(transform.position, targetPosition) < stoppingDistance)
        {
            currentPositionIdx++;
            if(currentPositionIdx >= targetPositionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
            else
            {
                targetPosition = targetPositionList[currentPositionIdx];

                GridPosition unitGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
                GridPosition targetGridPosition = LevelGrid.Instance.GetGridPosition(targetPosition);
                
                if(unitGridPosition.floor != targetGridPosition.floor)
                {
                    OnStartChangingFloors?.Invoke(unitGridPosition, targetGridPosition);
                    isChangingFloors = true;
                    differentFloorTeleportTimer = differentFloorTeleportTimerMax; 
                }
            }
        }
    }
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        currentPositionIdx = 0;
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

        targetPositionList = new();

        foreach(GridPosition pathGridPosition in pathGridPositionList)
        {
            targetPositionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        OnStartMoving?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for(int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {

                for(int floor = -maxMoveDistance; floor <= maxMoveDistance; floor++)
                {
                    GridPosition offsetGridPosition = new(x, z, floor);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                    if (testGridPosition == unitGridPosition)
                        continue;

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                        continue;

                    if (LevelGrid.Instance.HasUnitOnGridPosition(testGridPosition))
                        continue;

                    if (!Pathfinding.Instance.IsWalkable(testGridPosition))
                        continue;

                    if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                        continue;

                    int pathfindingDistanceMulitplier = 10;
                    if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathfindingDistanceMulitplier)
                        continue;

                    validGridPositionList.Add(testGridPosition);
                }
            }
        }
        return validGridPositionList;
    }

    private void RotateTowardsDirection(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    public override string GetActionName() => actionName;

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10,
        };
    }
}
