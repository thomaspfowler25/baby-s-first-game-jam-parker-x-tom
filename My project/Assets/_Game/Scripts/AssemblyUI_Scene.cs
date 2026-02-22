using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AssemblyUI_Scene : MonoBehaviour
{
    [Header("UI")]
    public Button startMatchButton;
    public TextMeshProUGUI statusText;

    public AssemblyAttachmentsUI attachmentsUI;

    private RobotBodyType? _p1Choice = null;
    private RobotBodyType? _p2Choice = null;

    private void Start()
    {
        Refresh();
    }

    public void ChooseP1_Tank()   { _p1Choice = RobotBodyType.Tank;   Refresh(); if (attachmentsUI != null) attachmentsUI.RebuildAll(); }
    public void ChooseP1_Knight() { _p1Choice = RobotBodyType.Knight; Refresh(); if (attachmentsUI != null) attachmentsUI.RebuildAll(); }
    public void ChooseP1_Ninja()  { _p1Choice = RobotBodyType.Ninja;  Refresh(); if (attachmentsUI != null) attachmentsUI.RebuildAll(); }

    public void ChooseP2_Tank()   { _p2Choice = RobotBodyType.Tank;   Refresh(); if (attachmentsUI != null) attachmentsUI.RebuildAll(); }
    public void ChooseP2_Knight() { _p2Choice = RobotBodyType.Knight; Refresh(); if (attachmentsUI != null) attachmentsUI.RebuildAll(); }
    public void ChooseP2_Ninja()  { _p2Choice = RobotBodyType.Ninja;  Refresh(); if (attachmentsUI != null) attachmentsUI.RebuildAll(); }

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

    public void StartMatch()
    {
        if (!_p1Choice.HasValue || !_p2Choice.HasValue) return;

        // Ensure GameSettings exists
        if (GameSettings.Instance == null)
        {
            var go = new GameObject("GameSettings");
            go.AddComponent<GameSettings>();
        }

        GameSettings.Instance.p1BodyType = _p1Choice.Value;
        GameSettings.Instance.p2BodyType = _p2Choice.Value;

        SceneManager.LoadScene("FightScene");
    }
}