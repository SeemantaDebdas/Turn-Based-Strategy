using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PathNode 
{
    GridPosition gridPosition;
    
    int gCost; //cost from start node to current node
    int hCost; //heuristic cost from this node to end node
    int fCost; // gCost + fCost. If value is lower, we take that node in the path

    PathNode cameFromPathNode;
    bool isWalkable = true;

    public PathNode(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    public int GetGCost() => gCost;
    public int GetHCost() => hCost;
    public int GetFCost() => fCost;

    public void SetGCost(int gCost) => this.gCost = gCost;
    public void SetHCost(int hCost) => this.hCost = hCost;
    public void CalculateFCost() => fCost = gCost + hCost;


    public PathNode GetCameFromPathNode() => cameFromPathNode;
    public void SetCameFromPathNode(PathNode pathNode) => cameFromPathNode = pathNode;
    public void ResetCameFromPathNode() => cameFromPathNode = null;


    public GridPosition GetGridPosition() => gridPosition;

    public bool IsWalkable() => isWalkable;
    public void SetIsWalkable(bool isWalkable) => this.isWalkable = isWalkable;

    public override string ToString() => gridPosition.ToString();
}
