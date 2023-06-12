using System;
using UnityEngine;

public class GridSystem<TGridObject>
{
    int width = 0;
    int height = 0;
    int floor = 0;
    int floorHeight = 0;
    
    float cellSize = 0;

    TGridObject[,] gridObjectArray;

    public GridSystem(int width, int height, int floor,int floorHeight, float cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.floor = floor;
        this.floorHeight = floorHeight;

        gridObjectArray = new TGridObject[width, height];
        
        for(int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new(x, z, floor);
                gridObjectArray[x,z] = createGridObject(this, gridPosition);
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize + 
                new Vector3(0, gridPosition.floor, 0) * floorHeight;
    }
    
    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize),
            floor
        );
    }

    public void CreateDebugObjects(Transform debugPrefab)
    {
        Transform debugObjectParent = new GameObject("DebugObjectParent").transform;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z, floor);

                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                debugTransform.SetParent(debugObjectParent);
                debugTransform.name = $"{x},{z}";
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));

            }
        }
    }

    public TGridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.x, gridPosition.z];
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0
            && gridPosition.z >= 0
            && gridPosition.x < width
            && gridPosition.z < height
            && gridPosition.floor == floor;
    }

    public int GetHeight() => height;

    public int GetWidth() => width; 
}
