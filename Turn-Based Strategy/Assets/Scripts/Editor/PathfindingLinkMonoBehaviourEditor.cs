using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathfindingLinkMonoBehaviour))]
public class PathfindingLinkMonoBehaviourEditor : Editor
{
    private void OnSceneGUI()
    {
        PathfindingLinkMonoBehaviour pathfindingLinkMonoBehaviour = (PathfindingLinkMonoBehaviour)target;

        EditorGUI.BeginChangeCheck();

        Vector3 newLinkPositionA = Handles.PositionHandle(pathfindingLinkMonoBehaviour.linkPositionA, Quaternion.identity);
        Vector3 newLinkPositionB = Handles.PositionHandle(pathfindingLinkMonoBehaviour.linkPositionB, Quaternion.identity);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(pathfindingLinkMonoBehaviour, "Change Link Position"); 
            pathfindingLinkMonoBehaviour.linkPositionA = newLinkPositionA;
            pathfindingLinkMonoBehaviour.linkPositionB = newLinkPositionB;
        }
    }
}
