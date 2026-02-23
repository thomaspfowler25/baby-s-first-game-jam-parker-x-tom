using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AssemblyBuilderController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject bodySelectPanel;
    public GameObject builderPanel;

    [Header("UI")]
    public TextMeshProUGUI selectedAttachmentText;

    [Header("Slot Boards (parents for slot buttons)")]
    public Transform p1SlotBoard;
    public Transform p2SlotBoard;

    [Header("Prefab")]
    public GameObject slotDotButtonPrefab; // a small circular button with TMP (optional)

    private AttachmentType _selected = AttachmentType.Thruster;

    private void Start()
    {
        builderPanel.SetActive(false);
        bodySelectPanel.SetActive(true);
        SetSelected(_selected);
    }

    // Call this after both bodies chosen
    public void OpenBuilder()
    {
        EnsureGameSettingsExists();

        // Clear attachments when entering builder (optional, but usually desired)
        GameSettings.Instance.ClearAllAttachments();

        bodySelectPanel.SetActive(false);
        builderPanel.SetActive(true);

        RebuildSlotBoards();
    }

    public void SetSelectedThruster() => SetSelected(AttachmentType.Thruster);
    public void SetSelectedWheel()    => SetSelected(AttachmentType.Wheel);
    public void SetSelectedSaw()      => SetSelected(AttachmentType.Saw);
    public void SetSelectedLaser()    => SetSelected(AttachmentType.Laser);
    public void SetSelectedCannon()   => SetSelected(AttachmentType.Cannon);
    public void SetSelectedSpike()    => SetSelected(AttachmentType.Spike);
    public void SetSelectedNone()     => SetSelected(AttachmentType.None);

    private void SetSelected(AttachmentType t)
    {
        _selected = t;
        if (selectedAttachmentText != null)
            selectedAttachmentText.text = $"Selected: {_selected}";
    }

    private void RebuildSlotBoards()
    {
        ClearChildren(p1SlotBoard);
        ClearChildren(p2SlotBoard);

        // Build based on body types selected earlier
        BuildForPlayer(1, GameSettings.Instance.p1BodyType, p1SlotBoard);
        BuildForPlayer(2, GameSettings.Instance.p2BodyType, p2SlotBoard);
    }

    private void BuildForPlayer(int player, RobotBodyType bodyType, Transform root)
    {
        GetCounts(bodyType, out int bottom, out int sides, out int head);

        // Create groups in a clean order
        CreateGroup(player, root, "Bottom", bottom);
        CreateGroup(player, root, "Left", sides);
        CreateGroup(player, root, "Right", sides);
        CreateGroup(player, root, "Head", head);
    }

    private void CreateGroup(int player, Transform root, string group, int count)
{
    // Label (with proper sizing)
    var labelGO = new GameObject($"{group}_Label");
    labelGO.transform.SetParent(root, false);

    var label = labelGO.AddComponent<TextMeshProUGUI>();
    label.text = group;
    label.fontSize = 28;
    label.alignment = TextAlignmentOptions.Left;

    var labelRT = labelGO.GetComponent<RectTransform>();
    labelRT.sizeDelta = new Vector2(600f, 40f);

    var labelLE = labelGO.AddComponent<LayoutElement>();
    labelLE.minHeight = 40f;
    labelLE.preferredHeight = 40f;

    // Row holder (with layout)
    var row = new GameObject($"{group}_Row");
    row.transform.SetParent(root, false);

    var rowRT = row.AddComponent<RectTransform>();
    rowRT.sizeDelta = new Vector2(600f, 70f);

    var rowLE = row.AddComponent<LayoutElement>();
    rowLE.minHeight = 70f;
    rowLE.preferredHeight = 70f;

    var h = row.AddComponent<HorizontalLayoutGroup>();
    h.childAlignment = TextAnchor.MiddleLeft;
    h.spacing = 12;
    h.childForceExpandHeight = false;
    h.childForceExpandWidth = false;

    for (int i = 0; i < count; i++)
    {
        int idx = i;
        var dot = Instantiate(slotDotButtonPrefab, row.transform);
        dot.name = $"{group}_Slot_{idx}";

        var btn = dot.GetComponent<Button>();
        var txt = dot.GetComponentInChildren<TextMeshProUGUI>();

        // show current equipped
        var cur = GetAttachment(player, group, idx);
        if (txt != null) txt.text = cur == AttachmentType.None ? "-" : cur.ToString().Substring(0, 1);

        btn.onClick.AddListener(() =>
        {
            SetAttachment(player, group, idx, _selected);
            var newVal = GetAttachment(player, group, idx);
            if (txt != null) txt.text = newVal == AttachmentType.None ? "-" : newVal.ToString().Substring(0, 1);
        });
    }
}

    public void DoneBuilding()
    {
        SceneManager.LoadScene("FightScene");
    }

    private void EnsureGameSettingsExists()
    {
        if (GameSettings.Instance != null) return;
        var go = new GameObject("GameSettings");
        go.AddComponent<GameSettings>();
    }

    private void ClearChildren(Transform t)
    {
        for (int i = t.childCount - 1; i >= 0; i--)
            Destroy(t.GetChild(i).gameObject);
    }

    private void GetCounts(RobotBodyType type, out int bottom, out int sides, out int head)
    {
        switch (type)
        {
            case RobotBodyType.Tank: bottom = 4; sides = 4; head = 2; break;
            case RobotBodyType.Knight: bottom = 3; sides = 3; head = 1; break;
            case RobotBodyType.Ninja: bottom = 2; sides = 2; head = 0; break;
            default: bottom = 3; sides = 3; head = 1; break;
        }
    }

    private AttachmentType GetAttachment(int player, string group, int idx)
    {
        var gs = GameSettings.Instance;
        if (player == 1)
        {
            if (group == "Bottom") return gs.p1Bottom[idx];
            if (group == "Left") return gs.p1Left[idx];
            if (group == "Right") return gs.p1Right[idx];
            if (group == "Head") return gs.p1Head[idx];
        }
        else
        {
            if (group == "Bottom") return gs.p2Bottom[idx];
            if (group == "Left") return gs.p2Left[idx];
            if (group == "Right") return gs.p2Right[idx];
            if (group == "Head") return gs.p2Head[idx];
        }
        return AttachmentType.None;
    }

    private void SetAttachment(int player, string group, int idx, AttachmentType value)
    {
        var gs = GameSettings.Instance;
        if (player == 1)
        {
            if (group == "Bottom") gs.p1Bottom[idx] = value;
            if (group == "Left") gs.p1Left[idx] = value;
            if (group == "Right") gs.p1Right[idx] = value;
            if (group == "Head") gs.p1Head[idx] = value;
        }
        else
        {
            if (group == "Bottom") gs.p2Bottom[idx] = value;
            if (group == "Left") gs.p2Left[idx] = value;
            if (group == "Right") gs.p2Right[idx] = value;
            if (group == "Head") gs.p2Head[idx] = value;
        }
    }
}
