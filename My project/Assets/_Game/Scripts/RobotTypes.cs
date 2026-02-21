using UnityEngine;

public enum PlayerId
{
    Player1 = 1,
    Player2 = 2
}

public enum RobotBodyType
{
    Tank,
    Knight,
    Ninja
}

[System.Serializable]
public struct RobotBodyStats
{
    public float maxHearts;

    // Physics feel
    public float mass;
    public float linearAcceleration;
    public float maxSpeed;
    public float drag;
    public float angularDrag;
}

public static class RobotBodyStatsDB
{
    public static RobotBodyStats Get(RobotBodyType type)
    {
        // These values are starter tuning only.
        // We'll tweak after you test.
        switch (type)
        {
            case RobotBodyType.Tank:
                return new RobotBodyStats
                {
                    maxHearts = 5f,
                    mass = 4f,
                    linearAcceleration = 35f,
                    maxSpeed = 8f,
                    drag = 1.6f,
                    angularDrag = 6f
                };

            case RobotBodyType.Knight:
                return new RobotBodyStats
                {
                    maxHearts = 3f,
                    mass = 2.5f,
                    linearAcceleration = 45f,
                    maxSpeed = 10f,
                    drag = 1.1f,
                    angularDrag = 4f
                };

            case RobotBodyType.Ninja:
                return new RobotBodyStats
                {
                    maxHearts = 1.5f,
                    mass = 1.6f,
                    linearAcceleration = 60f,
                    maxSpeed = 13f,
                    drag = 0.6f,
                    angularDrag = 2f
                };

            default:
                return new RobotBodyStats
                {
                    maxHearts = 3f,
                    mass = 2.5f,
                    linearAcceleration = 45f,
                    maxSpeed = 10f,
                    drag = 1.1f,
                    angularDrag = 4f
                };
        }
    }
}
