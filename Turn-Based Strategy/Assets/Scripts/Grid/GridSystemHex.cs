using System;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemHex<TGridObject>
{
    int width = 0;
    int height = 0;
    float cellSize = 0;

    TGridObject[,] gridObjectArray;

    const float HEX_VERTICAL_OFFSET = 0.75f;
    const float HEX_HORIZONTAL_OFFSET = 0.5f;

    public GridSystemHex(int width, int height, float cellSize, Func<GridSystemHex<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridObjectArray = new TGridObject[width, height];
        
        for(int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new(x, z);
                gridObjectArray[x,z] = createGridObject(this, gridPosition);
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        Vector3 worldPosition = new Vector3(gridPosition.x, 0, 0) * cellSize;

        worldPosition += new Vector3(0, 0, gridPosition.z) * cellSize * HEX_VERTICAL_OFFSET;    

        if (gridPosition.z % 2 != 0)
        {
            worldPosition += HEX_HORIZONTAL_OFFSET * cellSize * Vector3.right;
        }

        return worldPosition;
    }
    
    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        GridPosition roughXZ =  new(
            Mathf.RoundToInt(worldPosition.x /cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize / HEX_VERTICAL_OFFSET)
        );

        bool oddRow = roughXZ.z % 2 == 1;

        List<GridPosition> neighbourGridPosList = new()
        {
            roughXZ + new GridPosition(-1, 0),
            roughXZ + new GridPosition(+1, 0),

            roughXZ + new GridPosition(0, +1),
            roughXZ + new GridPosition(0, -1),

            roughXZ + new GridPosition(oddRow ? +1:-1, +1),
            roughXZ + new GridPosition(oddRow ? +1:-1, -1),
        };

        GridPosition closestGridPosition = roughXZ;

        for(int i = 0; i < neighbourGridPosList.Count; i++)
        {
            if(Vector3.Distance(worldPosition, GetWorldPosition(neighbourGridPosList[i])) <
                Vector3.Distance(worldPosition, GetWorldPosition(closestGridPosition)))
            {
                closestGridPosition = neighbourGridPosList[i];
            }
        }

        return closestGridPosition; 
    }

    public void CreateDebugObjects(Transform debugPrefab)
    {
        Transform debugObjectParent = new GameObject("DebugObjectParent").transform;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);

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
            && gridPosition.z < height;
    }

    public int GetHeight() => height;

    public int GetWidth() => width; 
}
