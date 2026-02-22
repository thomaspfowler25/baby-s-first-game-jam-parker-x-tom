using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public AssemblyUI assemblyUI;
    public MatchManager matchManager;

    public RobotController player1Robot;
    public RobotController player2Robot;

    private RobotHealth _p1Health;
    private RobotHealth _p2Health;

    private void Awake()
    {
        _p1Health = player1Robot.GetComponent<RobotHealth>();
        _p2Health = player2Robot.GetComponent<RobotHealth>();

        // Prevent match from starting immediately
        matchManager.enabled = false;
        assemblyUI.Show();

        // Also disable player input while assembling
        player1Robot.SetInputsEnabled(false);
        player2Robot.SetInputsEnabled(false);
    }

    // Hook this to the START MATCH button
    public void OnStartMatchPressed()
    {
        if (!assemblyUI.TryApplyChoices())
            return;

        // Apply stats + hearts based on chosen body type
        player1Robot.ApplyCurrentBodyStats();
        player2Robot.ApplyCurrentBodyStats();

        _p1Health.ApplyBodyType(player1Robot.BodyType, resetToFull: true);
        _p2Health.ApplyBodyType(player2Robot.BodyType, resetToFull: true);

        // Hide assembly and begin match
        assemblyUI.Hide();
        matchManager.enabled = true;

        // Let MatchManager do its normal countdown + round loop
    }
}