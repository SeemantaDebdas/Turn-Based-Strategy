using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] bool isEnemy = false;

    const int ACTION_POINTS_MAX = 2;

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    GridPosition gridPosition;

    BaseAction[] baseActionArray;

    int actionPoints = ACTION_POINTS_MAX;

    HealthSystem healthSystem;

    private void Awake()
    {
        baseActionArray = GetComponents<BaseAction>();
        healthSystem = GetComponent<HealthSystem>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        Debug.Log(gridPosition); 

        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        healthSystem.OnDie += HealthSystem_OnDie;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }


    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if(gridPosition != newGridPosition)
        {
            //Unit Changed Grid Position
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;

            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
        }
    }
    
    public bool IsEnemy() => isEnemy;

    public T GetAction<T>() where T:BaseAction
    {
        foreach(BaseAction baseAction in baseActionArray)
        {
            if (baseAction is T t)
                return t;
        }
        return null;
    }

    public Vector3 GetWorldPosition() => transform.position;

    public GridPosition GetGridPosition() => gridPosition;

    public BaseAction[] GetBaseActionArray() => baseActionArray;

    public int GetActionPoints() => actionPoints;  

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        return false;
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        return actionPoints >= baseAction.GetActionPointsCost();
    }

    void SpendActionPoints(int amount)
    {
        actionPoints -= amount;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void TurnSystem_OnTurnChanged(object sender, int e)
    {
        if((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) ||
            (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
        {
            actionPoints = ACTION_POINTS_MAX;
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Damage(int damagePoints)
    {
        healthSystem.Damage(damagePoints);
    }

    public float GetHealthNormalized() => healthSystem.GetNormalizedHealthPoints();

    private void HealthSystem_OnDie(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
    }
}
