using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    [SerializeField] Transform gridDebugObjectPrefab;
    [SerializeField] int width = 10;
    [SerializeField] int height = 10;
    [SerializeField] int cellSize = 2;
    [SerializeField] int floorAmount = 2;

    List<GridSystem<GridObject>> gridSystemList = new();

    public static LevelGrid Instance { get; private set; }

    public event EventHandler OnAnyUnitMovedGridPosition;

    public readonly int FLOOR_HEIGHT = 3;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Deleting already exisiting instance of Level Grid: {transform}  -  {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        for(int floor = 0; floor < floorAmount; floor++)
        {
            GridSystem<GridObject> gridSystem = new(width, height, floor, FLOOR_HEIGHT, cellSize,
                                                    (GridSystem<GridObject> g, GridPosition p) => new GridObject(g, p)); 

            gridSystemList.Add(gridSystem); 
        }

        ///gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }

    private void Start()
    {
        Pathfinding.Instance.Setup(width, height, cellSize);
    }

    public int GetHeight() => GetGridSystem(0).GetHeight();

    public int GetWidth() => GetGridSystem(0).GetWidth();

    public GridSystem<GridObject> GetGridSystem(int floor)
    {
        return gridSystemList[floor];
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        gridObject.AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit);
        AddUnitAtGridPosition(toGridPosition, unit);

        OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }

    public int GetFloor(Vector3 worldPosition) => Mathf.RoundToInt(worldPosition.y / FLOOR_HEIGHT);

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        int floor = GetFloor(worldPosition);
        return GetGridSystem(floor).GetGridPosition(worldPosition);
    }
    public Vector3 GetWorldPosition(GridPosition gridPosition) => GetGridSystem(gridPosition.floor).GetWorldPosition(gridPosition);
    public bool IsValidGridPosition(GridPosition gridPosition) => GetGridSystem(gridPosition.floor).IsValidGridPosition(gridPosition);
    public bool HasUnitOnGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }
    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }

    public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetInteractable;
    }
    public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        gridObject.SetInteractable(interactable);
    }
}
