using UnityEngine;

[CreateAssetMenu(fileName = "Player Settings", menuName = "Flat Earth/Player Settings", order = 1)]
public sealed class PlayerMovementSettings : ScriptableObject {
    #region Singleton

    private static PlayerMovementSettings instance;
    private static PlayerMovementSettings Instance {
        get {
            if (!instance) {
                instance = Resources.Load<PlayerMovementSettings>("Player Settings");
            }
            return instance;
        }
    }

    #endregion

    [SerializeField] public float moveSpeed = 10f;
    [SerializeField] public float minBounceForce = 5f;
    [SerializeField] public float maxBounceForce = 20f;

    public static float MoveSpeed => Instance.moveSpeed;
    public static float MinBounceForce => Instance.minBounceForce;
    public static float MaxBounceForce => Instance.maxBounceForce;

}