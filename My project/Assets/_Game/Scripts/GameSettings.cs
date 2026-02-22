using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    public RobotBodyType p1BodyType = RobotBodyType.Knight;
    public RobotBodyType p2BodyType = RobotBodyType.Knight;

    // Max possible slots across all body types:
    // Bottom: 4, Left: 4, Right: 4, Head: 2
    // We'll only *use* as many as the chosen body type supports.
    [Header("P1 Attachments")]
    public AttachmentType[] p1Bottom = new AttachmentType[4];
    public AttachmentType[] p1Left = new AttachmentType[4];
    public AttachmentType[] p1Right = new AttachmentType[4];
    public AttachmentType[] p1Head = new AttachmentType[2];

    [Header("P2 Attachments")]
    public AttachmentType[] p2Bottom = new AttachmentType[4];
    public AttachmentType[] p2Left = new AttachmentType[4];
    public AttachmentType[] p2Right = new AttachmentType[4];
    public AttachmentType[] p2Head = new AttachmentType[2];

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ClearAllAttachments()
    {
        ClearArray(p1Bottom); ClearArray(p1Left); ClearArray(p1Right); ClearArray(p1Head);
        ClearArray(p2Bottom); ClearArray(p2Left); ClearArray(p2Right); ClearArray(p2Head);
    }

    private void ClearArray(AttachmentType[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
            arr[i] = AttachmentType.None;
    }
}