using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SawAttachment : AttachmentBase
{
    public float damagePerSecond = 2f;
    public float spinDegreesPerSecond = 720f;

    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void Update()
    {
        if (spinDegreesPerSecond != 0f)
            transform.Rotate(0f, 0f, spinDegreesPerSecond * Time.deltaTime);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var enemy = FindEnemyHealth(other);
        if (enemy == null) return;

        enemy.ApplyDamage(damagePerSecond * Time.fixedDeltaTime);
    }
}
