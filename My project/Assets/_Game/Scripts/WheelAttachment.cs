using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WheelAttachment : AttachmentBase
{
    [Header("Wheel Tuning")]
    public float driveForce = 30f;

    private int _groundContacts = 0;

    private void Awake()
    {
        // Wheels should collide (NOT trigger) so they can detect contact.
        var col = GetComponent<Collider2D>();
        col.isTrigger = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsOwnRobot(collision)) return;
        _groundContacts++;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsOwnRobot(collision)) return;
        _groundContacts = Mathf.Max(0, _groundContacts - 1);
    }

    private bool IsOwnRobot(Collision2D collision)
    {
        // Ignore collisions with our own robot colliders
        if (collision.rigidbody == null) return false;
        return collision.rigidbody.gameObject == OwnerRb.gameObject;
    }

    private void FixedUpdate()
    {
        if (!InputsEnabled) return;

        // Only drive if the wheel is touching something
        if (_groundContacts <= 0) return;

        float h = HorizontalAxis;
        if (Mathf.Abs(h) < 0.01f) return;

        // Drive along the robot's local right direction.
        // Works on floors and also works reasonably if robot is rotated.
        Vector2 dir = Owner.transform.right * h;
        OwnerRb.AddForceAtPosition(dir * driveForce, transform.position, ForceMode2D.Force);
    }
}