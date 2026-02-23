using UnityEngine;

public class CannonAttachment : AttachmentBase
{
    public Projectile projectilePrefab;

    public float fireRate = 1.2f;
    public float projectileSpeed = 12f;
    public float damageHearts = 1.5f;
    public float knockbackImpulse = 4f;

    public float gravityScale = 1f;
    public float recoilImpulse = 6f;
    public float muzzleOffset = 0.35f;

    private float _nextFireTime = 0f;

    private void Update()
    {
        if (!InputsEnabled) return;
        if (!FireHeldForMount) return;
        if (projectilePrefab == null) return;
        if (Time.time < _nextFireTime) return;

        _nextFireTime = Time.time + (1f / fireRate);

        Vector2 dir = MountOutDirWorld.normalized;
        Vector3 spawnPos = transform.position + (Vector3)(dir * muzzleOffset);

        var proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        proj.Init(Owner, dir, projectileSpeed, damageHearts, gravityScale, knockbackImpulse);

        // Recoil
        if (OwnerRb != null)
            OwnerRb.AddForce(-dir * recoilImpulse, ForceMode2D.Impulse);
    }
}
