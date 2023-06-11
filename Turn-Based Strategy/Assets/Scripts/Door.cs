using DG.Tweening;
using System;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] bool isOpen;
    [SerializeField] Transform leftPanel, rightPanel;
    GridPosition gridPosition;

    event Action onInteractionComplete;
    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);

        if (leftPanel == null)
            leftPanel = transform.GetChild(0);
        if (rightPanel == null)
            rightPanel = transform.GetChild(1);

        if (isOpen)
        {
            OpenDoor();
        }
        else
        {
            CLoseDoor();
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        Pathfinding.Instance.SetIsWalkable(gridPosition ,true);

        leftPanel.DOScaleX(0.1f, 1).SetEase(Ease.InBounce);
        rightPanel.DOScaleX(-0.1f, 1).SetEase(Ease.InBounce).
            OnComplete(() => onInteractionComplete?.Invoke()); 
    }

    private void CLoseDoor()
    {
        isOpen = false;
        Pathfinding.Instance.SetIsWalkable(gridPosition, false);

        leftPanel.DOScaleX(1f, 1).SetEase(Ease.InBounce);
        rightPanel.DOScaleX(-1f, 1).SetEase(Ease.InBounce).
            OnComplete(() => onInteractionComplete?.Invoke()); 
    }

    public void Interact(Action onInteractionComplete)
    {
        this.onInteractionComplete = onInteractionComplete;
        if (isOpen)
        {
            CLoseDoor();
        }
        else
        {
            OpenDoor();
        }
    }
}
