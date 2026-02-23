using System.Collections.Generic;
using UnityEngine;

public class AttachmentSpawner : MonoBehaviour
{
    [Header("Fallback Visual Prefab (used if type prefab not assigned)")]
    public GameObject attachmentVisualPrefab;

    [Header("Type Prefabs (assign the ones you have)")]
    public GameObject thrusterPrefab;
    public GameObject wheelPrefab;
    public GameObject sawPrefab;
    public GameObject laserPrefab;
    public GameObject cannonPrefab;
    public GameObject spikePrefab;

    [Header("Robots")]
    public RobotSlots p1Slots;
    public RobotSlots p2Slots;

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
        var owner = slots.GetComponent<RobotController>();

        SpawnGroup(bottom, slots.BottomSlots, owner, SlotGroup.Bottom);
        SpawnGroup(left, slots.LeftSideSlots, owner, SlotGroup.LeftSide);
        SpawnGroup(right, slots.RightSideSlots, owner, SlotGroup.RightSide);
        SpawnGroup(head, slots.HeadSlots, owner, SlotGroup.Head);
    }

    private void SpawnGroup(AttachmentType[] saved, List<Transform> slotList, RobotController owner, SlotGroup group)
    {
        int count = Mathf.Min(saved.Length, slotList.Count);

        for (int i = 0; i < count; i++)
        {
            var type = saved[i];
            if (type == AttachmentType.None) continue;

            var slot = slotList[i];

            GameObject prefab = PrefabFor(type);
            GameObject go;

            if (prefab != null)
            {
                go = Instantiate(prefab, slot);
            }
            else
            {
                // fallback colored block
                go = Instantiate(attachmentVisualPrefab, slot);
                var sr = go.GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = ColorFor(type);
            }

            go.name = $"{group}_Attachment_{i}_{type}";
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            _spawned.Add(go);

            // Init attachment scripts if present
            var attachment = go.GetComponent<AttachmentBase>();
            if (attachment != null && owner != null)
            {
                attachment.Init(owner, type, group, i);
            }
        }
    }

    private GameObject PrefabFor(AttachmentType type)
    {
        switch (type)
        {
            case AttachmentType.Thruster: return thrusterPrefab;
            case AttachmentType.Wheel: return wheelPrefab;
            case AttachmentType.Saw: return sawPrefab;
            case AttachmentType.Laser: return laserPrefab;
            case AttachmentType.Cannon: return cannonPrefab;
            case AttachmentType.Spike: return spikePrefab;
            default: return null;
        }
    }

    private Color ColorFor(AttachmentType type)
    {
        switch (type)
        {
            case AttachmentType.Thruster: return new Color(1f, 0.5f, 0f, 1f);
            case AttachmentType.Wheel: return new Color(0.2f, 0.2f, 0.2f, 1f);
            case AttachmentType.Saw: return new Color(0.8f, 0.8f, 0.8f, 1f);
            case AttachmentType.Laser: return new Color(0.2f, 1f, 0.2f, 1f);
            case AttachmentType.Cannon: return new Color(0.2f, 0.6f, 1f, 1f);
            case AttachmentType.Spike: return new Color(1f, 0.2f, 0.2f, 1f);
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