using UnityEngine;

[RequireComponent(typeof(RobotConfig))]
[RequireComponent(typeof(SpriteRenderer))]
public class RobotBodyVisual : MonoBehaviour
{
    [Header("Body Sprites")]
    public Sprite tankSprite;
    public Sprite knightSprite;
    public Sprite ninjaSprite;

    [Header("Optional: per-body scale (leave all 1,1,1 if you don't need)")]
    public Vector3 tankScale = Vector3.one;
    public Vector3 knightScale = Vector3.one;
    public Vector3 ninjaScale = Vector3.one;

    private RobotConfig _config;
    private SpriteRenderer _sr;

    private void Awake()
    {
        _config = GetComponent<RobotConfig>();
        _sr = GetComponent<SpriteRenderer>();
        Apply();
    }

    public void Apply()
    {
        switch (_config.bodyType)
        {
            case RobotBodyType.Tank:
                _sr.sprite = tankSprite;
                transform.localScale = tankScale;
                break;

            case RobotBodyType.Knight:
                _sr.sprite = knightSprite;
                transform.localScale = knightScale;
                break;

            case RobotBodyType.Ninja:
                _sr.sprite = ninjaSprite;
                transform.localScale = ninjaScale;
                break;
        }
    }
}