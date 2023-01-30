using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] Image selectedImage;

    TextMeshProUGUI buttonText;
    Button button;
    BaseAction action;

    private void Awake()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponent<Button>();
    }

    public void SetBaseAction(BaseAction action)
    {
        buttonText.text = action.GetActionName().ToUpper();
        button.onClick.AddListener(() =>
        {
            UnitActionSystem.Instance.SetSelectedAction(action);
        });

        this.action = action;
    }

    public void UpdateSelectedVisual()
    {
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        selectedImage.gameObject.SetActive(action == selectedAction);
    }
}
