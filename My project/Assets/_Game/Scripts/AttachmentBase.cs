using UnityEngine;

public abstract class AttachmentBase : MonoBehaviour
{
    public RobotController Owner { get; private set; }
    public Rigidbody2D OwnerRb { get; private set; }
    public AttachmentType Type { get; private set; }
    public SlotGroup MountGroup { get; private set; }
    public int MountIndex { get; private set; }

    public void Init(RobotController owner, AttachmentType type, SlotGroup group, int index)
    {
        Owner = owner;
        OwnerRb = owner != null ? owner.GetComponent<Rigidbody2D>() : null;
        Type = type;
        MountGroup = group;
        MountIndex = index;

        IgnoreOwnerCollisions();
        OnInitialized();
    }

    protected virtual void OnInitialized() { }

    protected bool InputsEnabled => Owner != null && Owner.InputsEnabled;

    protected bool UpHeld =>
        Owner.PlayerId == PlayerId.Player1 ? Input.GetKey(KeyCode.W) : Input.GetKey(KeyCode.UpArrow);

    protected bool DownHeld =>
        Owner.PlayerId == PlayerId.Player1 ? Input.GetKey(KeyCode.S) : Input.GetKey(KeyCode.DownArrow);

    protected bool LeftHeld =>
        Owner.PlayerId == PlayerId.Player1 ? Input.GetKey(KeyCode.A) : Input.GetKey(KeyCode.LeftArrow);

    protected bool RightHeld =>
        Owner.PlayerId == PlayerId.Player1 ? Input.GetKey(KeyCode.D) : Input.GetKey(KeyCode.RightArrow);

    protected float HorizontalAxis
    {
        get
        {
            float v = 0f;
            if (LeftHeld) v -= 1f;
            if (RightHeld) v += 1f;
            return v;
        }
    }

    // Outward direction from the robot for this mount point.
    protected Vector2 MountOutDirWorld
    {
        get
        {
            if (Owner == null) return Vector2.right;
            switch (MountGroup)
            {
                case SlotGroup.LeftSide: return -Owner.transform.right;
                case SlotGroup.RightSide: return Owner.transform.right;
                case SlotGroup.Head: return Owner.transform.up;
                case SlotGroup.Bottom: return -Owner.transform.up;
                default: return Owner.transform.right;
            }
        }
    }

    // For weapons: which key triggers this mount
    protected bool FireHeldForMount
    {
        get
        {
            switch (MountGroup)
            {
                case SlotGroup.LeftSide: return LeftHeld;
                case SlotGroup.RightSide: return RightHeld;
                case SlotGroup.Head: return UpHeld;
                case SlotGroup.Bottom: return DownHeld;
                default: return false;
            }
        }
    }

    private void IgnoreOwnerCollisions()
    {
        if (Owner == null) return;

        var myCols = GetComponentsInChildren<Collider2D>(true);
        var ownerCols = Owner.GetComponentsInChildren<Collider2D>(true);

        for (int i = 0; i < myCols.Length; i++)
        {
            for (int j = 0; j < ownerCols.Length; j++)
            {
                if (myCols[i] != null && ownerCols[j] != null)
                    Physics2D.IgnoreCollision(myCols[i], ownerCols[j], true);
            }
        }
    }

    protected RobotHealth FindEnemyHealth(Collider2D other)
    {
        if (Owner == null) return null;

        RobotHealth h = null;

        if (other.attachedRigidbody != null)
            h = other.attachedRigidbody.GetComponent<RobotHealth>();

        if (h == null)
            h = other.GetComponentInParent<RobotHealth>();

        if (h == null) return null;

        var otherController = h.GetComponent<RobotController>();
        if (otherController != null && otherController.PlayerId == Owner.PlayerId)
            return null; // friendly

        return h;
    }
}
