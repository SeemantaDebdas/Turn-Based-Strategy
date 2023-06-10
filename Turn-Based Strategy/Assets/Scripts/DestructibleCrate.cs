using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour
{
    [SerializeField] Transform destructibleCrateDestroyedPrefab;

    public static event Action<DestructibleCrate> OnAnyCratesDestroyed;
    GridPosition gridPosition;

    public GridPosition GetGridPosition { get { return gridPosition; } }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
    }

    public void Damage()
    {
        OnAnyCratesDestroyed?.Invoke(this);

        Transform destroyedCrateSpawn = Instantiate(destructibleCrateDestroyedPrefab, transform.position, transform.rotation);
        ApplyExplosionToRagdoll(destroyedCrateSpawn, 150f, destroyedCrateSpawn.position, 10f);

        Destroy(gameObject);
    }

    void ApplyExplosionToRagdoll(Transform root, float force, Vector3 position, float radius)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent(out Rigidbody rb))
            {
                rb.AddExplosionForce(force, position, radius);
            }
            ApplyExplosionToRagdoll(child, force, position, radius);
        }
    }
}
