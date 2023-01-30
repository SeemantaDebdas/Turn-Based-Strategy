using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI turnText;
    [SerializeField] Button endTurnButton;

    private void Start()
    {
        endTurnButton.onClick.AddListener(() =>
        {
            TurnSystem.Instance.NextTurn();
        });

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, int turnNumber)
    {
        UpdateTurnText(turnNumber);
    }

    private void UpdateTurnText(int turnNumber)
    {
        turnText.text = "Turn Text: " + turnNumber;
    }
}
