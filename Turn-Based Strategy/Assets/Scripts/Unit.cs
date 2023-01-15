using System;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Unit : MonoBehaviour
{
    [SerializeField] float stoppingDistance = 0.1f;
    [SerializeField] float movementSpeed = 2.87f;
    [SerializeField] float rotateSpeed = 50f;

    Vector3 targetPosition = Vector3.zero;
    GridPosition gridPosition;

    #region Animation Variables

    Animator anim;

    const string animAimIdle = "Aim Idle";
    const string animRun = "Run";

    int AimIdleHash = Animator.StringToHash(animAimIdle);
    int RunHash = Animator.StringToHash(animRun);

    float fixedTimeDuration = 0.1f;

    #endregion

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        targetPosition = transform.position;
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
    }

    private void Update()
    {
        if(Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += movementSpeed * Time.deltaTime * direction;

            RotateTowardsDirection(direction);
            PlayAnimation(RunHash, animRun);
        }
        else
        {
            PlayAnimation(AimIdleHash, animAimIdle);
        }

        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if(gridPosition != newGridPosition)
        {
            //Unit Changed Grid Position
            LevelGrid.Instance.UnitMovedGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }
    }
    public void Move(Vector3 movePosition)
    {
        this.targetPosition = movePosition;
    }

    private void RotateTowardsDirection(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }


    void PlayAnimation(int animHash, string animString)
    {
        AnimatorClipInfo[] clips = anim.GetCurrentAnimatorClipInfo(0);
        if (anim.IsInTransition(0) || clips[0].clip.name == animString) return;
        
        anim.CrossFadeInFixedTime(animHash, fixedTimeDuration);
    }
}
