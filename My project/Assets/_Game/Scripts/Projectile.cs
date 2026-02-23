using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    public float lifetime = 3f;

    private RobotController _owner;
    private PlayerId _ownerId;
    private float _damageHearts;
    private float _knockbackImpulse;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Init(RobotController owner, Vector2 dir, float speed, float damageHearts, float gravityScale, float knockbackImpulse)
    {
        _owner = owner;
        _ownerId = owner.PlayerId;
        _damageHearts = damageHearts;
        _knockbackImpulse = knockbackImpulse;

        _rb.gravityScale = gravityScale;
        _rb.linearVelocity = dir.normalized * speed;

        // Ignore collisions with the owner robot
        var myCol = GetComponent<Collider2D>();
        var ownerCols = owner.GetComponentsInChildren<Collider2D>(true);
        for (int i = 0; i < ownerCols.Length; i++)
        {
            if (ownerCols[i] != null)
                Physics2D.IgnoreCollision(myCol, ownerCols[i], true);
        }

        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.collider, collision.rigidbody);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other, other.attachedRigidbody);
    }

    private void HandleHit(Collider2D col, Rigidbody2D rb)
    {
        if (col == null) return;

        RobotHealth h = null;
        if (rb != null) h = rb.GetComponent<RobotHealth>();
        if (h == null) h = col.GetComponentInParent<RobotHealth>();

        if (h != null)
        {
            var otherController = h.GetComponent<RobotController>();
            if (otherController != null && otherController.PlayerId == _ownerId)
                return;

            h.ApplyDamage(_damageHearts);

            if (rb != null && _knockbackImpulse > 0f)
            {
                Vector2 dir = (rb.worldCenterOfMass - _rb.position).normalized;
                rb.AddForce(dir * _knockbackImpulse, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject);
    }
}
