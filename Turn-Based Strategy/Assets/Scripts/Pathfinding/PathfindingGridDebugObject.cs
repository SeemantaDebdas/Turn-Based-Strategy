using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PathfindingGridDebugObject : GridDebugObject
{
    [SerializeField] TextMeshPro gCost;
    [SerializeField] TextMeshPro fCost;
    [SerializeField] TextMeshPro hCost;
    [SerializeField] SpriteRenderer isWalkableSprite;

    PathNode pathNode;

    public override void SetGridObject(object gridObject)
    {
        base.SetGridObject(gridObject);
        pathNode = (PathNode)gridObject;
    }

    protected override void Update()
    {
        base.Update();
        gCost.text = pathNode.GetGCost().ToString();
        hCost.text = pathNode.GetHCost().ToString();
        fCost.text = pathNode.GetFCost().ToString();

        Color isWalkableColor = Color.green;
        Color notIsWalkableColor = Color.red;

        isWalkableColor.a = notIsWalkableColor.a = isWalkableSprite.color.a;
        isWalkableSprite.color = pathNode.IsWalkable() ? isWalkableColor : notIsWalkableColor;
    }
}
