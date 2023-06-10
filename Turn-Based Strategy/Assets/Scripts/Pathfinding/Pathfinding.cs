using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] Transform debugObject;
    [SerializeField] LayerMask obstacleLayer;

    int width;
    int height;
    int cellSize;
    GridSystem<PathNode> gridSystem;

    const int MOVE_STRAIGHT_COST = 10;
    const int MOVE_DIAGONAL_COST = 14;

    public static Pathfinding Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Deleting already exisiting instance of Pathfinding: {transform}  -  {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;


    }

    public void Setup(int width, int height, int cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridSystem = new GridSystem<PathNode>(width, height, cellSize,
                     (GridSystem<PathNode> g, GridPosition p) => new PathNode(p));
        //gridSystem.CreateDebugObjects(debugObject);

        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                GridPosition raycastGridPosition = new(x, z);
                Vector3 raycastWorldPosition = LevelGrid.Instance.GetWorldPosition(raycastGridPosition);

                float rasycastOffset = 5f;
                if(Physics.Raycast(raycastWorldPosition + Vector3.down * rasycastOffset, Vector3.up, rasycastOffset * 2, obstacleLayer))
                {
                    GetNode(x,z).SetIsWalkable(false);
                }
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new();
        List<PathNode> closedList = new();

        PathNode startNode = gridSystem.GetGridObject(startGridPosition);
        openList.Add(startNode);

        PathNode endNode = gridSystem.GetGridObject(endGridPosition);

        for(int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for(int z = 0; z < gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new(x, z);
                PathNode pathNode = gridSystem.GetGridObject(gridPosition);

                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if(currentNode == endNode)
            {
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode))
                    continue;

                if (!neighbourNode.IsWalkable())
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + 
                                     CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

                if(tentativeGCost < neighbourNode.GetGCost())
                {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.SetGCost(tentativeGCost);
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        Debug.Log($"{this} reached null point");

        pathLength = 0;
        return null;
    }


    private int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        GridPosition gridPositionDifference = gridPositionA - gridPositionB;
        int xDistance = Mathf.Abs(gridPositionDifference.x);
        int zDistance = Mathf.Abs(gridPositionDifference.z);

        int remaining = Mathf.Abs(xDistance - zDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }
    
    private PathNode GetLowestFCostPathNode(List<PathNode> openList)
    {
        PathNode lowestFCostPathNode = openList[0];
        for(int i = 0; i < openList.Count; i++)
        {
            if(openList[i].GetFCost() < lowestFCostPathNode.GetFCost())
                lowestFCostPathNode = openList[i];
        }

        return lowestFCostPathNode;
    }

    PathNode GetNode(int x, int z)
    {
        return gridSystem.GetGridObject(new GridPosition(x,z));
    }

    List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new();

        GridPosition currentNodeGridPosition = currentNode.GetGridPosition();

        if(currentNodeGridPosition.x - 1 >= 0)
        {
            //Left
            neighbourList.Add(GetNode(currentNodeGridPosition.x - 1, currentNodeGridPosition.z + 0));

            //Left Up
            if(currentNodeGridPosition.z + 1 < gridSystem.GetHeight())
                neighbourList.Add(GetNode(currentNodeGridPosition.x - 1, currentNodeGridPosition.z + 1));

            //Left Down
            if(currentNodeGridPosition.z - 1 >= 0)
                neighbourList.Add(GetNode(currentNodeGridPosition.x - 1, currentNodeGridPosition.z - 1));
        }

        if (currentNodeGridPosition.x + 1 < gridSystem.GetWidth())
        {
            //Right
            neighbourList.Add(GetNode(currentNodeGridPosition.x + 1, currentNodeGridPosition.z + 0));

            //Right Up
            if (currentNodeGridPosition.z + 1 < gridSystem.GetHeight())
                neighbourList.Add(GetNode(currentNodeGridPosition.x + 1, currentNodeGridPosition.z + 1));

            //Right Down
            if (currentNodeGridPosition.z - 1 >= 0)
                neighbourList.Add(GetNode(currentNodeGridPosition.x + 1, currentNodeGridPosition.z - 1));
        }

        //Up
        if (currentNodeGridPosition.z + 1 < gridSystem.GetHeight())
            neighbourList.Add(GetNode(currentNodeGridPosition.x + 0, currentNodeGridPosition.z + 1));

        //Down
        if (currentNodeGridPosition.z - 1 >= 0)
            neighbourList.Add(GetNode(currentNodeGridPosition.x + 0, currentNodeGridPosition.z - 1));

        return neighbourList;
    }    

    List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new();
        pathNodeList.Add(endNode);
        PathNode currentNode = endNode;

        while(currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        pathNodeList.Reverse();

        List<GridPosition> pathNodeGridPositionList = new();

        foreach(PathNode pathNode in pathNodeList)
        {
            pathNodeGridPositionList.Add(pathNode.GetGridPosition());
        }

        return pathNodeGridPositionList;
    }

    public bool IsWalkable(GridPosition gridPosition)
    {
        return gridSystem.GetGridObject(gridPosition).IsWalkable();
    }

    public void SetIsWalkable(GridPosition gridPosition, bool isWalkable)
    {
        gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }
}
