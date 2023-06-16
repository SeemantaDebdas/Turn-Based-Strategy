using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] Transform debugObject;
    [SerializeField] LayerMask obstacleLayer, floorLayer;
    [SerializeField] Transform pathfindingLinkContainer;

    int width;
    int height;
    int cellSize;
    int floorAmount;

    List<GridSystem<PathNode>> gridSystemList;
    List<PathfindingLink> pathfindingLinkList = new();

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

    public void Setup(int width, int height, int cellSize, int floorAmount)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.floorAmount = floorAmount;

        gridSystemList = new();

        for(int floor = 0; floor < floorAmount; floor++)
        {
            GridSystem<PathNode> gridSystem = new GridSystem<PathNode>(width, height, floor,LevelGrid.Instance.FLOOR_HEIGHT, cellSize,
                         (GridSystem<PathNode> g, GridPosition p) => new PathNode(p));
            //gridSystem.CreateDebugObjects(debugObject);

            gridSystemList.Add(gridSystem);
        }


        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {

                for (int floor = 0; floor < floorAmount; floor++)
                {
                    GridPosition raycastGridPosition = new(x, z, floor);
                    Vector3 raycastWorldPosition = LevelGrid.Instance.GetWorldPosition(raycastGridPosition);

                    float rasycastOffset = 1f;
                    if(Physics.Raycast(raycastWorldPosition + Vector3.down * rasycastOffset, Vector3.up, rasycastOffset * 2, obstacleLayer))
                    {
                        GetNode(x, z, floor).SetIsWalkable(false); 
                    }

                    if(!Physics.Raycast(raycastWorldPosition + Vector3.up * rasycastOffset, Vector3.down, rasycastOffset * 2, floorLayer))
                    {
                        GetNode(x, z, floor).SetIsWalkable(false);
                    }
                }

            }
        }

        foreach(Transform pathfindingLinkTransform in pathfindingLinkContainer)
        {
            if(pathfindingLinkTransform.TryGetComponent(out PathfindingLinkMonoBehaviour pathfindingLinkMonoBehaviour))
            {
                pathfindingLinkList.Add(pathfindingLinkMonoBehaviour.GetPathfindingLink());
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new();
        List<PathNode> closedList = new();

        PathNode startNode = GetGridSystem(startGridPosition.floor).GetGridObject(startGridPosition);
        openList.Add(startNode);

        PathNode endNode = GetGridSystem(endGridPosition.floor).GetGridObject(endGridPosition);

        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                for (int floor = 0; floor < floorAmount; floor++)
                {
                    GridPosition gridPosition = new(x, z, floor);
                    PathNode pathNode = GetGridSystem(floor).GetGridObject(gridPosition);

                    pathNode.SetGCost(int.MaxValue);
                    pathNode.SetHCost(0);
                    pathNode.CalculateFCost();
                    pathNode.ResetCameFromPathNode();
                }
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
    private GridSystem<PathNode> GetGridSystem(int floor)
    {
        return gridSystemList[floor];
    }

    PathNode GetNode(int x, int z, int floor)
    {
        return GetGridSystem(floor).GetGridObject(new GridPosition(x,z, floor));
    }


    List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new();

        GridPosition currentNodeGridPosition = currentNode.GetGridPosition();

        if(currentNodeGridPosition.x - 1 >= 0)
        {
            //Left
            neighbourList.Add(GetNode(currentNodeGridPosition.x - 1, currentNodeGridPosition.z + 0, currentNodeGridPosition.floor));

            //Left Up
            if(currentNodeGridPosition.z + 1 < height)
                neighbourList.Add(GetNode(currentNodeGridPosition.x - 1, currentNodeGridPosition.z + 1, currentNodeGridPosition.floor));

            //Left Down
            if(currentNodeGridPosition.z - 1 >= 0)
                neighbourList.Add(GetNode(currentNodeGridPosition.x - 1, currentNodeGridPosition.z - 1, currentNodeGridPosition.floor));
        }

        if (currentNodeGridPosition.x + 1 < width)
        {
            //Right
            neighbourList.Add(GetNode(currentNodeGridPosition.x + 1, currentNodeGridPosition.z + 0, currentNodeGridPosition.floor));

            //Right Up
            if (currentNodeGridPosition.z + 1 < height)
                neighbourList.Add(GetNode(currentNodeGridPosition.x + 1, currentNodeGridPosition.z + 1, currentNodeGridPosition.floor));

            //Right Down
            if (currentNodeGridPosition.z - 1 >= 0)
                neighbourList.Add(GetNode(currentNodeGridPosition.x + 1, currentNodeGridPosition.z - 1, currentNodeGridPosition.floor));
        }

        //Up
        if (currentNodeGridPosition.z + 1 < height)
            neighbourList.Add(GetNode(currentNodeGridPosition.x + 0, currentNodeGridPosition.z + 1, currentNodeGridPosition.floor));

        //Down
        if (currentNodeGridPosition.z - 1 >= 0)
            neighbourList.Add(GetNode(currentNodeGridPosition.x + 0, currentNodeGridPosition.z - 1, currentNodeGridPosition.floor));

        List<PathNode> totalNeighbourList = new();
        totalNeighbourList.AddRange(neighbourList);

        List<GridPosition> pathfindingLinkGridPositionList = GetPathfindingLinkConnectedGridPositionList(currentNodeGridPosition); 

        foreach(GridPosition pathfindingLinkGridPosition in pathfindingLinkGridPositionList)
        {
            totalNeighbourList.Add(
                GetNode(
                    pathfindingLinkGridPosition.x,
                    pathfindingLinkGridPosition.z,
                    pathfindingLinkGridPosition.floor
                )
            );
        }

        return totalNeighbourList;
    }   
    
    List<GridPosition> GetPathfindingLinkConnectedGridPositionList(GridPosition  gridPosition)
    {
        List<GridPosition> gridPositionList = new();

        foreach(PathfindingLink pathfindingLink in pathfindingLinkList)
        {
            if(pathfindingLink.gridPositionA == gridPosition)
            {
                gridPositionList.Add(pathfindingLink.gridPositionB);
            }
            if (pathfindingLink.gridPositionB == gridPosition)
            {
                gridPositionList.Add(pathfindingLink.gridPositionA);
            }
        }

        return gridPositionList;
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
        return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).IsWalkable();
    }

    public void SetIsWalkable(GridPosition gridPosition, bool isWalkable)
    {
        GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).SetIsWalkable(isWalkable);
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
