using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingLinkMonoBehaviour : MonoBehaviour
{
    public Vector3 linkPositionA, linkPositionB;

    public PathfindingLink GetPathfindingLink()
    {
        return new PathfindingLink
        {
            gridPositionA = LevelGrid.Instance.GetGridPosition(linkPositionA),
            gridPositionB = LevelGrid.Instance.GetGridPosition(linkPositionB)
        };
    }
}
