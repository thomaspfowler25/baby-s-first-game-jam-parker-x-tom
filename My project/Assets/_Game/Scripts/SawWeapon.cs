using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SawWeapon : MonoBehaviour
{
    [Tooltip("Damage in hearts per second while touching an enemy robot.")]
    public float damagePerSecond = 2f;

    [Tooltip("Optional: spin for visuals.")]
    public float spinDegreesPerSecond = 360f;

    private RobotController _owner;

    private void Awake()
    {
        _owner = GetComponentInParent<RobotController>();

        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void Update()
    {
        if (Mathf.Abs(spinDegreesPerSecond) > 0.01f)
        {
            transform.Rotate(0f, 0f, spinDegreesPerSecond * Time.deltaTime);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (_owner == null) return;

        RobotHealth otherHealth = null;

        // Prefer the root rigidbody object
        if (other.attachedRigidbody != null)
            otherHealth = other.attachedRigidbody.GetComponent<RobotHealth>();

        if (otherHealth == null)
            otherHealth = other.GetComponentInParent<RobotHealth>();

        if (otherHealth == null) return;

        // Prevent friendly fire (same player)
        var otherController = otherHealth.GetComponent<RobotController>();
        if (otherController != null && otherController.PlayerId == _owner.PlayerId)
            return;

        float dmg = damagePerSecond * Time.fixedDeltaTime;
        otherHealth.ApplyDamage(dmg);
    }
}
