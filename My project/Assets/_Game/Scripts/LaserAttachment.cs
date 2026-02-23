using UnityEngine;

public class LaserAttachment : AttachmentBase
{
    public Projectile projectilePrefab;

    public float fireRate = 8f;
    public float projectileSpeed = 28f;
    public float damageHearts = 0.5f;
    public float knockbackImpulse = 1.5f;
    public float muzzleOffset = 0.25f;

    private float _nextFireTime = 0f;

    private void Update()
    {
        if (!InputsEnabled) return;
        if (!FireHeldForMount) return;
        if (projectilePrefab == null) return;
        if (Time.time < _nextFireTime) return;

        _nextFireTime = Time.time + (1f / fireRate);

        Fire(gravityScale: 0f);
    }

    private void Fire(float gravityScale)
    {
        Vector2 dir = MountOutDirWorld.normalized;
        Vector3 spawnPos = transform.position + (Vector3)(dir * muzzleOffset);

        var proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        proj.Init(Owner, dir, projectileSpeed, damageHearts, gravityScale, knockbackImpulse);
    }
}
