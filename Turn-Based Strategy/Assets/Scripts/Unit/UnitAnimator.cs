using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Transform bulletProjectileTransform;
    [SerializeField] Transform shootPointTransform;
    [SerializeField] GameObject rifle, sword;

    #region Animation Variables

    const string animAimIdle = "Aim Idle";
    const string animRun = "Run";
    const string animFiring = "Firing";
    const string swordSlash = "Sword Slash";

    readonly float fixedTimeDuration = 0.1f;

    #endregion

    private void Start()
    {
        if(TryGetComponent(out MoveAction moveAction)){
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }

        if (TryGetComponent(out ShootAction shootAction))
            shootAction.OnShoot += ShootAction_OnShoot;

        if (TryGetComponent(out SwordAction swordAction))
        {
            swordAction.OnSwordActionStarted += SwordAction_OnSwordActionStarted;
            swordAction.OnSwordActionCompleted += SwordAction_OnSwordActionCompleted;
        }

        EquipRifle();
    }


    private void SwordAction_OnSwordActionStarted()
    {
        EquipSword();
        PlayAnimation(swordSlash);
    }
    private void SwordAction_OnSwordActionCompleted()
    {
        EquipRifle();
    }

    private void Update()
    {

        //Transition from Firing -> AimIdle 
        //if firing animation is 90% complete

        AnimatorClipInfo[] clips = anim.GetCurrentAnimatorClipInfo(0);
        if (clips[0].clip.name == animFiring || clips[0].clip.name == swordSlash)
        {
            if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
            {
                PlayAnimation(animAimIdle);
            }
        }
        
    }


    void PlayAnimation(string animString)
    {
        AnimatorClipInfo[] clips = anim.GetCurrentAnimatorClipInfo(0);
        if (anim.IsInTransition(0) || clips[0].clip.name == animString) return;

        int animHash = Animator.StringToHash(animString);

        anim.CrossFadeInFixedTime(animHash, fixedTimeDuration);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        PlayAnimation(animAimIdle);
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        PlayAnimation(animRun);
    }
    private void ShootAction_OnShoot(object sender, Unit targetUnit)
    {
        PlayAnimation(animFiring);

        BulletProjectile bullet = 
            Instantiate(bulletProjectileTransform,
                        shootPointTransform.position,
                        Quaternion.identity).
            GetComponent<BulletProjectile>();


        Vector3 targetPosition = targetUnit.transform.position;
        targetPosition.y = shootPointTransform.position.y;
        bullet.Setup(targetPosition);
    }

    void EquipSword()
    {
        sword.SetActive(true);
        rifle.SetActive(false);
    }

    void EquipRifle()
    {
        rifle.SetActive(true);
        sword.SetActive(false);
    }
}
