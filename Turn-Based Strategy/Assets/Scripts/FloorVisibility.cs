using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorVisibility : MonoBehaviour
{
    [SerializeField] bool dynamicFloorPosition = false;
    [SerializeField] List<Renderer> ignoreRendererList = null;
    
    Renderer[] rendererArray;
    int floor;

    private void Awake()
    {
        rendererArray = GetComponentsInChildren<Renderer>(true);

        floor = LevelGrid.Instance.GetFloor(transform.position); 

        if(floor == 0 && !dynamicFloorPosition)
        {
            Destroy(this);
        }
    }

    void Show()
    {
        foreach (Renderer renderer in rendererArray)
        {
            if (ignoreRendererList.Contains(renderer)) continue;
            renderer.enabled = true; 
        }
    }

    void Hide()
    {
        foreach (Renderer renderer in rendererArray)
        {
            if (ignoreRendererList.Contains(renderer)) continue;
            renderer.enabled = false;
        }
    }

    private void Update()
    {
        if (dynamicFloorPosition)
        {
            floor = LevelGrid.Instance.GetFloor(transform.position);
        }

        bool showObject = CameraController.Instance.GetCameraHeight() > LevelGrid.Instance.FLOOR_HEIGHT * floor;
        if (showObject || floor == 0)
            Show();
        else
            Hide();
    }
}
