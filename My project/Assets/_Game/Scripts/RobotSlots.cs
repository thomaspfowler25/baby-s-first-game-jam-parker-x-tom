using System.Collections.Generic;
using UnityEngine;

public enum SlotGroup
{
    Bottom,
    LeftSide,
    RightSide,
    Head
}

public class RobotSlots : MonoBehaviour
{
    [Header("Marker Prefab")]
    public GameObject slotMarkerPrefab;

    [Header("Visual Offsets (relative to robot center)")]
    public float bottomY = -0.6f;
    public float headY = 0.6f;
    public float sideX = 0.6f;

    [Header("Spacing")]
    public float bottomSpacing = 0.35f; // spacing between bottom slots
    public float sideSpacing = 0.35f;   // spacing between side slots
    public float headSpacing = 0.35f;   // spacing between head slots (if 2)
    [Header("Marker Size")]
    public float markerScale = 0.16f;

    [Header("Debug")]
    public bool regenerateNow = false;

    public List<Transform> BottomSlots { get; private set; } = new();
    public List<Transform> LeftSideSlots { get; private set; } = new();
    public List<Transform> RightSideSlots { get; private set; } = new();
    public List<Transform> HeadSlots { get; private set; } = new();

    private RobotConfig _config;

    private void Awake()
    {
        _config = GetComponent<RobotConfig>();
        RegenerateSlots();
    }

    private void Update()
    {
        // Simple debug toggle: flip this checkbox in playmode to rebuild.
        if (regenerateNow)
        {
            regenerateNow = false;
            RegenerateSlots();
        }
    }

    public void RegenerateSlots()
    {
        ClearOldSlots();

        if (_config == null) _config = GetComponent<RobotConfig>();

        int bottomCount, sideCount, headCount;
        GetCounts(_config.bodyType, out bottomCount, out sideCount, out headCount);

        // Create parents for cleanliness
        var bottomParent = new GameObject("Slots_Bottom").transform;
        bottomParent.SetParent(transform, false);

        var leftParent = new GameObject("Slots_LeftSide").transform;
        leftParent.SetParent(transform, false);

        var rightParent = new GameObject("Slots_RightSide").transform;
        rightParent.SetParent(transform, false);

        var headParent = new GameObject("Slots_Head").transform;
        headParent.SetParent(transform, false);

        // Bottom slots (centered)
        CreateRowSlots(
            bottomParent,
            SlotGroup.Bottom,
            bottomCount,
            centerLocalPos: new Vector2(0f, bottomY),
            spacing: bottomSpacing,
            horizontal: true
        );

        // Side slots (vertical row)
        CreateRowSlots(
            leftParent,
            SlotGroup.LeftSide,
            sideCount,
            centerLocalPos: new Vector2(-sideX, 0f),
            spacing: sideSpacing,
            horizontal: false
        );

        CreateRowSlots(
            rightParent,
            SlotGroup.RightSide,
            sideCount,
            centerLocalPos: new Vector2(sideX, 0f),
            spacing: sideSpacing,
            horizontal: false
        );

        // Head slots (centered near top)
        CreateRowSlots(
            headParent,
            SlotGroup.Head,
            headCount,
            centerLocalPos: new Vector2(0f, headY),
            spacing: headSpacing,
            horizontal: true
        );
    }

    private void GetCounts(RobotBodyType type, out int bottom, out int sides, out int head)
    {
        switch (type)
        {
            case RobotBodyType.Tank:
                bottom = 4; sides = 4; head = 2;
                break;
            case RobotBodyType.Knight:
                bottom = 3; sides = 3; head = 1;
                break;
            case RobotBodyType.Ninja:
                bottom = 2; sides = 2; head = 0;
                break;
            default:
                bottom = 3; sides = 3; head = 1;
                break;
        }
    }

    private void CreateRowSlots(
        Transform parent,
        SlotGroup group,
        int count,
        Vector2 centerLocalPos,
        float spacing,
        bool horizontal
    )
    {
        if (count <= 0) return;

        float start = -(count - 1) * 0.5f * spacing;

        for (int i = 0; i < count; i++)
        {
            float offset = start + i * spacing;

            Vector2 localPos = horizontal
                ? new Vector2(centerLocalPos.x + offset, centerLocalPos.y)
                : new Vector2(centerLocalPos.x, centerLocalPos.y + offset);

            var slot = new GameObject($"{group}_Slot_{i}").transform;
            slot.SetParent(parent, false);
            slot.localPosition = localPos;

            AddSlotToList(group, slot);

            // Spawn marker visuals
            if (slotMarkerPrefab != null)
            {
                var marker = Instantiate(slotMarkerPrefab, slot);
                marker.name = "Marker";
                marker.transform.localPosition = Vector3.zero;
                marker.transform.localRotation = Quaternion.identity;
                marker.transform.localScale = new Vector3(markerScale, markerScale, 1f);
            }
        }
    }

    private void AddSlotToList(SlotGroup group, Transform slot)
    {
        switch (group)
        {
            case SlotGroup.Bottom: BottomSlots.Add(slot); break;
            case SlotGroup.LeftSide: LeftSideSlots.Add(slot); break;
            case SlotGroup.RightSide: RightSideSlots.Add(slot); break;
            case SlotGroup.Head: HeadSlots.Add(slot); break;
        }
    }

    private void ClearOldSlots()
    {
        BottomSlots.Clear();
        LeftSideSlots.Clear();
        RightSideSlots.Clear();
        HeadSlots.Clear();

        // Remove any prior slot parents
        var toDestroy = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Slots_"))
                toDestroy.Add(child);
        }

        for (int i = 0; i < toDestroy.Count; i++)
        {
            Destroy(toDestroy[i].gameObject);
        }
    }
}