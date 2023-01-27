using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    float totalSpinAmount = 0;

    void Update()
    {
        if (!isActive) return;

        if(totalSpinAmount > 360)
        {
            isActive = false;
            onActionComplete();
        }
        
        if (isActive)
        {
            float spinAmount = 360 * Time.deltaTime;
            transform.eulerAngles += Vector3.up * spinAmount;
            totalSpinAmount += spinAmount;
        }
    }

    public void Spin(Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
        totalSpinAmount = 0;
        isActive = true;
    }
}