using UnityEngine;

public class FightSceneLoader : MonoBehaviour
{
    public RobotSlots player1Slots;
    public RobotSlots player2Slots;

    public AttachmentSpawner attachmentSpawner;

    public RobotConfig player1Config;
    public RobotConfig player2Config;

    public RobotController player1Controller;
    public RobotController player2Controller;

    public RobotHealth player1Health;
    public RobotHealth player2Health;

    private void Start()
    {
        if (GameSettings.Instance == null)
        {
            Debug.LogWarning("GameSettings not found. Using defaults already on robots.");
            return;
        }

        // Apply body types onto configs
        player1Config.bodyType = GameSettings.Instance.p1BodyType;
        player2Config.bodyType = GameSettings.Instance.p2BodyType;

        // Apply physics stats + hearts
        player1Controller.ApplyCurrentBodyStats();
        player2Controller.ApplyCurrentBodyStats();

        player1Health.ApplyBodyType(player1Config.bodyType, resetToFull: true);
        player2Health.ApplyBodyType(player2Config.bodyType, resetToFull: true);

        player1Slots.RegenerateSlots();
        player2Slots.RegenerateSlots();

        attachmentSpawner.SpawnAll();
    }
}