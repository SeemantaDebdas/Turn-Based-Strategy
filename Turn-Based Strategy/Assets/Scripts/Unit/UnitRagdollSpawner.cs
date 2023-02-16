using System;
using UnityEngine;

public class UnitRagdollSpawner : MonoBehaviour
{
    [SerializeField] HealthSystem healthSystem;
    [SerializeField] GameObject ragdollPrefab;
    [SerializeField] Transform originalRootBone;

    private void Start()
    {
        healthSystem.OnDie += HealthSystem_OnDie;
    }

    private void HealthSystem_OnDie(object sender, EventArgs e)
    {
        UnitRagdoll unitRagdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation).
            GetComponent<UnitRagdoll>();

        unitRagdoll.Setup(originalRootBone);
    }
}
