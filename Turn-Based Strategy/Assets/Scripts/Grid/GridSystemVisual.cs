using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class GridSystemVisual : MonoBehaviour
{
    public enum GridVisualType
    {
        White,
        Blue, 
        Red,
        RedSoft,
        Green,
    }

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType type;
        public Material material;
    }


    [SerializeField] Transform gridSystemVisualSingle;
    [SerializeField] List<GridVisualTypeMaterial> gridVisualTypeMaterialList;


    GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    public static GridSystemVisual Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Deleting already exisiting instance of Grid System Visual: {transform}  -  {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[
            LevelGrid.Instance.GetWidth(),
            LevelGrid.Instance.GetHeight()
            ];

        Transform gridSystemVisualParentTransform = new GameObject("GridVisualParent").transform;

        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for(int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(new GridPosition(x, z));
                Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSingle, worldPosition, Quaternion.identity);
                gridSystemVisualSingleTransform.SetParent(gridSystemVisualParentTransform);
                gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;

        UpdateGridVisual();
    }


    private void OnDestroy()
    {
        UnitActionSystem.Instance.OnSelectedActionChanged -= UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition -= LevelGrid_OnAnyUnitMovedGridPosition;
        Unit.OnAnyUnitDead -= Unit_OnAnyUnitDead;
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }
    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }
    public  void HideAllGridPosition()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                gridSystemVisualSingleArray[x,z].Hide();
            }
        }
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
    {
        for(int i = 0; i < gridPositionList.Count; i++)
        {
            Material gridVisualTypeMaterial = GetGridVisualTypeMaterial(gridVisualType);
            gridSystemVisualSingleArray[gridPositionList[i].x, gridPositionList[i].z].Show(gridVisualTypeMaterial);
        }
    }

    void UpdateGridVisual()
    {
        HideAllGridPosition(); 

        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        GridVisualType gridVisualType;

        switch (selectedAction)
        {
            default:
            case MoveAction:
                gridVisualType = GridVisualType.White;
                break;
            case SpinAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case GrenadeAction:
                gridVisualType = GridVisualType.Green;
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetShootRange(), GridVisualType.RedSoft); 
                break;
            case SwordAction swordAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRangeSquare(selectedUnit.GetGridPosition(), swordAction.GetSwordRange(), GridVisualType.RedSoft);
                break;
        }

        ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType); 
    }

    Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach(var gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if (gridVisualTypeMaterial.type == gridVisualType)
                return gridVisualTypeMaterial.material;
        }

        Debug.LogError("Grid Visual Type Material not found: " + gridVisualType);
        return null;
    }

    public void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> validGridPositionList = new();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition offsetGridPosition = new(x, z);
                GridPosition testGridPosition = gridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    continue;

                float taxiCabDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (taxiCabDistance > range)
                    continue;

                validGridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(validGridPositionList, gridVisualType);
    }

    public void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> validGridPositionList = new();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition offsetGridPosition = new(x, z);
                GridPosition testGridPosition = gridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    continue;

                validGridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(validGridPositionList, gridVisualType);
    }
}
