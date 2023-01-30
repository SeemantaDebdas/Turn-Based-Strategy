using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    [SerializeField] Transform gridSystemVisualSingle;
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
    }

    private void Update()
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

    public void ShowGridPositionList(List<GridPosition> gridPositionList)
    {
        for(int i = 0; i < gridPositionList.Count; i++)
        {
            gridSystemVisualSingleArray[gridPositionList[i].x, gridPositionList[i].z].Show();
        }
    }

    void UpdateGridVisual()
    {
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        HideAllGridPosition(); 
        ShowGridPositionList(selectedAction.GetValidActionGridPositionList()); 
    }
}
