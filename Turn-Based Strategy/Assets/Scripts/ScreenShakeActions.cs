using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    private void Start()
    {
        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
        Grenade.OnAnyGrenadeExploded += Grenade_OnGrenadeExploded;
    }

    private void Grenade_OnGrenadeExploded(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(5f);
    }

    private void ShootAction_OnAnyShoot(object sender, Unit e)
    {
        ScreenShake.Instance.Shake();
    }
}
