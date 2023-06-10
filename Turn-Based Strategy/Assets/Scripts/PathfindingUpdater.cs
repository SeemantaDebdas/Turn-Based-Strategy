using System;
using System.Collections;
using UnityEngine;

public class PathfindingUpdater : MonoBehaviour
{
    private void Start()
    {
        DestructibleCrate.OnAnyCratesDestroyed += DestructibleCrate_OnAnyCratesDestroyed;
    }

    private void DestructibleCrate_OnAnyCratesDestroyed(DestructibleCrate crate)
    {
        Pathfinding.Instance.SetIsWalkable(crate.GetGridPosition, true);
    }
}