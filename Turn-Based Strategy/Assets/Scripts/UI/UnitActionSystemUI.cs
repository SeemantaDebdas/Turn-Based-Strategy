using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] Transform actionButtonContainer;
    [SerializeField] GameObject actionButtonUIPrefab;
    [SerializeField] TextMeshProUGUI actionPointsText;

    List<ActionButtonUI> actionButtonUIList = new();

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;

        CreateActionButtons();
        UpdateSelectedVisual();
        UpdateActionPointsText();
    }



    void CreateActionButtons()
    {
        actionButtonUIList.Clear();

        foreach (Transform child in actionButtonContainer)
            Destroy(child.gameObject);

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction[] baseActionArray = selectedUnit.GetBaseActionArray();

        foreach(BaseAction action in baseActionArray)
        {
            ActionButtonUI actionButtonUI = 
                Instantiate(actionButtonUIPrefab, actionButtonContainer).GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(action);
            actionButtonUIList.Add(actionButtonUI);
        }
    }
    
    void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        CreateActionButtons();
        UpdateSelectedVisual();
        UpdateActionPointsText();
    }
    void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateSelectedVisual();
    }

    private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }

    private void TurnSystem_OnTurnChanged(object sender, int e)
    {
        UpdateActionPointsText();
    }
    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }

    void UpdateSelectedVisual()
    {
        foreach(ActionButtonUI actionButtonUI in actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }

    void UpdateActionPointsText() 
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        actionPointsText.text = "Action Points: " + selectedUnit.GetActionPoints();
    }
}
