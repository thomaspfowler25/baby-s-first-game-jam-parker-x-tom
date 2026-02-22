using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssemblyUI : MonoBehaviour
{
    [Header("Scene References")]
    public GameObject assemblyPanel;
    public Button startMatchButton;
    public TextMeshProUGUI statusText;

    [Header("Robots to configure")]
    public RobotConfig player1Config;
    public RobotConfig player2Config;

    private RobotBodyType? _p1Choice = null;
    private RobotBodyType? _p2Choice = null;

    public void Show()
    {
        assemblyPanel.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        assemblyPanel.SetActive(false);
    }

    public void ChooseP1_Tank()   { _p1Choice = RobotBodyType.Tank;   Refresh(); }
    public void ChooseP1_Knight() { _p1Choice = RobotBodyType.Knight; Refresh(); }
    public void ChooseP1_Ninja()  { _p1Choice = RobotBodyType.Ninja;  Refresh(); }

    public void ChooseP2_Tank()   { _p2Choice = RobotBodyType.Tank;   Refresh(); }
    public void ChooseP2_Knight() { _p2Choice = RobotBodyType.Knight; Refresh(); }
    public void ChooseP2_Ninja()  { _p2Choice = RobotBodyType.Ninja;  Refresh(); }

    private void Refresh()
    {
        string p1 = _p1Choice.HasValue ? _p1Choice.Value.ToString() : "(none)";
        string p2 = _p2Choice.HasValue ? _p2Choice.Value.ToString() : "(none)";

        if (statusText != null)
            statusText.text = $"P1: {p1}    P2: {p2}";

        bool ready = _p1Choice.HasValue && _p2Choice.HasValue;
        if (startMatchButton != null)
            startMatchButton.interactable = ready;
    }

    public bool TryApplyChoices()
    {
        if (!_p1Choice.HasValue || !_p2Choice.HasValue)
            return false;

        player1Config.bodyType = _p1Choice.Value;
        player2Config.bodyType = _p2Choice.Value;
        return true;
    }
}