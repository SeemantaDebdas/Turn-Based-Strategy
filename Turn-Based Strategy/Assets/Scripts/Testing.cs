using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    LineRenderer lineRenderer;
    List<GridPosition> pathfindingGridPosition;

    private void Awake()
    {
         lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            pathfindingGridPosition = new();
            GridPosition startGridPosition = new(0, 0, 0);
            GridPosition endGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPositionOnlyVisible());
            pathfindingGridPosition = Pathfinding.Instance.FindPath(startGridPosition, endGridPosition, out int pathLength);

            for (int i = 0; i < pathfindingGridPosition.Count; i++)
            {
                Debug.Log(pathfindingGridPosition[i].ToString());
            }

            for (int i = 0; i < pathfindingGridPosition.Count; i++)
            {
                lineRenderer.positionCount = pathfindingGridPosition.Count;
                lineRenderer.SetPosition(i, LevelGrid.Instance.GetWorldPosition(pathfindingGridPosition[i]));
                //Debug.DrawLine(LevelGrid.Instance.GetWorldPosition(pathfindingGridPosition[i]), LevelGrid.Instance.GetWorldPosition(pathfindingGridPosition[i + 1]), Color.red, 10f);
            }
        }
    }
}

