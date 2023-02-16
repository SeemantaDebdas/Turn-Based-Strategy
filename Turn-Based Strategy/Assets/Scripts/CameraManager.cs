using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] GameObject actionCamera = null;

    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionComplete += BaseAction_OnAnyActionComplete;

        HideActionCamera();
    }

    private void OnDestroy()
    {
        BaseAction.OnAnyActionStarted -= BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionComplete -= BaseAction_OnAnyActionComplete;
    }

    void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                PositionActionCamera(shootAction);
                ShowActionCamera();
                break;
        }
    }

    void BaseAction_OnAnyActionComplete(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction:
                HideActionCamera();
                break;
        }
    }

    private void PositionActionCamera(ShootAction shootAction)
    {
        Unit targetUnit = shootAction.GetTargetUnit();
        Unit shooterUnit = shootAction.GetUnit();

        Vector3 shootDirection = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;

        float characterHeight = 1.7f;
        Vector3 characterHeightOffset = Vector3.up * characterHeight;

        float shoulderOffsetAmount = 0.5f;
        Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDirection * shoulderOffsetAmount;

        actionCamera.transform.position = shooterUnit.GetWorldPosition()                                          
                                          + characterHeightOffset 
                                          + shoulderOffset 
                                          + (shootDirection * -1);

        actionCamera.transform.LookAt(targetUnit.transform);
    }

    void ShowActionCamera() => actionCamera.SetActive(true);
    void HideActionCamera() => actionCamera.SetActive(false);
}
