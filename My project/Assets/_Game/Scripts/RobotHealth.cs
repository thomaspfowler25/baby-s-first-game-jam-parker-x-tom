using System;
using UnityEngine;

[RequireComponent(typeof(RobotConfig))]
public class RobotHealth : MonoBehaviour
{
    public float MaxHearts { get; private set; }
    public float CurrentHearts { get; private set; }

    public event Action<RobotHealth> OnDied;
    public event Action<RobotHealth> OnHealthChanged;

    private RobotConfig _config;

    private void Awake()
    {
        _config = GetComponent<RobotConfig>();
        ApplyBodyType(_config.bodyType, resetToFull: true);
    }

    /// <summary>
    /// Applies a body type's max hearts and optionally refills current hearts.
    /// </summary>
    public void ApplyBodyType(RobotBodyType bodyType, bool resetToFull)
    {
        MaxHearts = RobotBodyStatsDB.Get(bodyType).maxHearts;

        if (resetToFull)
            CurrentHearts = MaxHearts;
        else
            CurrentHearts = Mathf.Clamp(CurrentHearts, 0f, MaxHearts);

        OnHealthChanged?.Invoke(this);
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
            OnDied?.Invoke(this);
    }
}