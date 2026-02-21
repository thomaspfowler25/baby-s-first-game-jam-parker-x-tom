using System;
using UnityEngine;

[RequireComponent(typeof(RobotConfig))]
public class RobotHealth : MonoBehaviour
{
    public float MaxHearts { get; private set; }
    public float CurrentHearts { get; private set; }

    public event Action<RobotHealth> OnDied;
    public event Action<RobotHealth> OnHealthChanged;

    [Header("Optional Override (leave at -1)")]
    [Tooltip("Set to > 0 to override max hearts (debug/testing).")]
    public float maxHeartsOverride = -1f;

    private RobotConfig _config;

    private void Awake()
    {
        _config = GetComponent<RobotConfig>();

        var stats = RobotBodyStatsDB.Get(_config.bodyType);
        MaxHearts = (maxHeartsOverride > 0f) ? maxHeartsOverride : stats.maxHearts;
        CurrentHearts = MaxHearts;
    }

    public void ResetToFull()
    {
        CurrentHearts = MaxHearts;
        OnHealthChanged?.Invoke(this);
    }

    public void ApplyDamage(float hearts)
    {
        if (hearts <= 0f) return;
        if (CurrentHearts <= 0f) return;

        CurrentHearts -= hearts;
        if (CurrentHearts < 0f) CurrentHearts = 0f;

        OnHealthChanged?.Invoke(this);

        if (CurrentHearts <= 0f)
        {
            OnDied?.Invoke(this);
        }
    }
}
