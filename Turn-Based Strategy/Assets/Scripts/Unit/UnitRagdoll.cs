using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] Transform ragdollRootBone;
    [SerializeField] float explosionForce = 300f;
    [SerializeField] float explosionRadius = 10f;
    public void Setup(Transform originalRootBone)
    {
        MatchAllChildTransforms(originalRootBone, ragdollRootBone);
        ApplyExplosionToRagdoll(ragdollRootBone, explosionForce, transform.position, explosionRadius);
    }

    void MatchAllChildTransforms(Transform root, Transform clone)
    {
        foreach(Transform child in root)
        {
            Transform cloneChild = clone.Find(child.name);

            if(cloneChild != null)
            {
                cloneChild.SetPositionAndRotation(child.position, child.rotation);
                MatchAllChildTransforms(child, cloneChild);
            }
        }
    }

    void ApplyExplosionToRagdoll(Transform root, float force, Vector3 position, float radius)
    {
        foreach(Transform child in root)
        {
            if(child.TryGetComponent(out Rigidbody rb))
            {
                rb.AddExplosionForce(force, position, radius);
            }
            ApplyExplosionToRagdoll(child, force, position, radius);
        }
    }
}
