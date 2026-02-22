using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssemblyAttachmentsUI : MonoBehaviour
{
    [Header("UI")]
    public Transform p1SlotsRoot;
    public Transform p2SlotsRoot;

    public GameObject slotButtonPrefab; // prefab with Button + TMP text
    public TextMeshProUGUI hintText;

    // how many slots are active for current chosen bodies
    private int _p1Bottom, _p1Side, _p1Head;
    private int _p2Bottom, _p2Side, _p2Head;

    private void Start()
    {
        if (hintText != null)
        {
            hintText.text = "Click a slot to cycle: None → Thruster → Wheel → Saw → Laser → Cannon → Spike";
        }

        RebuildAll();
    }

    public void RebuildAll()
    {
        if (GameSettings.Instance == null) return;

        GetCounts(GameSettings.Instance.p1BodyType, out _p1Bottom, out _p1Side, out _p1Head);
        GetCounts(GameSettings.Instance.p2BodyType, out _p2Bottom, out _p2Side, out _p2Head);

        BuildPlayerUI(
            player: 1,
            root: p1SlotsRoot,
            bottomCount: _p1Bottom,
            sideCount: _p1Side,
            headCount: _p1Head
        );

        BuildPlayerUI(
            player: 2,
            root: p2SlotsRoot,
            bottomCount: _p2Bottom,
            sideCount: _p2Side,
            headCount: _p2Head
        );
    }

    private void BuildPlayerUI(int player, Transform root, int bottomCount, int sideCount, int headCount)
    {
        // clear old buttons
        for (int i = root.childCount - 1; i >= 0; i--)
            Destroy(root.GetChild(i).gameObject);

        AddLabel(root, $"PLAYER {player}");

        AddGroup(root, player, "Bottom", bottomCount);
        AddGroup(root, player, "Left", sideCount);
        AddGroup(root, player, "Right", sideCount);
        AddGroup(root, player, "Head", headCount);
    }

    private void AddGroup(Transform root, int player, string groupName, int count)
    {
        AddLabel(root, groupName);

        var row = new GameObject($"{groupName}_Row");
        row.transform.SetParent(root, false);

        var h = row.AddComponent<HorizontalLayoutGroup>();
        h.childAlignment = TextAnchor.MiddleLeft;
        h.spacing = 10;
        h.childForceExpandHeight = false;
        h.childForceExpandWidth = false;

        for (int i = 0; i < count; i++)
        {
            int slotIndex = i;
            var go = Instantiate(slotButtonPrefab, row.transform);
            go.name = $"{groupName}_Slot_{i}";

            var btn = go.GetComponent<Button>();
            var txt = go.GetComponentInChildren<TextMeshProUGUI>();

            AttachmentType current = GetAttachment(player, groupName, slotIndex);
            if (txt != null) txt.text = current.ToString();

            btn.onClick.AddListener(() =>
            {
                var cur = GetAttachment(player, groupName, slotIndex);
                var next = AttachmentCycle.Next(cur);
                SetAttachment(player, groupName, slotIndex, next);
                if (txt != null) txt.text = next.ToString();
            });
        }
    }

    private void AddLabel(Transform root, string text)
    {
        var go = new GameObject($"Label_{text}");
        go.transform.SetParent(root, false);

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 28;
        tmp.alignment = TextAlignmentOptions.Left;

        var le = go.AddComponent<LayoutElement>();
        le.minHeight = 40;
    }

    private void GetCounts(RobotBodyType type, out int bottom, out int sides, out int head)
    {
        switch (type)
        {
            case RobotBodyType.Tank:   bottom = 4; sides = 4; head = 2; break;
            case RobotBodyType.Knight: bottom = 3; sides = 3; head = 1; break;
            case RobotBodyType.Ninja:  bottom = 2; sides = 2; head = 0; break;
            default:                   bottom = 3; sides = 3; head = 1; break;
        }
    }

    private AttachmentType GetAttachment(int player, string group, int idx)
    {
        var gs = GameSettings.Instance;
        if (player == 1)
        {
            if (group == "Bottom") return gs.p1Bottom[idx];
            if (group == "Left")   return gs.p1Left[idx];
            if (group == "Right")  return gs.p1Right[idx];
            if (group == "Head")   return gs.p1Head[idx];
        }
        else
        {
            if (group == "Bottom") return gs.p2Bottom[idx];
            if (group == "Left")   return gs.p2Left[idx];
            if (group == "Right")  return gs.p2Right[idx];
            if (group == "Head")   return gs.p2Head[idx];
        }

        return AttachmentType.None;
    }

    private void SetAttachment(int player, string group, int idx, AttachmentType value)
    {
        var gs = GameSettings.Instance;
        if (player == 1)
        {
            if (group == "Bottom") gs.p1Bottom[idx] = value;
            if (group == "Left")   gs.p1Left[idx] = value;
            if (group == "Right")  gs.p1Right[idx] = value;
            if (group == "Head")   gs.p1Head[idx] = value;
        }
        else
        {
            if (group == "Bottom") gs.p2Bottom[idx] = value;
            if (group == "Left")   gs.p2Left[idx] = value;
            if (group == "Right")  gs.p2Right[idx] = value;
            if (group == "Head")   gs.p2Head[idx] = value;
        }
    }
}