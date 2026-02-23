using UnityEngine;

public class ThrusterAttachment : AttachmentBase
{
    [Header("Thruster Tuning")]
    public float thrustForce = 25f;

    private void FixedUpdate()
    {
        if (!InputsEnabled) return;

        // Spec: holding W / UpArrow fires thrusters (especially meaningful on bottom slots)
        if (!UpHeld) return;

        // Apply force in robot's "up" direction, at the thruster's position.
        // Off-center thrusters create torque automatically.
        Vector2 forceDir = Owner.transform.up;
        OwnerRb.AddForceAtPosition(forceDir * thrustForce, transform.position, ForceMode2D.Force);
    }
}