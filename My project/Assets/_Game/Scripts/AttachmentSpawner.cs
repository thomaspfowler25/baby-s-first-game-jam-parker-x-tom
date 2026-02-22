using System.Collections.Generic;
using UnityEngine;

public class AttachmentSpawner : MonoBehaviour
{
    [Header("Visual Prefab")]
    public GameObject attachmentVisualPrefab;

    [Header("Robots")]
    public RobotSlots p1Slots;
    public RobotSlots p2Slots;

    // Keep track of what we spawned so we can delete it without FindObjectsOfType.
    private readonly List<GameObject> _spawned = new List<GameObject>();

    public void SpawnAll()
    {
        ClearExisting();

        if (GameSettings.Instance == null) return;

        SpawnForPlayer(
            GameSettings.Instance.p1Bottom, GameSettings.Instance.p1Left, GameSettings.Instance.p1Right, GameSettings.Instance.p1Head,
            p1Slots
        );

        SpawnForPlayer(
            GameSettings.Instance.p2Bottom, GameSettings.Instance.p2Left, GameSettings.Instance.p2Right, GameSettings.Instance.p2Head,
            p2Slots
        );
    }

    private void SpawnForPlayer(AttachmentType[] bottom, AttachmentType[] left, AttachmentType[] right, AttachmentType[] head, RobotSlots slots)
    {
        SpawnGroup(bottom, slots.BottomSlots, "Bottom");
        SpawnGroup(left, slots.LeftSideSlots, "Left");
        SpawnGroup(right, slots.RightSideSlots, "Right");
        SpawnGroup(head, slots.HeadSlots, "Head");
    }

    private void SpawnGroup(AttachmentType[] saved, List<Transform> slotList, string groupName)
    {
        if (attachmentVisualPrefab == null) return;

        int count = Mathf.Min(saved.Length, slotList.Count);

        for (int i = 0; i < count; i++)
        {
            if (saved[i] == AttachmentType.None) continue;

            var slot = slotList[i];

            var go = Instantiate(attachmentVisualPrefab, slot);
            go.name = $"{groupName}_Attachment_{i}_{saved[i]}";
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            _spawned.Add(go);

            // Color by type (simple placeholder visuals)
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = ColorFor(saved[i]);
        }
    }

    private Color ColorFor(AttachmentType type)
    {
        switch (type)
        {
            case AttachmentType.Thruster: return new Color(1f, 0.5f, 0f, 1f);   // orange
            case AttachmentType.Wheel:    return new Color(0.2f, 0.2f, 0.2f, 1f); // dark gray
            case AttachmentType.Saw:      return new Color(0.8f, 0.8f, 0.8f, 1f); // light gray
            case AttachmentType.Laser:    return new Color(0.2f, 1f, 0.2f, 1f);   // green
            case AttachmentType.Cannon:   return new Color(0.2f, 0.6f, 1f, 1f);   // blue
            case AttachmentType.Spike:    return new Color(1f, 0.2f, 0.2f, 1f);   // red
            default: return Color.white;
        }
    }

    private void ClearExisting()
    {
        for (int i = 0; i < _spawned.Count; i++)
        {
            if (_spawned[i] != null)
                Destroy(_spawned[i]);
        }
        _spawned.Clear();
    }
}