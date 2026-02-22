using UnityEngine;

public enum AttachmentType
{
    None = 0,
    Thruster = 1,
    Wheel = 2,
    Saw = 3,
    Laser = 4,
    Cannon = 5,
    Spike = 6
    // Tracks later (multi-slot), we'll add after this step works
}

public static class AttachmentCycle
{
    // Order for clicking/cycling a slot in assembly
    public static AttachmentType Next(AttachmentType current)
    {
        switch (current)
        {
            case AttachmentType.None: return AttachmentType.Thruster;
            case AttachmentType.Thruster: return AttachmentType.Wheel;
            case AttachmentType.Wheel: return AttachmentType.Saw;
            case AttachmentType.Saw: return AttachmentType.Laser;
            case AttachmentType.Laser: return AttachmentType.Cannon;
            case AttachmentType.Cannon: return AttachmentType.Spike;
            case AttachmentType.Spike: return AttachmentType.None;
            default: return AttachmentType.None;
        }
    }
}