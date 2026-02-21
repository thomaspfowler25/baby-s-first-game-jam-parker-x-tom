using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(RobotConfig))]
public class RobotController : MonoBehaviour
{
    public bool InputsEnabled { get; private set; } = true;

    public PlayerId PlayerId => _config.playerId;
    public RobotBodyType BodyType => _config.bodyType;

    [Header("Movement Multipliers (tweakable)")]
    [Tooltip("1 = default from body type stats.")]
    public float accelerationMultiplier = 1f;

    [Tooltip("1 = default from body type stats.")]
    public float maxSpeedMultiplier = 1f;

    private Rigidbody2D _rb;
    private RobotConfig _config;

    // cached per-body stats (after multipliers)
    private float _accel;
    private float _maxSpeed;

    // input cached per-frame
    private float _hInput;
    private bool _upInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _config = GetComponent<RobotConfig>();
        ApplyBodyStats();
    }

    private void ApplyBodyStats()
    {
        var stats = RobotBodyStatsDB.Get(_config.bodyType);

        _rb.mass = stats.mass;
        _rb.linearDamping = stats.drag;
        _rb.angularDamping = stats.angularDrag;

        _accel = stats.linearAcceleration * Mathf.Max(0.01f, accelerationMultiplier);
        _maxSpeed = stats.maxSpeed * Mathf.Max(0.01f, maxSpeedMultiplier);
    }

    private void Update()
    {
        if (!InputsEnabled)
        {
            _hInput = 0f;
            _upInput = false;
            return;
        }

        // Player 1 = WASD, Player 2 = Arrow keys
        if (_config.playerId == PlayerId.Player1)
        {
            _hInput = 0f;
            if (Input.GetKey(KeyCode.A)) _hInput -= 1f;
            if (Input.GetKey(KeyCode.D)) _hInput += 1f;
            _upInput = Input.GetKey(KeyCode.W);
        }
        else
        {
            _hInput = 0f;
            if (Input.GetKey(KeyCode.LeftArrow)) _hInput -= 1f;
            if (Input.GetKey(KeyCode.RightArrow)) _hInput += 1f;
            _upInput = Input.GetKey(KeyCode.UpArrow);
        }
    }

    private void FixedUpdate()
    {
        if (!InputsEnabled) return;

        // Up thruster (relative to current robot rotation)
        if (_upInput)
        {
            _rb.AddForce((Vector2)transform.up * _accel, ForceMode2D.Force);
        }

        // Side thrust (relative to current robot rotation)
        if (Mathf.Abs(_hInput) > 0.01f)
        {
            _rb.AddForce((Vector2)transform.right * (_hInput * _accel), ForceMode2D.Force);
        }

        // clamp speed so robots don't accelerate forever
        float speed = _rb.linearVelocity.magnitude;
        if (speed > _maxSpeed)
        {
            _rb.linearVelocity = _rb.linearVelocity.normalized * _maxSpeed;
        }
    }

    public void SetInputsEnabled(bool enabled)
    {
        InputsEnabled = enabled;
        if (!enabled)
        {
            _hInput = 0f;
            _upInput = false;
        }
    }

    public void HardResetPose(Vector3 position)
    {
        transform.position = position;
        transform.rotation = Quaternion.identity;
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;
    }

    // Handy button: if you change bodyType during play mode, you can call this from the Inspector context menu later.
    public void ReapplyBodyStatsForTesting()
    {
        ApplyBodyStats();
    }
}
