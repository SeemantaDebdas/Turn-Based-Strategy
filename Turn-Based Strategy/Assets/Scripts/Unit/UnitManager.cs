using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    List<Unit> unitList = new List<Unit>();
    List<Unit> friendlyUnitList = new List<Unit>();
    List<Unit> enemyUnitList = new List<Unit>();

    public static UnitManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Deleting already exisiting instance of Unit Manager: {transform}  -  {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += UnitOnAnyUnitDead;
    }

    private void OnDestroy()
    {
        Unit.OnAnyUnitSpawned -= Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead -= UnitOnAnyUnitDead;
    }

    void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
        Unit unit = (Unit)sender;

        unitList.Add(unit);

        if (unit.IsEnemy())
            enemyUnitList.Add(unit);
        else
            friendlyUnitList.Add(unit);
    }

    void UnitOnAnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = (Unit)sender;

        unitList.Remove(unit);

        if (unit.IsEnemy())
            enemyUnitList.Remove(unit);
        else
            friendlyUnitList.Remove(unit);
    }

    public List<Unit> GetUnitList() => unitList;
    public List<Unit> GetEnemyUnitList() => enemyUnitList;
    public List<Unit> GetFriendlyUnitList() => friendlyUnitList;
}
