using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SpikeAttachment : AttachmentBase
{
    public float hitDamage = 1f;
    public float knockbackImpulse = 8f;
    public float recoilImpulse = 2.5f;
    public float hitCooldown = 0.35f;

    private float _nextHitTime = 0f;

    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!InputsEnabled) { /* still allow passive? choose: allow anyway */ }
        if (Time.time < _nextHitTime) return;

        var enemy = FindEnemyHealth(other);
        if (enemy == null) return;

        _nextHitTime = Time.time + hitCooldown;

        enemy.ApplyDamage(hitDamage);

        // Push enemy outward from this spike mount direction
        var enemyRb = enemy.GetComponent<Rigidbody2D>();
        if (enemyRb != null)
            enemyRb.AddForce(MountOutDirWorld * knockbackImpulse, ForceMode2D.Impulse);

        // Small recoil for the owner (optional)
        if (OwnerRb != null)
            OwnerRb.AddForce(-MountOutDirWorld * recoilImpulse, ForceMode2D.Impulse);
    }
}
